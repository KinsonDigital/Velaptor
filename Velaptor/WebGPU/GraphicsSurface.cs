// <copyright file="GraphicsSurface.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using System.Diagnostics.CodeAnalysis;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Connects an OS window to the WebGPU rendering system and manages the swap chain
/// that controls how rendered frames reach the display.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with native WebGPU and windowing libraries.")]
internal sealed class GraphicsSurface : IDisposable
{
    private readonly GraphicsDevice gd;
    private readonly IWindow window;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;
    private SafeSurfaceHandle? handle;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSurface"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="window">The window to create the surface for.</param>
    public GraphicsSurface(GraphicsDevice gd, IWindow window)
    {
        this.gd = gd;
        this.window = window;
    }

    /// <summary>
    /// Gets the raw WebGPU surface handle — the platform-specific binding between the
    /// OS window and the WebGPU instance. Must be initialized via <see cref="Initialize"/>
    /// before use.
    /// </summary>
    public SafeSurfaceHandle Handle
    {
        get
        {
            if (this.handle is null)
            {
                throw new InvalidOperationException(
                    "The WebGPU surface has not been initialized. Call Initialize() first.");
            }

            return this.handle;
        }
    }

    /// <summary>
    /// Gets the pixel format the swap chain textures are allocated in.
    /// Populated by <see cref="InitializeFormat"/> by querying the adapter for its preference.
    /// </summary>
    public TextureFormat Format { get; private set; }

    /// <summary>
    /// Initializes the WebGPU surface by creating the platform-specific surface handle.
    /// This must be called after the window is fully created and shown.
    /// After calling this, <see cref="Handle"/> will be available.
    /// </summary>
    public void Initialize()
    {
        if (this.isInitialized)
        {
            return;
        }

        this.handle = new SafeSurfaceHandle(this.gd.Wgpu, this.window, this.gd.Instance);

        if (this.handle.IsInvalid)
        {
            throw new Exception("Failed to create WebGPU surface.");
        }

        this.isInitialized = true;
    }

    /// <summary>
    /// Queries the adapter's preferred pixel format. Call after <see cref="Initialize"/>
    /// and after the device exists, but before configuring the swap chain — this lets
    /// pipelines be built with the correct color-target format before the window size
    /// has been finalized.
    /// </summary>
    public void InitializeFormat()
    {
        if (this.gd.Adapter is null || this.gd.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before querying the surface format.");
        }

        unsafe
        {
            Format = this.gd.Wgpu.SurfaceGetPreferredFormat(Handle, this.gd.Adapter);
        }
    }

    /// <summary>
    /// (Re)configures the swap chain to match the current framebuffer size.
    /// Must be called once after the window reaches its final initial size and
    /// again after every window resize.
    /// </summary>
    public void Configure()
    {
        if (this.gd.Adapter is null || this.gd.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before configuring the surface.");
        }

        unsafe
        {
            var size = this.window.FramebufferSize;

            Format = this.gd.Wgpu.SurfaceGetPreferredFormat(Handle, this.gd.Adapter);

            var config = new SurfaceConfiguration
            {
                Device = (Device*)this.gd.Handle.DangerousGetHandle(),
                Format = Format,
                Usage = TextureUsage.RenderAttachment,
                Width = (uint)size.X,
                Height = (uint)size.Y,
                PresentMode = PresentMode.Fifo,
            };

            this.gd.Wgpu.SurfaceConfigure(Handle, in config);
        }
    }

    /// <summary>
    /// Gets the current surface texture from the swap chain for rendering this frame.
    /// </summary>
    /// <returns>The surface texture handle for this frame.</returns>
    public SafeSurfaceTextureHandle GetSurfaceTexture()
    {
        // Per WebGPU spec: the previously-obtained surface texture and all texture
        // views created from it must be released BEFORE calling getCurrentTexture
        // again. Release the old texture first, then acquire the new one.
        this.surfaceTextureHandle?.Dispose();
        this.surfaceTextureHandle = null;

        unsafe
        {
            SurfaceTexture st = default;
            this.gd.Wgpu.SurfaceGetCurrentTexture(Handle, ref st);

            this.surfaceTextureHandle = new SafeSurfaceTextureHandle(this.gd.Wgpu, (nint)st.Texture, st.Status);
        }

        return this.surfaceTextureHandle;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.surfaceTextureHandle?.Dispose();
        this.handle?.Dispose();
        GC.SuppressFinalize(this);
    }
}

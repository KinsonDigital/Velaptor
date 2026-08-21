// <copyright file="GraphicsSurface.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using NativeInterop.WebGpu.Handles;

/// <inheritdoc/>
internal sealed class GraphicsSurface : IGraphicsSurface
{
    private readonly IGraphicsDevice grfxDevice;
    private readonly IWindow window;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;
    private SafeSurfaceHandle? handle;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSurface"/> class.
    /// </summary>
    /// <param name="grfxDevice">The graphics device.</param>
    /// <param name="window">The window to create the surface for.</param>
    public GraphicsSurface(IGraphicsDevice grfxDevice, IWindow window)
    {
        ArgumentNullException.ThrowIfNull(grfxDevice);
        ArgumentNullException.ThrowIfNull(window);

        this.grfxDevice = grfxDevice;
        this.window = window;
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public TextureFormat Format { get; private set; }

    /// <inheritdoc/>
    public void Initialize()
    {
        if (this.isInitialized)
        {
            return;
        }

        this.handle = new SafeSurfaceHandle(this.grfxDevice.Wgpu, this.window, this.grfxDevice.Instance);

        if (this.handle.IsInvalid)
        {
            throw new InvalidOperationException("Failed to create WebGPU surface.");
        }

        this.isInitialized = true;
    }

    /// <inheritdoc/>
    public void InitializeFormat()
    {
        if (this.grfxDevice.Adapter is null || this.grfxDevice.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before querying the surface format.");
        }

        Format = this.grfxDevice.Wgpu.SurfaceGetPreferredFormat(Handle, this.grfxDevice.Adapter);
    }

    /// <inheritdoc/>
    public bool Configure()
    {
        if (this.grfxDevice.Adapter is null || this.grfxDevice.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before configuring the surface.");
        }

        var size = this.window.FramebufferSize;

        // When the window is minimized the framebuffer size is (0, 0).
        // wgpuSurfaceConfigure panics with a validation error if either
        // dimension is zero, so skip the call and let the next resize
        // (when the window is restored) reconfigure the surface.
        if (size.X == 0 || size.Y == 0)
        {
            return false;
        }

        Format = this.grfxDevice.Wgpu.SurfaceGetPreferredFormat(Handle, this.grfxDevice.Adapter);

        this.grfxDevice.Wgpu.SurfaceConfigure(
            Handle,
            this.grfxDevice.Handle,
            Format,
            TextureUsage.RenderAttachment,
            (uint)size.X,
            (uint)size.Y,
            PresentMode.Fifo);

        return true;
    }

    /// <inheritdoc/>
    public SafeSurfaceTextureHandle GetSurfaceTexture()
    {
        // Per WebGPU spec: the previously-obtained surface texture and all texture
        // views created from it must be released BEFORE calling getCurrentTexture
        // again. Release the old texture first, then acquire the new one.
        this.surfaceTextureHandle?.Dispose();
        this.surfaceTextureHandle = null;

        this.surfaceTextureHandle = this.grfxDevice.Wgpu.SurfaceGetCurrentTexture(Handle);

        return this.surfaceTextureHandle;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.surfaceTextureHandle?.Dispose();
        this.handle?.Dispose();
    }
}

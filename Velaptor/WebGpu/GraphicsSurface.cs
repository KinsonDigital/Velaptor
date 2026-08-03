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
    private readonly IGraphicsDevice gd;
    private readonly IWindow window;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;
    private SafeSurfaceHandle? handle;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSurface"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="window">The window to create the surface for.</param>
    public GraphicsSurface(IGraphicsDevice gd, IWindow window)
    {
        ArgumentNullException.ThrowIfNull(gd);
        ArgumentNullException.ThrowIfNull(window);

        this.gd = gd;
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

        this.handle = new SafeSurfaceHandle(this.gd.Wgpu, this.window, this.gd.Instance);

        if (this.handle.IsInvalid)
        {
            throw new InvalidOperationException("Failed to create WebGPU surface.");
        }

        this.isInitialized = true;
    }

    /// <inheritdoc/>
    public void InitializeFormat()
    {
        if (this.gd.Adapter is null || this.gd.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before querying the surface format.");
        }

        Format = this.gd.Wgpu.SurfaceGetPreferredFormat(Handle, this.gd.Adapter);
    }

    /// <inheritdoc/>
    public void Configure()
    {
        if (this.gd.Adapter is null || this.gd.Handle is null)
        {
            throw new InvalidOperationException("Device and adapter must be initialized before configuring the surface.");
        }

        var size = this.window.FramebufferSize;

        Format = this.gd.Wgpu.SurfaceGetPreferredFormat(Handle, this.gd.Adapter);

        this.gd.Wgpu.SurfaceConfigure(
            Handle,
            this.gd.Handle,
            Format,
            TextureUsage.RenderAttachment,
            (uint)size.X,
            (uint)size.Y,
            PresentMode.Fifo);
    }

    /// <inheritdoc/>
    public SafeSurfaceTextureHandle GetSurfaceTexture()
    {
        // Per WebGPU spec: the previously-obtained surface texture and all texture
        // views created from it must be released BEFORE calling getCurrentTexture
        // again. Release the old texture first, then acquire the new one.
        this.surfaceTextureHandle?.Dispose();
        this.surfaceTextureHandle = null;

        this.surfaceTextureHandle = this.gd.Wgpu.SurfaceGetCurrentTexture(Handle);

        return this.surfaceTextureHandle;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.surfaceTextureHandle?.Dispose();
        this.handle?.Dispose();
    }
}

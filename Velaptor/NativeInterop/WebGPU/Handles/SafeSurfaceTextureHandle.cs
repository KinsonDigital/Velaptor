// <copyright file="SafeSurfaceTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using System;
using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

/// <summary>
/// Manages the lifecycle of a WebGPU surface texture handle.
/// </summary>
internal sealed class SafeSurfaceTextureHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceTextureHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="textureHandle">The native texture handle.</param>
    /// <param name="surfaceTextureStatus">The status of getting the surface texture.</param>
    public SafeSurfaceTextureHandle(IWgpuInvoker wgpu, nint textureHandle, SurfaceGetCurrentTextureStatus surfaceTextureStatus)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;

        SetHandle(textureHandle);
        SurfaceTextureStatus = surfaceTextureStatus;
    }

    /// <summary>
    /// Gets the status of getting the current surface texture.
    /// </summary>
    public SurfaceGetCurrentTextureStatus SurfaceTextureStatus { get; private set; }

    /// <summary>
    /// Disposes of the current handle and recreates the handle.
    /// </summary>
    /// <param name="surfaceHandle">The surface handle.</param>
    public void ResetHandle(SafeSurfaceHandle surfaceHandle)
    {
        ReleaseHandle();
        SetHandle(this.wgpu.UnsafeSurfaceGetCurrentTexture(surfaceHandle));
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.TextureRelease(this.handle);

        return true;
    }
}

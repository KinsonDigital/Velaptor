// <copyright file="SafeSurfaceTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using System;
using Silk.NET.WebGPU;

/// <summary>
/// Manages the lifecycle of a WebGPU surface texture handle.
/// </summary>
/// <remarks>
/// Surface textures are short-lived (one per frame). The old texture must be released
/// before storing a new one in <see cref="UpdateHandleAndStatus"/>.
/// </remarks>
internal sealed class SafeSurfaceTextureHandle : IDisposable
{
    private readonly IWgpuInvoker wgpu;
    private nint handle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceTextureHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="texturePointer">The native texture pointer.</param>
    /// <param name="surfaceTextureStatus">The status of getting the surface texture.</param>
    public SafeSurfaceTextureHandle(IWgpuInvoker wgpu, nint texturePointer, SurfaceGetCurrentTextureStatus surfaceTextureStatus)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;

        this.handle = texturePointer;
        SurfaceTextureStatus = surfaceTextureStatus;
    }

    /// <summary>
    /// Gets a value indicating whether the handle is invalid.
    /// </summary>
    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    /// <summary>
    /// Gets the status of getting the current surface texture.
    /// </summary>
    public SurfaceGetCurrentTextureStatus SurfaceTextureStatus { get; private set; }

    /// <summary>
    /// Gets the raw native handle.
    /// </summary>
    /// <returns>The native pointer.</returns>
    public nint DangerousGetHandle() => this.handle;

    /// <summary>
    /// Releases the previous surface texture and stores the new one.
    /// </summary>
    /// <param name="newTexturePointer">The new surface texture pointer.</param>
    /// <param name="newStatus">The new surface texture status.</param>
    public void UpdateHandleAndStatus(nint newTexturePointer, SurfaceGetCurrentTextureStatus newStatus)
    {
        // Release the previous surface texture before storing the new one.
        if (!IsInvalid)
        {
            this.wgpu.TextureRelease(this.handle);
        }

        this.handle = newTexturePointer;
        SurfaceTextureStatus = newStatus;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (IsInvalid)
        {
            return;
        }

        this.wgpu.TextureRelease(this.handle);
        this.handle = IntPtr.Zero;
    }
}

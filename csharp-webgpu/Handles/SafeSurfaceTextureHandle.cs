// <copyright file="SafeSurfaceTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using NativeInterop.WebGPU;
using Silk.NET.WebGPU;

internal class SafeSurfaceTextureHandle : IDisposable
{
    private readonly WGPUInvoker wgpu;
    private nint handle;

    public SafeSurfaceTextureHandle(WGPUInvoker wgpu, nint texturePointer, SurfaceGetCurrentTextureStatus surfaceTextureStatus)
    {
        this.wgpu = wgpu;

        this.handle = texturePointer;
        SurfaceTextureStatus = surfaceTextureStatus;
    }

    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    public SurfaceGetCurrentTextureStatus SurfaceTextureStatus { get; private set; }

    public nint DangerousGetHandle() => handle;

    public void UpdateHandleAndStatus(nint newTexturePointer, SurfaceGetCurrentTextureStatus newStatus)
    {
        // Release the previous surface texture before storing the new one.
        // Without this, each frame leaks the old swap-chain texture.
        if (!IsInvalid)
        {
            this.wgpu.TextureRelease(this.handle);
        }

        this.handle = newTexturePointer;
        SurfaceTextureStatus = newStatus;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (IsInvalid)
        {
            return;
        }

        this.wgpu.TextureRelease(this.handle);
        this.handle = IntPtr.Zero;
    }
}

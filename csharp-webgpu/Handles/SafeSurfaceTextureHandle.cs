// <copyright file="SafeSurfaceTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Silk.NET.WebGPU;

internal class SafeSurfaceTextureHandle : IDisposable
{
    private readonly WebGPU wgpu;
    private nint handle;

    public SafeSurfaceTextureHandle(WebGPU wgpu, nint texturePointer, SurfaceGetCurrentTextureStatus surfaceTextureStatus)
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
        this.handle = newTexturePointer;
        SurfaceTextureStatus = newStatus;
    }

    public void Dispose() => Dispose(disposing: true);

    private void Dispose(bool disposing)
    {
        if (IsInvalid)
        {
            return;
        }

        unsafe
        {
            this.wgpu.TextureRelease((Texture*)this.handle);
        }

        this.handle = IntPtr.Zero;
    }
}

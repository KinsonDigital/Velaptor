// <copyright file="SafeTextureViewHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Silk.NET.WebGPU;

internal class SafeTextureViewHandle : IDisposable
{
    private readonly WebGPU wgpu;
    private nint handle;

    public SafeTextureViewHandle(WebGPU wgpu, nint texture, in TextureViewDescriptor textureViewDescriptor)
    {
        this.wgpu = wgpu;

        SetHandle(texture, in textureViewDescriptor);
    }

    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    public nint DangerousGetHandle() => this.handle;

    public void UpdateHandle(nint texture, in TextureViewDescriptor textureViewDescriptor) => SetHandle(texture, in textureViewDescriptor);

    public void Dispose() => Dispose(disposing: true);

    private void Dispose(bool disposing)
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.TextureViewRelease((TextureView*)this.handle);
            }
        }

        this.handle = IntPtr.Zero;
    }

    private void SetHandle(nint texture, in TextureViewDescriptor textureViewDescriptor)
    {
        unsafe
        {
            this.handle = (nint)this.wgpu.TextureCreateView((Texture*)texture, in textureViewDescriptor);
        }
    }
}

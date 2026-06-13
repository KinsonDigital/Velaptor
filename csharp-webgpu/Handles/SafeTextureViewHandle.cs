// <copyright file="SafeTextureViewHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using NativeInterop.WebGPU;

internal class SafeTextureViewHandle : IDisposable
{
    private readonly WGPUInvoker wgpu;
    private nint handle;

    public SafeTextureViewHandle(WGPUInvoker wgpu, nint texture, ref readonly Silk.NET.WebGPU.TextureViewDescriptor textureViewDescriptor)
    {
        this.wgpu = wgpu;
        SetHandle(texture, in textureViewDescriptor);
    }

    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    public nint DangerousGetHandle() => this.handle;

    public void UpdateHandle(nint texture, ref readonly Silk.NET.WebGPU.TextureViewDescriptor textureViewDescriptor) => SetHandle(texture, in textureViewDescriptor);

    public void Dispose() => Dispose(disposing: true);

    private void Dispose(bool disposing)
    {
        if (!IsInvalid)
        {
            this.wgpu.TextureViewRelease(this.handle);
        }

        this.handle = IntPtr.Zero;
    }

    private void SetHandle(nint texture, ref readonly Silk.NET.WebGPU.TextureViewDescriptor textureViewDescriptor)
    {
        this.handle = this.wgpu.TextureCreateView(texture, in textureViewDescriptor);
    }
}

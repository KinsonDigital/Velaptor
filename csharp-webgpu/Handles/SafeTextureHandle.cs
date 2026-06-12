// <copyright file="SafeTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeTextureHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeTextureHandle(WebGPU wgpu, SafeDeviceHandle deviceHandle, in TextureDescriptor textureDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)this.wgpu.DeviceCreateTexture((Device*)deviceHandle.DangerousGetHandle(), in textureDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.TextureDestroy((Texture*)this.handle);
                this.wgpu.TextureRelease((Texture*)this.handle);
            }
        }

        return true;
    }
}

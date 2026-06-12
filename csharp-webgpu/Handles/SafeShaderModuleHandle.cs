// <copyright file="SafeShaderModuleHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeShaderModuleHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeShaderModuleHandle(WebGPU wgpu, SafeDeviceHandle deviceHandle, ShaderModuleDescriptor shaderModuleDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)this.wgpu.DeviceCreateShaderModule((Device*)deviceHandle.DangerousGetHandle(), in shaderModuleDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.ShaderModuleRelease((ShaderModule*)this.handle);
            }
        }

        return true;
    }
}

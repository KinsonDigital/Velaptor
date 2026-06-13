// <copyright file="SafeSamplerHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

internal class SafeSamplerHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeSamplerHandle(WGPUInvoker wgpu, SafeDeviceHandle deviceHandle, ref readonly Silk.NET.WebGPU.SamplerDescriptor samplerDesc)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateSampler(deviceHandle, in samplerDesc));
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.SamplerRelease(this.handle);
        }

        return true;
    }
}

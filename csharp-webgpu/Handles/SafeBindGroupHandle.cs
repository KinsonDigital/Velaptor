// <copyright file="SafeBindGroupHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

internal class SafeBindGroupHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeBindGroupHandle(WGPUInvoker wgpu, SafeDeviceHandle deviceHandle, ref readonly Silk.NET.WebGPU.BindGroupDescriptor bindGroupDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateBindGroup(deviceHandle, in bindGroupDescriptor));
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.BindGroupRelease(this.handle);
        }

        return true;
    }
}

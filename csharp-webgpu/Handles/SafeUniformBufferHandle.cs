// <copyright file="SafeUniformBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

internal class SafeUniformBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeUniformBufferHandle(WGPUInvoker wgpu, SafeDeviceHandle grfxDeviceHandle, ref readonly Silk.NET.WebGPU.BufferDescriptor bufferDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateBuffer(grfxDeviceHandle, in bufferDescriptor));
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.BufferRelease(this.handle);
        }

        return true;
    }
}

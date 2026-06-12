// <copyright file="SafeUniformBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeUniformBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeUniformBufferHandle(WebGPU wgpu, SafeDeviceHandle grfxDeviceHandle, in BufferDescriptor bufferDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)wgpu.DeviceCreateBuffer((Device*)grfxDeviceHandle.DangerousGetHandle(), in bufferDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.BufferRelease((Buffer*)this.handle);
            }
        }

        return true;
    }
}

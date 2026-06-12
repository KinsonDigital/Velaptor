// <copyright file="QueueSafeHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeQueueHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeQueueHandle(WebGPU wgpu, SafeDeviceHandle deviceHandle)
        : base(ownsHandle: true)
    {
        unsafe
        {
            this.wgpu = wgpu;

            SetHandle((nint)this.wgpu.DeviceGetQueue((Device*)deviceHandle.DangerousGetHandle()));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.QueueRelease((Queue*)this.handle);
            }
        }

        return true;
    }
}

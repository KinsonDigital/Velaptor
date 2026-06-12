// <copyright file="SafeDeviceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeDeviceHandle(WebGPU wgpu, IntPtr handle)
        : base(true)
    {
        this.wgpu = wgpu;

        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.DeviceRelease((Device*)this.handle);
            }
        }

        return true;
    }
}

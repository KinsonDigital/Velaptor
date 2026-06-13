// <copyright file="SafeDeviceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;
using Silk.NET.WebGPU;

internal class SafeDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeDeviceHandle(WGPUInvoker wgpu, IntPtr handle)
        : base(true)
    {
        this.wgpu = wgpu;

        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.DeviceRelease(this.handle);
        }

        return true;
    }
}

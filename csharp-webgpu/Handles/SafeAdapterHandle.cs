// <copyright file="SafeAdapterHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeAdapterHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeAdapterHandle(WebGPU wgpu, IntPtr handle)
        : base(ownsHandle: true)
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
                this.wgpu.AdapterRelease((Adapter*)this.handle);
            }
        }

        return true;
    }
}

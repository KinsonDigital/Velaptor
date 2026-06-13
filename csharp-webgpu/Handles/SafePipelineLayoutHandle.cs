// <copyright file="SafePipelineLayoutHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;
using Silk.NET.WebGPU;

internal class SafePipelineLayoutHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private WGPUInvoker wgpu;

    public SafePipelineLayoutHandle(WGPUInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.PipelineLayoutRelease(this.handle);
        }

        return true;
    }
}

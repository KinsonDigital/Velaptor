// <copyright file="SafeRenderPipelineHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

internal class SafeRenderPipelineHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeRenderPipelineHandle(WGPUInvoker wgpu, SafeDeviceHandle deviceHandle, ref readonly Silk.NET.WebGPU.RenderPipelineDescriptor pipelineDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateRenderPipeline(deviceHandle, in pipelineDescriptor));
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.RenderPipelineRelease(this.handle);
        }

        return true;
    }
}

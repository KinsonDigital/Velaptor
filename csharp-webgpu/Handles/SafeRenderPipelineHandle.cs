// <copyright file="SafeRenderPipelineHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeRenderPipelineHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeRenderPipelineHandle(WebGPU wgpu, SafeDeviceHandle deviceHandle, RenderPipelineDescriptor pipelineDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)this.wgpu.DeviceCreateRenderPipeline((Device*)deviceHandle.DangerousGetHandle(), in pipelineDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.RenderPipelineRelease((RenderPipeline*)this.handle);
            }
        }

        return true;
    }
}

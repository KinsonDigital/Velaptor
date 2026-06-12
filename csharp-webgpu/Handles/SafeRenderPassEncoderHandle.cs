// <copyright file="SafeRenderPassEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeRenderPassEncoderHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeRenderPassEncoderHandle(WebGPU wgpu, SafeCommandEncoderHandle cmdEncoderHandle, in RenderPassDescriptor renderPassDescriptor)
        : base(ownsHandle: true)
    {
        unsafe
        {
            this.wgpu = wgpu;

            SetHandle((nint)this.wgpu.CommandEncoderBeginRenderPass(
                (CommandEncoder*)cmdEncoderHandle.DangerousGetHandle(),
                in renderPassDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.RenderPassEncoderEnd((RenderPassEncoder*)this.handle);
                this.wgpu.RenderPassEncoderRelease((RenderPassEncoder*)this.handle);
            }
        }

        return true;
    }
}

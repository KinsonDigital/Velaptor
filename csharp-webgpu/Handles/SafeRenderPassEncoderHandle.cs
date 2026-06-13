// <copyright file="SafeRenderPassEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

internal class SafeRenderPassEncoderHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeRenderPassEncoderHandle(WGPUInvoker wgpu, SafeCommandEncoderHandle cmdEncoderHandle, ref readonly Silk.NET.WebGPU.RenderPassDescriptor renderPassDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.CommandEncoderBeginRenderPass(cmdEncoderHandle, in renderPassDescriptor));
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.RenderPassEncoderEnd(this.handle);
            this.wgpu.RenderPassEncoderRelease(this.handle);
        }

        return true;
    }
}

// <copyright file="SafePipelineLayoutHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU pipeline layout.
/// </summary>
internal sealed class SafePipelineLayoutHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafePipelineLayoutHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="pipelineLayoutHandle">The native pipeline layout handle.</param>
    public SafePipelineLayoutHandle(IWgpuInvoker wgpu, nint pipelineLayoutHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(pipelineLayoutHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.PipelineLayoutRelease(this.handle);

        return true;
    }
}

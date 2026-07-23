// <copyright file="SafePipelineLayoutHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;

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
    /// <param name="handle">The native handle.</param>
    public SafePipelineLayoutHandle(IWgpuInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(handle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.PipelineLayoutRelease(this.handle);
        }

        return true;
    }
}

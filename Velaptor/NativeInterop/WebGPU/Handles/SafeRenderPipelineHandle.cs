// <copyright file="SafeRenderPipelineHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGPU.Handles;

using System;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// A safe handle for a WebGPU render pipeline.
/// </summary>
internal sealed class SafeRenderPipelineHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWGPUInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeRenderPipelineHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeRenderPipelineHandle(IWGPUInvoker wgpu, nint handle)
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
            this.wgpu.RenderPipelineRelease(this.handle);
        }

        return true;
    }
}

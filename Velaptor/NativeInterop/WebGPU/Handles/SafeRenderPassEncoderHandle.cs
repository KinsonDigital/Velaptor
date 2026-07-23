// <copyright file="SafeRenderPassEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// A safe handle for a WebGPU render pass encoder.
/// </summary>
internal sealed class SafeRenderPassEncoderHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeRenderPassEncoderHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeRenderPassEncoderHandle(IWgpuInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;

        SetHandle(handle);
    }

    /// <summary>
    /// Ends the render pass, signaling that all draw commands for this pass
    /// are complete. Must be called explicitly before <see cref="SafeHandle.Dispose"/>
    /// to properly finalize the pass on the GPU timeline.
    /// </summary>
    public void End()
    {
        if (!IsInvalid)
        {
            this.wgpu.RenderPassEncoderEnd(this.handle);
        }
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.RenderPassEncoderRelease(this.handle);
        }

        return true;
    }
}

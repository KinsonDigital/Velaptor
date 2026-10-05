// <copyright file="SafeSamplerHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU sampler.
/// </summary>
internal sealed class SafeSamplerHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSamplerHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="samplerHandle">The native handle.</param>
    public SafeSamplerHandle(IWgpuInvoker wgpu, nint samplerHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(samplerHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.SamplerRelease(this.handle);

        return true;
    }
}

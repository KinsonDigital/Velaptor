// <copyright file="SafeBindGroupLayoutHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU bind group layout.
/// </summary>
internal sealed class SafeBindGroupLayoutHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeBindGroupLayoutHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="bindGroupLayoutHandle">The native bind group layout handle.</param>
    public SafeBindGroupLayoutHandle(IWgpuInvoker wgpu, nint bindGroupLayoutHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(bindGroupLayoutHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.BindGroupLayoutRelease(this.handle);

        return true;
    }
}

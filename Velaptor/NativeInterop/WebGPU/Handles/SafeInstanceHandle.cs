// <copyright file="SafeInstanceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGPU.Handles;

using System;
using Microsoft.Win32.SafeHandles;

/// <summary>
/// A safe handle for a WebGPU instance.
/// </summary>
internal sealed class SafeInstanceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWGPUInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeInstanceHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeInstanceHandle(IWGPUInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle(handle);
        }
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.InstanceRelease(this.handle);
        }

        return true;
    }
}

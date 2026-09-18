// <copyright file="SafeDeviceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU device.
/// </summary>
internal sealed class SafeDeviceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeDeviceHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="deviceHandle">The native device handle.</param>
    public SafeDeviceHandle(IWgpuInvoker wgpu, nint deviceHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(deviceHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.DeviceRelease(this.handle);

        return true;
    }
}

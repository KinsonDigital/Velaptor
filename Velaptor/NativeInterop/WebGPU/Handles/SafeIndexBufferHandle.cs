// <copyright file="SafeIndexBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU index buffer allocated on the device.
/// </summary>
internal sealed class SafeIndexBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeIndexBufferHandle"/> class.
    /// Creates a new buffer on the device.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="deviceHandle">The device handle.</param>
    /// <param name="bufferDescriptor">Description of the buffer to create.</param>
    public SafeIndexBufferHandle(
        IWgpuInvoker wgpu,
        SafeDeviceHandle deviceHandle,
        ref readonly Silk.NET.WebGPU.BufferDescriptor bufferDescriptor)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(deviceHandle);

        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateBuffer(deviceHandle, in bufferDescriptor));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeIndexBufferHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeIndexBufferHandle(IWgpuInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        this.wgpu = wgpu;

        SetHandle(handle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.BufferDestroy(this.handle);
        this.wgpu.BufferRelease(this.handle);

        return true;
    }
}

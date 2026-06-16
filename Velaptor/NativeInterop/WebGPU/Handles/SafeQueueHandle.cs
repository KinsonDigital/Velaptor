// <copyright file="SafeQueueHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGPU.Handles;

using Microsoft.Win32.SafeHandles;

/// <summary>
/// A safe handle for a WebGPU queue.
/// </summary>
internal sealed class SafeQueueHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWGPUInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeQueueHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="deviceHandle">The device handle to get the queue from.</param>
    public SafeQueueHandle(IWGPUInvoker wgpu, SafeDeviceHandle deviceHandle)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceGetQueue(deviceHandle));
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.QueueRelease(this.handle);
        }

        return true;
    }
}

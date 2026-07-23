// <copyright file="SafeCommandEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;
using System;

/// <summary>
/// A safe handle for a WebGPU command encoder.
/// </summary>
/// <remarks>
/// Command encoders are short-lived (one per frame) and do not own the native handle in
/// the traditional sense. They are allocated and released each frame.
/// </remarks>
internal sealed class SafeCommandEncoderHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeCommandEncoderHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeCommandEncoderHandle(IWgpuInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(handle);
    }

    /// <summary>
    /// Releases the previous encoder, creates a new one from <paramref name="descriptor"/>,
    /// and stores the new handle.
    /// </summary>
    /// <param name="descriptor">The command encoder creation descriptor.</param>
    public void UpdateHandle(in CommandEncoderDescriptor descriptor)
    {
        if (!IsInvalid)
        {
            this.wgpu.CommandEncoderRelease(this.handle);
        }

        var newHandle = this.wgpu.DeviceCreateCommandEncoder(this.wgpu.Device, in descriptor);
        SetHandle(newHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.CommandEncoderRelease(this.handle);
        }

        return true;
    }
}

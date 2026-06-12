// <copyright file="SafeIndexBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

/// <summary>
/// A safe handle for a WebGPU index buffer allocated on the device.
/// The buffer is created with <c>Index | CopyDst</c> usage so that index
/// data can be uploaded from the CPU with <c>QueueWriteBuffer</c>.
/// </summary>
internal sealed class SafeIndexBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeIndexBufferHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU API instance.</param>
    /// <param name="deviceHandle">The device handle.</param>
    /// <param name="bufferDescriptor">Description of the buffer to create.</param>
    public SafeIndexBufferHandle(WebGPU wgpu, SafeDeviceHandle deviceHandle, in BufferDescriptor bufferDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)wgpu.DeviceCreateBuffer((Device*)deviceHandle.DangerousGetHandle(), in bufferDescriptor));
        }
    }

    /// <summary>
    /// Releases the buffer handle by destroying the buffer and releasing it on the GPU.
    /// </summary>
    /// <returns>True if the handle was released successfully.</returns>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.BufferDestroy((Buffer*)this.handle);
                this.wgpu.BufferRelease((Buffer*)this.handle);
            }
        }

        return true;
    }
}

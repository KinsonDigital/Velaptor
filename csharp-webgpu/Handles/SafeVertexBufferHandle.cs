// <copyright file="SafeVertexBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;

/// <summary>
/// A safe handle for a WebGPU vertex buffer allocated on the device.
/// The buffer is created with <c>Vertex | CopyDst</c> usage so that vertex
/// data can be uploaded from the CPU with <c>QueueWriteBuffer</c>.
/// </summary>
internal sealed class SafeVertexBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeVertexBufferHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="deviceHandle">The device handle.</param>
    /// <param name="bufferDescriptor">Description of the buffer to create.</param>
    public SafeVertexBufferHandle(WGPUInvoker wgpu, SafeDeviceHandle deviceHandle, ref readonly Silk.NET.WebGPU.BufferDescriptor bufferDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateBuffer(deviceHandle, in bufferDescriptor));
    }

    /// <summary>
    /// Releases the buffer handle by destroying the buffer and releasing it on the GPU.
    /// </summary>
    /// <returns>True if the handle was released successfully.</returns>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.BufferDestroy(this.handle);
            this.wgpu.BufferRelease(this.handle);
        }

        return true;
    }
}

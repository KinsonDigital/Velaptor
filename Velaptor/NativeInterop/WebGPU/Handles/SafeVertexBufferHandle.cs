// <copyright file="SafeVertexBufferHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU vertex buffer allocated on the device.
/// The buffer is created with <c>Vertex | CopyDst</c> usage so that vertex
/// data can be uploaded from the CPU with <c>QueueWriteBuffer</c>.
/// </summary>
internal sealed class SafeVertexBufferHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeVertexBufferHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="vertexBufferHandle">The native vertex buffer handle.</param>
    public SafeVertexBufferHandle(IWgpuInvoker wgpu, nint vertexBufferHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        this.wgpu = wgpu;

        SetHandle(vertexBufferHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.BufferDestroy(this.handle);
        this.wgpu.BufferRelease(this.handle);

        return true;
    }
}

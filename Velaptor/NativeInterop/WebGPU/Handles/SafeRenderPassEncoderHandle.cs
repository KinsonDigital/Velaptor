// <copyright file="SafeRenderPassEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.InteropServices;
using Silk.NET.WebGPU;

/// <summary>
/// A safe handle for a WebGPU render pass encoder.
/// </summary>
internal sealed class SafeRenderPassEncoderHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeRenderPassEncoderHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="beginRenderPassHandle">The native begin render pass handle.</param>
    public SafeRenderPassEncoderHandle(IWgpuInvoker wgpu, nint beginRenderPassHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;

        SetHandle(beginRenderPassHandle);
    }

    /// <summary>
    /// Ends the render pass, signaling that all draw commands for this pass
    /// are complete. Must be called explicitly before <see cref="SafeHandle.Dispose"/>
    /// to properly finalize the pass on the GPU timeline.
    /// </summary>
    public void End()
    {
        if (!IsInvalid)
        {
            this.wgpu.RenderPassEncoderEnd(this.handle);
        }
    }

    /// <summary>
    /// Disposes of the current handle and recreates the handle.
    /// </summary>
    /// <param name="encoderHandle">The command encoder to begin the pass on.</param>
    /// <param name="textureViewHandle">The texture view to render into.</param>
    /// <param name="clearValue">The clear color.</param>
    public void ResetHandle(SafeCommandEncoderHandle encoderHandle, SafeTextureViewHandle textureViewHandle, Color clearValue)
    {
        ReleaseHandle();

        var newEncoderHandle = this.wgpu.UnsafeCommandEncoderBeginRenderPass(
            encoderHandle,
            textureViewHandle,
            LoadOp.Clear,
            StoreOp.Store,
            clearValue.R,
            clearValue.G,
            clearValue.B,
            clearValue.A);

        SetHandle(newEncoderHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.RenderPassEncoderRelease(this.handle);

        return true;
    }
}

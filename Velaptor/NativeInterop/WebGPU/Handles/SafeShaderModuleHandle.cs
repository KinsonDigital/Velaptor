// <copyright file="SafeShaderModuleHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;

/// <summary>
/// A safe handle for a WebGPU shader module.
/// </summary>
internal sealed class SafeShaderModuleHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeShaderModuleHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="shaderModuleHandle">The native shader module handle.</param>
    public SafeShaderModuleHandle(IWgpuInvoker wgpu, nint shaderModuleHandle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(shaderModuleHandle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.ShaderModuleRelease(this.handle);

        return true;
    }
}

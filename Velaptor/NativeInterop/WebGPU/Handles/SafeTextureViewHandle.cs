// <copyright file="SafeTextureViewHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using System;
using Silk.NET.WebGPU;

/// <summary>
/// A safe handle for a WebGPU texture view.
/// </summary>
internal sealed class SafeTextureViewHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTextureViewHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="texture">The texture pointer.</param>
    /// <param name="descriptor">The texture view descriptor.</param>
    public SafeTextureViewHandle(IWgpuInvoker wgpu, nint texture, in TextureViewDescriptor descriptor = default)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(this.wgpu.TextureCreateView(texture, in descriptor));
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.TextureViewRelease(this.handle);

        return true;
    }
}

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
    /// <param name="surfaceTextureHandle">The surface texture handle.</param>
    /// <param name="descriptor">The texture view descriptor.</param>
    // TODO: Look into swapping out the nint surfaceTextureHandle with the type 'SafeTextureHandle' type.
    public SafeTextureViewHandle(IWgpuInvoker wgpu, nint surfaceTextureHandle, in TextureViewDescriptor descriptor = default)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(this.wgpu.TextureCreateView(surfaceTextureHandle, in descriptor));
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.TextureViewRelease(this.handle);

        return true;
    }
}

// <copyright file="SafeTextureHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

/// <summary>
/// A safe handle for a WebGPU texture.
/// </summary>
internal sealed class SafeTextureHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTextureHandle"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="deviceHandle">The device handle.</param>
    /// <param name="textureDescriptor">The texture creation descriptor.</param>
    public SafeTextureHandle(IWgpuInvoker wgpu, SafeDeviceHandle deviceHandle, in TextureDescriptor textureDescriptor)
        : base(ownsHandle: true)
    {
        this.wgpu = wgpu;
        SetHandle(this.wgpu.DeviceCreateTexture(deviceHandle, in textureDescriptor));
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.TextureDestroy(this.handle);
            this.wgpu.TextureRelease(this.handle);
        }

        return true;
    }
}

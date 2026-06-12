// <copyright file="SafeBindGroupLayoutHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeBindGroupLayoutHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly GraphicsDevice graphicsDevice;

    public SafeBindGroupLayoutHandle(GraphicsDevice graphicsDevice, IntPtr handle)
        : base(ownsHandle: true)
    {
        this.graphicsDevice = graphicsDevice;

        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.graphicsDevice.Wgpu.BindGroupLayoutRelease((BindGroupLayout*)this.handle);
            }
        }

        return true;
    }
}

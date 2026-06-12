// <copyright file="SafeBindGroupHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeBindGroupHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly GraphicsDevice graphicsDevice;

    public SafeBindGroupHandle(GraphicsDevice graphicsDevice, in BindGroupDescriptor bindGroupDescriptor)
        : base(ownsHandle: true)
    {
        this.graphicsDevice = graphicsDevice;

        unsafe
        {
            SetHandle((nint)graphicsDevice.Wgpu.DeviceCreateBindGroup((Device*)graphicsDevice.Handle.DangerousGetHandle(), in bindGroupDescriptor));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.graphicsDevice.Wgpu.BindGroupRelease((BindGroup*)this.handle);
            }
        }

        return true;
    }
}

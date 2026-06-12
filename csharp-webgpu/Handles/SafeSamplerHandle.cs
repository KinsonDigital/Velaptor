// <copyright file="SafeSamplerHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;

internal class SafeSamplerHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly GraphicsDevice graphicsDevice;

    public SafeSamplerHandle(GraphicsDevice graphicsDevice, in SamplerDescriptor samplerDesc)
        : base(ownsHandle: true)
    {
        unsafe
        {
            this.graphicsDevice = graphicsDevice;
            SetHandle((nint)this.graphicsDevice.Wgpu.DeviceCreateSampler((Device*)this.graphicsDevice.Handle.DangerousGetHandle(), in samplerDesc));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.graphicsDevice.Wgpu.SamplerRelease((Sampler*)this.handle);
            }
        }

        return true;
    }
}

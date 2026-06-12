// <copyright file="SafeSurfaceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

internal class SafeSurfaceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WebGPU wgpu;

    public SafeSurfaceHandle(WebGPU wgpu, IWindow window, SafeInstanceHandle instance)
        : base(true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)window.CreateWebGPUSurface(wgpu, (Instance*)instance.DangerousGetHandle()));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            unsafe
            {
                this.wgpu.SurfaceUnconfigure((Surface*)this.handle);
                this.wgpu.SurfaceRelease((Surface*)this.handle);
            }
        }

        return true;
    }
}

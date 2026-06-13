// <copyright file="SafeSurfaceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Microsoft.Win32.SafeHandles;
using NativeInterop.WebGPU;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

internal class SafeSurfaceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly WGPUInvoker wgpu;

    public SafeSurfaceHandle(WGPUInvoker wgpu, IWindow window, SafeInstanceHandle instance)
        : base(true)
    {
        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)window.CreateWebGPUSurface(wgpu.Wgpu, (Instance*)instance.DangerousGetHandle()));
        }
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            this.wgpu.SurfaceUnconfigure(this.handle);
            this.wgpu.SurfaceRelease(this.handle);
        }

        return true;
    }
}

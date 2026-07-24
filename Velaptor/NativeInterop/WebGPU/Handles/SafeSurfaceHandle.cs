// <copyright file="SafeSurfaceHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Handles;

using System;
using Microsoft.Win32.SafeHandles;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

/// <summary>
/// A safe handle for a WebGPU surface.
/// </summary>
internal sealed class SafeSurfaceHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private readonly IWgpuInvoker wgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceHandle"/> class
    /// from a window, creating the WebGPU surface from the window's native handle.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="window">The window to create the surface from.</param>
    /// <param name="instance">The WebGPU instance.</param>
    public SafeSurfaceHandle(IWgpuInvoker wgpu, IWindow window, SafeInstanceHandle instance)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(instance);

        this.wgpu = wgpu;

        unsafe
        {
            SetHandle((nint)window.CreateWebGPUSurface(wgpu.Wgpu, (Instance*)instance.DangerousGetHandle()));
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceHandle"/> class
    /// from a raw native handle.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    /// <param name="handle">The native handle.</param>
    public SafeSurfaceHandle(IWgpuInvoker wgpu, nint handle)
        : base(ownsHandle: true)
    {
        ArgumentNullException.ThrowIfNull(wgpu);

        this.wgpu = wgpu;
        SetHandle(handle);
    }

    /// <inheritdoc/>
    protected override bool ReleaseHandle()
    {
        this.wgpu.SurfaceUnconfigure(this.handle);
        this.wgpu.SurfaceRelease(this.handle);

        return true;
    }
}

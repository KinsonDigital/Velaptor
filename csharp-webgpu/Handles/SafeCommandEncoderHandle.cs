// <copyright file="SafeCommandEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using NativeInterop.WebGPU;

internal class SafeCommandEncoderHandle : IDisposable
{
    private readonly WGPUInvoker wgpu;
    private readonly SafeDeviceHandle deviceHandle;
    private nint handle;

    public SafeCommandEncoderHandle(WGPUInvoker wgpu, SafeDeviceHandle deviceHandle, ref readonly Silk.NET.WebGPU.CommandEncoderDescriptor commandEncoderDescriptor)
    {
        this.wgpu = wgpu;
        this.deviceHandle = deviceHandle;
        SetHandle(in commandEncoderDescriptor);
    }

    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    public nint DangerousGetHandle() => this.handle;

    public void UpdateHandle(ref readonly Silk.NET.WebGPU.CommandEncoderDescriptor commandEncoderDescriptor) => SetHandle(in commandEncoderDescriptor);

    public void Dispose() => Dispose(disposing: true);

    private void Dispose(bool disposing)
    {
        if (IsInvalid)
        {
            return;
        }

        this.wgpu.CommandEncoderRelease(this.handle);
        this.handle = IntPtr.Zero;
    }

    private void SetHandle(ref readonly Silk.NET.WebGPU.CommandEncoderDescriptor commandEncoderDescriptor)
    {
        this.handle = this.wgpu.DeviceCreateCommandEncoder(this.deviceHandle, in commandEncoderDescriptor);
    }
}

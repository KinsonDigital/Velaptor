// <copyright file="SafeCommandEncoderHandle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu.Handles;

using Silk.NET.WebGPU;

internal class SafeCommandEncoderHandle : IDisposable
{
    private readonly GraphicsDevice graphicsDevice;
    private nint handle;

    public SafeCommandEncoderHandle(GraphicsDevice graphicsDevice, in CommandEncoderDescriptor commandEncoderDescriptor)
    {
        this.graphicsDevice = graphicsDevice;

        SetHandle(in commandEncoderDescriptor);
    }

    public bool IsInvalid => this.handle == IntPtr.Zero || this.handle == new nint(-1);

    public nint DangerousGetHandle() => this.handle;

    public void UpdateHandle(in CommandEncoderDescriptor commandEncoderDescriptor) => SetHandle(in commandEncoderDescriptor);

    public void Dispose() => Dispose(disposing: true);

    private void Dispose(bool disposing)
    {
        if (IsInvalid)
        {
            return;
        }

        unsafe
        {
            this.graphicsDevice.Wgpu.CommandEncoderRelease((CommandEncoder*)this.handle);
        }

        this.handle = IntPtr.Zero;
    }

    private void SetHandle(in CommandEncoderDescriptor commandEncoderDescriptor)
    {
        unsafe
        {
            this.handle = (nint)this.graphicsDevice.Wgpu.DeviceCreateCommandEncoder(
                (Device*)this.graphicsDevice.Handle.DangerousGetHandle(),
                in commandEncoderDescriptor);
        }
    }
}

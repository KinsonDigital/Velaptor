// <copyright file="SafeIndexBufferHandleTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.WebGpu.Handles;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Xunit;

/// <summary>
/// Tests the <see cref="SafeIndexBufferHandle"/> class.
/// </summary>
public sealed class SafeIndexBufferHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeIndexBufferHandleTests"/> class.
    /// </summary>
    public SafeIndexBufferHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(BufferDescriptor);

        // Act
        var act = () => new SafeIndexBufferHandle(null, deviceHandle, ref descriptor);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullDeviceHandleParam_ThrowsException()
    {
        // Arrange
        var descriptor = default(BufferDescriptor);

        // Act
        var act = () => new SafeIndexBufferHandle(this.mockWgpu, null, ref descriptor);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'deviceHandle')");
    }

    [Fact]
    public void Ctor_NintOverload_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeIndexBufferHandle(null, Handle);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithValidHandle_SetsHandle()
    {
        // Arrange & Act
        var sut = new SafeIndexBufferHandle(this.mockWgpu, Handle);

        // Assert
        sut.DangerousGetHandle().ShouldBe(Handle);
    }

    [Fact]
    public void Ctor_WithInvalidHandle_SetsInvalidHandle()
    {
        // Arrange & Act
        var sut = new SafeIndexBufferHandle(this.mockWgpu, nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }

    [Fact]
    public void Ctor_WithDeviceAndDescriptor_CallsDeviceCreateBuffer()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(BufferDescriptor);
        this.mockWgpu.DeviceCreateBuffer(deviceHandle, in descriptor).Returns(Handle);

        // Act
        var sut = new SafeIndexBufferHandle(this.mockWgpu, deviceHandle, ref descriptor);

        // Assert
        this.mockWgpu.Received(1).DeviceCreateBuffer(deviceHandle, in descriptor);
        sut.DangerousGetHandle().ShouldBe(Handle);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_DestroysAndReleasesHandle()
    {
        // Arrange
        var sut = new SafeIndexBufferHandle(this.mockWgpu, Handle);

        // Act
        sut.Dispose();

        // Assert
        Received.InOrder(() =>
        {
            this.mockWgpu.BufferDestroy(Handle);
            this.mockWgpu.BufferRelease(Handle);
        });
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotDestroyOrReleaseHandle()
    {
        // Arrange
        var sut = new SafeIndexBufferHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().BufferDestroy(Arg.Any<nint>());
        this.mockWgpu.DidNotReceive().BufferRelease(Arg.Any<nint>());
    }

    #endregion

    /// <summary>
    /// Creates a <see cref="SafeDeviceHandle"/> for use in tests.
    /// </summary>
    /// <returns>The device handle.</returns>
    private SafeDeviceHandle CreateDeviceHandle() => new (this.mockWgpu, 0xABCD);
}

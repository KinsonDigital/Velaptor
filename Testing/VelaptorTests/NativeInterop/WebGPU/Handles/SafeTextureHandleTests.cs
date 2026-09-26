// <copyright file="SafeTextureHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeTextureHandle"/> class.
/// </summary>
public sealed class SafeTextureHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTextureHandleTests"/> class.
    /// </summary>
    public SafeTextureHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);

        // Act
        var act = () => new SafeTextureHandle(null, deviceHandle, in descriptor);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullDeviceHandleParam_ThrowsException()
    {
        // Arrange
        var textureDescriptor = default(TextureDescriptor);

        // Act
        var act = () => new SafeTextureHandle(this.mockWgpu, null, textureDescriptor);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'deviceHandle')");
    }

    [Fact]
    public void Ctor_WhenInvoked_CallsDeviceCreateTexture()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);
        this.mockWgpu.DeviceCreateTexture(deviceHandle, in descriptor).Returns(Handle);

        // Act
        var sut = new SafeTextureHandle(this.mockWgpu, deviceHandle, in descriptor);

        // Assert
        this.mockWgpu.Received(1).DeviceCreateTexture(deviceHandle, in descriptor);
        sut.DangerousGetHandle().ShouldBe(Handle);
    }

    [Fact]
    public void Ctor_WhenDeviceCreateTextureReturnsInvalidHandle_SetsInvalidHandle()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);
        this.mockWgpu.DeviceCreateTexture(deviceHandle, in descriptor).Returns(nint.Zero);

        // Act
        var sut = new SafeTextureHandle(this.mockWgpu, deviceHandle, in descriptor);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_DestroysAndReleasesHandle()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);
        this.mockWgpu.DeviceCreateTexture(deviceHandle, in descriptor).Returns(Handle);
        var sut = new SafeTextureHandle(this.mockWgpu, deviceHandle, in descriptor);

        // Act
        sut.Dispose();

        // Assert
        Received.InOrder(() =>
        {
            this.mockWgpu.TextureDestroy(Handle);
            this.mockWgpu.TextureRelease(Handle);
        });
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotDestroyOrReleaseHandle()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);
        this.mockWgpu.DeviceCreateTexture(deviceHandle, in descriptor).Returns(nint.Zero);
        var sut = new SafeTextureHandle(this.mockWgpu, deviceHandle, in descriptor);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().TextureDestroy(Arg.Any<nint>());
        this.mockWgpu.DidNotReceive().TextureRelease(Arg.Any<nint>());
    }

    [Fact]
    public void ReleaseHandle_WithValidHandle_ReturnsTrue()
    {
        // Arrange
        var deviceHandle = CreateDeviceHandle();
        var descriptor = default(TextureDescriptor);
        this.mockWgpu.DeviceCreateTexture(deviceHandle, in descriptor).Returns(Handle);
        var sut = new SafeTextureHandle(this.mockWgpu, deviceHandle, in descriptor);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).TextureDestroy(Handle);
    }

    #endregion

    /// <summary>
    /// Creates a <see cref="SafeDeviceHandle"/> for use in tests.
    /// </summary>
    /// <returns>The device handle.</returns>
    private SafeDeviceHandle CreateDeviceHandle() => new (this.mockWgpu, 0xABCD);
}

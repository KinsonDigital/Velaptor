// <copyright file="SafeQueueHandleTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.WebGpu.Handles;

using System;
using NSubstitute;
using Shouldly;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Xunit;

/// <summary>
/// Tests the <see cref="SafeQueueHandle"/> class.
/// </summary>
public sealed class SafeQueueHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;
    private readonly SafeDeviceHandle deviceHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeQueueHandleTests"/> class.
    /// </summary>
    public SafeQueueHandleTests()
    {
        this.mockWgpu = Substitute.For<IWgpuInvoker>();
        this.deviceHandle = new SafeDeviceHandle(this.mockWgpu, 0x5678);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange
        var device = new SafeDeviceHandle(Substitute.For<IWgpuInvoker>(), 0x5678);

        // Arrange & Act
        var act = () => new SafeQueueHandle(null, device);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullDeviceHandleParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeQueueHandle(this.mockWgpu, null);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'deviceHandle')");
    }

    [Fact]
    public void Ctor_WhenInvoked_CallsDeviceGetQueue()
    {
        // Arrange
        this.mockWgpu.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(Handle);

        // Act
        var sut = new SafeQueueHandle(this.mockWgpu, this.deviceHandle);

        // Assert
        this.mockWgpu.Received(1).DeviceGetQueue(this.deviceHandle);
        sut.DangerousGetHandle().ShouldBe(Handle);
    }

    [Fact]
    public void Ctor_WhenDeviceGetQueueReturnsInvalidHandle_SetsInvalidHandle()
    {
        // Arrange
        this.mockWgpu.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(nint.Zero);

        // Act
        var sut = new SafeQueueHandle(this.mockWgpu, this.deviceHandle);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        this.mockWgpu.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(Handle);
        var sut = new SafeQueueHandle(this.mockWgpu, this.deviceHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).QueueRelease(Handle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        this.mockWgpu.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(nint.Zero);
        var sut = new SafeQueueHandle(this.mockWgpu, this.deviceHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().QueueRelease(Arg.Any<nint>());
    }
    #endregion
}

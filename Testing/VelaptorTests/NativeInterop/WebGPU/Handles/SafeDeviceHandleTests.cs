// <copyright file="SafeDeviceHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeDeviceHandle"/> class.
/// </summary>
public sealed class SafeDeviceHandleTests
{
    private readonly IWgpuInvoker mockWgpu;
    private readonly nint validHandle = 0x1234;
    private readonly nint invalidHandle = nint.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeDeviceHandleTests"/> class.
    /// </summary>
    public SafeDeviceHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeDeviceHandle(null, this.validHandle);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithValidHandle_SetsHandle()
    {
        // Act
        var sut = CreateHandle();

        // Assert
        sut.IsInvalid.ShouldBeFalse();
        sut.DangerousGetHandle().ShouldBe(this.validHandle);
    }

    [Fact]
    public void Ctor_WithInvalidHandle_SetsInvalidHandle()
    {
        // Act
        var sut = new SafeDeviceHandle(this.mockWgpu, this.invalidHandle);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        var sut = CreateHandle();

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).DeviceRelease(this.validHandle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        var sut = new SafeDeviceHandle(this.mockWgpu, this.invalidHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().DeviceRelease(Arg.Any<nint>());
    }
    #endregion

    /// <summary>
    /// Creates a new <see cref="SafeDeviceHandle"/> for testing.
    /// </summary>
    /// <returns>The handle to test.</returns>
    private SafeDeviceHandle CreateHandle() => new (this.mockWgpu, this.validHandle);
}

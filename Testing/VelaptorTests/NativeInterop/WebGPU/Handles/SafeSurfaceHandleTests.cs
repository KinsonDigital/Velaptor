// <copyright file="SafeSurfaceHandleTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.WebGpu.Handles;

using System;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Shouldly;
using Silk.NET.Windowing;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Xunit;

/// <summary>
/// Tests the <see cref="SafeSurfaceHandle"/> class.
/// </summary>
public sealed class SafeSurfaceHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceHandleTests"/> class.
    /// </summary>
    public SafeSurfaceHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeSurfaceHandle(null, nint.Zero);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Arrange
        var instance = new SafeInstanceHandle(this.mockWgpu, 0xABCD);

        // Act
        var act = () => new SafeSurfaceHandle(this.mockWgpu, null, instance);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'window')");
    }

    [Fact]
    public void Ctor_WithNullInstanceParam_ThrowsException()
    {
        // Arrange
        var mockWindow = Substitute.For<IWindow>();

        // Act
        var act = () => new SafeSurfaceHandle(this.mockWgpu, mockWindow, null);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'instance')");
    }

    [Fact]
    public void Ctor_WithWindowOverload_WhenInvoked_CreatesSurface()
    {
        // Arrange & Act
        var sut = new SafeSurfaceHandle(this.mockWgpu, 0xABCD);

        // Assert
        sut.DangerousGetHandle().ShouldBe(0xABCD);
    }

    [Fact]
    public void Ctor_WithNintOverload_WithValidHandle_SetsHandle()
    {
        // Arrange & Act
        var sut = new SafeSurfaceHandle(this.mockWgpu, Handle);

        // Assert
        sut.DangerousGetHandle().ShouldBe(Handle);
    }

    [Fact]
    public void Ctor_WithNintOverload_WithInvalidHandle_SetsInvalidHandle()
    {
        // Arrange & Act
        var sut = new SafeSurfaceHandle(this.mockWgpu, nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_UnconfiguresAndReleasesHandle()
    {
        // Arrange
        var sut = new SafeSurfaceHandle(this.mockWgpu, Handle);

        // Act
        sut.Dispose();

        // Assert
        Received.InOrder(() =>
        {
            this.mockWgpu.SurfaceUnconfigure(Handle);
            this.mockWgpu.SurfaceRelease(Handle);
        });
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotUnconfigureOrReleaseHandle()
    {
        // Arrange
        var sut = new SafeSurfaceHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().SurfaceUnconfigure(Arg.Any<nint>());
        this.mockWgpu.DidNotReceive().SurfaceRelease(Arg.Any<nint>());
    }

    [Fact]
    public void ReleaseHandle_WithValidHandle_ReturnsTrue()
    {
        // Arrange
        var sut = new SafeSurfaceHandle(this.mockWgpu, Handle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).SurfaceUnconfigure(Handle);
    }

    #endregion
}

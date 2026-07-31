// <copyright file="SafeSurfaceHandleTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.NativeInterop.WebGpu.Handles;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Xunit;

/// <summary>
/// Tests the <see cref="SafeSurfaceHandle"/> class.
/// </summary>
public sealed class SafeSurfaceHandleTests
{
    private const nint UnsafeInstanceHandle = 0x10;
    private const nint UnsafeSurfaceHandle = 0x11;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IWindow mockWindow;
    private readonly SafeInstanceHandle instanceHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceHandleTests"/> class.
    /// </summary>
    public SafeSurfaceHandleTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockWgpuInvoker.CreateWebGpuSurface(Arg.Any<WebGPU>(), Arg.Any<IWindow>(), Arg.Any<SafeInstanceHandle>())
            .Returns(UnsafeSurfaceHandle);

        this.mockWindow = Substitute.For<IWindow>();
        this.instanceHandle = new SafeInstanceHandle(this.mockWgpuInvoker, UnsafeInstanceHandle);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Arrange
        var instance = new SafeInstanceHandle(this.mockWgpuInvoker, 0xABCD);

        // Act
        var act = () => new SafeSurfaceHandle(this.mockWgpuInvoker, null, instance);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'window')");
    }

    [Fact]
    public void Ctor_WithNullInstanceHandleParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeSurfaceHandle(this.mockWgpuInvoker, this.mockWindow, null);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'instanceHandle')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_UnconfiguresAndReleasesHandle()
    {
        // Arrange
        var sut = new SafeSurfaceHandle(this.mockWgpuInvoker, this.mockWindow, this.instanceHandle);

        // Act
        sut.Dispose();

        // Assert
        Received.InOrder(() =>
        {
            this.mockWgpuInvoker.SurfaceUnconfigure(UnsafeSurfaceHandle);
            this.mockWgpuInvoker.SurfaceRelease(UnsafeSurfaceHandle);
        });
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotUnconfigureOrReleaseHandle()
    {
        // Arrange
        this.mockWgpuInvoker.CreateWebGpuSurface(Arg.Any<WebGPU>(), Arg.Any<IWindow>(), Arg.Any<SafeInstanceHandle>())
            .Returns(0x0);
        var sut = new SafeSurfaceHandle(this.mockWgpuInvoker, this.mockWindow, this.instanceHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.DidNotReceive().SurfaceUnconfigure(Arg.Any<nint>());
        this.mockWgpuInvoker.DidNotReceive().SurfaceRelease(Arg.Any<nint>());
    }

    [Fact]
    public void ReleaseHandle_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        var sut = new SafeSurfaceHandle(this.mockWgpuInvoker, this.mockWindow, this.instanceHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).SurfaceUnconfigure(UnsafeSurfaceHandle);
    }
    #endregion
}

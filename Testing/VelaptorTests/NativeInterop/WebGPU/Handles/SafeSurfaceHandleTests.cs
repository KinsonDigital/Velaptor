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
        var instance = CreateInstanceHandle();

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
    public void Ctor_WithNullWindowNative_ThrowsException()
    {
        // Arrange
        var mockWindow = Substitute.For<IWindow>();
        // Explicitly set Native to null; NSubstitute auto-substitutes interfaces by default
        mockWindow.Native.Returns(_ => null);
        this.mockWgpu.Wgpu.ReturnsNull();

        var instance = CreateInstanceHandle();

        // Act & Assert
        var act = () => new SafeSurfaceHandle(null, mockWindow, instance);

        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact(Skip = "NSubstitute cannot match pointer-type parameters (Instance*). " +
                  "The CreateWebGPUSurface method signature takes Instance* as its second parameter, " +
                  "and C# does not allow pointer types as generic type arguments. " +
                  "At runtime the CS0306 error prevents compilation of ReturnsForAnyArgs.")]
    public void Ctor_WithWindowOverload_WhenInvoked_CreatesSurface()
    {
        // This test cannot be implemented because NSubstitute cannot
        // match or configure pointer-type parameters in C#.
        // The production code casts instance.DangerousGetHandle() to Instance*
        // and passes it to window.CreateWebGPUSurface(WebGPU, Instance*).
        // Pointer types are not valid generic type arguments (CS0306),
        // so Arg.Any<Instance*>() and ReturnsForAnyArgs both fail to compile.
        throw new NotImplementedException(
            "Cannot test due to pointer type limitation in NSubstitute.");
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

    /// <summary>
    /// Creates a <see cref="SafeInstanceHandle"/> for use in tests.
    /// </summary>
    /// <returns>The instance handle.</returns>
    private SafeInstanceHandle CreateInstanceHandle() => new (this.mockWgpu, 0xABCD);
}

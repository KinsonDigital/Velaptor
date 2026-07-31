// <copyright file="SafeVertexBufferHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeVertexBufferHandle"/> class.
/// </summary>
public sealed class SafeVertexBufferHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeVertexBufferHandleTests"/> class.
    /// </summary>
    public SafeVertexBufferHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_NintOverload_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeVertexBufferHandle(null, Handle);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithValidHandle_SetsHandle()
    {
        // Arrange & Act
        var sut = new SafeVertexBufferHandle(this.mockWgpu, Handle);

        // Assert
        sut.DangerousGetHandle().ShouldBe(Handle);
    }

    [Fact]
    public void Ctor_WithInvalidHandle_SetsInvalidHandle()
    {
        // Arrange & Act
        var sut = new SafeVertexBufferHandle(this.mockWgpu, nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_DestroysAndReleasesHandle()
    {
        // Arrange
        var sut = new SafeVertexBufferHandle(this.mockWgpu, Handle);

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
        var sut = new SafeVertexBufferHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().BufferDestroy(Arg.Any<nint>());
        this.mockWgpu.DidNotReceive().BufferRelease(Arg.Any<nint>());
    }

    #endregion
}

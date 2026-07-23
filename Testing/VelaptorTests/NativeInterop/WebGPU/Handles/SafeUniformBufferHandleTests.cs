// <copyright file="SafeUniformBufferHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeUniformBufferHandle"/> class.
/// </summary>
public sealed class SafeUniformBufferHandleTests
{
    private readonly IWgpuInvoker mockWgpu;
    private readonly nint validHandle = 0x1234;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeUniformBufferHandleTests"/> class.
    /// </summary>
    public SafeUniformBufferHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeUniformBufferHandle(null, this.validHandle);

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
        var sut = new SafeUniformBufferHandle(this.mockWgpu, nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_DestroysAndReleasesHandle()
    {
        // Arrange
        var sut = CreateHandle();

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).BufferDestroy(this.validHandle);
        this.mockWgpu.Received(1).BufferRelease(this.validHandle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotDestroyOrReleaseHandle()
    {
        // Arrange
        var sut = new SafeUniformBufferHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().BufferDestroy(Arg.Any<nint>());
        this.mockWgpu.DidNotReceive().BufferRelease(Arg.Any<nint>());
    }

    [Fact]
    public void ReleaseHandle_WithValidHandle_ReturnsTrue()
    {
        // Arrange
        var sut = CreateHandle();

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).BufferDestroy(this.validHandle);
    }

    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SafeUniformBufferHandle"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SafeUniformBufferHandle CreateHandle() => new (this.mockWgpu, this.validHandle);
}

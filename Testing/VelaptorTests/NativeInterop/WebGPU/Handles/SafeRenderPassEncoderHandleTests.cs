// <copyright file="SafeRenderPassEncoderHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeRenderPassEncoderHandle"/> class.
/// </summary>
public sealed class SafeRenderPassEncoderHandleTests
{
    private readonly IWgpuInvoker mockWgpu;
    private readonly nint validHandle = 0x1234;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeRenderPassEncoderHandleTests"/> class.
    /// </summary>
    public SafeRenderPassEncoderHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeRenderPassEncoderHandle(null, this.validHandle);

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
        var sut = new SafeRenderPassEncoderHandle(this.mockWgpu, nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void End_WithValidHandle_CallsRenderPassEncoderEnd()
    {
        // Arrange
        var sut = CreateHandle();

        // Act
        sut.End();

        // Assert
        this.mockWgpu.Received(1).RenderPassEncoderEnd(this.validHandle);
    }

    [Fact]
    public void End_WithInvalidHandle_DoesNotCallRenderPassEncoderEnd()
    {
        // Arrange
        var sut = new SafeRenderPassEncoderHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.End();

        // Assert
        this.mockWgpu.DidNotReceive().RenderPassEncoderEnd(Arg.Any<nint>());
    }

    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        var sut = CreateHandle();

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).RenderPassEncoderRelease(this.validHandle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        var sut = new SafeRenderPassEncoderHandle(this.mockWgpu, nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().RenderPassEncoderRelease(Arg.Any<nint>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SafeRenderPassEncoderHandle"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private SafeRenderPassEncoderHandle CreateHandle() => new (this.mockWgpu, this.validHandle);
}

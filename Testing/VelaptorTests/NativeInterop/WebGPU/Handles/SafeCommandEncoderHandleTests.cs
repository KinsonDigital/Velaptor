// <copyright file="SafeCommandEncoderHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeCommandEncoderHandle"/> class.
/// </summary>
public sealed class SafeCommandEncoderHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeCommandEncoderHandleTests"/> class.
    /// </summary>
    public SafeCommandEncoderHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeCommandEncoderHandle(null, Handle);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithValidHandle_SetsHandle()
    {
        // Arrange
        var expectedHandle = Handle;

        // Act
        var sut = CreateCommandEncoderHandle(expectedHandle);

        // Assert
        sut.DangerousGetHandle().ShouldBe(expectedHandle);
    }

    [Fact]
    public void Ctor_WithInvalidHandle_SetsInvalidHandle()
    {
        // Arrange & Act
        var sut = CreateCommandEncoderHandle(nint.Zero);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        var sut = CreateCommandEncoderHandle(Handle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).CommandEncoderRelease(Handle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        var sut = CreateCommandEncoderHandle(nint.Zero);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().CommandEncoderRelease(Arg.Any<nint>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SafeCommandEncoderHandle"/> for the purpose of testing.
    /// </summary>
    /// <param name="handle">The native handle.</param>
    /// <returns>The instance to test.</returns>
    private SafeCommandEncoderHandle CreateCommandEncoderHandle(nint handle)
        => new (this.mockWgpu, handle);
}

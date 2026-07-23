// <copyright file="SafeTextureViewHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeTextureViewHandle"/> class.
/// </summary>
public sealed class SafeTextureViewHandleTests
{
    private readonly IWgpuInvoker mockWgpu;
    private readonly nint texture = 0x7777;
    private readonly nint validHandle = 0x1234;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTextureViewHandleTests"/> class.
    /// </summary>
    public SafeTextureViewHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeTextureViewHandle(null, this.texture);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WhenInvoked_CallsTextureCreateView()
    {
        // Arrange
        this.mockWgpu.TextureCreateView(Arg.Any<nint>(), Arg.Any<TextureViewDescriptor>()).Returns(this.validHandle);

        // Act
        var sut = new SafeTextureViewHandle(this.mockWgpu, this.texture);

        // Assert
        this.mockWgpu.Received(1).TextureCreateView(this.texture, Arg.Any<TextureViewDescriptor>());
        sut.DangerousGetHandle().ShouldBe(this.validHandle);
    }

    [Fact]
    public void Ctor_WhenTextureCreateViewReturnsInvalidHandle_SetsInvalidHandle()
    {
        // Arrange
        this.mockWgpu.TextureCreateView(Arg.Any<nint>(), Arg.Any<TextureViewDescriptor>()).Returns(nint.Zero);

        // Act
        var sut = new SafeTextureViewHandle(this.mockWgpu, this.texture);

        // Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        this.mockWgpu.TextureCreateView(Arg.Any<nint>(), Arg.Any<TextureViewDescriptor>()).Returns(this.validHandle);
        var sut = new SafeTextureViewHandle(this.mockWgpu, this.texture);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).TextureViewRelease(this.validHandle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        this.mockWgpu.TextureCreateView(Arg.Any<nint>(), Arg.Any<TextureViewDescriptor>()).Returns(nint.Zero);
        var sut = new SafeTextureViewHandle(this.mockWgpu, this.texture);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().TextureViewRelease(Arg.Any<nint>());
    }
    #endregion
}

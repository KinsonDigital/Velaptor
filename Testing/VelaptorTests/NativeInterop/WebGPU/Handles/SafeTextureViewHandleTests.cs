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
    private const nint UnsafeDeviceHandle = 0x1;
    private const nint UnsafeTextureHandle = 0x2;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly SafeTextureHandle textureHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeTextureViewHandleTests"/> class.
    /// </summary>
    public SafeTextureViewHandleTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeTextureHandle>(), Arg.Any<TextureViewDescriptor>())
            .Returns(UnsafeTextureHandle);

        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);
        this.textureHandle = new SafeTextureHandle(this.mockWgpuInvoker, deviceHandle, default);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeTextureViewHandle(null,  this.textureHandle);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WhenInvoked_CallsTextureCreateView()
    {
        // Arrange & Act
        var sut = new SafeTextureViewHandle(this.mockWgpuInvoker, this.textureHandle);

        // Assert
        this.mockWgpuInvoker.Received(1).TextureCreateView(this.textureHandle, Arg.Any<TextureViewDescriptor>());
        sut.DangerousGetHandle().ShouldBe(UnsafeTextureHandle);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Dispose_WithValidHandle_ReleasesHandle()
    {
        // Arrange
        var sut = new SafeTextureViewHandle(this.mockWgpuInvoker, this.textureHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).TextureViewRelease(UnsafeTextureHandle);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseHandle()
    {
        // Arrange
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeTextureHandle>(), Arg.Any<TextureViewDescriptor>()).Returns(nint.Zero);
        var sut = new SafeTextureViewHandle(this.mockWgpuInvoker, this.textureHandle);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.DidNotReceive().TextureViewRelease(Arg.Any<nint>());
    }
    #endregion
}

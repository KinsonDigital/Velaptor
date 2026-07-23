// <copyright file="SafeSurfaceTextureHandleTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="SafeSurfaceTextureHandle"/> class.
/// </summary>
public sealed class SafeSurfaceTextureHandleTests
{
    private const nint Handle = 0x1234;
    private readonly IWgpuInvoker mockWgpu;

    /// <summary>
    /// Initializes a new instance of the <see cref="SafeSurfaceTextureHandleTests"/> class.
    /// </summary>
    public SafeSurfaceTextureHandleTests() => this.mockWgpu = Substitute.For<IWgpuInvoker>();

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsHandleAndStatus()
    {
        // Arrange
        var texturePointer = Handle;
        var status = SurfaceGetCurrentTextureStatus.Success;

        // Act
        var sut = CreateSurfaceTextureHandle(texturePointer, status);

        // Assert
        sut.DangerousGetHandle().ShouldBe(texturePointer);
        sut.SurfaceTextureStatus.ShouldBe(status);
    }

    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new SafeSurfaceTextureHandle(null, Handle, SurfaceGetCurrentTextureStatus.Success);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }
    #endregion

    #region Property Tests
    [Fact]
    public void IsInvalid_WithValidHandle_ReturnsFalse()
    {
        // Arrange
        var sut = CreateSurfaceTextureHandle(Handle, SurfaceGetCurrentTextureStatus.Success);

        // Act & Assert
        sut.IsInvalid.ShouldBeFalse();
    }

    [Fact]
    public void IsInvalid_WithZeroHandle_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSurfaceTextureHandle(nint.Zero, SurfaceGetCurrentTextureStatus.Success);

        // Act & Assert
        sut.IsInvalid.ShouldBeTrue();
    }

    [Fact]
    public void IsInvalid_WithMinusOneHandle_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSurfaceTextureHandle(new nint(-1), SurfaceGetCurrentTextureStatus.Success);

        // Act & Assert
        sut.IsInvalid.ShouldBeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void UpdateHandleAndStatus_WithValidCurrentHandle_ReleasesOldAndUpdates()
    {
        // Arrange
        var oldHandle = Handle;
        var newHandle = (nint)0x9999;
        var oldStatus = SurfaceGetCurrentTextureStatus.Success;
        var newStatus = SurfaceGetCurrentTextureStatus.Timeout;
        var sut = CreateSurfaceTextureHandle(oldHandle, oldStatus);

        // Act
        sut.UpdateHandleAndStatus(newHandle, newStatus);

        // Assert
        this.mockWgpu.Received(1).TextureRelease(oldHandle);
        sut.DangerousGetHandle().ShouldBe(newHandle);
        sut.SurfaceTextureStatus.ShouldBe(newStatus);
    }

    [Fact]
    public void UpdateHandleAndStatus_WithInvalidCurrentHandle_SkipsReleaseAndUpdates()
    {
        // Arrange
        var newHandle = (nint)0x9999;
        var oldStatus = SurfaceGetCurrentTextureStatus.Success;
        var newStatus = SurfaceGetCurrentTextureStatus.Timeout;
        var sut = CreateSurfaceTextureHandle(nint.Zero, oldStatus);

        // Act
        sut.UpdateHandleAndStatus(newHandle, newStatus);

        // Assert
        this.mockWgpu.DidNotReceive().TextureRelease(Arg.Any<nint>());
        sut.DangerousGetHandle().ShouldBe(newHandle);
        sut.SurfaceTextureStatus.ShouldBe(newStatus);
    }

    [Fact]
    public void Dispose_WithValidHandle_ReleasesTextureAndZeroesHandle()
    {
        // Arrange
        var handle = Handle;
        var sut = CreateSurfaceTextureHandle(handle, SurfaceGetCurrentTextureStatus.Success);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).TextureRelease(handle);
        sut.DangerousGetHandle().ShouldBe(IntPtr.Zero);
    }

    [Fact]
    public void Dispose_WithInvalidHandle_DoesNotReleaseTexture()
    {
        // Arrange
        var sut = CreateSurfaceTextureHandle(nint.Zero, SurfaceGetCurrentTextureStatus.Success);

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.DidNotReceive().TextureRelease(Arg.Any<nint>());
    }

    [Fact]
    public void Dispose_WhenCalledTwice_OnlyReleasesOnce()
    {
        // Arrange
        var handle = Handle;
        var sut = CreateSurfaceTextureHandle(handle, SurfaceGetCurrentTextureStatus.Success);

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).TextureRelease(handle);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="SafeSurfaceTextureHandle"/> for the purpose of testing.
    /// </summary>
    /// <param name="texturePointer">The native texture pointer.</param>
    /// <param name="surfaceTextureStatus">The surface texture status.</param>
    /// <returns>The instance to test.</returns>
    private SafeSurfaceTextureHandle CreateSurfaceTextureHandle(nint texturePointer, SurfaceGetCurrentTextureStatus surfaceTextureStatus)
        => new (this.mockWgpu, texturePointer, surfaceTextureStatus);
}

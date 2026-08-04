﻿// <copyright file="FrameTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.WebGpu;
using Xunit;
using Color = System.Drawing.Color;

/// <summary>
/// Tests the <see cref="Frame"/> class.
/// </summary>
public class FrameTests
{
    private const nint UnsafeDeviceHandle = 0x11;
    private const nint UnsafeInstanceHandle = 0x33;
    private const nint UnsafeTextureHandle = 0x44;
    private const nint UnsafeTextureViewHandle = 0x55;
    private const nint UnsafeCmdEncoderHandle = 0x66;
    private const nint UnsafeRenderPassHandle = 0x67;
    private readonly SafeDeviceHandle deviceHandle;
    private readonly SafeSurfaceHandle surfaceHandle;
    private readonly SafeCommandEncoderHandle cmdEncoderHandle;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsDevice mockDevice;
    private readonly IGraphicsSurface mockSurface;
    private readonly Color testColor = Color.FromArgb(11, 22, 33, 44);
    private SafeSurfaceTextureHandle surfaceTextureHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameTests"/> class.
    /// </summary>
    public FrameTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        this.cmdEncoderHandle = new SafeCommandEncoderHandle(this.mockWgpuInvoker, UnsafeCmdEncoderHandle);
        this.mockWgpuInvoker.DeviceCreateCommandEncoder(Arg.Any<SafeDeviceHandle>(), Arg.Any<CommandEncoderDescriptor>())
            .Returns(this.cmdEncoderHandle);
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeSurfaceTextureHandle>(), Arg.Any<TextureViewDescriptor>())
            .Returns(UnsafeTextureViewHandle);

        this.deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);

        this.mockDevice = Substitute.For<IGraphicsDevice>();
        this.mockDevice.Wgpu.Returns(this.mockWgpuInvoker);
        this.mockDevice.Handle.Returns(this.deviceHandle);

        var mockWindow = Substitute.For<IWindow>();

        var instanceHandle = new SafeInstanceHandle(this.mockWgpuInvoker, UnsafeInstanceHandle);
        this.surfaceHandle = new SafeSurfaceHandle(this.mockWgpuInvoker, mockWindow, instanceHandle);
        this.surfaceTextureHandle = new SafeSurfaceTextureHandle(this.mockWgpuInvoker, UnsafeTextureHandle, SurfaceGetCurrentTextureStatus.Success);

        this.mockSurface = Substitute.For<IGraphicsSurface>();
        this.mockSurface.Handle.Returns(this.surfaceHandle);
        this.mockSurface.GetSurfaceTexture().Returns(this.surfaceTextureHandle);
        this.mockSurface.Configure().Returns(true);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGDParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Frame(null, this.mockSurface);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'gd')");
    }

    [Fact]
    public void Ctor_WithNullSurfaceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Frame(this.mockDevice, null);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'surface')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void IsValid_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.IsValid;

        // Assert
        actual.ShouldBeFalse();
    }

    [Fact]
    public void RenderPass_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.RenderPass;

        // Assert
        actual.ShouldBeNull();
    }

    [Fact]
    public void RenderPass_WithStartedFrame_ReturnsCorrectResult()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        var actual = sut.RenderPass;

        // Assert
        actual.ShouldNotBeNull();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Reconfigure_WhenInvoked_AllowsReconfiguration()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        sut.Submit();
        sut.Reconfigure();
        sut.Begin(this.testColor);

        // Assert
        this.mockSurface.Received(2).Configure();
    }

    [Fact]
    public void Initialize_WhenInvoked_InitializesFrame()
    {
        // Arrange
        var sut  = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Initialize();

        // Assert
        this.mockSurface.Received(1).Initialize();
        this.mockDevice.Received(1).InitializeAdapter(this.surfaceHandle);
        this.mockDevice.Received(1).InitializeDevice();
        this.mockSurface.Received(1).InitializeFormat();
    }

    [Fact]
    public void Begin_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () =>
        {
            sut.Begin(this.testColor);
        };

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Cannot begin frame. WebGPU has not been initialized.");
    }

    [Fact]
    public void Begin_WhenRenderPassHasNotAlreadyBegun_BeginsFrame()
    {
        // Arrange
        const double expectedRed  = 0.08627451211214066;
        const double expectedGreen  = 0.12941177189350128;
        const double expectedBlue  = 0.1725490242242813;
        const double expectedAlpha  = 0.04313725605607033;

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var actual = sut.Begin(this.testColor);

        // Assert
        actual.ShouldBeTrue();
        sut.IsValid.ShouldBeTrue();

        this.mockSurface.Received(1).Configure();
        this.mockSurface.Received(1).GetSurfaceTexture();
        this.mockWgpuInvoker.Received(1).DeviceCreateCommandEncoder(this.deviceHandle, Arg.Any<CommandEncoderDescriptor>());
        this.mockWgpuInvoker.Received(1).CommandEncoderBeginRenderPass(
            Arg.Is<SafeCommandEncoderHandle>(value => !value.IsInvalid),
            Arg.Is<SafeTextureViewHandle>(value => !value.IsInvalid),
            LoadOp.Clear,
            StoreOp.Store,
            expectedRed,
            expectedGreen,
            expectedBlue,
            expectedAlpha);
    }

    [Fact]
    public void Begin_WhenInvokedSecondTimeWithoutCallingSubmitFirst_ThrowsException()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var firstBegin = sut.Begin(this.testColor);
        var act = () =>
        {
            sut.Begin(this.testColor);
        };

        // Assert
        firstBegin.ShouldBeTrue();
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(Frame)}.{nameof(Frame.Begin)}()' method has already invoked.");
    }

    [Fact]
    public void Begin_WhenSurfaceIsConfigured_DoesNotAttemptToConfigureAgain()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        sut.Submit();
        sut.Begin(this.testColor);

        // Assert
        this.mockSurface.Received(1).Configure();
    }

    [Fact]
    public void Begin_WhenCalledSecondTime_ReusesHandlesViaResetHandle()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        sut.Submit();
        sut.Begin(this.testColor);

        // Assert - surfaceTextureHandle is reused via ResetHandle (not recreated)
        this.mockSurface.Received(1).GetSurfaceTexture(); // Only called once - second frame uses ResetHandle
        this.mockWgpuInvoker.Received(1).UnsafeSurfaceGetCurrentTexture(Arg.Any<SafeSurfaceHandle>()); // ResetHandle called once

        // Assert - textureViewHandle is reused via ResetHandle
        this.mockWgpuInvoker.Received(2).TextureCreateView(Arg.Any<SafeSurfaceTextureHandle>(), Arg.Any<TextureViewDescriptor>()); // Once in ctor, once in ResetHandle

        // Assert - cmdEncoderHandle is reused via ResetHandle (Submit() no longer nulls it)
        this.mockWgpuInvoker.Received(1).DeviceCreateCommandEncoder(this.deviceHandle, Arg.Any<CommandEncoderDescriptor>()); // Only called once
        this.mockWgpuInvoker.Received(1).UnsafeDeviceCreateCommandEncoder(Arg.Any<SafeDeviceHandle>(), Arg.Any<CommandEncoderDescriptor>()); // ResetHandle called once

        // Assert - renderPassHandle is reused via ResetHandle (Submit() no longer nulls it)
        this.mockWgpuInvoker.Received(1).CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()); // Only called once
        this.mockWgpuInvoker.Received(1).UnsafeCommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()); // ResetHandle called once
    }

    [Fact]
    public void Begin_WithUnsuccessfulTextureStatus_ReturnsInvalid()
    {
        // Arrange
        this.surfaceTextureHandle = CreateSurfaceTextureHandle(UnsafeTextureHandle, SurfaceGetCurrentTextureStatus.OutOfMemory);
        this.mockSurface.GetSurfaceTexture().Returns(this.surfaceTextureHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var actual = sut.Begin(this.testColor);

        // Assert
        actual.ShouldBeFalse();
        sut.IsValid.ShouldBeFalse();
        this.mockSurface.Received(1).Configure();
        this.mockSurface.Received(1).GetSurfaceTexture();
        this.mockWgpuInvoker.DidNotReceive().DeviceCreateCommandEncoder(Arg.Any<SafeDeviceHandle>(),  Arg.Any<CommandEncoderDescriptor>());
        this.mockWgpuInvoker.DidNotReceive().CommandEncoderBeginRenderPass(Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>());
    }

    [Fact]
    public void Begin_WhenCalledAfterUnsuccessfulTextureStatus_ReusesHandleButStatusNotUpdated()
    {
        // NOTE: This test documents current behavior where ResetHandle() doesn't update the status.
        // This is likely a bug - ResetHandle() should update the status, but it can't with the current API.
        // Arrange - First call fails
        var failedHandle = CreateSurfaceTextureHandle(UnsafeTextureHandle, SurfaceGetCurrentTextureStatus.OutOfMemory);

        this.mockSurface.GetSurfaceTexture().Returns(failedHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var firstResult = sut.Begin(this.testColor);
        var secondResult = sut.Begin(this.testColor); // Second attempt - still fails because status not updated

        // Assert - First call creates handle via GetSurfaceTexture, second reuses via ResetHandle
        this.mockSurface.Received(1).GetSurfaceTexture();
        this.mockWgpuInvoker.Received(1).UnsafeSurfaceGetCurrentTexture(this.surfaceHandle);
        firstResult.ShouldBeFalse();
        secondResult.ShouldBeFalse(); // Still fails because status not updated
    }

    [Fact]
    public void Begin_WhenTextureViewHandleIsInvalid_ReturnsInvalid()
    {
        // Arrange
        this.surfaceTextureHandle = CreateSurfaceTextureHandle(nint.Zero, SurfaceGetCurrentTextureStatus.Success);
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeSurfaceTextureHandle>(), Arg.Any<TextureViewDescriptor>()).Returns(nint.Zero);
        this.mockSurface.GetSurfaceTexture().Returns(this.surfaceTextureHandle);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var actual = sut.Begin(this.testColor);

        // Assert
        actual.ShouldBeFalse();
        sut.IsValid.ShouldBeFalse();
        this.mockWgpuInvoker.DidNotReceive().DeviceCreateCommandEncoder(Arg.Any<SafeDeviceHandle>(),  Arg.Any<CommandEncoderDescriptor>());
        this.mockWgpuInvoker.DidNotReceive().CommandEncoderBeginRenderPass(Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>());
    }

    [Fact]
    public void Begin_WhenCalledAfterInvalidTextureView_ReusesHandleViaResetHandle()
    {
        // Arrange - First call creates invalid texture view, second call succeeds via ResetHandle
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeSurfaceTextureHandle>(), Arg.Any<TextureViewDescriptor>())
            .Returns(nint.Zero, UnsafeTextureViewHandle);

        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var firstResult = sut.Begin(this.testColor);
        var secondResult = sut.Begin(this.testColor); // Second attempt after failure

        // Assert - First call creates handle, second call reuses via ResetHandle (which also calls TextureCreateView)
        this.mockWgpuInvoker.Received(2).TextureCreateView(Arg.Any<SafeSurfaceTextureHandle>(), Arg.Any<TextureViewDescriptor>());
        firstResult.ShouldBeFalse();
        secondResult.ShouldBeTrue();
    }

    [Theory]
    [InlineData(TextureFormat.Bgra8UnormSrgb, new[] { 0.008023192174732685, 0.015208514407277107, 0.02518685720860958, 0.04313725605607033 })]
    [InlineData(TextureFormat.Rgba8UnormSrgb, new[] { 0.008023192174732685, 0.015208514407277107, 0.02518685720860958, 0.04313725605607033 })]
    public void Begin_WithCertainTextureFormats_UsesLinearClearColors(TextureFormat format, double []expectedColorValues)
    {
        // Arrange
        var expectedRed = expectedColorValues[0];
        var expectedGreen = expectedColorValues[1];
        var expectedBlue = expectedColorValues[2];
        var expectedAlpha = expectedColorValues[3];

        this.mockSurface.Format.Returns(format);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);

        // Assert
        this.mockWgpuInvoker.Received(1).CommandEncoderBeginRenderPass(
            Arg.Is<SafeCommandEncoderHandle>(value => !value.IsInvalid),
            Arg.Is<SafeTextureViewHandle>(value => !value.IsInvalid),
            LoadOp.Clear,
            StoreOp.Store,
            expectedRed,
            expectedGreen,
            expectedBlue,
            expectedAlpha);
    }

    [Fact]
    public void Submit_WhenInvokedWithoutBeginNotBeingInvokedFirst_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var act = () =>
        {
            sut.Submit();
        };

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(Frame)}.{nameof(Frame.Submit)}()' method was invoked without invoking '{nameof(Frame)}.{nameof(Frame.Begin)}()'.");
    }

    [Fact]
    public void Submit_WithNullRenderPassHandle_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        var act = () => sut.Submit();

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Render pass handle null. You must invoke the 'Frame.Begin()' method first before invoking the 'Frame.Submit()'.");
    }

    [Fact]
    public void Submit_WithNullEncoderHandle_ThrowsException()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeCmdEncoderHandle);

        // Ensure that the render pass encoder handle is not null so we can get to the encoder handle null check
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);
        this.mockWgpuInvoker.DeviceCreateCommandEncoder(Arg.Any<SafeDeviceHandle>(), Arg.Any<CommandEncoderDescriptor>())
            .Returns((SafeCommandEncoderHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        var act = () => sut.Submit();

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Encoder handle null. You must invoke the 'Frame.Begin()' method first before invoking the 'Frame.Submit()'.");
    }

    [Fact]
    public void Submit_WhenInvoked_SubmitsFrame()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        var queueHandle = new SafeQueueHandle(this.mockWgpuInvoker, this.deviceHandle);
        const nint unsafeCmdBufferHandle = 0x1357;

        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);
        this.mockWgpuInvoker.CommandEncoderFinish(Arg.Any<SafeCommandEncoderHandle>(), Arg.Any<CommandBufferDescriptor>())
            .Returns(unsafeCmdBufferHandle);
        this.mockDevice.Queue.Returns(queueHandle);

        this.mockWgpuInvoker.DeviceCreateCommandEncoder(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<CommandEncoderDescriptor>()).Returns(this.cmdEncoderHandle);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        sut.Submit();

        // Assert
        this.mockWgpuInvoker.Received(1).RenderPassEncoderEnd(UnsafeRenderPassHandle);
        this.mockWgpuInvoker.DidNotReceive().RenderPassEncoderRelease(UnsafeRenderPassHandle); // renderPassHandle is not released in Submit()
        this.mockWgpuInvoker.Received(1).CommandEncoderFinish(this.cmdEncoderHandle,  Arg.Any<CommandBufferDescriptor>());
        this.mockWgpuInvoker.DidNotReceive().CommandEncoderRelease(Arg.Any<nint>()); // cmdEncoderHandle is not released in Submit()
        this.mockWgpuInvoker.Received(1).QueueSubmit(queueHandle, 1, unsafeCmdBufferHandle);
        this.mockWgpuInvoker.Received(1).CommandBufferRelease(unsafeCmdBufferHandle);
        this.mockWgpuInvoker.Received(1).SurfacePresent(this.surfaceHandle);
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfFrame()
    {
        // Arrange
        var renderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, UnsafeRenderPassHandle);
        this.mockWgpuInvoker.CommandEncoderBeginRenderPass(
            Arg.Any<SafeCommandEncoderHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<LoadOp>(),
            Arg.Any<StoreOp>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>(),
            Arg.Any<double>()).Returns(renderPassEncoderHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Begin(this.testColor);
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).RenderPassEncoderRelease(UnsafeRenderPassHandle);
        this.mockWgpuInvoker.Received(1).CommandEncoderRelease(UnsafeCmdEncoderHandle);
        this.mockWgpuInvoker.Received(1).TextureViewRelease(UnsafeTextureViewHandle);
        this.mockWgpuInvoker.Received(1).TextureRelease(UnsafeTextureHandle);
    }
    #endregion

    /// <summary>
    /// Creates a surface texture handle using the given unsafe handle and texture status.
    /// </summary>
    /// <returns>The new texture handle.</returns>
    private SafeSurfaceTextureHandle CreateSurfaceTextureHandle(nint unsafeHandle, SurfaceGetCurrentTextureStatus textureStatus)
        => new (this.mockWgpuInvoker, unsafeHandle, textureStatus);

    /// <summary>
    /// Creates a new instance of <see cref="Frame"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Frame CreateSystemUnderTest()
        => new (this.mockDevice, this.mockSurface);
}

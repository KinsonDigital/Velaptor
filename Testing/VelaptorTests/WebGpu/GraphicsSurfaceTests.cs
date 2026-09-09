﻿// <copyright file="GraphicsSurfaceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.WebGpu;
using Xunit;

/// <summary>
/// Tests the <see cref="GraphicsSurface"/> class.
/// </summary>
public class GraphicsSurfaceTests
{
    private const nint InstanceHandle = 0x123;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsDevice mockGraphicsDevice;
    private readonly IWindow mockWindow;
    private readonly SafeInstanceHandle safeInstanceHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSurfaceTests"/> class.
    /// </summary>
    public GraphicsSurfaceTests()
    {
        this.mockWindow = Substitute.For<IWindow>();

        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        this.safeInstanceHandle = new SafeInstanceHandle(this.mockWgpuInvoker, InstanceHandle);
        this.mockWgpuInvoker.CreateWebGpuSurface(null, this.mockWindow, this.safeInstanceHandle).Returns(InstanceHandle);

        this.mockGraphicsDevice = Substitute.For<IGraphicsDevice>();
        this.mockGraphicsDevice.Wgpu.Returns(this.mockWgpuInvoker);
        this.mockGraphicsDevice.Instance.Returns(this.safeInstanceHandle);
    }

    #region Constructors
    [Fact]
    public void Ctor_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange & Act
        var act = void () => _ = new GraphicsSurface(null, this.mockWindow);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'grfxDevice')");
    }

    [Fact]
    public void Ctor_WithNullWindowParam_ThrowsException()
    {
        // Arrange
        var mockGd = Substitute.For<IGraphicsDevice>();

        // Act
        var act = () =>
        {
            _ = new GraphicsSurface(mockGd, null);
        };

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'window')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Handle_WithNullHandle_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Handle;

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("The WebGPU surface has not been initialized. Call Initialize() first.");
    }

    [Fact]
    public void Format_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Format;

        // Assert
        actual.ShouldBe(default);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Initialize_WhenInvoked_InitializesDevice()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Initialize();

        // Assert
        _ = this.mockGraphicsDevice.Received(1).Wgpu;
    }

    [Fact]
    public void Initialize_WithInvalidHandle_ThrowsException()
    {
        // Arrange
        var instanceHandle = new SafeInstanceHandle(this.mockWgpuInvoker, nint.Zero);
        this.mockGraphicsDevice.Instance.Returns(instanceHandle);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Initialize();

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Failed to create WebGPU surface.");
    }

    [Fact]
    public void InitializeFormat_WhenInvoked_InitializesFormat()
    {
        // Arrange
        var expected = TextureFormat.Rgba32float;
        this.mockWgpuInvoker.SurfaceGetPreferredFormat(Arg.Any<SafeSurfaceHandle>(), Arg.Any<SafeAdapterHandle>())
            .Returns(expected);
        this.mockGraphicsDevice.Adapter.Returns(new SafeAdapterHandle(this.mockWgpuInvoker, 0x123));
        this.mockGraphicsDevice.Handle.Returns(new SafeDeviceHandle(this.mockWgpuInvoker, 0x456));
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.InitializeFormat();

        // Assert
        sut.Format.ShouldBe(TextureFormat.Rgba32float);
    }

    [Fact]
    public void InitializeFormat_WithNullAdapterHandle_ThrowsException()
    {
        // Arrange
        var expected = TextureFormat.Rgba32float;
        this.mockWgpuInvoker.SurfaceGetPreferredFormat(Arg.Any<SafeSurfaceHandle>(), Arg.Any<SafeAdapterHandle>())
            .Returns(expected);
        this.mockGraphicsDevice.Adapter.Returns((SafeAdapterHandle?)null);
        this.mockGraphicsDevice.Handle.Returns(new SafeDeviceHandle(this.mockWgpuInvoker, 0x456));
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();

        var act = () => sut.InitializeFormat();

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Device and adapter must be initialized before querying the surface format.");
    }

    [Fact]
    public void InitializeFormat_WithNullDeviceHandle_ThrowsException()
    {
        // Arrange
        var expected = TextureFormat.Rgba32float;
        this.mockWgpuInvoker.SurfaceGetPreferredFormat(Arg.Any<SafeSurfaceHandle>(), Arg.Any<SafeAdapterHandle>())
            .Returns(expected);
        this.mockGraphicsDevice.Adapter.Returns(new SafeAdapterHandle(this.mockWgpuInvoker, 0x123));
        this.mockGraphicsDevice.Handle.Returns((SafeDeviceHandle?)null);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();

        var act = () => sut.InitializeFormat();

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Device and adapter must be initialized before querying the surface format.");
    }

    [Fact]
    public void Configure_WhenInvoked_ConfiguresSurface()
    {
        // Arrange
        var expectedFormat = TextureFormat.Rgba32float;
        var expectedSize = new Vector2D<int>(800, 600);
        var expectedDeviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, 0x456);

        this.mockWindow.FramebufferSize.Returns(expectedSize);
        this.mockGraphicsDevice.Adapter.Returns(new SafeAdapterHandle(this.mockWgpuInvoker, 0x123));
        this.mockGraphicsDevice.Handle.Returns(expectedDeviceHandle);
        this.mockWgpuInvoker.SurfaceGetPreferredFormat(Arg.Any<SafeSurfaceHandle>(), Arg.Any<SafeAdapterHandle>())
            .Returns(expectedFormat);
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.Configure();

        // Assert
        sut.Format.ShouldBe(expectedFormat);
        sut.Handle.ShouldBeEquivalentTo(new SafeSurfaceHandle(this.mockWgpuInvoker, this.mockWindow, this.safeInstanceHandle));
        this.mockWgpuInvoker.Received(1).SurfaceGetPreferredFormat(Arg.Any<SafeSurfaceHandle>(), Arg.Any<SafeAdapterHandle>());
        this.mockWgpuInvoker.Received(1).SurfaceConfigure(
            Arg.Any<SafeSurfaceHandle>(),
            expectedDeviceHandle,
            expectedFormat,
            TextureUsage.RenderAttachment,
            (uint)expectedSize.X,
            (uint)expectedSize.Y,
            PresentMode.Fifo);
    }

    [Fact]
    public void Configure_WithZeroFrameBufferSize_ReturnsFalseAndDoesNotConfigure()
    {
        // Arrange
        var expectedSize = new Vector2D<int>(0, 0);
        this.mockWindow.FramebufferSize.Returns(expectedSize);
        this.mockGraphicsDevice.Adapter.Returns(new SafeAdapterHandle(this.mockWgpuInvoker, 0x123));
        this.mockGraphicsDevice.Handle.Returns(new SafeDeviceHandle(this.mockWgpuInvoker, 0x456));
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        var result = sut.Configure();

        // Assert
        result.ShouldBeFalse();
        this.mockWgpuInvoker.DidNotReceive().SurfaceConfigure(
            Arg.Any<SafeSurfaceHandle>(),
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<TextureFormat>(),
            Arg.Any<TextureUsage>(),
            Arg.Any<uint>(),
            Arg.Any<uint>(),
            Arg.Any<PresentMode>());
    }

    [Fact]
    public void Configure_WithNullAdapter_ThrowsException()
    {
        // Arrange
        this.mockGraphicsDevice.Adapter.Returns((SafeAdapterHandle?)null);
        this.mockGraphicsDevice.Handle.Returns(new SafeDeviceHandle(this.mockWgpuInvoker, 0x456));
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        Action act = () => { _ = sut.Configure(); };

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Device and adapter must be initialized before configuring the surface.");
    }

    [Fact]
    public void Configure_WithNullDeviceHandle_ThrowsException()
    {
        // Arrange
        this.mockGraphicsDevice.Adapter.Returns(new SafeAdapterHandle(this.mockWgpuInvoker, 0x123));
        this.mockGraphicsDevice.Handle.Returns((SafeDeviceHandle?)null);
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        Action act = () => { _ = sut.Configure(); };

        // Assert
        var exception = act.ShouldThrow<InvalidOperationException>();
        exception.Message.ShouldBe("Device and adapter must be initialized before configuring the surface.");
    }

    [Fact]
    public void GetSurfaceTexture_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new SafeSurfaceTextureHandle(this.mockWgpuInvoker, 0x789, default);
        this.mockWgpuInvoker.SurfaceGetCurrentTexture(Arg.Any<SafeSurfaceHandle>())
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.GetSurfaceTexture();
        var actual = sut.GetSurfaceTexture();

        // Assert
        this.mockWgpuInvoker.Received(1).TextureRelease(Arg.Any<nint>());
        actual.ShouldBe(expected);
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfGraphicsSurface()
    {
        // Arrange
        var expected = new SafeSurfaceTextureHandle(this.mockWgpuInvoker, 0x789, default);
        this.mockWgpuInvoker.SurfaceGetCurrentTexture(Arg.Any<SafeSurfaceHandle>())
            .Returns(expected);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.GetSurfaceTexture();
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).SurfaceUnconfigure(InstanceHandle);
        this.mockWgpuInvoker.Received(1).SurfaceRelease(InstanceHandle);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GraphicsSurface"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GraphicsSurface CreateSystemUnderTest()
        => new (this.mockGraphicsDevice, this.mockWindow);
}

// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using Carbonate.OneWay;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Xunit;
using Color = System.Drawing.Color;

/// <summary>
/// Tests the <see cref="TextureFactory"/> class.
/// </summary>
public class TextureFactoryTests
{
    private readonly IWgpuInvoker mockWgpu;
    private readonly IGraphicsDevice mockGd;
    private readonly IReactableFactory mockReactableFactory;
    private readonly SafeBindGroupLayoutHandle bindGroupLayout;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
    /// </summary>
    public TextureFactoryTests()
    {
        this.mockWgpu = Substitute.For<IWgpuInvoker>();
        this.mockGd = Substitute.For<IGraphicsDevice>();

        var mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(mockDisposeReactable);

        this.bindGroupLayout = new SafeBindGroupLayoutHandle(this.mockWgpu, new nint(100));
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWGPUInvoker_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                null,
                this.mockGd,
                this.mockReactableFactory,
                this.bindGroupLayout);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullGraphicsDevice_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockWgpu,
                null,
                this.mockReactableFactory,
                this.bindGroupLayout);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'gd')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockWgpu,
                this.mockGd,
                null,
                this.bindGroupLayout);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Create_WithNullName_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create(null, "test-path", new ImageData(new Color[0, 0]));

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create(string.Empty, "test-path", new ImageData(new Color[0, 0]));

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Create_WithNullFilePath_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create("test-name", null, new ImageData(new Color[0, 0]));

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WithEmptyFilePath_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create("test-name", string.Empty, new ImageData(new Color[0, 0]));

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("The value cannot be an empty string. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WhenInvoked_WorksCorrectly()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Create("test-name", "test-path", new ImageData(new Color[4, 4]));

        // Assert
        // NOTE: These are only here to prove that the same injected objects are the ones being used.
        this.mockWgpu.Received(1).DeviceCreateTexture(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<TextureDescriptor>());
        this.mockWgpu.Received(1).QueueWriteTexture(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<uint>(),
            Arg.Any<uint>(),
            Arg.Any<uint>(),
            Arg.Any<byte[]>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureFactory CreateSystemUnderTest()
    {
        // Set up mocks required by the Texture constructor called inside Create()
        var deviceHandle = new SafeDeviceHandle(this.mockWgpu, new nint(1));
        var queueHandle = new SafeQueueHandle(this.mockWgpu, deviceHandle);

        this.mockGd.Handle.Returns(deviceHandle);
        this.mockGd.Queue.Returns(queueHandle);

        this.mockWgpu.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(new nint(50));
        this.mockWgpu.DeviceCreateTexture(Arg.Any<SafeDeviceHandle>(), Arg.Any<TextureDescriptor>())
            .Returns(new nint(100));
        this.mockWgpu.TextureCreateView(Arg.Any<nint>(), Arg.Any<TextureViewDescriptor>())
            .Returns(new nint(200));
        this.mockWgpu.DeviceCreateSampler(Arg.Any<SafeDeviceHandle>(), Arg.Any<SamplerDescriptor>())
            .Returns(new nint(300));
        this.mockWgpu.DeviceCreateBindGroup(Arg.Any<SafeDeviceHandle>(), Arg.Any<BindGroupDescriptor>())
            .Returns(new nint(400));

        return new TextureFactory(
            this.mockWgpu,
            this.mockGd,
            this.mockReactableFactory,
            this.bindGroupLayout);
    }
}

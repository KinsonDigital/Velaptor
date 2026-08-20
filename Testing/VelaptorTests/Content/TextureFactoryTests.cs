// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using Carbonate.OneWay;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.Content;
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
    private const nint UnsafeSamplerHandle = 0x1;
    private const nint UnsafeBindGroupHandle = 0x2;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsDevice mockGd;
    private readonly IReactableFactory mockReactableFactory;
    private readonly ITextureIdGenerator mockTextureIdGenerator;
    private readonly SafeBindGroupLayoutHandle bindGroupLayout;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
    /// </summary>
    public TextureFactoryTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        this.mockGd = Substitute.For<IGraphicsDevice>();
        // Set up mocks required by the Texture constructor called inside Create()
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, new nint(1));
        var queueHandle = new SafeQueueHandle(this.mockWgpuInvoker, deviceHandle);

        this.mockGd.Handle.Returns(deviceHandle);
        this.mockGd.Queue.Returns(queueHandle);

        this.mockWgpuInvoker.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(new nint(50));
        this.mockWgpuInvoker.DeviceCreateTexture(Arg.Any<SafeDeviceHandle>(), Arg.Any<TextureDescriptor>())
            .Returns(new nint(100));
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeTextureHandle>(), Arg.Any<TextureViewDescriptor>())
            .Returns(new nint(200));
        this.mockWgpuInvoker.DeviceCreateSampler(Arg.Any<SafeDeviceHandle>(), Arg.Any<SamplerDescriptor>())
            .Returns(new SafeSamplerHandle(this.mockWgpuInvoker, UnsafeSamplerHandle));
        this.mockWgpuInvoker.DeviceCreateBindGroup(
                Arg.Any<SafeDeviceHandle>(),
                Arg.Any<SafeBindGroupLayoutHandle>(),
                Arg.Any<SafeTextureViewHandle>(),
                Arg.Any<SafeSamplerHandle>())
            .Returns(new SafeBindGroupHandle(this.mockWgpuInvoker, UnsafeBindGroupHandle));

        var mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(mockDisposeReactable);

        this.mockTextureIdGenerator = Substitute.For<ITextureIdGenerator>();

        this.bindGroupLayout = new SafeBindGroupLayoutHandle(this.mockWgpuInvoker, new nint(100));
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
                this.mockTextureIdGenerator,
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
                this.mockWgpuInvoker,
                null,
                this.mockReactableFactory,
                this.mockTextureIdGenerator,
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
                this.mockWgpuInvoker,
                this.mockGd,
                null,
                this.mockTextureIdGenerator,
                this.bindGroupLayout);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullTextureIdGeneratorParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockWgpuInvoker,
                this.mockGd,
                this.mockReactableFactory,
                null,
                this.bindGroupLayout);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'textureIdGenerator')");
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
        this.mockWgpuInvoker.Received(1).DeviceCreateTexture(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<TextureDescriptor>());
        this.mockWgpuInvoker.Received(1).QueueWriteTexture(
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
        return new TextureFactory(
            this.mockWgpuInvoker,
            this.mockGd,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            this.bindGroupLayout);
    }
}

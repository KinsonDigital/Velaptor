// <copyright file="TextureTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Drawing;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using NSubstitute;
using Shouldly;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Xunit;
using WgpuSamplerDescriptor = Silk.NET.WebGPU.SamplerDescriptor;
using WgpuTextureDescriptor = Silk.NET.WebGPU.TextureDescriptor;
using WgpuTextureViewDescriptor = Silk.NET.WebGPU.TextureViewDescriptor;

/// <summary>
/// Tests the <see cref="Texture"/> class.
/// </summary>
public class TextureTests
{
    private const string TextureName = "test-texture";
    private const string TexturePath = @"C:\temp\test-texture.png";
    private const nint UnsafeSamplerHandle = 0x1;
    private const nint UnsafeBindGroupHandle = 0x2;
    private const nint UnsafeTextureViewHandle = 0x3;
    private const nint UnsafeDeviceHandle = 0x4;
    private const nint UnsafeBindGroupLayoutHandle = 0x5;
    private const nint UnsafeQueueHandle = 0x6;
    private const uint TextureId = 123;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsDevice mockGrfxDevice;
    private readonly IDisposable mockDisposeUnsubscriber;
    private readonly IReactableFactory mockReactableFactory;
    private readonly ITextureIdGenerator mockTextureIdGenerator;
    private readonly SafeBindGroupLayoutHandle bindGroupLayout;
    private readonly ImageData imageData;
    private IReceiveSubscription<DisposeTextureData>? disposeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureTests"/> class.
    /// </summary>
    public TextureTests()
    {
        this.imageData = new ImageData(new Color[2, 3]);

        for (var y = 0; y < this.imageData.Height; y++)
        {
            for (var x = 0; x < this.imageData.Width; x++)
            {
                this.imageData.Pixels[x, y] = y switch
                {
                    0 => Color.FromArgb(255, 255, 0, 0), // Row 1
                    1 => Color.FromArgb(255, 0, 255, 0), // Row 2
                    2 => Color.FromArgb(255, 0, 0, 255), // Row 3
                    _ => throw new Exception($"Row '{y}' does not exist when setting up image data for test."),
                };
            }
        }

        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, new nint(1));
        var queueHandle = new SafeQueueHandle(this.mockWgpuInvoker, deviceHandle);

        this.mockGrfxDevice = Substitute.For<IGraphicsDevice>();
        this.mockGrfxDevice.Handle.Returns(deviceHandle);
        this.mockGrfxDevice.Queue.Returns(queueHandle);

        this.mockWgpuInvoker.DeviceGetQueue(Arg.Any<SafeDeviceHandle>()).Returns(UnsafeQueueHandle);
        this.mockWgpuInvoker.DeviceCreateTexture(Arg.Any<SafeDeviceHandle>(), Arg.Any<WgpuTextureDescriptor>())
            .Returns(UnsafeDeviceHandle);
        this.mockWgpuInvoker.TextureCreateView(Arg.Any<SafeTextureHandle>(), Arg.Any<WgpuTextureViewDescriptor>())
            .Returns(UnsafeTextureViewHandle);
        this.mockWgpuInvoker.DeviceCreateSampler(Arg.Any<SafeDeviceHandle>(), Arg.Any<WgpuSamplerDescriptor>())
            .Returns(new SafeSamplerHandle(this.mockWgpuInvoker, UnsafeSamplerHandle));
        this.mockWgpuInvoker.DeviceCreateBindGroup(
                Arg.Any<SafeDeviceHandle>(),
                Arg.Any<SafeBindGroupLayoutHandle>(),
                Arg.Any<SafeTextureViewHandle>(),
                Arg.Any<SafeSamplerHandle>())
            .Returns(new SafeBindGroupHandle(this.mockWgpuInvoker, UnsafeBindGroupHandle));

        this.mockDisposeUnsubscriber = Substitute.For<IDisposable>();

        var mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();
        mockDisposeReactable.Subscribe(Arg.Any<IReceiveSubscription<DisposeTextureData>>())
            .Returns(this.mockDisposeUnsubscriber)
            .AndDoes(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<DisposeTextureData>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                this.disposeReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(mockDisposeReactable);

        this.mockTextureIdGenerator = Substitute.For<ITextureIdGenerator>();
        this.mockTextureIdGenerator.GenerateNextId().Returns(TextureId);

        this.bindGroupLayout = new SafeBindGroupLayoutHandle(this.mockWgpuInvoker, UnsafeBindGroupLayoutHandle);
    }

    #region Constructor Tests
    [Fact]
    public void InternalCtor_WithNullWGPUParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            null,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void InternalCtor_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            null,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'grfxDevice')");
    }

    [Fact]
    public void InternalCtor_WithNullBindGroupLayoutParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            null,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'bindGroupLayout')");
    }

    [Fact]
    public void InternalCtor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            null,
            this.mockTextureIdGenerator,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void InternalCtor_WithNullTextureIdGeneratorParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            null,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'textureIdGenerator')");
    }

    [Fact]
    public void InternalCtor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            null,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void InternalCtor_WithEmptyName_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            string.Empty,
            TexturePath,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void InternalCtor_WithNullFilePath_ThrowsException()
    {
        // Act & Assert
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            null,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentNullException>();
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'filePath')");
    }

    [Fact]
    public void InternalCtor_WithEmptyFilePath_ThrowsException()
    {
        // Act & Assert
        var act = () => new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            string.Empty,
            this.imageData);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The value cannot be an empty string. (Parameter 'filePath')");
    }

    [Fact]
    public void InternalCtor_WithEmptyImageData_ThrowsException()
    {
        // Act & Assert
        var act = () => CreateSystemUnderTest(true);

        // Assert
        var exception = act.ShouldThrow<ArgumentException>();
        exception.Message.ShouldBe("The image data must not be empty. (Parameter 'imageData')");
    }

    [Fact]
    public void InternalCtor_WithNullGraphicsDeviceHandle_ThrowsException()
    {
        // Arrange
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, new nint(1));
        var queueHandle = new SafeQueueHandle(this.mockWgpuInvoker, deviceHandle);

        this.mockGrfxDevice.Handle.Returns((SafeDeviceHandle?)null);
        this.mockGrfxDevice.Queue.Returns(queueHandle);

        // Act
        var act = () =>
        {
            _ = new Texture(this.mockWgpuInvoker,
                this.mockGrfxDevice,
                this.bindGroupLayout,
                this.mockReactableFactory,
                this.mockTextureIdGenerator,
                "test-texture.png",
                @"C:\temp\test-texture.png",
                this.imageData);
        };

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(GraphicsDevice)}.{nameof(GraphicsDevice.Handle)}' cannot be null. Could not upload texture data to GPU.");
    }

    [Fact]
    public void InternalCtor_WhenInvoked_UploadsTextureDataToGpu()
    {
        // Arrange
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, new nint(1));
        var queueHandle = new SafeQueueHandle(this.mockWgpuInvoker, deviceHandle);

        this.mockGrfxDevice.Handle.Returns(deviceHandle);
        this.mockGrfxDevice.Queue.Returns(queueHandle);

        // Act
        _ = new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            "test-texture.png",
            @"C:\temp\test-texture.png",
            this.imageData);

        // Assert
        this.mockWgpuInvoker.Received(1).DeviceCreateTexture(Arg.Any<SafeDeviceHandle>(), Arg.Any<WgpuTextureDescriptor>());
        this.mockWgpuInvoker.Received(1).QueueWriteTexture(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<uint>(),
            Arg.Any<uint>(),
            Arg.Any<uint>(),
            Arg.Any<byte[]>());
        this.mockWgpuInvoker.Received(1).TextureCreateView(Arg.Any<SafeTextureHandle>(), Arg.Any<WgpuTextureViewDescriptor>());
        this.mockWgpuInvoker.Received(1).DeviceCreateSampler(Arg.Any<SafeDeviceHandle>(), Arg.Any<WgpuSamplerDescriptor>());
        this.mockWgpuInvoker.Received(1).DeviceCreateBindGroup(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<SafeBindGroupLayoutHandle>(),
            Arg.Any<SafeTextureViewHandle>(),
            Arg.Any<SafeSamplerHandle>());
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Id_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Id;

        // Assert
        actual.ShouldBeGreaterThanOrEqualTo(0u);
    }

    [Fact]
    public void Name_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.ShouldBe(TextureName);
    }

    [Fact]
    public void Path_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.ShouldBe(TexturePath);
    }

    [Fact]
    public void Width_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Width;

        // Assert
        actual.ShouldBe(2u);
    }

    [Fact]
    public void Height_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Height;

        // Assert
        actual.ShouldBe(3u);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ReactableNotifications_WithDifferentTextureID_DoesNotDisposeOfTexture()
    {
        // Arrange
        var disposeTextureData = new DisposeTextureData { TextureId = 987 };

        CreateSystemUnderTest();

        // Act
        // Dispose twice to ensure that handles will not be disposed twice
        this.disposeReactor?.OnReceive(disposeTextureData);

        // Assert
        this.mockWgpuInvoker.DidNotReceive().BindGroupRelease(UnsafeBindGroupHandle);
        this.mockWgpuInvoker.DidNotReceive().SamplerRelease(UnsafeSamplerHandle);
        this.mockWgpuInvoker.DidNotReceive().TextureViewRelease(UnsafeTextureViewHandle);
        this.mockWgpuInvoker.DidNotReceive().TextureDestroy(UnsafeDeviceHandle);
        this.mockWgpuInvoker.DidNotReceive().TextureRelease(UnsafeDeviceHandle);
    }

    [Fact]
    public void ReactableNotifications_WithMatchingTextureId_DisposesOfTexture()
    {
        // Arrange
        var disposeTextureData = new DisposeTextureData { TextureId = TextureId };

        CreateSystemUnderTest();

        // Act
        // Dispose twice to ensure that handles will not be disposed twice
        this.disposeReactor?.OnReceive(disposeTextureData);
        this.disposeReactor?.OnReceive(disposeTextureData);

        // Assert
        this.mockWgpuInvoker.Received(1).BindGroupRelease(UnsafeBindGroupHandle);
        this.mockWgpuInvoker.Received(1).SamplerRelease(UnsafeSamplerHandle);
        this.mockWgpuInvoker.Received(1).TextureViewRelease(UnsafeTextureViewHandle);
        this.mockWgpuInvoker.Received(1).TextureDestroy(UnsafeDeviceHandle);
        this.mockWgpuInvoker.Received(1).TextureRelease(UnsafeDeviceHandle);
    }

    [Fact]
    public void ReactableNotifications_WhenPushingDisposeTextureNotification_DisposesOfTexture()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var disposeTextureData = new DisposeTextureData { TextureId = sut.Id };

        // Act
        this.disposeReactor?.OnReceive(disposeTextureData);

        // Assert
        this.mockWgpuInvoker.Received(1).TextureDestroy(Arg.Any<nint>());
    }
    #endregion

    /// <summary>
    /// Creates a texture for the purpose of testing.
    /// </summary>
    /// <returns>The texture instance to test.</returns>
    private Texture CreateSystemUnderTest(bool useEmptyData = false)
    {
        return new Texture(
            this.mockWgpuInvoker,
            this.mockGrfxDevice,
            this.bindGroupLayout,
            this.mockReactableFactory,
            this.mockTextureIdGenerator,
            TextureName,
            TexturePath,
            useEmptyData ? default : this.imageData);
    }
}

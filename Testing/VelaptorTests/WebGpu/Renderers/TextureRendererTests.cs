// <copyright file="TextureRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Renderers;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.OneWay;
using Carbonate.NonDirectional;
using Carbonate.OneWay;
using Helpers;
using NSubstitute;
using Shouldly;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Graphics.Renderers.Exceptions;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Batching;
using Velaptor.WebGpu.Buffers;
using Velaptor.WebGpu.Renderers;
using Xunit;

/// <summary>
/// Tests the viewport subscription behavior in WebGPU renderers.
/// </summary>
/// <remarks>
/// The WebGPU renderers use concrete sealed dependency types
/// (<see cref="Velaptor.WebGpu.Buffers.TextureGpuBuffer"/>,
/// <see cref="Velaptor.WebGpu.GraphicsTexturePipeline"/>, etc.) that cannot be mocked
/// with NSubstitute because they require GPU initialization. Full renderer construction
/// testing requires extracting interfaces for these types, which is a separate architectural task.
///
/// These tests verify the subscription pattern that all 4 renderers use to listen for
/// <see cref="PushNotifications.ViewPortSizeChangedId"/>.
/// </remarks>
public class TextureRendererTests : TestsBase
{
    private const uint TextureId = 123u;
    private const uint AtlasTextureId = 456u;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsTexturePipeline mockTexturePipeline;
    private readonly IWebGpuBuffer<TextureBatchItem> mockBuffer;
    private readonly IFrame mockFrame;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable<ViewPortSizeData> mockViewPortReactable;
    private readonly IDisposable mockFrameBeginUnsubscriber;
    private readonly IDisposable mockBatchBeginUnsubscriber;
    private readonly IDisposable mockRenderTexturesUnsubscriber;
    private readonly IDisposable mockViewPortUnsubscriber;
    private readonly TextureBindGroupRegistry bindGroupRegistry;
    private readonly ITexture mockTexture;
    private IReceiveSubscription? frameBeginSubscription;
    private IReceiveSubscription? batchBeginSubscription;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSubscription;
    private IReceiveSubscription<Memory<RenderItem<TextureBatchItem>>>? renderBatchSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureRendererTests"/> class.
    /// </summary>
    public TextureRendererTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockTexturePipeline = Substitute.For<IGraphicsTexturePipeline>();
        this.mockBuffer = Substitute.For<IWebGpuBuffer<TextureBatchItem>>();
        this.mockFrame = Substitute.For<IFrame>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        this.mockFrameBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockBatchBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockRenderTexturesUnsubscriber = Substitute.For<IDisposable>();
        this.mockViewPortUnsubscriber = Substitute.For<IDisposable>();

        var mockPushReactable = Substitute.For<IPushReactable>();

        // Capture the subscriptions
        mockPushReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription>();

                if (subscription.Id == PushNotifications.FrameHasBegunId)
                {
                    this.frameBeginSubscription = subscription;
                }
                else if (subscription.Id == PushNotifications.BatchHasBegunId)
                {
                    this.batchBeginSubscription = subscription;
                }
                else
                {
                    throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
                }
            });

        // Capture the unsubscribers
        mockPushReactable
            .Subscribe(Arg.Any<IReceiveSubscription>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription>();

                if (subscription.Id == PushNotifications.FrameHasBegunId)
                {
                    return this.mockFrameBeginUnsubscriber;
                }

                return subscription.Id == PushNotifications.BatchHasBegunId
                    ? this.mockBatchBeginUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        // Capture the subscriptions
        var mockRenderBatchReactable = Substitute.For<IRenderBatchReactable<TextureBatchItem>>();
        mockRenderBatchReactable
            .When(x => x.Subscribe(
                Arg.Any<IReceiveSubscription<Memory<RenderItem<TextureBatchItem>>>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<TextureBatchItem>>>>();

                if (subscription.Id == PushNotifications.RenderTexturesId)
                {
                    this.renderBatchSubscription = subscription;
                }
                else
                {
                    throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
                }
            });

        // Capture the unsubscribers
        mockRenderBatchReactable
            .Subscribe(Arg.Any<IReceiveSubscription<Memory<RenderItem<TextureBatchItem>>>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<TextureBatchItem>>>>();

                return subscription.Id == PushNotifications.RenderTexturesId
                    ? this.mockRenderTexturesUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        this.mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        this.mockViewPortReactable
            .Subscribe(Arg.Any<IReceiveSubscription<ViewPortSizeData>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<ViewPortSizeData>>();

                return subscription.Id == PushNotifications.ViewPortSizeChangedId
                    ? this.mockViewPortUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        this.mockViewPortReactable
            .When(x => x.Subscribe(
                Arg.Any<IReceiveSubscription<ViewPortSizeData>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<ViewPortSizeData>>();

                if (subscription.Id == PushNotifications.ViewPortSizeChangedId)
                {
                    this.viewPortSubscription = subscription;
                }
                else
                {
                    throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
                }
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateRenderTextureReactable().Returns(mockRenderBatchReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(this.mockViewPortReactable);

        this.bindGroupRegistry = new TextureBindGroupRegistry();

        this.mockTexture = Substitute.For<ITexture>();
        this.mockTexture.Id.Returns(TextureId);
        this.mockTexture.Width.Returns(100u);
        this.mockTexture.Height.Returns(200u);
    }

    #region Method Tests
    [Fact]
    public void Render_WhenBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(ITextureRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(
            this.mockTexture,
            11,
            22,
            33);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Theory]
    [InlineData(0, 100)]
    [InlineData(200, 0)]
    public void Render_WithWidthOrHeightTooSmall_ThrowsException(int srcWidth, int srcHeight)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(
            this.mockTexture,
            new Rectangle(0, 0, srcWidth, srcHeight),
            Rectangle.Empty,
            11f,
            22f,
            Color.FromArgb(33, 44, 55, 66),
            RenderEffects.None,
            77);

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
    }

    [Fact]
    public void Render_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(
            this.mockTexture,
            11,
            22,
            33);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 33, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingRectsOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            4f,
            Color.FromArgb(5,
                6,
                7,
                8),
            RenderEffects.None,
            9,
            10);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingRectsOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";

        var mockAtlas = CreateMockAtlasData();
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            4f,
            Color.FromArgb(5,
                6,
                7,
                8),
            RenderEffects.None,
            frameNumber,
            9);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingRectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var mockAtlas = CreateMockAtlasData();

        // Act
        sut.Render(mockAtlas,
            "test-texture",
            new Vector2(11,
                22),
            33f,
            44f,
            Color.FromArgb(55,
                66,
                77,
                88),
            RenderEffects.None,
            0,
            111);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 111, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(
            null,
            1,
            2,
            3);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            33f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, 33f, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleAndSizeOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, 1, 2, 3, 4, 5);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleAndSizeOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, 33f, 44f, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleAndSizeAndColorOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, 1, 2, 3, 4, Color.FromArgb(5, 6, 7, 8), 9);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndAngleAndSizeAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, 33f, 44f, Color.FromArgb(55, 66, 77, 88), 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndEffectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.FlipHorizontally,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, RenderEffects.FlipHorizontally, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, Color.FromArgb(55, 66, 77, 88), 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndColorAndEffectsOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(
            null,
            11,
            22,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.FlipHorizontally,
            77);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingXAndYAndColorAndEffectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.FlipHorizontally,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, 11, 22, Color.FromArgb(55, 66, 77, 88), RenderEffects.FlipHorizontally, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, new Vector2(1, 2), 3, 4);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            33f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), 33f, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleAndSizeOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, new Vector2(1, 2), 3, 4, 7);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleAndSizeOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), 33f, 44f, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleAndSizeAndColorOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, new Vector2(1, 2), 3, 4, Color.FromArgb(5, 6, 7, 8), 9);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingPosAndAngleAndSizeAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), 33f, 44f, Color.FromArgb(55, 66, 77, 88), 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndEffectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.FlipHorizontally,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), RenderEffects.FlipHorizontally, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), Color.FromArgb(55, 66, 77, 88), 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingPosAndColorAndEffectsOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, new Vector2(1, 2), Color.FromArgb(5, 6, 7, 8), RenderEffects.FlipHorizontally, 9);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingPosAndColorAndEffectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(0, 0, 100, 200),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.FlipHorizontally,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Vector2(11, 22), Color.FromArgb(55, 66, 77, 88), RenderEffects.FlipHorizontally, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingSrcAndDestRectsOverloadWithNullTexture_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(
            null,
            new Rectangle(1, 2, 3, 4),
            new Rectangle(5, 6, 7, 8),
            9,
            10f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.FlipHorizontally,
            99);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Fact]
    public void Render_WhenInvokingSrcAndDestRectsOverload_AddsItemToBatchManager()
    {
        // Arrange
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(11, 22, 33, 44),
            new Rectangle(55, 66, 77, 88),
            44f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.FlipHorizontally,
            TextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(this.mockTexture, new Rectangle(11, 22, 33, 44), new Rectangle(55, 66, 77, 88), 44f, 33f, Color.FromArgb(55, 66, 77, 88), RenderEffects.FlipHorizontally, 99);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 99, Arg.Any<DateTime>());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var mockAtlas = CreateMockAtlasData();

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas, "test-texture", new Vector2(1, 2), frameNumber, 7);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndColorOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null,
            "test-texture",
            new Vector2(1,
                2),
            Color.FromArgb(5,
                6,
                7,
                8),
            9,
            10);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasAndColorOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";

        var mockAtlas = CreateMockAtlasData();

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(1,
                2),
            Color.FromArgb(53,
                4,
                5,
                6),
            frameNumber,
            8);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            1f,
            0f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), Color.FromArgb(55, 66, 77, 88), 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(1, 2), 3, 4, 5);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasAndAngleOverloadInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";
        var mockAtlas = CreateMockAtlasData();

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(11,
                22),
            33f,
            frameNumber,
            77);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            1f,
            33f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), 33f, 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndSizeOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, "test-texture", new Vector2(1, 2), 3, 4, 5, 6);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasAndAngleAndSizeOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";

        var mockAtlas = CreateMockAtlasData();

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            4f,
            frameNumber,
            5);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndSizeOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), 33f, 44f, 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndColorOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () =>  sut.Render(null,
            "test-texture",
            new Vector2(1,
                2),
            3,
            Color.FromArgb(5,
                6,
                7,
                8),
            9,
            10);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasAndAngleAndColorOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";

        var mockAtlas = CreateMockAtlasData();

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            Color.FromArgb(4,
                5,
                6,
                7),
            frameNumber,
            8);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            1f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), 33f, Color.FromArgb(55, 66, 77, 88), 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndSizeAndColorOverloadWithNullAtlas_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            4f,
            Color.FromArgb(5,
                6,
                7,
                8),
            9,
            10);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'atlas')");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(4)]
    public void Render_WhenInvokingAtlasAndAngleAndSizeAndColorOverloadWithInvalidFrameNumber_ThrowsException(int frameNumber)
    {
        // Arrange
        var expectedMsg = $"The frame number '{frameNumber}' is invalid for atlas 'test-atlas' and sub-texture 'test-texture'" +
                          ".\nThe frame number must be greater than or equal to 0 and less than or equal to the total number of frames.";

        var mockAtlas = CreateMockAtlasData();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(mockAtlas,
            "test-texture",
            new Vector2(1,
                2),
            3f,
            4f,
            Color.FromArgb(5,
                6,
                7,
                8),
            frameNumber,
            9);

        // Assert
        act.ShouldThrow<RendererException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WhenInvokingAtlasAndAngleAndSizeAndColorOverload_AddsItemToBatchManager()
    {
        // Arrange
        var mockAtlas = CreateMockAtlasData();
        var expectedBatchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.FromArgb(55, 66, 77, 88),
            RenderEffects.None,
            AtlasTextureId);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockAtlas, "test-texture", new Vector2(11, 22), 33f, 44f, Color.FromArgb(55, 66, 77, 88), 0, 77);

        // Assert
        this.mockBatchingManager.Received(1).AddTextureItem(expectedBatchItem, 77, Arg.Any<DateTime>());
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfRenderer()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockFrameBeginUnsubscriber.Received(1).Dispose();
        this.mockBatchBeginUnsubscriber.Received(1).Dispose();
        this.mockRenderTexturesUnsubscriber.Received(1).Dispose();
        this.mockViewPortUnsubscriber.Received(1).Dispose();
    }
    #endregion

    #region Internal Method Tests
    [Fact]
    public void RenderBase_WithNullTextureParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(null, 11, 22, 33, 44);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'texture')");
    }

    [Theory]
    [InlineData(0, 200)]
    [InlineData(100, 0)]
    public void RenderBase_WithInvalidSrcRectWidthOrHeight_ThrowsException(int srcRectWidth, int srcRectHeight)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(
            this.mockTexture,
            new Rectangle(1, 2, srcRectWidth, srcRectHeight),
            new Rectangle(4, 5, 6, 7),
            8,
            9,
            Color.White,
            RenderEffects.None,
            10);

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe("The source rectangle must have a width and height greater than zero. (Parameter 'srcRect')");
    }

    [Fact]
    public void RenderBatch_WithNoItemsToRender_DoesNotRenderBatch()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, 0x571);
        var bindGroupHandle = new SafeBindGroupHandle(this.mockWgpuInvoker, 0x497);

        this.mockWgpuInvoker.DeviceCreateBindGroup(Arg.Any<SafeDeviceHandle>(),
                Arg.Any<SafeBindGroupLayoutHandle>(),
                Arg.Any<SafeTextureViewHandle>(),
                Arg.Any<SafeSamplerHandle>())
            .Returns(bindGroupHandle);

        this.mockFrame.RenderPass.Returns(renderPassHandle);

        var mockGrfxDevice = Substitute.For<IGraphicsDevice>();
        mockGrfxDevice.Handle.Returns(deviceHandle);

        this.bindGroupRegistry.Register(TextureId, bindGroupHandle);

        var sut = CreateSystemUnderTest();

        this.batchBeginSubscription.OnReceive();
        var itemsToRender = new Memory<RenderItem<TextureBatchItem>>([]);

        sut.Render(
            this.mockTexture,
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44,
            55,
            Color.White,
            RenderEffects.None,
            1);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockTexturePipeline.DidNotReceive().Bind(Arg.Any<SafeRenderPassEncoderHandle>());
        this.mockBuffer.DidNotReceive().UploadData(Arg.Any<TextureBatchItem>(), Arg.Any<uint>());
        this.mockWgpuInvoker.DidNotReceive()
            .RenderPassEncoderSetBindGroup(renderPassHandle,
                Arg.Any<uint>(),
                Arg.Any<SafeBindGroupHandle>(),
                Arg.Any<nuint>(),
                Arg.Any<nint>());
        this.mockBuffer.DidNotReceive().Draw(Arg.Any<SafeRenderPassEncoderHandle>(), Arg.Any<uint>(), Arg.Any<uint>());
    }

    [Fact]
    public void RenderBatch_WithNullFrameRenderPass_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchBeginSubscription.OnReceive();

        var batchItem = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);
        var renderItem = new RenderItem<TextureBatchItem> { Item = batchItem, };
        var itemsToRender = new Memory<RenderItem<TextureBatchItem>>([renderItem]);

        sut.Render(
            this.mockTexture,
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44,
            55,
            Color.White,
            RenderEffects.None,
            1);

        // Act
        var act = () => this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The `{nameof(IFrame.RenderPass)}` cannot be null.  Cannot render texture.");
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Needed for brevity")]
    public void RenderBatch_WhenInvoked_RendersBatch()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, 0x571);
        var bindGroupHandle = new SafeBindGroupHandle(this.mockWgpuInvoker, 0x497);

        this.mockWgpuInvoker.DeviceCreateBindGroup(Arg.Any<SafeDeviceHandle>(),
                Arg.Any<SafeBindGroupLayoutHandle>(),
                Arg.Any<SafeTextureViewHandle>(),
                Arg.Any<SafeSamplerHandle>())
            .Returns(bindGroupHandle);

        this.mockFrame.RenderPass.Returns(renderPassHandle);

        var mockGrfxDevice = Substitute.For<IGraphicsDevice>();
        mockGrfxDevice.Handle.Returns(deviceHandle);

        // Bind the texture ID
        this.bindGroupRegistry.Register(123, bindGroupHandle);
        this.bindGroupRegistry.Register(456, bindGroupHandle);

        var sut = CreateSystemUnderTest();

        this.batchBeginSubscription.OnReceive();

        var batchItem1 = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            123);

        var batchItem2 = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            123);

        var batchItem3 = new TextureBatchItem(
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44f,
            33f,
            Color.White,
            RenderEffects.None,
            789);

        var renderItem1 = new RenderItem<TextureBatchItem> { Item = batchItem1, };
        var renderItem2 = new RenderItem<TextureBatchItem> { Item = batchItem2, };
        var renderItem3 = new RenderItem<TextureBatchItem> { Item = batchItem3, };
        var itemsToRender = new Memory<RenderItem<TextureBatchItem>>([renderItem1, renderItem2, renderItem3]);

        sut.Render(
            this.mockTexture,
            new Rectangle(5, 6, 50, 60),
            new Rectangle(11, 22, 100, 200),
            44,
            55,
            Color.White,
            RenderEffects.None,
            1);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockTexturePipeline.Received(1).Bind(renderPassHandle);
        this.mockBuffer.Received(1).UploadData(batchItem1, 0);
        this.mockWgpuInvoker.Received(1).RenderPassEncoderSetBindGroup(renderPassHandle, 0, bindGroupHandle, 0x0, 0x0);
        this.mockBuffer.Received(1).Draw(renderPassHandle, 2, 0);
    }
    #endregion

    #region Reactable Subscription Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void RenderBatchSubscription_WhenDisposing_Unsubscribes()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.batchBeginSubscription.OnUnsubscribe();

        // Assert
        this.mockBatchBeginUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void RenderTexturesSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.renderBatchSubscription.OnUnsubscribe();

        // Assert
        this.mockRenderTexturesUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void FrameBeginSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.frameBeginSubscription.OnUnsubscribe();

        // Assert
        this.mockFrameBeginUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void ViewPortSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSubscription.OnUnsubscribe();

        // Assert
        this.mockViewPortUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void ViewportSubscription_WhenViewportChanges_ReceivesCorrectNotification()
    {
        // Arrange
        var bufferWindowSize = new Vector2(800f, 600f); // Default, matches WebGpuBufferBase default
        var viewportReactable = this.mockReactableFactory.CreateViewPortReactable();

        // Act — simulate what CreateOneWayReceive does internally
        var reactor = Substitute.For<IReceiveSubscription<ViewPortSizeData>>();
        reactor.Id.Returns(PushNotifications.ViewPortSizeChangedId);
        reactor
            .When(x => x.OnReceive(Arg.Any<ViewPortSizeData>()))
            .Do(callInfo =>
            {
                var data = callInfo.Arg<ViewPortSizeData>();
                bufferWindowSize = new Vector2(data.Width, data.Height);
            });

        viewportReactable.Subscribe(reactor);

        // Simulate the push notification
        reactor.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockViewPortReactable.Received(1).Subscribe(
            Arg.Is<IReceiveSubscription<ViewPortSizeData>>(r =>
                r.Id == PushNotifications.ViewPortSizeChangedId));
        bufferWindowSize.ShouldBe(new Vector2(1920f, 1080f));
    }
    #endregion

    /// <summary>
    /// Creates a mock <see cref="IAtlasData"/> with a single frame for the sub-texture "test-texture".
    /// </summary>
    /// <returns>The mock atlas data.</returns>
    private static IAtlasData CreateMockAtlasData()
    {
        var mockAtlasTexture = Substitute.For<ITexture>();
        mockAtlasTexture.Id.Returns(AtlasTextureId);
        mockAtlasTexture.Width.Returns(100u);
        mockAtlasTexture.Height.Returns(200u);

        var frames = new[]
        {
            new AtlasSubTextureData
            {
                Bounds = new Rectangle(5, 6, 50, 60),
                Name = "test-texture",
                FrameIndex = 0,
            },
        };

        var mockAtlas = Substitute.For<IAtlasData>();
        mockAtlas.Name.Returns("test-atlas");
        mockAtlas.Texture.Returns(mockAtlasTexture);
        mockAtlas.GetFrames("test-texture").Returns(frames);

        return mockAtlas;
    }

    /// <summary>
    /// Creates a new instance of <see cref="TextureRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureRenderer CreateSystemUnderTest()
        => new (
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
            this.mockBatchingManager,
            this.mockReactableFactory);
}

// <copyright file="ShapeRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
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
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Batching;
using Velaptor.WebGpu.Buffers;
using Velaptor.WebGpu.Renderers;
using Xunit;

/// <summary>
/// Tests the <see cref="ShapeRenderer"/> class.
/// </summary>
public class ShapeRendererTests : TestsBase
{
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsShapePipeline mockShapePipeline;
    private readonly IWebGpuBuffer<ShapeBatchItem> mockBuffer;
    private readonly IFrame mockFrame;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IDisposable mockFrameBeginUnsubscriber;
    private readonly IDisposable mockBatchBeginUnsubscriber;
    private readonly IDisposable mockRenderShapesUnsubscriber;
    private readonly IDisposable mockViewPortUnsubscriber;
    private IReceiveSubscription? frameBeginSubscription;
    private IReceiveSubscription? batchBeginSubscription;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSubscription;
    private IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>? renderBatchSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRendererTests"/> class.
    /// </summary>
    public ShapeRendererTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockShapePipeline = Substitute.For<IGraphicsShapePipeline>();
        this.mockBuffer = Substitute.For<IWebGpuBuffer<ShapeBatchItem>>();
        this.mockFrame = Substitute.For<IFrame>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        this.mockFrameBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockBatchBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockRenderShapesUnsubscriber = Substitute.For<IDisposable>();
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
        var mockRenderBatchReactable = Substitute.For<IRenderBatchReactable<ShapeBatchItem>>();
        mockRenderBatchReactable
            .When(x => x.Subscribe(
                Arg.Any<IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>>();

                if (subscription.Id == PushNotifications.RenderShapesId)
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
            .Subscribe(Arg.Any<IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>>();

                return subscription.Id == PushNotifications.RenderShapesId
                    ? this.mockRenderShapesUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        var mockViewPortReactable = Substitute.For<IPushReactable<ViewPortSizeData>>();
        mockViewPortReactable
            .Subscribe(Arg.Any<IReceiveSubscription<ViewPortSizeData>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<ViewPortSizeData>>();

                return subscription.Id == PushNotifications.ViewPortSizeChangedId
                    ? this.mockViewPortUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        mockViewPortReactable
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
        this.mockReactableFactory.CreateRenderShapeReactable().Returns(mockRenderBatchReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            null,
            this.mockShapePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullPipelineParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            null,
            this.mockBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'pipeline')");
    }

    [Fact]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockShapePipeline,
            null,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'buffer')");
    }

    [Fact]
    public void Ctor_WithNullFrameParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockShapePipeline,
            this.mockBuffer,
            null,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'frame')");
    }

    [Fact]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockShapePipeline,
            this.mockBuffer,
            this.mockFrame,
            null,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'batchManager')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockShapePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            null);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Render_WithRectShapeAndBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IShapeRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(default(RectShape), 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WithCircleShapeAndBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IShapeRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(default(CircleShape), 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WithRectShape_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var rect = new RectShape
        {
            Position = new Vector2(10, 20),
            Width = 100f,
            Height = 200f,
            Color = Color.Red,
            IsSolid = true,
            BorderThickness = 5f,
            CornerRadius = new CornerRadius(10f),
            GradientType = ColorGradient.Horizontal,
            GradientStart = Color.Blue,
            GradientStop = Color.Green,
        };

        var expectedBatchItem = new ShapeBatchItem(
            rect.Position,
            rect.Width,
            rect.Height,
            rect.Color,
            rect.IsSolid,
            rect.BorderThickness,
            rect.CornerRadius,
            rect.GradientType,
            rect.GradientStart,
            rect.GradientStop);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(rect, 1);

        // Assert
        this.mockBatchingManager.Received(1).AddShapeItem(expectedBatchItem, 1, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithCircleShape_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var circle = new CircleShape
        {
            Position = new Vector2(10, 20),
            Diameter = 100f,
            Color = Color.Red,
            IsSolid = true,
            BorderThickness = 5f,
            GradientType = ColorGradient.Vertical,
            GradientStart = Color.Blue,
            GradientStop = Color.Green,
        };

        var expectedBatchItem = new ShapeBatchItem(
            circle.Position,
            circle.Diameter,
            circle.Diameter,
            circle.Color,
            circle.IsSolid,
            circle.BorderThickness,
            new CornerRadius(circle.Diameter / 2f),
            circle.GradientType,
            circle.GradientStart,
            circle.GradientStop);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(circle, 2);

        // Assert
        this.mockBatchingManager.Received(1).AddShapeItem(expectedBatchItem, 2, Arg.Any<DateTime>());
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
        this.mockRenderShapesUnsubscriber.Received(1).Dispose();
        this.mockViewPortUnsubscriber.Received(1).Dispose();
    }
    #endregion

    #region Internal Method Tests
    [Fact]
    public void RenderBatch_WithNoItemsToRender_DoesNotRenderBatch()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var itemsToRender = new Memory<RenderItem<ShapeBatchItem>>([]);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockBuffer.DidNotReceive().UploadData(Arg.Any<ShapeBatchItem>(), Arg.Any<uint>());
        this.mockBuffer.DidNotReceive().Draw(Arg.Any<SafeRenderPassEncoderHandle>(), Arg.Any<uint>(), Arg.Any<uint>());
    }

    [Fact]
    public void RenderBatch_WithNullFrameRenderPass_ThrowsException()
    {
        // Arrange
        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new ShapeBatchItem(
            new Vector2(10, 20),
            100f,
            200f,
            Color.Red,
            true,
            5f,
            new CornerRadius(10f),
            ColorGradient.None,
            Color.White,
            Color.White);
        var renderItem = new RenderItem<ShapeBatchItem> { Item = batchItem };
        var itemsToRender = new Memory<RenderItem<ShapeBatchItem>>([renderItem]);

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
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem1 = new ShapeBatchItem(
            new Vector2(10, 20),
            100f,
            200f,
            Color.Red,
            true,
            5f,
            new CornerRadius(10f),
            ColorGradient.Horizontal,
            Color.Blue,
            Color.Green);

        var batchItem2 = new ShapeBatchItem(
            new Vector2(30, 40),
            50f,
            60f,
            Color.Blue,
            false,
            3f,
            new CornerRadius(5f),
            ColorGradient.Vertical,
            Color.Yellow,
            Color.Orange);

        var renderItem1 = new RenderItem<ShapeBatchItem> { Item = batchItem1 };
        var renderItem2 = new RenderItem<ShapeBatchItem> { Item = batchItem2 };
        var itemsToRender = new Memory<RenderItem<ShapeBatchItem>>([renderItem1, renderItem2]);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockShapePipeline.Received(1).Bind(renderPassHandle);
        this.mockBuffer.Received(1).UploadData(batchItem1, 0);
        this.mockBuffer.Received(1).UploadData(batchItem2, 1);
        this.mockBuffer.Received(1).Draw(renderPassHandle, 2, 0);
    }

    [Fact]
    public void RenderBatch_WhenInvokedAcrossMultipleBatches_OffsetsGpuDataIndex()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new ShapeBatchItem(
            new Vector2(10, 20),
            100f,
            200f,
            Color.Red,
            true,
            5f,
            new CornerRadius(10f),
            ColorGradient.None,
            Color.White,
            Color.White);

        var renderItem = new RenderItem<ShapeBatchItem> { Item = batchItem };

        // First batch - 2 items
        var firstBatch = new Memory<RenderItem<ShapeBatchItem>>([renderItem, renderItem]);
        this.renderBatchSubscription.OnReceive(firstBatch);

        // Act - Second batch with 1 item
        var secondBatch = new Memory<RenderItem<ShapeBatchItem>>([renderItem]);
        this.renderBatchSubscription.OnReceive(secondBatch);

        // Assert
        // First batch: UploadData at indexes 0, 1; Draw with firstItem=0
        // Second batch: UploadData at index 2; Draw with firstItem=2
        this.mockBuffer.Received(3).UploadData(batchItem, Arg.Any<uint>());
        this.mockBuffer.Received(1).UploadData(batchItem, 2);
        this.mockBuffer.Received(1).Draw(renderPassHandle, 1, 2);
    }

    [Fact]
    public void RenderBatch_WhenFrameHasBegun_ResetsBatchOffset()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new ShapeBatchItem(
            new Vector2(10, 20),
            100f,
            200f,
            Color.Red,
            true,
            5f,
            new CornerRadius(10f),
            ColorGradient.None,
            Color.White,
            Color.White);
        var renderItem = new RenderItem<ShapeBatchItem> { Item = batchItem };

        // Simulate a render to advance the batch offset
        var batch = new Memory<RenderItem<ShapeBatchItem>>([renderItem]);
        this.renderBatchSubscription.OnReceive(batch);

        // Act — simulate a new frame begin
        this.frameBeginSubscription.OnReceive();
        this.renderBatchSubscription.OnReceive(batch);

        // Assert — after frame begin, offset resets to 0, so Draw is called with firstItem=0 both times
        this.mockBuffer.Received(2).Draw(renderPassHandle, 1, 0);
    }
    #endregion

    #region Reactable Subscription Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void BatchBeginSubscription_WhenUnsubscribing_DisposesUnsubscriber()
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
    public void RenderShapesSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.renderBatchSubscription.OnUnsubscribe();

        // Assert
        this.mockRenderShapesUnsubscriber.Received(1).Dispose();
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
        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSubscription.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockBuffer.WindowSize.ShouldBe(new Vector2(1920f, 1080f));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeRenderer"/> for the purpose of testing.
    /// </summary>
    /// <param name="wgpu">The WGPU invoker.</param>
    /// <param name="pipeline">The graphics shape pipeline.</param>
    /// <param name="buffer">The WebGPU buffer.</param>
    /// <param name="frame">The frame.</param>
    /// <param name="batchManager">The batching manager.</param>
    /// <param name="reactableFactory">The reactable factory.</param>
    /// <returns>The instance to test.</returns>
    private ShapeRenderer CreateSystemUnderTest(
        IWgpuInvoker? wgpu = null,
        IGraphicsShapePipeline? pipeline = null,
        IWebGpuBuffer<ShapeBatchItem>? buffer = null,
        IFrame? frame = null,
        IBatchingManager? batchManager = null,
        IReactableFactory? reactableFactory = null)
        => new (
            wgpu ?? this.mockWgpuInvoker,
            pipeline ?? this.mockShapePipeline,
            buffer ?? this.mockBuffer,
            frame ?? this.mockFrame,
            batchManager ?? this.mockBatchingManager,
            reactableFactory ?? this.mockReactableFactory);
}

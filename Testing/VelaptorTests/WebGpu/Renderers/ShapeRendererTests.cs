// <copyright file="ShapeRendererTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
// ReSharper disable CompareOfFloatsByEqualityOperator
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
using BufferUsage = Silk.NET.WebGPU.BufferUsage;
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
    private readonly IGraphicsDevice mockGraphicsDevice;
    private readonly IGraphicsSurface mockGraphicsSurface;
    private readonly IGraphicsShapePipeline mockShapePipeline;
    private readonly IGraphicsLinePipeline mockLinePipeline;
    private readonly IWebGpuBuffer<ShapeBatchItem> mockShapeBuffer;
    private readonly IWebGpuBuffer<LineBatchItem> mockLineBuffer;
    private readonly IFrame mockFrame;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IDisposable mockFrameBeginUnsubscriber;
    private readonly IDisposable mockBatchBeginUnsubscriber;
    private readonly IDisposable mockRenderShapesUnsubscriber;
    private readonly IDisposable mockRenderLinesUnsubscriber;
    private readonly IDisposable mockViewPortUnsubscriber;
    private IReceiveSubscription? frameBeginSubscription;
    private IReceiveSubscription? batchBeginSubscription;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSubscription;
    private IReceiveSubscription<Memory<RenderItem<ShapeBatchItem>>>? renderBatchSubscription;
    private IReceiveSubscription<Memory<RenderItem<LineBatchItem>>>? renderLineBatchSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeRendererTests"/> class.
    /// </summary>
    public ShapeRendererTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockGraphicsDevice = Substitute.For<IGraphicsDevice>();
        this.mockGraphicsSurface = Substitute.For<IGraphicsSurface>();
        this.mockShapePipeline = Substitute.For<IGraphicsShapePipeline>();
        this.mockLinePipeline = Substitute.For<IGraphicsLinePipeline>();
        this.mockShapeBuffer = Substitute.For<IWebGpuBuffer<ShapeBatchItem>>();
        this.mockLineBuffer = Substitute.For<IWebGpuBuffer<LineBatchItem>>();
        this.mockFrame = Substitute.For<IFrame>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        this.mockFrameBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockBatchBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockRenderShapesUnsubscriber = Substitute.For<IDisposable>();
        this.mockRenderLinesUnsubscriber = Substitute.For<IDisposable>();
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

        var mockRenderLineReactable = Substitute.For<IRenderBatchReactable<LineBatchItem>>();
        mockRenderLineReactable
            .When(x => x.Subscribe(
                Arg.Any<IReceiveSubscription<Memory<RenderItem<LineBatchItem>>>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<LineBatchItem>>>>();

                if (subscription.Id == PushNotifications.RenderLinesId)
                {
                    this.renderLineBatchSubscription = subscription;
                }
                else
                {
                    throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
                }
            });

        mockRenderLineReactable
            .Subscribe(Arg.Any<IReceiveSubscription<Memory<RenderItem<LineBatchItem>>>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<LineBatchItem>>>>();

                return subscription.Id == PushNotifications.RenderLinesId
                    ? this.mockRenderLinesUnsubscriber
                    : throw new Exception($"The ID '{subscription.Id}' is incorrect or not setup for proper testing.");
            });

        this.mockReactableFactory.CreateRenderLineReactable().Returns(mockRenderLineReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(mockViewPortReactable);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            null,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'wgpu')");
    }

    [Fact]
    public void Ctor_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            null,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'grfxDevice')");
    }

    [Fact]
    public void Ctor_WithNullSurfaceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            null,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'surface')");
    }

    [Fact]
    public void Ctor_WithNullPipelineParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            null,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'shapePipeline')");
    }

    [Fact]
    public void Ctor_WithNullLinePipelineParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            null,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'linePipeline')");
    }

    [Fact]
    public void Ctor_WithNullBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            null,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'shapeBuffer')");
    }

    [Fact]
    public void Ctor_WithNullLineBufferParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            null,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'lineBuffer')");
    }

    [Fact]
    public void Ctor_WithNullFrameParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new ShapeRenderer(
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
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
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
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
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
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
    public void Render_WithLineAndBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IShapeRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(new Line(new Vector2(1, 2), new Vector2(3, 4)), 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void RenderLine_WithStartEndOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var start = new Vector2(10, 20);
        var end = new Vector2(30, 40);
        var expectedBatchItem = new LineBatchItem(start, end, Color.White, 1u);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.RenderLine(start, end, 3);

        // Assert
        this.mockBatchingManager.Received(1).AddLineItem(expectedBatchItem, 3, Arg.Any<DateTime>());
    }

    [Fact]
    public void RenderLine_WithStartEndColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var start = new Vector2(10, 20);
        var end = new Vector2(30, 40);
        var color = Color.Red;
        var expectedBatchItem = new LineBatchItem(start, end, color, 1u);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.RenderLine(start, end, color, 4);

        // Assert
        this.mockBatchingManager.Received(1).AddLineItem(expectedBatchItem, 4, Arg.Any<DateTime>());
    }

    [Fact]
    public void RenderLine_WithStartEndThicknessOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var start = new Vector2(10, 20);
        var end = new Vector2(30, 40);
        var expectedBatchItem = new LineBatchItem(start, end, Color.White, 5u);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.RenderLine(start, end, 5u, 6);

        // Assert
        this.mockBatchingManager.Received(1).AddLineItem(expectedBatchItem, 6, Arg.Any<DateTime>());
    }

    [Fact]
    public void RenderLine_WithStartEndColorThicknessOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var start = new Vector2(10, 20);
        var end = new Vector2(30, 40);
        var color = Color.Blue;
        var expectedBatchItem = new LineBatchItem(start, end, color, 7u);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.RenderLine(start, end, color, 7u, 8);

        // Assert
        this.mockBatchingManager.Received(1).AddLineItem(expectedBatchItem, 8, Arg.Any<DateTime>());
    }

    [Fact]
    public void RenderLine_WithLineStructOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var line = new Line(new Vector2(10, 20), new Vector2(30, 40), Color.Green, 3f);
        var expectedBatchItem = new LineBatchItem(line.P1, line.P2, line.Color, (uint)line.Thickness);

        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(line, 9);

        // Assert
        this.mockBatchingManager.Received(1).AddLineItem(expectedBatchItem, 9, Arg.Any<DateTime>());
    }

    [Fact]
    public void RenderLine_WithLineStructOverloadAndBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IShapeRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(new Line(new Vector2(1, 2), new Vector2(3, 4)), 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
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
        this.mockRenderLinesUnsubscriber.Received(1).Dispose();
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
        this.mockShapeBuffer.DidNotReceive().UploadData(Arg.Any<ShapeBatchItem>(), Arg.Any<uint>());
        this.mockShapeBuffer.DidNotReceive().Draw(Arg.Any<SafeRenderPassEncoderHandle>(), Arg.Any<uint>(), Arg.Any<uint>());
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
        this.mockShapeBuffer.Received(1).UploadData(batchItem1, 0);
        this.mockShapeBuffer.Received(1).UploadData(batchItem2, 1);
        this.mockShapeBuffer.Received(1).Draw(renderPassHandle, 2, 0);
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
        this.mockShapeBuffer.Received(3).UploadData(batchItem, Arg.Any<uint>());
        this.mockShapeBuffer.Received(1).UploadData(batchItem, 2);
        this.mockShapeBuffer.Received(1).Draw(renderPassHandle, 1, 2);
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
        this.mockShapeBuffer.Received(2).Draw(renderPassHandle, 1, 0);
    }

    [Fact]
    public void RenderLineBatch_WithNoItemsToRender_DoesNotRenderBatch()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var itemsToRender = new Memory<RenderItem<LineBatchItem>>([]);

        // Act
        this.renderLineBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockLineBuffer.DidNotReceive().UploadData(Arg.Any<LineBatchItem>(), Arg.Any<uint>());
        this.mockLineBuffer.DidNotReceive().Draw(Arg.Any<SafeRenderPassEncoderHandle>(), Arg.Any<uint>(), Arg.Any<uint>());
    }

    [Fact]
    public void RenderLineBatch_WithNullFrameRenderPass_ThrowsException()
    {
        // Arrange
        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(30, 40),
            Color.Red,
            5u);
        var renderItem = new RenderItem<LineBatchItem> { Item = batchItem };
        var itemsToRender = new Memory<RenderItem<LineBatchItem>>([renderItem]);

        // Act
        var act = () => this.renderLineBatchSubscription.OnReceive(itemsToRender);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The `{nameof(IFrame.RenderPass)}` cannot be null.  Cannot render texture.");
    }

    [Fact]
    [SuppressMessage("ReSharper", "RedundantArgumentDefaultValue", Justification = "Needed for brevity")]
    public void RenderLineBatch_WhenInvoked_RendersBatch()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem1 = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(30, 40),
            Color.Red,
            5u);

        var batchItem2 = new LineBatchItem(
            new Vector2(50, 60),
            new Vector2(70, 80),
            Color.Blue,
            3u);

        var renderItem1 = new RenderItem<LineBatchItem> { Item = batchItem1 };
        var renderItem2 = new RenderItem<LineBatchItem> { Item = batchItem2 };
        var itemsToRender = new Memory<RenderItem<LineBatchItem>>([renderItem1, renderItem2]);

        // Act
        this.renderLineBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockLinePipeline.Received(1).Bind(renderPassHandle);
        this.mockLineBuffer.Received(1).UploadData(batchItem1, 0);
        this.mockLineBuffer.Received(1).UploadData(batchItem2, 1);
        this.mockLineBuffer.Received(1).Draw(renderPassHandle, 2, 0);
    }

    [Fact]
    public void RenderLineBatch_WhenInvokedAcrossMultipleBatches_OffsetsGpuDataIndex()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(30, 40),
            Color.Red,
            5u);

        var renderItem = new RenderItem<LineBatchItem> { Item = batchItem };

        // First batch - 2 items
        var firstBatch = new Memory<RenderItem<LineBatchItem>>([renderItem, renderItem]);
        this.renderLineBatchSubscription.OnReceive(firstBatch);

        // Act - Second batch with 1 item
        var secondBatch = new Memory<RenderItem<LineBatchItem>>([renderItem]);
        this.renderLineBatchSubscription.OnReceive(secondBatch);

        // Assert
        // First batch: UploadData at indexes 0, 1; Draw with firstItem=0
        // Second batch: UploadData at index 2; Draw with firstItem=2
        this.mockLineBuffer.Received(3).UploadData(batchItem, Arg.Any<uint>());
        this.mockLineBuffer.Received(1).UploadData(batchItem, 2);
        this.mockLineBuffer.Received(1).Draw(renderPassHandle, 1, 2);
    }

    [Fact]
    public void RenderLineBatch_WhenFrameHasBegun_ResetsBatchOffset()
    {
        // Arrange
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(30, 40),
            Color.Red,
            5u);
        var renderItem = new RenderItem<LineBatchItem> { Item = batchItem };

        // Simulate a render to advance the batch offset
        var batch = new Memory<RenderItem<LineBatchItem>>([renderItem]);
        this.renderLineBatchSubscription.OnReceive(batch);

        // Act — simulate a new frame begin
        this.frameBeginSubscription.OnReceive();
        this.renderLineBatchSubscription.OnReceive(batch);

        // Assert — after frame begin, offset resets to 0, so Draw is called with firstItem=0 both times
        this.mockLineBuffer.Received(2).Draw(renderPassHandle, 1, 0);
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
    public void RenderLinesSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.renderLineBatchSubscription.OnUnsubscribe();

        // Assert
        this.mockRenderLinesUnsubscriber.Received(1).Dispose();
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
        this.mockShapeBuffer.WindowSize.ShouldBe(new Vector2(1920f, 1080f));
        this.mockLineBuffer.WindowSize.ShouldBe(new Vector2(1920f, 1080f));
    }
    #endregion

    #region DPI Scale Uniform Tests
    [Fact]
    public void ViewportSubscription_WhenViewportChanges_CreatesDpiScaleUniformBuffer()
    {
        // Arrange
        var device = SetupGraphicsDevice();
        var vertexBuffer = new SafeVertexBufferHandle(this.mockWgpuInvoker, 0x5678);
        this.mockWgpuInvoker.DeviceCreateVertexBuffer(
                Arg.Any<SafeDeviceHandle>(), Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<BufferUsage>())
            .Returns(vertexBuffer);

        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSubscription.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockWgpuInvoker.Received(1).DeviceCreateVertexBuffer(
            device,
            "Shape DPI Scale Uniform Buffer",
            8,
            BufferUsage.Uniform | BufferUsage.CopyDst);
    }

    [Fact]
    public void ViewportSubscription_WhenViewportChanges_UploadsDpiScaleData()
    {
        // Arrange
        _ = SetupGraphicsDevice();
        var queue = this.mockGraphicsDevice.Queue;
        var vertexBuffer = new SafeVertexBufferHandle(this.mockWgpuInvoker, 0x5678);
        this.mockWgpuInvoker.DeviceCreateVertexBuffer(
                Arg.Any<SafeDeviceHandle>(), Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<BufferUsage>())
            .Returns(vertexBuffer);
        this.mockGraphicsSurface.FramebufferSize.Returns((3840, 2160));

        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSubscription.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockWgpuInvoker.Received(1).QueueWriteBuffer(
            queue,
            vertexBuffer.DangerousGetHandle(),
            0,
            Arg.Is<float[]>(data => data.Length == 2 && data[0] == 2f && data[1] == 2f));
    }

    [Fact]
    public void ViewportSubscription_WhenViewportChanges_CreatesDpiScaleBindGroup()
    {
        // Arrange
        var device = SetupGraphicsDevice();
        var vertexBuffer = new SafeVertexBufferHandle(this.mockWgpuInvoker, 0x5678);
        var bindGroupLayout = new SafeBindGroupLayoutHandle(this.mockWgpuInvoker, 0x9ABC);
        this.mockWgpuInvoker.DeviceCreateVertexBuffer(
                Arg.Any<SafeDeviceHandle>(), Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<BufferUsage>())
            .Returns(vertexBuffer);
        this.mockShapePipeline.BindGroupLayout.Returns(bindGroupLayout);

        _ = CreateSystemUnderTest();

        // Act
        this.viewPortSubscription.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

        // Assert
        this.mockWgpuInvoker.Received(1).DeviceCreateBufferBindGroupHandle(
            device,
            "Shape DPI Scale Bind Group",
            bindGroupLayout,
            vertexBuffer,
            0,
            8);
    }

    [Fact]
    public void RenderBatch_WhenDpiScaleBindGroupExists_SetsBindGroup()
    {
        // Arrange
        _ = SetupGraphicsDevice();
        var vertexBuffer = new SafeVertexBufferHandle(this.mockWgpuInvoker, 0x5678);
        var bindGroupLayout = new SafeBindGroupLayoutHandle(this.mockWgpuInvoker, 0x9ABC);
        var bindGroup = new SafeBindGroupHandle(this.mockWgpuInvoker, 0xBEEF);
        var renderPassHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x123);

        this.mockWgpuInvoker.DeviceCreateVertexBuffer(
                Arg.Any<SafeDeviceHandle>(), Arg.Any<string>(), Arg.Any<ulong>(), Arg.Any<BufferUsage>())
            .Returns(vertexBuffer);
        this.mockShapePipeline.BindGroupLayout.Returns(bindGroupLayout);
        this.mockWgpuInvoker.DeviceCreateBufferBindGroupHandle(
                Arg.Any<SafeDeviceHandle>(),
                Arg.Any<string>(),
                Arg.Any<SafeBindGroupLayoutHandle>(),
                Arg.Any<SafeVertexBufferHandle>(),
                Arg.Any<ulong>(),
                Arg.Any<ulong>())
            .Returns(bindGroup);
        this.mockFrame.RenderPass.Returns(renderPassHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();
        this.viewPortSubscription.OnReceive(new ViewPortSizeData { Width = 1920, Height = 1080 });

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
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockWgpuInvoker.Received(1).RenderPassEncoderSetBindGroup(renderPassHandle, 0, bindGroup, 0, 0);
    }

    [Fact]
    public void RenderBatch_WhenDpiScaleBindGroupDoesNotExist_DoesNotSetBindGroup()
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
        var itemsToRender = new Memory<RenderItem<ShapeBatchItem>>([renderItem]);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockWgpuInvoker.DidNotReceive().RenderPassEncoderSetBindGroup(
            Arg.Any<SafeRenderPassEncoderHandle>(),
            Arg.Any<uint>(),
            Arg.Any<SafeBindGroupHandle>(),
            Arg.Any<nuint>(),
            Arg.Any<nint>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ShapeRenderer CreateSystemUnderTest()
        => new (
            this.mockWgpuInvoker,
            this.mockGraphicsDevice,
            this.mockGraphicsSurface,
            this.mockShapePipeline,
            this.mockLinePipeline,
            this.mockShapeBuffer,
            this.mockLineBuffer,
            this.mockFrame,
            this.mockBatchingManager,
            this.mockReactableFactory);

    /// <summary>
    /// Configures the mock graphics device so that <see cref="ShapeRenderer"/> treats it as initialized,
    /// enabling the DPI scale uniform buffer and bind group to be created.
    /// </summary>
    /// <returns>The configured device handle.</returns>
    private SafeDeviceHandle SetupGraphicsDevice()
    {
        var device = new SafeDeviceHandle(this.mockWgpuInvoker, 0x1234);
        var queue = new SafeQueueHandle(this.mockWgpuInvoker, device);

        this.mockGraphicsDevice.Handle.Returns(device);
        this.mockGraphicsDevice.Queue.Returns(queue);

        return device;
    }
}

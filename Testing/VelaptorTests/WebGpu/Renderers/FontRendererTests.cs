// <copyright file="FontRendererTests.cs" company="KinsonDigital">
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
using Velaptor.Content;
using Velaptor.Content.Fonts;
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
/// Tests the <see cref="FontRenderer"/> class.
/// </summary>
public class FontRendererTests : TestsBase
{
    private const uint AtlasTextureId = 789u;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsTexturePipeline mockTexturePipeline;
    private readonly IWebGpuBuffer<FontGlyphBatchItem> mockBuffer;
    private readonly IFrame mockFrame;
    private readonly IBatchingManager mockBatchingManager;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable<ViewPortSizeData> mockViewPortReactable;
    private readonly IDisposable mockFrameBeginUnsubscriber;
    private readonly IDisposable mockBatchBeginUnsubscriber;
    private readonly IDisposable mockRenderFontsUnsubscriber;
    private readonly IDisposable mockViewPortUnsubscriber;
    private readonly TextureBindGroupRegistry bindGroupRegistry;
    private IReceiveSubscription? frameBeginSubscription;
    private IReceiveSubscription? batchBeginSubscription;
    private IReceiveSubscription<ViewPortSizeData>? viewPortSubscription;
    private IReceiveSubscription<Memory<RenderItem<FontGlyphBatchItem>>>? renderBatchSubscription;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontRendererTests"/> class.
    /// </summary>
    public FontRendererTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.mockTexturePipeline = Substitute.For<IGraphicsTexturePipeline>();
        this.mockBuffer = Substitute.For<IWebGpuBuffer<FontGlyphBatchItem>>();
        this.mockFrame = Substitute.For<IFrame>();
        this.mockBatchingManager = Substitute.For<IBatchingManager>();

        this.mockFrameBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockBatchBeginUnsubscriber = Substitute.For<IDisposable>();
        this.mockRenderFontsUnsubscriber = Substitute.For<IDisposable>();
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
        var mockRenderBatchReactable = Substitute.For<IRenderBatchReactable<FontGlyphBatchItem>>();
        mockRenderBatchReactable
            .When(x => x.Subscribe(
                Arg.Any<IReceiveSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>()))
            .Do(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>();

                if (subscription.Id == PushNotifications.RenderFontsId)
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
            .Subscribe(Arg.Any<IReceiveSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>())
            .Returns(callInfo =>
            {
                var subscription = callInfo.Arg<IReceiveSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>();

                return subscription.Id == PushNotifications.RenderFontsId
                    ? this.mockRenderFontsUnsubscriber
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
        this.mockReactableFactory.CreateRenderFontReactable().Returns(mockRenderBatchReactable);
        this.mockReactableFactory.CreateViewPortReactable().Returns(this.mockViewPortReactable);

        this.bindGroupRegistry = new TextureBindGroupRegistry();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullWgpuParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new FontRenderer(
            null,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
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
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            null,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
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
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            null,
            this.mockFrame,
            this.bindGroupRegistry,
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
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            null,
            this.bindGroupRegistry,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'frame')");
    }

    [Fact]
    public void Ctor_WithNullBindGroupRegistryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            null,
            this.mockBatchingManager,
            this.mockReactableFactory);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'bindGroupRegistry')");
    }

    [Fact]
    public void Ctor_WithNullBatchManagerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
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
        var act = () => new FontRenderer(
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
            this.mockBatchingManager,
            null);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Render_WhenBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        var mockFont = CreateMockFont();
        const string expectedMsg =
            $"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IFontRenderer.Render)}()' methods.";
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Render(mockFont, "test", 10, 20, 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe(expectedMsg);
    }

    [Fact]
    public void Render_WithNullFontParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        var act = () => sut.Render(null, "test", 10, 20, 0);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe($"Cannot render a null '{nameof(IFont)}'. (Parameter 'font')");
    }

    [Fact]
    public void Render_WithZeroFontSize_DoesNotAddToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont(size: 0);
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "test", 10, 20, 0);

        // Assert
        this.mockBatchingManager.DidNotReceive().AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithEmptyText_DoesNotAddToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, string.Empty, 10, 20, 0);

        // Assert
        this.mockBatchingManager.DidNotReceive().AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A\r\nB", 10, 20, 1f, 0f, Color.White, 0);

        // Assert
        this.mockBatchingManager.Received(2).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithVector2PositionOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", new Vector2(10, 20), 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithRenderSizeAndAngleOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", 10, 20, 1.5f, 45f, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithVector2PositionAndRenderSizeAndAngleOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", new Vector2(10, 20), 1.5f, 45f, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", 10, 20, Color.Red, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithVector2PositionAndColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", new Vector2(10, 20), Color.Red, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithAngleAndColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", 10, 20, 45f, Color.Red, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithVector2PositionAndAngleAndColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", new Vector2(10, 20), 45f, Color.Red, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithRenderSizeAndAngleAndColorOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        // Act
        sut.Render(mockFont, "A", 10, 20, 1.5f, 45f, Color.Red, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithGlyphMetricsOverload_WhenInvoked_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var glyphMetrics = new[]
        {
            (CreateGlyphMetrics('A'), Color.White),
        };

        // Act
        sut.Render(mockFont, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithGlyphMetricsOverload_WithNullFontParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var glyphMetrics = new[]
        {
            (CreateGlyphMetrics('A'), Color.White),
        };

        // Act
        var act = () => sut.Render(null, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe($"Cannot render a null '{nameof(IFont)}'. (Parameter 'font')");
    }

    [Fact]
    public void Render_WithGlyphMetricsOverload_WithZeroFontSize_DoesNotAddToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont(size: 0);
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var glyphMetrics = new[]
        {
            (CreateGlyphMetrics('A'), Color.White),
        };

        // Act
        sut.Render(mockFont, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        this.mockBatchingManager.DidNotReceive().AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithGlyphMetricsOverload_WithEmptyGlyphs_DoesNotAddToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var glyphMetrics = Array.Empty<(GlyphMetrics, Color)>();

        // Act
        sut.Render(mockFont, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        this.mockBatchingManager.DidNotReceive().AddFontItem(Arg.Any<FontGlyphBatchItem>(), Arg.Any<int>(), Arg.Any<DateTime>());
    }

    [Fact]
    public void Render_WithGlyphMetricsOverload_WhenBatchHasNotBegun_ThrowsException()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();

        var glyphMetrics = new[]
        {
            (CreateGlyphMetrics('A'), Color.White),
        };

        // Act
        var act = () => sut.Render(mockFont, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(IBatcher.Begin)}()' method must be invoked first before any '{nameof(IFontRenderer.Render)}()' methods.");
    }

    [Fact]
    public void Render_WithNegativeHoriBearingY_AddsItemToBatchManager()
    {
        // Arrange
        var mockFont = CreateMockFont();
        var sut = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var glyphMetrics = new[]
        {
            (CreateGlyphMetrics('_', horiBearingY: -5f), Color.White),
        };

        // Act
        sut.Render(mockFont, new Span<(GlyphMetrics, Color)>(glyphMetrics), 10, 20, 1f, 0f, 0);

        // Assert
        this.mockBatchingManager.Received(1).AddFontItem(Arg.Any<FontGlyphBatchItem>(), 0, Arg.Any<DateTime>());
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
        this.mockRenderFontsUnsubscriber.Received(1).Dispose();
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

        var itemsToRender = new Memory<RenderItem<FontGlyphBatchItem>>([]);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockTexturePipeline.Received(1).Bind(renderPassHandle);
        this.mockBuffer.DidNotReceive().UploadData(Arg.Any<FontGlyphBatchItem>(), Arg.Any<uint>());
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
        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem = new FontGlyphBatchItem(
            new RectangleF(5, 6, 50, 60),
            new RectangleF(11, 22, 100, 200),
            'A',
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);
        var renderItem = new RenderItem<FontGlyphBatchItem> { Item = batchItem, };
        var itemsToRender = new Memory<RenderItem<FontGlyphBatchItem>>([renderItem]);

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
        var bindGroupHandle = new SafeBindGroupHandle(this.mockWgpuInvoker, 0x497);

        this.mockFrame.RenderPass.Returns(renderPassHandle);

        // Bind the texture ID
        this.bindGroupRegistry.Register(AtlasTextureId, bindGroupHandle);

        _ = CreateSystemUnderTest();
        this.batchBeginSubscription.OnReceive();

        var batchItem1 = new FontGlyphBatchItem(
            new RectangleF(5, 6, 50, 60),
            new RectangleF(11, 22, 100, 200),
            'A',
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);

        var batchItem2 = new FontGlyphBatchItem(
            new RectangleF(5, 6, 50, 60),
            new RectangleF(11, 22, 100, 200),
            'B',
            1f,
            0f,
            Color.White,
            RenderEffects.None,
            AtlasTextureId);

        var renderItem1 = new RenderItem<FontGlyphBatchItem> { Item = batchItem1, };
        var renderItem2 = new RenderItem<FontGlyphBatchItem> { Item = batchItem2, };
        var itemsToRender = new Memory<RenderItem<FontGlyphBatchItem>>([renderItem1, renderItem2]);

        // Act
        this.renderBatchSubscription.OnReceive(itemsToRender);

        // Assert
        this.mockTexturePipeline.Received(1).Bind(renderPassHandle);
        this.mockBuffer.Received(1).UploadData(batchItem1, 0);
        this.mockBuffer.Received(1).UploadData(batchItem2, 1);
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
    public void RenderFontsSubscription_WhenUnsubscribing_DisposesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.renderBatchSubscription.OnUnsubscribe();

        // Assert
        this.mockRenderFontsUnsubscriber.Received(1).Dispose();
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
    /// Creates a mock <see cref="IFont"/> for testing.
    /// </summary>
    /// <param name="size">The font size.</param>
    /// <returns>The mock font.</returns>
    private static IFont CreateMockFont(uint size = 12)
    {
        var mockAtlas = Substitute.For<ITexture>();
        mockAtlas.Id.Returns(AtlasTextureId);
        mockAtlas.Width.Returns(200u);
        mockAtlas.Height.Returns(200u);

        var mockFont = Substitute.For<IFont>();
        mockFont.Size.Returns(size);
        mockFont.Atlas.Returns(mockAtlas);
        mockFont.LineSpacing.Returns(20f);
        mockFont.Measure(Arg.Any<string>()).Returns(new SizeF(100f, 20f));
        mockFont.GetKerning(Arg.Any<uint>(), Arg.Any<uint>()).Returns(0f);

        mockFont.ToGlyphMetrics(Arg.Any<string>())
            .Returns(callInfo =>
            {
                var text = callInfo.Arg<string>();
                var metrics = new GlyphMetrics[text.Length];

                for (var i = 0; i < text.Length; i++)
                {
                    metrics[i] = CreateGlyphMetrics(text[i]);
                }

                return metrics;
            });

        return mockFont;
    }

    /// <summary>
    /// Creates a <see cref="GlyphMetrics"/> instance for testing.
    /// </summary>
    /// <param name="glyph">The glyph character.</param>
    /// <param name="horiBearingY">The horizontal bearing Y value.</param>
    /// <returns>The glyph metrics.</returns>
    private static GlyphMetrics CreateGlyphMetrics(char glyph, float horiBearingY = 15f)
    {
        return new GlyphMetrics
        {
            Glyph = glyph,
            GlyphBounds = new RectangleF(5, 6, 50, 60),
            HoriBearingX = 10f,
            HoriBearingY = horiBearingY,
            HorizontalAdvance = 60f,
            GlyphWidth = 50f,
            GlyphHeight = 60f,
            CharIndex = glyph,
        };
    }

    /// <summary>
    /// Creates a new instance of <see cref="FontRenderer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontRenderer CreateSystemUnderTest()
        => new (
            this.mockWgpuInvoker,
            this.mockTexturePipeline,
            this.mockBuffer,
            this.mockFrame,
            this.bindGroupRegistry,
            this.mockBatchingManager,
            this.mockReactableFactory);
}

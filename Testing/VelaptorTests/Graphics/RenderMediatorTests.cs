// <copyright file="RenderMediatorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using System.Collections.Generic;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Tests the <see cref="RenderMediator"/> class.
/// </summary>
public class RenderMediatorTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IDisposable> mockEndBatchUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IComparer<RenderItem<TextureBatchItem>>> mockTextureComparer;
    private readonly Mock<IComparer<RenderItem<FontGlyphBatchItem>>> mockFontComparer;
    private readonly Mock<IComparer<RenderItem<ShapeBatchItem>>> mockShapeComparer;
    private readonly Mock<IComparer<RenderItem<LineBatchItem>>> mockLineComparer;
    private readonly Mock<IRenderBatchReactable<TextureBatchItem>> mockTextureRenderBatchReactable;
    private readonly Mock<IRenderBatchReactable<FontGlyphBatchItem>> mockFontRenderBatchReactable;
    private readonly Mock<IRenderBatchReactable<ShapeBatchItem>> mockShapeRenderBatchReactable;
    private readonly Mock<IRenderBatchReactable<LineBatchItem>> mockLineRenderBatchReactable;

    private readonly Mock<IBatchPullReactable<TextureBatchItem>> mockTexturePullReactable;
    private readonly Mock<IBatchPullReactable<FontGlyphBatchItem>> mockFontPullReactable;
    private readonly Mock<IBatchPullReactable<ShapeBatchItem>> mockShapePullReactable;
    private readonly Mock<IBatchPullReactable<LineBatchItem>> mockLinePullReactable;

    private IReceiveSubscription? endBatchReactor;
    private IReceiveSubscription? shutDownReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorTests"/> class.
    /// </summary>
    public RenderMediatorTests()
    {
        this.mockEndBatchUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber = new Mock<IDisposable>();

        this.mockPushReactable = new Mock<IPushReactable>();
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasEndedId)
                {
                    reactor.Name.Should().Be($"RenderMediatorTests.Ctor - {nameof(PushNotifications.BatchHasEndedId)}");
                    this.endBatchReactor = reactor;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    reactor.Name.Should().Be($"RenderMediatorTests.Ctor - {nameof(PushNotifications.SystemShuttingDownId)}");
                    this.shutDownReactor = reactor;
                }
            })
            .Returns<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.BatchHasEndedId)
                {
                    return this.mockEndBatchUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The event ID '{reactor.Id}' is not setup for testing.");
                return null;
            });

        this.mockTexturePullReactable = new Mock<IBatchPullReactable<TextureBatchItem>>();
        this.mockFontPullReactable = new Mock<IBatchPullReactable<FontGlyphBatchItem>>();
        this.mockShapePullReactable = new Mock<IBatchPullReactable<ShapeBatchItem>>();
        this.mockLinePullReactable = new Mock<IBatchPullReactable<LineBatchItem>>();

        this.mockTextureRenderBatchReactable = new Mock<IRenderBatchReactable<TextureBatchItem>>();
        this.mockFontRenderBatchReactable = new Mock<IRenderBatchReactable<FontGlyphBatchItem>>();
        this.mockShapeRenderBatchReactable = new Mock<IRenderBatchReactable<ShapeBatchItem>>();
        this.mockLineRenderBatchReactable = new Mock<IRenderBatchReactable<LineBatchItem>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable()).Returns(this.mockPushReactable.Object);

        this.mockReactableFactory.Setup(m => m.CreateTexturePullBatchReactable())
            .Returns(this.mockTexturePullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateFontPullBatchReactable()).
            Returns(this.mockFontPullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateShapePullBatchReactable()).
            Returns(this.mockShapePullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateLinePullBatchReactable()).
            Returns(this.mockLinePullReactable.Object);

        this.mockReactableFactory.Setup(m => m.CreateRenderTextureReactable())
            .Returns(this.mockTextureRenderBatchReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderFontReactable())
            .Returns(this.mockFontRenderBatchReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderShapeReactable())
            .Returns(this.mockShapeRenderBatchReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRenderLineReactable())
            .Returns(this.mockLineRenderBatchReactable.Object);

        this.mockTextureComparer = new Mock<IComparer<RenderItem<TextureBatchItem>>>();
        this.mockFontComparer = new Mock<IComparer<RenderItem<FontGlyphBatchItem>>>();
        this.mockShapeComparer = new Mock<IComparer<RenderItem<ShapeBatchItem>>>();
        this.mockLineComparer = new Mock<IComparer<RenderItem<LineBatchItem>>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                null,
                this.mockTextureComparer.Object,
                this.mockFontComparer.Object,
                this.mockShapeComparer.Object,
                this.mockLineComparer.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullTextureItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory.Object,
                null,
                this.mockFontComparer.Object,
                this.mockShapeComparer.Object,
                this.mockLineComparer.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'textureItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullFontItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory.Object,
                this.mockTextureComparer.Object,
                null,
                this.mockShapeComparer.Object,
                this.mockLineComparer.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullShapeItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory.Object,
                this.mockTextureComparer.Object,
                this.mockFontComparer.Object,
                null,
                this.mockLineComparer.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'shapeItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullNullItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory.Object,
                this.mockTextureComparer.Object,
                this.mockFontComparer.Object,
                this.mockShapeComparer.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'lineItemComparer')");
    }
    #endregion

    #region Indirect Tests
    [Fact]
    public void ShutDownReactable_WhenReceivingNotification_ShutsDownMediator()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnReceive();

        // Assert
        this.mockEndBatchUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
    }

    [Fact]
    public void PushReactable_WithBatchEndNotification_CoordinatesRenderCalls()
    {
        // Arrange
        var textureItemA = CreateRenderItem(BatchItemFactory.CreateTextureItemWithOrderedValues(), 4);
        var fontItemA = CreateRenderItem(BatchItemFactory.CreateFontItemWithOrderedValues(), 3);
        var shapeItemA = CreateRenderItem(BatchItemFactory.CreateShapeItemWithOrderedValues(), 2);
        var lineItemA = CreateRenderItem(BatchItemFactory.CreateLineItemWithOrderedValues(), 1);

        var textureItemB = CreateRenderItem(BatchItemFactory.CreateTextureItemWithOrderedValues(), 4);
        var fontItemB = CreateRenderItem(BatchItemFactory.CreateFontItemWithOrderedValues(), 3);
        var shapeItemB = CreateRenderItem(BatchItemFactory.CreateShapeItemWithOrderedValues(), 2);
        var lineItemB = CreateRenderItem(BatchItemFactory.CreateLineItemWithOrderedValues(), 1);

        var textureItems = new[] { textureItemA, textureItemB };
        var fontItems = new[] { fontItemA, fontItemB };
        var shapeItems = new[] { shapeItemA, shapeItemB };
        var lineItems = new[] { lineItemA, lineItemB };

        this.mockTexturePullReactable.Setup(m => m.Pull(It.IsAny<Guid>()))
            .Returns<Guid>(_ => new Memory<RenderItem<TextureBatchItem>>(textureItems));

        this.mockFontPullReactable.Setup(m => m.Pull(It.IsAny<Guid>()))
            .Returns<Guid>(_ => new Memory<RenderItem<FontGlyphBatchItem>>(fontItems));

        this.mockShapePullReactable.Setup(m => m.Pull(It.IsAny<Guid>()))
            .Returns<Guid>(_ => new Memory<RenderItem<ShapeBatchItem>>(shapeItems));

        this.mockLinePullReactable.Setup(m => m.Pull(It.IsAny<Guid>()))
            .Returns<Guid>(_ => new Memory<RenderItem<LineBatchItem>>(lineItems));

        this.mockTextureRenderBatchReactable
            .Setup(m => m.Push(It.Ref<Memory<RenderItem<TextureBatchItem>>>.IsAny, It.IsAny<Guid>()))
            .Callback(AssertTextureItems);

        this.mockFontRenderBatchReactable
            .Setup(m => m.Push(It.Ref<Memory<RenderItem<FontGlyphBatchItem>>>.IsAny, It.IsAny<Guid>()))
            .Callback(AssertFontItems);

        this.mockShapeRenderBatchReactable
            .Setup(m => m.Push(It.Ref<Memory<RenderItem<ShapeBatchItem>>>.IsAny, It.IsAny<Guid>()))
            .Callback(AssertShapeItems);

        this.mockLineRenderBatchReactable
            .Setup(m => m.Push(It.Ref<Memory<RenderItem<LineBatchItem>>>.IsAny, It.IsAny<Guid>()))
            .Callback(AssertLineItems);

        _ = CreateSystemUnderTest();

        // Act
        this.endBatchReactor.OnReceive();

        // Assert
        this.mockTexturePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetTextureItemsId));
        this.mockFontPullReactable.VerifyOnce(m => m.Pull(PullResponses.GetFontItemsId));
        this.mockShapePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetShapeItemsId));
        this.mockLinePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetLineItemsId));

        void AssertTextureItems(in Memory<RenderItem<TextureBatchItem>> data, Guid eventId)
        {
            eventId.Should().Be(PushNotifications.RenderTexturesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertFontItems(in Memory<RenderItem<FontGlyphBatchItem>> data, Guid eventId)
        {
            eventId.Should().Be(PushNotifications.RenderFontsId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertShapeItems(in Memory<RenderItem<ShapeBatchItem>> data, Guid eventId)
        {
            eventId.Should().Be(PushNotifications.RenderShapesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertLineItems(in Memory<RenderItem<LineBatchItem>> data, Guid eventId)
        {
            eventId.Should().Be(PushNotifications.RenderLinesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        this.mockPushReactable.VerifyOnce(m => m.Push(PushNotifications.EmptyBatchId));
    }
    #endregion

    private static RenderItem<T> CreateRenderItem<T>(T item, int layer) => new () { Layer = layer, Item = item };

    /// <summary>
    /// Creates a new instance of <see cref="RenderMediator"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RenderMediator CreateSystemUnderTest()
        => new (this.mockReactableFactory.Object,
            this.mockTextureComparer.Object,
            this.mockFontComparer.Object,
            this.mockShapeComparer.Object,
            this.mockLineComparer.Object);
}

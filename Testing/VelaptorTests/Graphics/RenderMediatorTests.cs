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
public class RenderMediatorTests : TestsBase
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
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

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorTests"/> class.
    /// </summary>
    public RenderMediatorTests()
    {
        var mockEndBatchUnsubscriber = new Mock<IDisposable>();

        this.mockPushReactable = new Mock<IPushReactable>();
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor => this.endBatchReactor = reactor)
            .Returns<IReceiveSubscription>(_ => mockEndBatchUnsubscriber.Object);

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
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
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
            .WithMessage("Value cannot be null. (Parameter 'textureItemComparer')");
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
            .WithMessage("Value cannot be null. (Parameter 'fontItemComparer')");
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
            .WithMessage("Value cannot be null. (Parameter 'shapeItemComparer')");
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
            .WithMessage("Value cannot be null. (Parameter 'lineItemComparer')");
    }
    #endregion

    #region Indirect Tests
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
            .Setup(m => m.Push(It.IsAny<Guid>(), It.Ref<Memory<RenderItem<TextureBatchItem>>>.IsAny))
            .Callback(AssertTextureItems);

        this.mockFontRenderBatchReactable
            .Setup(m => m.Push(It.IsAny<Guid>(), It.Ref<Memory<RenderItem<FontGlyphBatchItem>>>.IsAny))
            .Callback(AssertFontItems);

        this.mockShapeRenderBatchReactable
            .Setup(m => m.Push(It.IsAny<Guid>(), It.Ref<Memory<RenderItem<ShapeBatchItem>>>.IsAny))
            .Callback(AssertShapeItems);

        this.mockLineRenderBatchReactable
            .Setup(m => m.Push(It.IsAny<Guid>(), It.Ref<Memory<RenderItem<LineBatchItem>>>.IsAny))
            .Callback(AssertLineItems);

        _ = CreateSystemUnderTest();

        // Act
        this.endBatchReactor.OnReceive();

        // Assert
        this.mockTexturePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetTextureItemsId));
        this.mockFontPullReactable.VerifyOnce(m => m.Pull(PullResponses.GetFontItemsId));
        this.mockShapePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetShapeItemsId));
        this.mockLinePullReactable.VerifyOnce(m => m.Pull(PullResponses.GetLineItemsId));

        void AssertTextureItems(Guid eventId, in Memory<RenderItem<TextureBatchItem>> data)
        {
            eventId.Should().Be(PushNotifications.RenderTexturesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertFontItems(Guid eventId, in Memory<RenderItem<FontGlyphBatchItem>> data)
        {
            eventId.Should().Be(PushNotifications.RenderFontsId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertShapeItems(Guid eventId, in Memory<RenderItem<ShapeBatchItem>> data)
        {
            eventId.Should().Be(PushNotifications.RenderShapesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        void AssertLineItems(Guid eventId, in Memory<RenderItem<LineBatchItem>> data)
        {
            eventId.Should().Be(PushNotifications.RenderLinesId);
            data.Span.ToArray().Should().HaveCount(2);
        }

        this.mockPushReactable.VerifyOnce(m => m.Push(PushNotifications.EmptyBatchId));
    }
    #endregion

    #region Reacteable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void EndBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Assert
        this.mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
            {
                reactor.Should().NotBeNull("It is required for unit testing.");
                reactor.Name.Should().Be($"RenderMediator.ctor() - {PushNotifications.BatchHasEndedId}");
            });

        // Act
        _ = CreateSystemUnderTest();
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

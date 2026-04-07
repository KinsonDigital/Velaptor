// <copyright file="RenderMediatorTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using System.Collections.Generic;
using Carbonate.Core.NonDirectional;
using Carbonate.NonDirectional;
using Shouldly;
using Helpers;
using NSubstitute;
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
    private readonly IReactableFactory mockReactableFactory;
    private readonly IPushReactable mockPushReactable;
    private readonly IComparer<RenderItem<TextureBatchItem>> mockTextureComparer;
    private readonly IComparer<RenderItem<FontGlyphBatchItem>> mockFontComparer;
    private readonly IComparer<RenderItem<ShapeBatchItem>> mockShapeComparer;
    private readonly IComparer<RenderItem<LineBatchItem>> mockLineComparer;
    private readonly IRenderBatchReactable<TextureBatchItem> mockTextureRenderBatchReactable;
    private readonly IRenderBatchReactable<FontGlyphBatchItem> mockFontRenderBatchReactable;
    private readonly IRenderBatchReactable<ShapeBatchItem> mockShapeRenderBatchReactable;
    private readonly IRenderBatchReactable<LineBatchItem> mockLineRenderBatchReactable;
    private readonly IBatchPullReactable<TextureBatchItem> mockTexturePullReactable;
    private readonly IBatchPullReactable<FontGlyphBatchItem> mockFontPullReactable;
    private readonly IBatchPullReactable<ShapeBatchItem> mockShapePullReactable;
    private readonly IBatchPullReactable<LineBatchItem> mockLinePullReactable;

    private IReceiveSubscription? endBatchReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RenderMediatorTests"/> class.
    /// </summary>
    public RenderMediatorTests()
    {
        var mockEndBatchUnsubscriber = Substitute.For<IDisposable>();
        this.mockPushReactable = Substitute.For<IPushReactable>();
        this.mockPushReactable
            .Subscribe(Arg.Any<IReceiveSubscription>())
            .Returns(_ => mockEndBatchUnsubscriber)
            .AndDoes(ci =>
            {
                this.endBatchReactor = ci.Arg<IReceiveSubscription>();
            });

        this.mockTexturePullReactable = Substitute.For<IBatchPullReactable<TextureBatchItem>>();
        this.mockFontPullReactable = Substitute.For<IBatchPullReactable<FontGlyphBatchItem>>();
        this.mockShapePullReactable = Substitute.For<IBatchPullReactable<ShapeBatchItem>>();
        this.mockLinePullReactable = Substitute.For<IBatchPullReactable<LineBatchItem>>();

        this.mockTextureRenderBatchReactable = Substitute.For<IRenderBatchReactable<TextureBatchItem>>();
        this.mockFontRenderBatchReactable = Substitute.For<IRenderBatchReactable<FontGlyphBatchItem>>();
        this.mockShapeRenderBatchReactable = Substitute.For<IRenderBatchReactable<ShapeBatchItem>>();
        this.mockLineRenderBatchReactable = Substitute.For<IRenderBatchReactable<LineBatchItem>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(this.mockPushReactable);

        this.mockReactableFactory.CreateTexturePullBatchReactable()
            .Returns(this.mockTexturePullReactable);
        this.mockReactableFactory.CreateFontPullBatchReactable()
            .Returns(this.mockFontPullReactable);
        this.mockReactableFactory.CreateShapePullBatchReactable()
            .Returns(this.mockShapePullReactable);
        this.mockReactableFactory.CreateLinePullBatchReactable()
            .Returns(this.mockLinePullReactable);
        this.mockReactableFactory.CreateRenderTextureReactable()
            .Returns(this.mockTextureRenderBatchReactable);
        this.mockReactableFactory.CreateRenderFontReactable()
            .Returns(this.mockFontRenderBatchReactable);
        this.mockReactableFactory.CreateRenderShapeReactable()
            .Returns(this.mockShapeRenderBatchReactable);
        this.mockReactableFactory.CreateRenderLineReactable()
            .Returns(this.mockLineRenderBatchReactable);

        this.mockTextureComparer = Substitute.For<IComparer<RenderItem<TextureBatchItem>>>();
        this.mockFontComparer = Substitute.For<IComparer<RenderItem<FontGlyphBatchItem>>>();
        this.mockShapeComparer = Substitute.For<IComparer<RenderItem<ShapeBatchItem>>>();
        this.mockLineComparer = Substitute.For<IComparer<RenderItem<LineBatchItem>>>();
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
                this.mockTextureComparer,
                this.mockFontComparer,
                this.mockShapeComparer,
                this.mockLineComparer);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WithNullTextureItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory,
                null,
                this.mockFontComparer,
                this.mockShapeComparer,
                this.mockLineComparer);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'textureItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullFontItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory,
                this.mockTextureComparer,
                null,
                this.mockShapeComparer,
                this.mockLineComparer);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'fontItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullShapeItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory,
                this.mockTextureComparer,
                this.mockFontComparer,
                null,
                this.mockLineComparer);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'shapeItemComparer')");
    }

    [Fact]
    public void Ctor_WithNullNullItemComparerParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RenderMediator(
                this.mockReactableFactory,
                this.mockTextureComparer,
                this.mockFontComparer,
                this.mockShapeComparer,
                null);
        };

        // Assert
        var exception = Should.Throw<ArgumentNullException>(act);
        exception.Message.ShouldBe("Value cannot be null. (Parameter 'lineItemComparer')");
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

        this.mockTexturePullReactable
            .Pull(Arg.Any<Guid>())
            .Returns(_ => new Memory<RenderItem<TextureBatchItem>>(textureItems));

        this.mockFontPullReactable
            .Pull(Arg.Any<Guid>())
            .Returns(_ => new Memory<RenderItem<FontGlyphBatchItem>>(fontItems));

        this.mockShapePullReactable
            .Pull(Arg.Any<Guid>())
            .Returns(_ => new Memory<RenderItem<ShapeBatchItem>>(shapeItems));

        this.mockLinePullReactable
            .Pull(Arg.Any<Guid>())
            .Returns(_ => new Memory<RenderItem<LineBatchItem>>(lineItems));

        this.mockTextureRenderBatchReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<Memory<RenderItem<TextureBatchItem>>>()))
            .Do(ci => AssertTextureItems(ci.Arg<Guid>(), ci.Arg<Memory<RenderItem<TextureBatchItem>>>()));

        this.mockFontRenderBatchReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<Memory<RenderItem<FontGlyphBatchItem>>>()))
            .Do(ci => AssertFontItems(ci.Arg<Guid>(), ci.Arg<Memory<RenderItem<FontGlyphBatchItem>>>()));

        this.mockShapeRenderBatchReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<Memory<RenderItem<ShapeBatchItem>>>()))
            .Do(ci => AssertShapeItems(ci.Arg<Guid>(), ci.Arg<Memory<RenderItem<ShapeBatchItem>>>()));

        this.mockLineRenderBatchReactable
            .When(x => x.Push(Arg.Any<Guid>(), Arg.Any<Memory<RenderItem<LineBatchItem>>>()))
            .Do(ci => AssertLineItems(ci.Arg<Guid>(), ci.Arg<Memory<RenderItem<LineBatchItem>>>()));

        _ = CreateSystemUnderTest();

        // Act
        this.endBatchReactor.OnReceive();

        // Assert
        this.mockTexturePullReactable.Received(1).Pull(PullResponses.GetTextureItemsId);
        this.mockFontPullReactable.Received(1).Pull(PullResponses.GetFontItemsId);
        this.mockShapePullReactable.Received(1).Pull(PullResponses.GetShapeItemsId);
        this.mockLinePullReactable.Received(1).Pull(PullResponses.GetLineItemsId);

        void AssertTextureItems(Guid eventId, in Memory<RenderItem<TextureBatchItem>> data)
        {
            eventId.ShouldBe(PushNotifications.RenderTexturesId);
            data.Span.ToArray().Length.ShouldBe(2);
        }

        void AssertFontItems(Guid eventId, in Memory<RenderItem<FontGlyphBatchItem>> data)
        {
            eventId.ShouldBe(PushNotifications.RenderFontsId);
            data.Span.ToArray().Length.ShouldBe(2);
        }

        void AssertShapeItems(Guid eventId, in Memory<RenderItem<ShapeBatchItem>> data)
        {
            eventId.ShouldBe(PushNotifications.RenderShapesId);
            data.Span.ToArray().Length.ShouldBe(2);
        }

        void AssertLineItems(Guid eventId, in Memory<RenderItem<LineBatchItem>> data)
        {
            eventId.ShouldBe(PushNotifications.RenderLinesId);
            data.Span.ToArray().Length.ShouldBe(2);
        }

        this.mockPushReactable.Received(1).Push(PushNotifications.EmptyBatchId);
    }
    #endregion

    #region Reacteable Tests
    [Fact]
    [Trait("Category", Subscription)]
    public void EndBatchReactable_WhenCreatingSubscription_CreatesSubscriptionCorrectly()
    {
        // Arrange & Assert
        this.mockPushReactable
            .When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(ci =>
            {
                var reactor = ci.Arg<IReceiveSubscription>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"RenderMediator.ctor() - {PushNotifications.BatchHasEndedId}");
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
        => new (this.mockReactableFactory,
            this.mockTextureComparer,
            this.mockFontComparer,
            this.mockShapeComparer,
            this.mockLineComparer);
}

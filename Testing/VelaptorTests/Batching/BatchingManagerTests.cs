// <copyright file="BatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Xunit;
using TextureBatchPullSubscription = Carbonate.Core.OneWay.IRespondSubscription<System.Memory<Velaptor.OpenGL.Batching.RenderItem<Velaptor.OpenGL.Batching.TextureBatchItem>>>;

/// <summary>
/// Tests the <see cref="BatchingManager"/> class.
/// </summary>
public class BatchingManagerTests : TestsBase
{
    private readonly IPushReactable<BatchSizeData> mockBatchSizeReactable;
    private readonly IReactableFactory mockReactableFactory;
    private readonly IDisposable mockBatchSizeUnsubscriber;
    private IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>? textureBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>? fontBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>? shapeBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<LineBatchItem>>>? lineBatchPullReactor;
    private IReceiveSubscription? emptyBatchReactor;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingManagerTests"/> class.
    /// </summary>
    public BatchingManagerTests()
    {
        this.mockBatchSizeUnsubscriber = Substitute.For<IDisposable>();
        var mockTexturePullUnsubscriber = Substitute.For<IDisposable>();
        var mockFontPullUnsubscriber = Substitute.For<IDisposable>();
        var mockShapePullUnsubscriber = Substitute.For<IDisposable>();
        var mockLinePullUnsubscriber = Substitute.For<IDisposable>();
        var mockEmptyBatchUnsubscriber = Substitute.For<IDisposable>();

        this.mockBatchSizeReactable = Substitute.For<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<BatchSizeData>>();
                this.batchSizeReactor = reactor;
            });

        this.mockBatchSizeReactable.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>()).Returns(_ => this.mockBatchSizeUnsubscriber);

        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();
                this.emptyBatchReactor = reactor;
            });
        mockPushReactable.Subscribe(Arg.Any<IReceiveSubscription>()).Returns(_ => mockEmptyBatchUnsubscriber);

        var mockTextureBatchPullReactable = Substitute.For<IBatchPullReactable<TextureBatchItem>>();
        mockTextureBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>();
                this.textureBatchPullReactor = reactor;
            });

        mockTextureBatchPullReactable.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>())
            .Returns(_ => mockTexturePullUnsubscriber);

        var mockFontBatchPullReactable = Substitute.For<IBatchPullReactable<FontGlyphBatchItem>>();
        mockFontBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>();
                this.fontBatchPullReactor = reactor;
            });
        mockFontBatchPullReactable.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>())
            .Returns(_ => mockFontPullUnsubscriber);

        var mockShapeBatchPullReactable = Substitute.For<IBatchPullReactable<ShapeBatchItem>>();
        mockShapeBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>()))
            .Do(
                callInfo =>
                {
                    var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>();
                    this.shapeBatchPullReactor = reactor;
                });
        mockShapeBatchPullReactable.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>())
            .Returns(_ => mockShapePullUnsubscriber);

        var mockLineBatchPullReactable = Substitute.For<IBatchPullReactable<LineBatchItem>>();
        mockLineBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>();
                this.lineBatchPullReactor = reactor;
            });
        mockLineBatchPullReactable.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>())
            .Returns(_ => mockLinePullUnsubscriber);

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateBatchSizeReactable().Returns(this.mockBatchSizeReactable);
        this.mockReactableFactory.CreateNoDataPushReactable().Returns(mockPushReactable);
        this.mockReactableFactory.CreateTexturePullBatchReactable().Returns(mockTextureBatchPullReactable);
        this.mockReactableFactory.CreateFontPullBatchReactable().Returns(mockFontBatchPullReactable);
        this.mockReactableFactory.CreateShapePullBatchReactable().Returns(mockShapeBatchPullReactable);
        this.mockReactableFactory.CreateLinePullBatchReactable().Returns(mockLineBatchPullReactable);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new BatchingManager(null);
        };

        // Assert
        act.ShouldThrow<ArgumentNullException>()
            .Message.ShouldBe("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void PushReactable_WhenReceivingEmptyBatchNotification_EmptiesBatch()
    {
        // Arrange
        const int batchSize = 50;
        const int halfBatchSize = batchSize / 2;

        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = batchSize });

        var textureItems = BatchItemFactory.CreateTextureItemsWithOrderedValues(totalItems: halfBatchSize);
        var fontItems = BatchItemFactory.CreateFontItemsWithOrderedValues(totalItems: halfBatchSize);
        var shapeItems = BatchItemFactory.CreateShapeItemsWithOrderedValues(totalItems: halfBatchSize);
        var lineItems = BatchItemFactory.CreateLineItemsWithOrderedValues(totalItems: halfBatchSize);

        for (var i = 0; i < textureItems.Length; i++)
        {
            sut.AddTextureItem(textureItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < fontItems.Length; i++)
        {
            sut.AddFontItem(fontItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < shapeItems.Length; i++)
        {
            sut.AddShapeItem(shapeItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < lineItems.Length; i++)
        {
            sut.AddLineItem(lineItems[i], i, DateTime.Now);
        }

        // Act
        this.emptyBatchReactor.OnReceive();

        // Assert
        sut.TextureItems.ToArray().ShouldAllBe(expected => expected == default);
        sut.FontItems.ToArray().ShouldAllBe(expected => expected == default);
        sut.ShapeItems.ToArray().ShouldAllBe(expected => expected == default);
        sut.LineItems.ToArray().ShouldAllBe(expected => expected == default);
    }

    [Fact]
    public void BatchSizeReactable_WhenReceivingNotification_SetsUpBatchArrays()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        sut.TextureItems.ToArray().Length.ShouldBe(2);
        sut.TextureItems.ToArray().ShouldAllBe(expected => expected == default);

        sut.FontItems.ToArray().Length.ShouldBe(2);
        sut.FontItems.ToArray().ShouldAllBe(expected => expected == default);

        sut.ShapeItems.ToArray().Length.ShouldBe(2);
        sut.ShapeItems.ToArray().ShouldAllBe(expected => expected == default);

        sut.LineItems.ToArray().Length.ShouldBe(2);
        sut.LineItems.ToArray().ShouldAllBe(expected => expected == default);

        this.mockBatchSizeUnsubscriber.Received(1).Dispose();
    }

    [Fact]
    public void TextureBatchPullReactable_WhenRequestingResponseFromPartiallyEmptyBatch_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3 });
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 456);
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 789);

        sut.AddTextureItem(itemA, 1, DateTime.Now);
        sut.AddTextureItem(itemB, 2, DateTime.Now);

        // Act
        var actual = this.textureBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Length.ShouldBe(2);
        actual.ToArray().Select(i => i.Item).ShouldNotContain(itemC);
        actual.ToArray()[0].Layer.ShouldBe(1);
        actual.ToArray()[0].Item.ShouldBeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.ShouldBe(2);
        actual.ToArray()[1].Item.ShouldBeEquivalentTo(itemB);
    }

    [Fact]
    public void FontBatchPullReactable_WhenRequestingResponseFromPartiallyEmptyBatch_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3 });
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 456);
        var itemC = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 789);

        sut.AddFontItem(itemA, 1, DateTime.Now);
        sut.AddFontItem(itemB, 2, DateTime.Now);

        // Act
        var actual = this.fontBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Length.ShouldBe(2);
        actual.ToArray().Select(i => i.Item).ShouldNotContain(itemC);
        actual.ToArray()[0].Layer.ShouldBe(1);
        actual.ToArray()[0].Item.ShouldBeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.ShouldBe(2);
        actual.ToArray()[1].Item.ShouldBeEquivalentTo(itemB);
    }

    [Fact]
    public void ShapeBatchPullReactable_WhenRequestingResponseFromPartiallyEmptyBatch_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3 });
        var itemA = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(11, 22));
        var itemB = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(33, 44));
        var itemC = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(55, 66));

        sut.AddShapeItem(itemA, 1, DateTime.Now);
        sut.AddShapeItem(itemB, 2, DateTime.Now);

        // Act
        var actual = this.shapeBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Length.ShouldBe(2);
        actual.ToArray().Select(i => i.Item).ShouldNotContain(itemC);
        actual.ToArray()[0].Layer.ShouldBe(1);
        actual.ToArray()[0].Item.ShouldBeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.ShouldBe(2);
        actual.ToArray()[1].Item.ShouldBeEquivalentTo(itemB);
    }

    [Fact]
    public void LineBatchPullReactable_WhenRequestingResponseFromPartiallyEmptyBatch_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3 });
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(11, 22));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(33, 44));
        var itemC = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(55, 66));

        sut.AddLineItem(itemA, 1, DateTime.Now);
        sut.AddLineItem(itemB, 2, DateTime.Now);

        // Act
        var actual = this.lineBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Length.ShouldBe(2);
        actual.ToArray().Select(i => i.Item).ShouldNotContain(itemC);
        actual.ToArray()[0].Layer.ShouldBe(1);
        actual.ToArray()[0].Item.ShouldBeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.ShouldBe(2);
        actual.ToArray()[1].Item.ShouldBeEquivalentTo(itemB);
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void TextureBatchPullReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange & Act & Assert
        var mockTextureBatchPullReactable = Substitute.For<IBatchPullReactable<TextureBatchItem>>();
        mockTextureBatchPullReactable.When(x => x.Subscribe(Arg.Any<TextureBatchPullSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<TextureBatchPullSubscription>();

                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetTextureItemsId)}");
            });
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void BatchSizeReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange
        IReceiveSubscription<BatchSizeData>? reactor = null;

        this.mockBatchSizeReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription<BatchSizeData>>()))
            .Do(callInfo =>
            {
                reactor = callInfo.Arg<IReceiveSubscription<BatchSizeData>>();
            });

        // Act
        _ = CreateSystemUnderTest();

        // Assert
        reactor.ShouldNotBeNull("It is required for unit testing.");
        reactor.Name.ShouldBe($"BatchingManager.ctor() - {PushNotifications.BatchSizeChangedId}");
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void PushReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange & Assert
        var mockPushReactable = Substitute.For<IPushReactable>();
        mockPushReactable.When(x => x.Subscribe(Arg.Any<IReceiveSubscription>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription>();
                reactor.ShouldNotBeNull("It is required for unit testing.");

                if (reactor.Id == PushNotifications.EmptyBatchId)
                {
                    reactor.Name.ShouldBe($"BatchingManagerTests.ctor() - {PushNotifications.EmptyBatchId}");
                }
            });

        _ = CreateSystemUnderTest();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void FontBatchReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange & Assert
        var mockFontBatchPullReactable = Substitute.For<IBatchPullReactable<FontGlyphBatchItem>>();
        mockFontBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"BatchingManagerTests.ctor() - {PullResponses.GetFontItemsId}");
            });

        // Act
        _ = CreateSystemUnderTest();
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void ShapeBatchReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange & Assert
        var mockShapeBatchPullReactable = Substitute.For<IBatchPullReactable<ShapeBatchItem>>();
        mockShapeBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
            });
    }

    [Fact]
    [Trait("Category", Subscription)]
    public void LineBatchPullReactable_WhenCreatingSubscription_SubscriptionCreatedCorrectly()
    {
        // Arrange & Assert
        var mockLineBatchPullReactable = Substitute.For<IBatchPullReactable<LineBatchItem>>();
        mockLineBatchPullReactable.When(x => x.Subscribe(Arg.Any<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>()))
            .Do(callInfo =>
            {
                var reactor = callInfo.Arg<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>();
                reactor.ShouldNotBeNull("It is required for unit testing.");
                reactor.Name.ShouldBe($"BatchingManagerTests.ctor() - {PullResponses.GetLineItemsId}");
            });

        // Act
        _ = CreateSystemUnderTest();
    }
    #endregion

    #region Method Tests
    [Fact]
    [Trait("Category", Method)]
    public void AddTextureItem_WithFullBatch_ResizesTextureBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

        var expectedA = new RenderItem<TextureBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<TextureBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<BatchSizeData>()))
            .Do(_ =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = BatchType.Texture });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Texture });

        sut.AddTextureItem(itemA, 1, renderStampA);
        sut.AddTextureItem(itemB, 2, renderStampB);

        // Act
        sut.AddTextureItem(itemC, 3, renderStampC);

        // Assert
        sut.TextureItems.ToArray().Length.ShouldBe(3, "The total number of items should have increased.");

        sut.TextureItems.ToArray().ShouldContain(expectedA);
        sut.TextureItems[0].ShouldBeEquivalentTo(expectedA, "The previously added items should be in the same order.");

        sut.TextureItems.ToArray().ShouldContain(expectedB);
        sut.TextureItems[1].ShouldBeEquivalentTo(expectedB, "The previously added items should be in the same order.");

        sut.TextureItems.ToArray().ShouldContain(expectedC);
        sut.TextureItems[2].ShouldBeEquivalentTo(expectedC, "The previously added items should be in the same order.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddTextureItem_WithFullBatchAndInvalidBatchType_ThrowsException()
    {
        // Arrange
        const int invalidValue = 1234;
        var expected = $"The value of argument 'batchType' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(BatchType)}'. (Parameter 'batchType')";

        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        this.mockBatchSizeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<BatchSizeData>()))
            .Do(_ =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = (BatchType)invalidValue });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Texture });

        sut.AddTextureItem(itemA, 1, DateTime.Now);
        sut.AddTextureItem(itemB, 2, DateTime.Now);

        // Act
        var act = () => sut.AddTextureItem(itemC, 3, DateTime.Now);

        // Assert
        act.ShouldThrow<InvalidEnumArgumentException>()
            .Message.ShouldBe(expected);
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddTextureItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 456);

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

        // Act
        sut.AddTextureItem(itemA, 1, renderStampA);
        sut.AddTextureItem(itemB, 2, renderStampB);

        // Assert
        sut.TextureItems.ToArray().ShouldContain(new RenderItem<TextureBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.TextureItems.ToArray().ShouldContain(new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddFontItem_WithFullBatch_ResizesFontBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

        var expectedA = new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<FontGlyphBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<BatchSizeData>()))
            .Do(_ =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = BatchType.Font });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Font });

        sut.AddFontItem(itemA, 1, renderStampA);
        sut.AddFontItem(itemB, 2, renderStampB);

        // Act
        sut.AddFontItem(itemC, 3, renderStampC);

        // Assert
        sut.FontItems.ToArray().Length.ShouldBe(3, "The total number of items should have increased.");

        sut.FontItems.ToArray().ShouldContain(expectedA);
        sut.FontItems[0].ShouldBeEquivalentTo(expectedA, "The previously added items should be in the same order.");

        sut.FontItems.ToArray().ShouldContain(expectedB);
        sut.FontItems[1].ShouldBeEquivalentTo(expectedB, "The previously added items should be in the same order.");

        sut.FontItems.ToArray().ShouldContain(expectedC);
        sut.FontItems[2].ShouldBeEquivalentTo(expectedC, "The previously added items should be in the same order.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddFontItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 456);

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

        // Act
        sut.AddFontItem(itemA, 1, renderStampA);
        sut.AddFontItem(itemB, 2, renderStampB);

        // Assert
        sut.FontItems.ToArray().ShouldContain(new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.FontItems.ToArray().ShouldContain(new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddShapeItem_WithFullBatch_ResizesShapeBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(30, 40));
        var itemC = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(50, 60));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

        var expectedA = new RenderItem<ShapeBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<ShapeBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<ShapeBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<BatchSizeData>()))
            .Do(_ =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = BatchType.Rect });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Rect });

        sut.AddShapeItem(itemA, 1, renderStampA);
        sut.AddShapeItem(itemB, 2, renderStampB);

        // Act
        sut.AddShapeItem(itemC, 3, renderStampC);

        // Assert
        sut.ShapeItems.ToArray().Length.ShouldBe(3, "The total number of items should have increased.");

        sut.ShapeItems.ToArray().ShouldContain(expectedA);
        sut.ShapeItems[0].ShouldBeEquivalentTo(expectedA, "The previously added items should be in the same order.");

        sut.ShapeItems.ToArray().ShouldContain(expectedB);
        sut.ShapeItems[1].ShouldBeEquivalentTo(expectedB, "The previously added items should be in the same order.");

        sut.ShapeItems.ToArray().ShouldContain(expectedC);
        sut.ShapeItems[2].ShouldBeEquivalentTo(expectedC, "The previously added items should be in the same order.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddShapeItem_WhenInvoked_SetsNewLineBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateShapeItemWithOrderedValues(new Vector2(30, 40));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

        // Act
        sut.AddShapeItem(itemA, 1, renderStampA);
        sut.AddShapeItem(itemB, 2, renderStampB);

        // Assert
        sut.ShapeItems.ToArray().ShouldContain(new RenderItem<ShapeBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.ShapeItems.ToArray().ShouldContain(new RenderItem<ShapeBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddLineItem_WithFullBatch_ResizesLineBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));
        var itemC = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(50, 60));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

        var expectedA = new RenderItem<LineBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<LineBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<LineBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.When(x => x.Push(Arg.Any<Guid>(), Arg.Any<BatchSizeData>()))
            .Do(_ =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = BatchType.Line });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Line });

        sut.AddLineItem(itemA, 1, renderStampA);
        sut.AddLineItem(itemB, 2, renderStampB);

        // Act
        sut.AddLineItem(itemC, 3, renderStampC);

        // Assert
        sut.LineItems.ToArray().Length.ShouldBe(3, "The total number of items should have increased.");

        sut.LineItems.ToArray().ShouldContain(expectedA);
        sut.LineItems[0].ShouldBeEquivalentTo(expectedA, "The previously added items should be in the same order.");

        sut.LineItems.ToArray().ShouldContain(expectedB);
        sut.LineItems[1].ShouldBeEquivalentTo(expectedB, "The previously added items should be in the same order.");

        sut.LineItems.ToArray().ShouldContain(expectedC);
        sut.LineItems[2].ShouldBeEquivalentTo(expectedC, "The previously added items should be in the same order.");
    }

    [Fact]
    [Trait("Category", Method)]
    public void AddLineItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

        // Act
        sut.AddLineItem(itemA, 1, renderStampA);
        sut.AddLineItem(itemB, 2, renderStampB);

        // Assert
        sut.LineItems.ToArray().ShouldContain(new RenderItem<LineBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.LineItems.ToArray().ShouldContain(new RenderItem<LineBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BatchingManager CreateSystemUnderTest() => new (this.mockReactableFactory);
}

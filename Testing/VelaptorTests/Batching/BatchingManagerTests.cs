// <copyright file="BatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.Linq;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using Factories;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Factories;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="BatchingManager"/> class.
/// </summary>
public class BatchingManagerTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IDisposable> mockTexturePullUnsubscriber;
    private readonly Mock<IDisposable> mockFontPullUnsubscriber;
    private readonly Mock<IDisposable> mockRectPullUnsubscriber;
    private readonly Mock<IDisposable> mockLinePullUnsubscriber;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;
    private IReceiveReactor? shutDownReactor;
    private IReceiveReactor? emptyBatchReactor;
    private IRespondReactor<Memory<RenderItem<TextureBatchItem>>>? textureBatchPullReactor;
    private IRespondReactor<Memory<RenderItem<FontGlyphBatchItem>>>? fontBatchPullReactor;
    private IRespondReactor<Memory<RenderItem<RectBatchItem>>>? rectBatchPullReactor;
    private IRespondReactor<Memory<RenderItem<LineBatchItem>>>? lineBatchPullReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchingManagerTests"/> class.
    /// </summary>
    public BatchingManagerTests()
    {
        this.mockBatchSizeUnsubscriber = new Mock<IDisposable>();
        this.mockBatchSizeUnsubscriber.Name = nameof(this.mockBatchSizeUnsubscriber);

        this.mockShutDownUnsubscriber = new Mock<IDisposable>();
        this.mockShutDownUnsubscriber.Name = nameof(this.mockShutDownUnsubscriber);

        this.mockTexturePullUnsubscriber = new Mock<IDisposable>();
        this.mockTexturePullUnsubscriber.Name = nameof(this.mockTexturePullUnsubscriber);

        this.mockFontPullUnsubscriber = new Mock<IDisposable>();
        this.mockFontPullUnsubscriber.Name = nameof(this.mockFontPullUnsubscriber);

        this.mockRectPullUnsubscriber = new Mock<IDisposable>();
        this.mockRectPullUnsubscriber.Name = nameof(this.mockRectPullUnsubscriber);

        this.mockLinePullUnsubscriber = new Mock<IDisposable>();
        this.mockLinePullUnsubscriber.Name = nameof(this.mockLinePullUnsubscriber);

        var mockEmptyBatchUnsubscriber = new Mock<IDisposable>();
        mockEmptyBatchUnsubscriber.Name = nameof(mockEmptyBatchUnsubscriber);

        var mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PushNotifications.BatchSizeSetId)}");

                this.batchSizeReactor = reactor;
            })
            .Returns<IReceiveReactor<BatchSizeData>>(_ => this.mockBatchSizeUnsubscriber.Object);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor>()))
            .Callback<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.EmptyBatchId)
                {
                    reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PushNotifications.EmptyBatchId)}");
                    this.emptyBatchReactor = reactor;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PushNotifications.SystemShuttingDownId)}");
                    this.shutDownReactor = reactor;
                }
            })
            .Returns<IReceiveReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PushNotifications.EmptyBatchId)
                {
                    return mockEmptyBatchUnsubscriber.Object;
                }

                if (reactor.Id == PushNotifications.SystemShuttingDownId)
                {
                    return this.mockShutDownUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockTextureBatchPullReactable = new Mock<IBatchPullReactable<TextureBatchItem>>();
        mockTextureBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondReactor<Memory<RenderItem<TextureBatchItem>>>>()))
            .Callback<IRespondReactor<Memory<RenderItem<TextureBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetTextureItemsId)}");

                if (reactor.Id == PullResponses.GetTextureItemsId)
                {
                    this.textureBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondReactor<Memory<RenderItem<TextureBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetTextureItemsId)
                {
                    return this.mockTexturePullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockFontBatchPullReactable = new Mock<IBatchPullReactable<FontGlyphBatchItem>>();
        mockFontBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondReactor<Memory<RenderItem<FontGlyphBatchItem>>>>()))
            .Callback<IRespondReactor<Memory<RenderItem<FontGlyphBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetFontItemsId)}");

                if (reactor.Id == PullResponses.GetFontItemsId)
                {
                    this.fontBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondReactor<Memory<RenderItem<FontGlyphBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetFontItemsId)
                {
                    return this.mockFontPullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockRectBatchPullReactable = new Mock<IBatchPullReactable<RectBatchItem>>();
        mockRectBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondReactor<Memory<RenderItem<RectBatchItem>>>>()))
            .Callback<IRespondReactor<Memory<RenderItem<RectBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetRectItemsId)
                {
                    this.rectBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondReactor<Memory<RenderItem<RectBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetRectItemsId)}");

                if (reactor.Id == PullResponses.GetRectItemsId)
                {
                    return this.mockRectPullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockLineBatchPullReactable = new Mock<IBatchPullReactable<LineBatchItem>>();
        mockLineBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondReactor<Memory<RenderItem<LineBatchItem>>>>()))
            .Callback<IRespondReactor<Memory<RenderItem<LineBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetLineItemsId)}");

                if (reactor.Id == PullResponses.GetLineItemsId)
                {
                    this.lineBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondReactor<Memory<RenderItem<LineBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetLineItemsId)
                {
                    return this.mockLinePullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable())
            .Returns(mockBatchSizeReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateNoDataPushReactable())
            .Returns(mockPushReactable.Object);

        this.mockReactableFactory.Setup(m => m.CreateTexturePullBatchReactable())
            .Returns(mockTextureBatchPullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateFontPullBatchReactable())
            .Returns(mockFontBatchPullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateRectPullBatchReactable())
            .Returns(mockRectBatchPullReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateLinePullBatchReactable())
            .Returns(mockLineBatchPullReactable.Object);
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
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Reactable Tests
    [Fact]
    public void PushReactable_WhenReceivingShutDownNotification_ShutDownManager()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.shutDownReactor.OnReceive();
        this.shutDownReactor.OnReceive();

        // Assert
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockShutDownUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockTexturePullUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockFontPullUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockRectPullUnsubscriber.VerifyOnce(m => m.Dispose());
        this.mockLinePullUnsubscriber.VerifyOnce(m => m.Dispose());
    }

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
        var rectItems = BatchItemFactory.CreateRectItemsWithOrderedValues(totalItems: halfBatchSize);
        var lineItems = BatchItemFactory.CreateLineItemsWithOrderedValues(totalItems: halfBatchSize);

        for (var i = 0; i < textureItems.Length; i++)
        {
            sut.AddTextureItem(textureItems[i], i);
        }

        for (var i = 0; i < fontItems.Length; i++)
        {
            sut.AddFontItem(fontItems[i], i);
        }

        for (var i = 0; i < rectItems.Length; i++)
        {
            sut.AddRectItem(rectItems[i], i);
        }

        for (var i = 0; i < lineItems.Length; i++)
        {
            sut.AddLineItem(lineItems[i], i);
        }

        // Act
        this.emptyBatchReactor.OnReceive();

        // Assert
        sut.TextureItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<TextureBatchItem>)));

        sut.FontItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<FontGlyphBatchItem>)));

        sut.RectItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<RectBatchItem>)));

        sut.LineItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<LineBatchItem>)));
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
        sut.TextureItems.ToArray().Should().HaveCount(2);
        sut.TextureItems.ToArray().Should()
            .AllSatisfy(expected => expected.Should().BeEquivalentTo(default(RenderItem<TextureBatchItem>)));

        sut.FontItems.ToArray().Should().HaveCount(2);
        sut.FontItems.ToArray().Should()
            .AllSatisfy(expected => expected.Should().BeEquivalentTo(default(RenderItem<FontGlyphBatchItem>)));

        sut.RectItems.ToArray().Should().HaveCount(2);
        sut.RectItems.ToArray().Should()
            .AllSatisfy(expected => expected.Should().BeEquivalentTo(default(RenderItem<RectBatchItem>)));

        sut.LineItems.ToArray().Should().HaveCount(2);
        sut.LineItems.ToArray().Should()
            .AllSatisfy(expected => expected.Should().BeEquivalentTo(default(RenderItem<LineBatchItem>)));

        this.mockBatchSizeUnsubscriber.VerifyOnce(m => m.Dispose());
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

        sut.AddTextureItem(itemA, 1);
        sut.AddTextureItem(itemB, 2);

        // Act
        var actual = this.textureBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Should().HaveCount(2);
        actual.ToArray().Select(i => i.Item).ToArray().Should().NotContain(itemC);
        actual.ToArray()[0].Layer.Should().Be(1);
        actual.ToArray()[0].Item.Should().BeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.Should().Be(2);
        actual.ToArray()[1].Item.Should().BeEquivalentTo(itemB);
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

        sut.AddFontItem(itemA, 1);
        sut.AddFontItem(itemB, 2);

        // Act
        var actual = this.fontBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Should().HaveCount(2);
        actual.ToArray().Select(i => i.Item).ToArray().Should().NotContain(itemC);
        actual.ToArray()[0].Layer.Should().Be(1);
        actual.ToArray()[0].Item.Should().BeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.Should().Be(2);
        actual.ToArray()[1].Item.Should().BeEquivalentTo(itemB);
    }

    [Fact]
    public void RectBatchPullReactable_WhenRequestingResponseFromPartiallyEmptyBatch_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3 });
        var itemA = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(11, 22));
        var itemB = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(33, 44));
        var itemC = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(55, 66));

        sut.AddRectItem(itemA, 1);
        sut.AddRectItem(itemB, 2);

        // Act
        var actual = this.rectBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Should().HaveCount(2);
        actual.ToArray().Select(i => i.Item).ToArray().Should().NotContain(itemC);
        actual.ToArray()[0].Layer.Should().Be(1);
        actual.ToArray()[0].Item.Should().BeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.Should().Be(2);
        actual.ToArray()[1].Item.Should().BeEquivalentTo(itemB);
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

        sut.AddLineItem(itemA, 1);
        sut.AddLineItem(itemB, 2);

        // Act
        var actual = this.lineBatchPullReactor.OnRespond();

        // Assert
        actual.ToArray().Should().HaveCount(2);
        actual.ToArray().Select(i => i.Item).ToArray().Should().NotContain(itemC);
        actual.ToArray()[0].Layer.Should().Be(1);
        actual.ToArray()[0].Item.Should().BeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.Should().Be(2);
        actual.ToArray()[1].Item.Should().BeEquivalentTo(itemB);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void AddTextureItem_WithNoEmptyItems_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 1 });
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 456);

        sut.AddTextureItem(itemA, 1);

        // Act
        var act = () => sut.AddTextureItem(itemB, 2);

        // Assert
        act.Should().Throw<Exception>().WithMessage("The texture batch is full.");
        sut.TextureItems.ToArray().Should().NotContain(new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddTextureItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 456);

        // Act
        sut.AddTextureItem(itemA, 1);
        sut.AddTextureItem(itemB, 2);

        // Assert
        sut.TextureItems.ToArray().Should().Contain(new RenderItem<TextureBatchItem> { Layer = 1, Item = itemA });
        sut.TextureItems.ToArray().Should().Contain(new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddFontItem_WithNoEmptyItems_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 1 });
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 456);

        sut.AddFontItem(itemA, 1);

        // Act
        var act = () => sut.AddFontItem(itemB, 2);

        // Assert
        act.Should().Throw<Exception>().WithMessage("The font batch is full.");
        sut.FontItems.ToArray().Should().NotContain(new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddFontItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 456);

        // Act
        sut.AddFontItem(itemA, 1);
        sut.AddFontItem(itemB, 2);

        // Assert
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA });
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddRectItem_WithNoEmptyItems_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 1 });
        var itemA = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(30, 40));

        sut.AddRectItem(itemA, 1);

        // Act
        var act = () => sut.AddRectItem(itemB, 2);

        // Assert
        act.Should().Throw<Exception>().WithMessage("The rect batch is full.");
        sut.RectItems.ToArray().Should().NotContain(new RenderItem<RectBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddRectItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(30, 40));

        // Act
        sut.AddRectItem(itemA, 1);
        sut.AddRectItem(itemB, 2);

        // Assert
        sut.RectItems.ToArray().Should().Contain(new RenderItem<RectBatchItem> { Layer = 1, Item = itemA });
        sut.RectItems.ToArray().Should().Contain(new RenderItem<RectBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddLineItem_WithNoEmptyItems_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 1 });
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));

        sut.AddLineItem(itemA, 1);

        // Act
        var act = () => sut.AddLineItem(itemB, 2);

        // Assert
        act.Should().Throw<Exception>().WithMessage("The line batch is full.");
        sut.LineItems.ToArray().Should().NotContain(new RenderItem<LineBatchItem> { Layer = 2, Item = itemB });
    }

    [Fact]
    public void AddLineItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));

        // Act
        sut.AddLineItem(itemA, 1);
        sut.AddLineItem(itemB, 2);

        // Assert
        sut.LineItems.ToArray().Should().Contain(new RenderItem<LineBatchItem> { Layer = 1, Item = itemA });
        sut.LineItems.ToArray().Should().Contain(new RenderItem<LineBatchItem> { Layer = 2, Item = itemB });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BatchingManager CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}

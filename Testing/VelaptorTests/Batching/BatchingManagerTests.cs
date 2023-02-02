// <copyright file="BatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Carbonate.Core.NonDirectional;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Batching;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="BatchingManager"/> class.
/// </summary>
public class BatchingManagerTests
{
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
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

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
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
            .Returns(this.mockBatchSizeReactable.Object);
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
            sut.AddTextureItem(textureItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < fontItems.Length; i++)
        {
            sut.AddFontItem(fontItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < rectItems.Length; i++)
        {
            sut.AddRectItem(rectItems[i], i, DateTime.Now);
        }

        for (var i = 0; i < lineItems.Length; i++)
        {
            sut.AddLineItem(lineItems[i], i, DateTime.Now);
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

        sut.AddTextureItem(itemA, 1, DateTime.Now);
        sut.AddTextureItem(itemB, 2, DateTime.Now);

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

        sut.AddFontItem(itemA, 1, DateTime.Now);
        sut.AddFontItem(itemB, 2, DateTime.Now);

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

        sut.AddRectItem(itemA, 1, DateTime.Now);
        sut.AddRectItem(itemB, 2, DateTime.Now);

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

        sut.AddLineItem(itemA, 1, DateTime.Now);
        sut.AddLineItem(itemB, 2, DateTime.Now);

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
    public void AddTextureItem_WithFullBatch_ResizesTextureBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30);

        var expectedA = new RenderItem<TextureBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<TextureBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
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
        sut.TextureItems.ToArray().Should().HaveCount(3, "the total number of items should of increased.");

        sut.TextureItems.ToArray().Should().Contain(expectedA);
        sut.TextureItems[0].Should().BeEquivalentTo(expectedA, "the previously added items should be in the same order.");

        sut.TextureItems.ToArray().Should().Contain(expectedB);
        sut.TextureItems[1].Should().BeEquivalentTo(expectedB, "the previously added items should be in the same order.");

        sut.TextureItems.ToArray().Should().Contain(expectedC);
        sut.TextureItems[2].Should().BeEquivalentTo(expectedC, "the previously added items should be in the same order.");
    }

    [Fact]
    public void AddTextureItem_WithFullBatchAndInvalidBatchType_ThrowsException()
    {
        // Arrange
        var expected = $"The value of the enum '{nameof(BatchType)}' used in the class '{nameof(BatchingManager)}' and";
        expected += " method 'SetNewBatchSize' is invalid and out of range.";

        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = (BatchType)1234 });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Texture });

        sut.AddTextureItem(itemA, 1, DateTime.Now);
        sut.AddTextureItem(itemB, 2, DateTime.Now);

        // Act
        var act = () => sut.AddTextureItem(itemC, 3, DateTime.Now);

        // Assert
        act.Should().Throw<EnumOutOfRangeException<BatchType>>()
            .WithMessage(expected);
    }

    [Fact]
    public void AddTextureItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(textureId: 456);

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);

        // Act
        sut.AddTextureItem(itemA, 1, renderStampA);
        sut.AddTextureItem(itemB, 2, renderStampB);

        // Assert
        sut.TextureItems.ToArray().Should().Contain(new RenderItem<TextureBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.TextureItems.ToArray().Should().Contain(new RenderItem<TextureBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    public void AddFontItem_WithFullBatch_ResizesFontBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateFontItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30);

        var expectedA = new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<FontGlyphBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
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
        sut.FontItems.ToArray().Should().HaveCount(3, "the total number of items should of increased.");

        sut.FontItems.ToArray().Should().Contain(expectedA);
        sut.FontItems[0].Should().BeEquivalentTo(expectedA, "the previously added items should be in the same order.");

        sut.FontItems.ToArray().Should().Contain(expectedB);
        sut.FontItems[1].Should().BeEquivalentTo(expectedB, "the previously added items should be in the same order.");

        sut.FontItems.ToArray().Should().Contain(expectedC);
        sut.FontItems[2].Should().BeEquivalentTo(expectedC, "the previously added items should be in the same order.");
    }

    [Fact]
    public void AddFontItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 123);
        var itemB = BatchItemFactory.CreateFontItemWithOrderedValues(textureId: 456);

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);

        // Act
        sut.AddFontItem(itemA, 1, renderStampA);
        sut.AddFontItem(itemB, 2, renderStampB);

        // Assert
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    public void AddRectItem_WithFullBatch_ResizesRectBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(30, 40));
        var itemC = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(50, 60));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30);

        var expectedA = new RenderItem<RectBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<RectBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<RectBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
            {
                this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 3, TypeOfBatch = BatchType.Rect });
            });

        var sut = CreateSystemUnderTest();

        // Initialize batch size
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2, TypeOfBatch = BatchType.Rect });

        sut.AddRectItem(itemA, 1, renderStampA);
        sut.AddRectItem(itemB, 2, renderStampB);

        // Act
        sut.AddRectItem(itemC, 3, renderStampC);

        // Assert
        sut.RectItems.ToArray().Should().HaveCount(3, "the total number of items should of increased.");

        sut.RectItems.ToArray().Should().Contain(expectedA);
        sut.RectItems[0].Should().BeEquivalentTo(expectedA, "the previously added items should be in the same order.");

        sut.RectItems.ToArray().Should().Contain(expectedB);
        sut.RectItems[1].Should().BeEquivalentTo(expectedB, "the previously added items should be in the same order.");

        sut.RectItems.ToArray().Should().Contain(expectedC);
        sut.RectItems[2].Should().BeEquivalentTo(expectedC, "the previously added items should be in the same order.");
    }

    [Fact]
    public void AddRectItem_WhenInvoked_SetsNewLineBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateRectItemWithOrderedValues(new Vector2(30, 40));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);

        // Act
        sut.AddRectItem(itemA, 1, renderStampA);
        sut.AddRectItem(itemB, 2, renderStampB);

        // Assert
        sut.RectItems.ToArray().Should().Contain(new RenderItem<RectBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.RectItems.ToArray().Should().Contain(new RenderItem<RectBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
    public void AddLineItem_WithFullBatch_ResizesLineBatch()
    {
        // Arrange
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));
        var itemC = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(50, 60));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30);

        var expectedA = new RenderItem<LineBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA };
        var expectedB = new RenderItem<LineBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB };
        var expectedC = new RenderItem<LineBatchItem> { Layer = 3, Item = itemC, RenderStamp = renderStampC };

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
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
        sut.LineItems.ToArray().Should().HaveCount(3, "the total number of items should of increased.");

        sut.LineItems.ToArray().Should().Contain(expectedA);
        sut.LineItems[0].Should().BeEquivalentTo(expectedA, "the previously added items should be in the same order.");

        sut.LineItems.ToArray().Should().Contain(expectedB);
        sut.LineItems[1].Should().BeEquivalentTo(expectedB, "the previously added items should be in the same order.");

        sut.LineItems.ToArray().Should().Contain(expectedC);
        sut.LineItems[2].Should().BeEquivalentTo(expectedC, "the previously added items should be in the same order.");
    }

    [Fact]
    public void AddLineItem_WhenInvoked_SetsNewBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        this.batchSizeReactor.OnReceive(new BatchSizeData { BatchSize = 2 });
        var itemA = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(10, 20));
        var itemB = BatchItemFactory.CreateLineItemWithOrderedValues(new Vector2(30, 40));

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20);

        // Act
        sut.AddLineItem(itemA, 1, renderStampA);
        sut.AddLineItem(itemB, 2, renderStampB);

        // Assert
        sut.LineItems.ToArray().Should().Contain(new RenderItem<LineBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.LineItems.ToArray().Should().Contain(new RenderItem<LineBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BatchingManager CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}

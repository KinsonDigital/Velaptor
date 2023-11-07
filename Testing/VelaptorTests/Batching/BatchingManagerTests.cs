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
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IDisposable> mockBatchSizeUnsubscriber;
    private readonly Mock<IDisposable> mockShutDownUnsubscriber;
    private readonly Mock<IDisposable> mockTexturePullUnsubscriber;
    private readonly Mock<IDisposable> mockFontPullUnsubscriber;
    private readonly Mock<IDisposable> mockShapePullUnsubscriber;
    private readonly Mock<IDisposable> mockLinePullUnsubscriber;
    private IReceiveSubscription<BatchSizeData>? batchSizeReactor;
    private IReceiveSubscription? shutDownReactor;
    private IReceiveSubscription? emptyBatchReactor;
    private IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>? textureBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>? fontBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>? shapeBatchPullReactor;
    private IRespondSubscription<Memory<RenderItem<LineBatchItem>>>? lineBatchPullReactor;

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

        this.mockShapePullUnsubscriber = new Mock<IDisposable>();
        this.mockShapePullUnsubscriber.Name = nameof(this.mockShapePullUnsubscriber);

        this.mockLinePullUnsubscriber = new Mock<IDisposable>();
        this.mockLinePullUnsubscriber.Name = nameof(this.mockLinePullUnsubscriber);

        var mockEmptyBatchUnsubscriber = new Mock<IDisposable>();
        mockEmptyBatchUnsubscriber.Name = nameof(mockEmptyBatchUnsubscriber);

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<BatchSizeData>>()))
            .Callback<IReceiveSubscription<BatchSizeData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PushNotifications.BatchSizeChangedId)}");

                this.batchSizeReactor = reactor;
            })
            .Returns<IReceiveSubscription<BatchSizeData>>(_ => this.mockBatchSizeUnsubscriber.Object);

        var mockPushReactable = new Mock<IPushReactable>();
        mockPushReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription>()))
            .Callback<IReceiveSubscription>(reactor =>
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
            .Returns<IReceiveSubscription>(reactor =>
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
            .Setup(m => m.Subscribe(It.IsAny<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>()))
            .Callback<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetTextureItemsId)}");

                if (reactor.Id == PullResponses.GetTextureItemsId)
                {
                    this.textureBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondSubscription<Memory<RenderItem<TextureBatchItem>>>>(reactor =>
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
            .Setup(m => m.Subscribe(It.IsAny<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>()))
            .Callback<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetFontItemsId)}");

                if (reactor.Id == PullResponses.GetFontItemsId)
                {
                    this.fontBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondSubscription<Memory<RenderItem<FontGlyphBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetFontItemsId)
                {
                    return this.mockFontPullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockShapeBatchPullReactable = new Mock<IBatchPullReactable<ShapeBatchItem>>();
        mockShapeBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>()))
            .Callback<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                if (reactor.Id == PullResponses.GetShapeItemsId)
                {
                    this.shapeBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondSubscription<Memory<RenderItem<ShapeBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetShapeItemsId)}");

                if (reactor.Id == PullResponses.GetShapeItemsId)
                {
                    return this.mockShapePullUnsubscriber.Object;
                }

                Assert.Fail($"The notification ID '{reactor.Id}' has not been properly mocked.");
                return null;
            });

        var mockLineBatchPullReactable = new Mock<IBatchPullReactable<LineBatchItem>>();
        mockLineBatchPullReactable
            .Setup(m => m.Subscribe(It.IsAny<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>()))
            .Callback<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                reactor.Name.Should().Be($"BatchingManagerTests.Ctor - {nameof(PullResponses.GetLineItemsId)}");

                if (reactor.Id == PullResponses.GetLineItemsId)
                {
                    this.lineBatchPullReactor = reactor;
                }
            })
            .Returns<IRespondSubscription<Memory<RenderItem<LineBatchItem>>>>(reactor =>
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
        this.mockReactableFactory.Setup(m => m.CreateShapePullBatchReactable())
            .Returns(mockShapeBatchPullReactable.Object);
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
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
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
        this.mockShapePullUnsubscriber.VerifyOnce(m => m.Dispose());
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
        sut.TextureItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<TextureBatchItem>)));

        sut.FontItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<FontGlyphBatchItem>)));

        sut.ShapeItems.ToArray().Should().AllSatisfy(expected =>
            expected.Should().BeEquivalentTo(default(RenderItem<ShapeBatchItem>)));

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

        sut.ShapeItems.ToArray().Should().HaveCount(2);
        sut.ShapeItems.ToArray().Should()
            .AllSatisfy(expected => expected.Should().BeEquivalentTo(default(RenderItem<ShapeBatchItem>)));

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
        actual.ToArray().Select(i => i.Item).Should().NotContain(itemC);
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
        actual.ToArray().Select(i => i.Item).Should().NotContain(itemC);
        actual.ToArray()[0].Layer.Should().Be(1);
        actual.ToArray()[0].Item.Should().BeEquivalentTo(itemA);

        actual.ToArray()[1].Layer.Should().Be(2);
        actual.ToArray()[1].Item.Should().BeEquivalentTo(itemB);
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
        actual.ToArray().Should().HaveCount(2);
        actual.ToArray().Select(i => i.Item).Should().NotContain(itemC);
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
        actual.ToArray().Select(i => i.Item).Should().NotContain(itemC);
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

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

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
        const int invalidValue = 1234;
        var expected = $"The value of argument 'batchType' ({invalidValue}) is invalid for Enum type " +
                       $"'{nameof(BatchType)}'. (Parameter 'batchType')";

        var itemA = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(10, 20, 30, 40));
        var itemB = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(50, 60, 70, 80));
        var itemC = BatchItemFactory.CreateTextureItemWithOrderedValues(new RectangleF(90, 100, 110, 120));

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
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
        act.Should()
            .Throw<InvalidEnumArgumentException>()
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

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

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

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);
        var renderStampC = new DateTime(1, 2, 3, 0, 0, 0, 30, DateTimeKind.Utc);

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

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

        // Act
        sut.AddFontItem(itemA, 1, renderStampA);
        sut.AddFontItem(itemB, 2, renderStampB);

        // Assert
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.FontItems.ToArray().Should().Contain(new RenderItem<FontGlyphBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
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

        this.mockBatchSizeReactable.Setup(m => m.Push(It.Ref<BatchSizeData>.IsAny, It.IsAny<Guid>()))
            .Callback((in BatchSizeData _, Guid _) =>
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
        sut.ShapeItems.ToArray().Should().HaveCount(3, "the total number of items should of increased.");

        sut.ShapeItems.ToArray().Should().Contain(expectedA);
        sut.ShapeItems[0].Should().BeEquivalentTo(expectedA, "the previously added items should be in the same order.");

        sut.ShapeItems.ToArray().Should().Contain(expectedB);
        sut.ShapeItems[1].Should().BeEquivalentTo(expectedB, "the previously added items should be in the same order.");

        sut.ShapeItems.ToArray().Should().Contain(expectedC);
        sut.ShapeItems[2].Should().BeEquivalentTo(expectedC, "the previously added items should be in the same order.");
    }

    [Fact]
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
        sut.ShapeItems.ToArray().Should().Contain(new RenderItem<ShapeBatchItem> { Layer = 1, Item = itemA, RenderStamp = renderStampA });
        sut.ShapeItems.ToArray().Should().Contain(new RenderItem<ShapeBatchItem> { Layer = 2, Item = itemB, RenderStamp = renderStampB });
    }

    [Fact]
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

        var renderStampA = new DateTime(1, 2, 3, 0, 0, 0, 10, DateTimeKind.Utc);
        var renderStampB = new DateTime(1, 2, 3, 0, 0, 0, 20, DateTimeKind.Utc);

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

// <copyright file="TextureBatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Carbonate.Core;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Velaptor.Batching;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureBatchingManager"/> class.
/// </summary>
public class TextureBatchingManagerTests
{
    private readonly Mock<IDisposable> mockUnsubscriber;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private IReceiveReactor<BatchSizeData>? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchingManagerTests"/> class.
    /// </summary>
    public TextureBatchingManagerTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockPushReactable = new Mock<IPushReactable>();

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactorObj => this.reactor = reactorObj)
            .Returns(this.mockUnsubscriber.Object);

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataReactable()).Returns(this.mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(this.mockBatchSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureBatchingManager(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizePushNotification_CreatesBatchItemList()
    {
        // Arrange
        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 4u });

        var sut = CreateSystemUnderTest();

        // Act
        this.reactor.OnReceive(mockMessage.Object);

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenReactableUnsubscribes_InvokesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.reactor.OnUnsubscribe();

        this.reactor.OnUnsubscribe();
        // Assert
        this.mockUnsubscriber.Verify(m => m.Dispose());
    }

    [Fact]
    public void Ctor_WhenBatchSizeNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(TextureBatchingManager)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{PushNotifications.BatchSizeSetId}'.";

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                this.reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.reactor.OnReceive(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expectedMsg);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void BatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem1 = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11,
            18);
        var batchItem2 = new TextureBatchItem(
            new RectangleF(24, 25, 26, 27),
            new RectangleF(20, 21, 22, 23),
            19,
            18,
            Color.FromArgb(29, 30, 31, 32),
            RenderEffects.FlipHorizontally,
            28,
            35);

        var batchItems = new List<TextureBatchItem> { batchItem1, batchItem2 };
        var expected = new ReadOnlyCollection<TextureBatchItem>(batchItems.ToReadOnlyCollection());
        var sut = CreateSystemUnderTest();

        // Act
        sut.BatchItems = batchItems.ToReadOnlyCollection();
        var actual = sut.BatchItems;

        // Assert
        actual.Should().BeEquivalentTo(expected.ToArray());
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Add_WhenBatchIsFull_RaisesBatchFilledEvent()
    {
        // Arrange
        var batchItem1 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            1,
            0,
            Color.Empty,
            RenderEffects.None,
            1,
            0);

        var batchItem2 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            0,
            Color.Empty,
            RenderEffects.None,
            2,
            0);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 1u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnReceive(mockMessage.Object);
        sut.Add(batchItem1);

        // Act
        sut.Add(batchItem2);

        // Assert
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderTexturesId));
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var expected = new[] { default(TextureBatchItem), default(TextureBatchItem) };
        var batchItem1 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            1,
            0,
            Color.Empty,
            RenderEffects.None,
            1,
            0);
        var batchItem2 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            0,
            Color.Empty,
            RenderEffects.None,
            2,
            0);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnReceive(mockMessage.Object);

        sut.Add(batchItem1);
        sut.Add(batchItem2);

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void EmptyBatch_WithNoItemsReadyToRender_DoesNotEmptyItems()
    {
        // Arrange
        var batchItem1 = default(TextureBatchItem);
        var batchItem2 = default(TextureBatchItem);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnReceive(mockMessage.Object);

        sut.BatchItems = new List<TextureBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems[0].Should().BeEquivalentTo(batchItem1);
        sut.BatchItems[1].Should().BeEquivalentTo(batchItem2);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureBatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureBatchingManager CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}

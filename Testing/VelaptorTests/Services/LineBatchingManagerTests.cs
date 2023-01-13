// <copyright file="LineBatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using Carbonate.Core;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.Factories;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="LineBatchingManager"/> class.
/// </summary>
public class LineBatchingManagerTests
{
    private readonly Mock<IDisposable> mockUnsubscriber;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchingManagerTests"/> class.
    /// </summary>
    public LineBatchingManagerTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockPushReactable = new Mock<IPushReactable>();

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactorObj => this.batchSizeReactor = reactorObj)
            .Returns(this.mockUnsubscriber.Object);

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateNoDataReactable()).Returns(this.mockPushReactable.Object);
        this.mockReactableFactory.Setup(m => m.CreateBatchSizeReactable()).Returns(this.mockBatchSizeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithReactableFactoryNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineBatchingManager(null);
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
        this.batchSizeReactor.OnReceive(mockMessage.Object);

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenReactableUnsubscribes_InvokesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.batchSizeReactor.OnUnsubscribe();
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        this.mockUnsubscriber.Verify(m => m.Dispose());
    }

    [Fact]
    public void Ctor_WhenReactableNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(LineBatchingManager)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                this.batchSizeReactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.batchSizeReactor.OnReceive(mockMessage.Object);

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
        var batchItem1 = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            9);
        var batchItem2 = new LineBatchItem(
            new Vector2(10, 11),
            new Vector2(12, 13),
            Color.FromArgb(14, 15, 16, 17),
            18);

        var batchItems = new List<LineBatchItem> { batchItem1, batchItem2 };
        var expected = new ReadOnlyCollection<LineBatchItem>(batchItems.ToReadOnlyCollection());
        var sut = CreateSystemUnderTest();

        // Act
        sut.BatchItems = batchItems.ToReadOnlyCollection();
        var actual = sut.BatchItems;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Add_WhenBatchIsFull_SendsRenderPushNotification()
    {
        // Arrange
        var batchItem1 = new LineBatchItem(Vector2.Zero, Vector2.Zero, Color.Empty, 1);
        var batchItem2 = new LineBatchItem(Vector2.Zero, Vector2.Zero, Color.Empty, 2);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 1u });

        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(mockMessage.Object);

        sut.Add(batchItem1);

        // Act
        sut.Add(batchItem2);

        // Assert
        this.mockPushReactable.Verify(m => m.Push(NotificationIds.RenderLinesId));
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var batchItem1 = new LineBatchItem(Vector2.One, Vector2.One, Color.White, 1);
        var batchItem2 = new LineBatchItem(Vector2.One, Vector2.One, Color.White, 2);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(mockMessage.Object);

        sut.Add(batchItem1);
        sut.Add(batchItem2);

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems[0].Should().BeEquivalentTo(default(LineBatchItem));
        sut.BatchItems[1].Should().BeEquivalentTo(default(LineBatchItem));
    }

    [Fact]
    public void EmptyBatch_WithNoItemsReadyToRender_DoesNotEmptyItems()
    {
        // Arrange
        var batchItem1 = default(LineBatchItem);
        var batchItem2 = default(LineBatchItem);

        var mockMessage = new Mock<IMessage<BatchSizeData>>();
        mockMessage.Setup(m => m.GetData(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.batchSizeReactor.OnReceive(mockMessage.Object);

        sut.BatchItems = new List<LineBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems[0].Should().BeEquivalentTo(batchItem1);
        sut.BatchItems[1].Should().BeEquivalentTo(batchItem2);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineBatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineBatchingManager CreateSystemUnderTest() => new (this.mockReactableFactory.Object);
}

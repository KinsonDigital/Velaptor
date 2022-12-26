// <copyright file="LineBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Exceptions;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="LineBatchingService"/> class.
/// </summary>
public class LineBatchingServiceTests
{
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReactor? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchingServiceTests"/> class.
    /// </summary>
    public LineBatchingServiceTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj => this.reactor = reactorObj)
            .Returns(this.mockUnsubscriber.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithReactableNullParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineBatchingService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizePushNotification_CreatesBatchItemList()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 4u });

        var sut = CreateSystemUnderTest();

        // Act
        this.reactor.OnNext(mockMessage.Object);

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenReactableUnsubscribes_InvokesUnsubscriber()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.reactor.OnComplete();
        this.reactor.OnComplete();

        // Assert
        this.mockUnsubscriber.Verify(m => m.Dispose());
    }

    [Fact]
    public void Ctor_WhenReactableNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(LineBatchingService)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeId}'.";

        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj =>
            {
                reactorObj.Should().NotBeNull("it is required for unit testing.");

                this.reactor = reactorObj;
            });

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(null))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.reactor.OnNext(mockMessage.Object);

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
    public void Add_WhenBatchIsFull_RaisesBatchFilledEvent()
    {
        // Arrange
        var batchItem1 = new LineBatchItem(Vector2.Zero, Vector2.Zero, Color.Empty, 1);
        var batchItem2 = new LineBatchItem(Vector2.Zero, Vector2.Zero, Color.Empty, 2);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 1u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnNext(mockMessage.Object);

        sut.Add(batchItem1);

        using var monitor = sut.Monitor();

        // Act
        sut.Add(batchItem2);

        // Assert
        monitor.Should().Raise(nameof(LineBatchingService.ReadyForRendering));
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var batchItem1 = new LineBatchItem(Vector2.One, Vector2.One, Color.White, 1);
        var batchItem2 = new LineBatchItem(Vector2.One, Vector2.One, Color.White, 2);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnNext(mockMessage.Object);

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

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnNext(mockMessage.Object);

        sut.BatchItems = new List<LineBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems[0].Should().BeEquivalentTo(batchItem1);
        sut.BatchItems[1].Should().BeEquivalentTo(batchItem2);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineBatchingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineBatchingService CreateSystemUnderTest() => new (this.mockReactable.Object);
}

// <copyright file="RectBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
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
using Velaptor.Graphics;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="RectBatchingService"/> class.
/// </summary>
public class RectBatchingServiceTests
{
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReactor? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectBatchingServiceTests"/> class.
    /// </summary>
    public RectBatchingServiceTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m => m.Subscribe(It.IsAny<IReactor>()))
            .Callback<IReactor>(reactorObj => this.reactor = reactorObj)
            .Returns(this.mockUnsubscriber.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new RectBatchingService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizePushNotification_CreatesBatchItemList()
    {
        // Arrange & Act
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 4u });

        var sut = CreateSystemUnderTest();
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
    public void Ctor_WhenBatchSizeNotificationHasAnIssue_ThrowsException()
    {
        // Arrange
        var expectedMsg = $"There was an issue with the '{nameof(RectBatchingService)}.Constructor()' subscription source";
        expectedMsg += $" for subscription ID '{NotificationIds.BatchSizeSetId}'.";

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
        var batchItem1 = new RectBatchItem(
            new Vector2(1f, 2f),
            3f,
            4f,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.None,
            Color.FromArgb(14, 15, 16, 17),
            Color.FromArgb(18, 19, 20, 21),
            0);
        var batchItem2 = new RectBatchItem(
            new Vector2(22f, 23f),
            24f,
            25f,
            Color.FromArgb(26, 27, 28, 29),
            true,
            30,
            new CornerRadius(31, 32, 33, 34),
            ColorGradient.None,
            Color.FromArgb(35, 36, 37, 38),
            Color.FromArgb(39, 40, 41, 42),
            0);

        var batchItems = new List<RectBatchItem> { batchItem1, batchItem2 };
        var expected = new ReadOnlyCollection<RectBatchItem>(batchItems.ToReadOnlyCollection());
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
        var batchItem1 = new RectBatchItem(width: 10, height: 20);
        var batchItem2 = new RectBatchItem(width: 30, height: 40);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 1u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnNext(mockMessage.Object);
        sut.Add(batchItem1);

        // Act & Assert
        Assert.Raises<EventArgs>(e =>
        {
            sut.ReadyForRendering += e;
        }, e =>
        {
            sut.ReadyForRendering -= e;
        }, () =>
        {
            sut.Add(batchItem2);
        });
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var batchItem1 = new RectBatchItem(width: 10);
        var batchItem2 = new RectBatchItem(width: 20);

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
        Assert.NotEqual(batchItem1, sut.BatchItems[0]);
    }

    [Fact]
    public void EmptyBatch_WithNoItemsReadyToRender_DoesNotEmptyItems()
    {
        // Arrange
        var batchItem1 = default(RectBatchItem);
        var batchItem2 = default(RectBatchItem);

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<BatchSizeData>(It.IsAny<Action<Exception>?>()))
            .Returns(new BatchSizeData { BatchSize = 2u });

        var sut = CreateSystemUnderTest();

        this.reactor.OnNext(mockMessage.Object);

        sut.BatchItems = new List<RectBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        sut.EmptyBatch();

        // Assert
        Assert.Equal(batchItem1, sut.BatchItems[0]);
        Assert.Equal(batchItem2, sut.BatchItems[1]);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="RectBatchingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RectBatchingService CreateSystemUnderTest() => new (this.mockReactable.Object);
}

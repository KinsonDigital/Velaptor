// <copyright file="LineBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.OpenGL;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="LineBatchingService"/> class.
/// </summary>
public class LineBatchingServiceTests
{
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReactor<BatchSizeData>? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchingServiceTests"/> class.
    /// </summary>
    public LineBatchingServiceTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockBatchSizeReactable = new Mock<IReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReactor<BatchSizeData>>()))
            .Callback<IReactor<BatchSizeData>>(reactorObj => this.reactor = reactorObj)
            .Returns(this.mockUnsubscriber.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullBatchSizeReactableParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new LineBatchingService(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'batchSizeReactable')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizePushNotification_CreatesBatchItemList()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(4u));

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenEndNotificationsIsInvoked_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.reactor.OnCompleted();
        this.reactor.OnCompleted();

        // Assert
        this.mockUnsubscriber.Verify(m => m.Dispose());
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(1u));
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
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
    private LineBatchingService CreateSystemUnderTest() => new (this.mockBatchSizeReactable.Object);
}

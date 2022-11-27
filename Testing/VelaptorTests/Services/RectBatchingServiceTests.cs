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
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.OpenGL;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="RectBatchingService"/> class.
/// </summary>
public class RectBatchingServiceTests
{
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReactor<BatchSizeData>? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectBatchingServiceTests"/> class.
    /// </summary>
    public RectBatchingServiceTests()
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
            _ = new RectBatchingService(null);
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(1u));
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
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
    private RectBatchingService CreateSystemUnderTest() => new (this.mockBatchSizeReactable.Object);
}

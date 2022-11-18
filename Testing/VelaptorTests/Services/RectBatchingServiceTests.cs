// <copyright file="RectBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using Velaptor.Services;
using Xunit;

namespace VelaptorTests.Services;

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
        var sut = CreateService();
        this.reactor.OnNext(new BatchSizeData(4u));

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenEndNotificationsIsInvoked_UnsubscribesFromReactable()
    {
        // Arrange
        _ = CreateService();

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
        var batchItem1 = new RectShape
        {
            Position = new Vector2(1f, 2f),
            Width = 3f,
            Height = 4f,
            Color = Color.FromArgb(5, 6, 7, 8),
            IsFilled = true,
            BorderThickness = 9,
            CornerRadius = new CornerRadius(10, 11, 12, 13),
            GradientType = ColorGradient.None,
            GradientStart = Color.FromArgb(14, 15, 16, 17),
            GradientStop = Color.FromArgb(18, 19, 20, 21),
        };
        var batchItem2 = new RectShape
        {
            Position = new Vector2(22f, 23f),
            Width = 24f,
            Height = 25f,
            Color = Color.FromArgb(26, 27, 28, 29),
            IsFilled = true,
            BorderThickness = 30,
            CornerRadius = new CornerRadius(31, 32, 33, 34),
            GradientType = ColorGradient.None,
            GradientStart = Color.FromArgb(35, 36, 37, 38),
            GradientStop = Color.FromArgb(39, 40, 41, 42),
        };

        var batchItems = new List<RectShape> { batchItem1, batchItem2 };
        var expected = new ReadOnlyCollection<RectShape>(batchItems.ToReadOnlyCollection());
        var service = CreateService();

        // Act
        service.BatchItems = batchItems.ToReadOnlyCollection();
        var actual = service.BatchItems;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Add_WhenBatchIsFull_RaisesBatchFilledEvent()
    {
        // Arrange
        var batchItem1 = new RectShape
        {
            Width = 10,
            Height = 20,
        };
        var batchItem2 = new RectShape
        {
            Width = 30,
            Height = 40,
        };

        var service = CreateService();
        this.reactor.OnNext(new BatchSizeData(1u));
        service.Add(batchItem1);

        // Act & Assert
        Assert.Raises<EventArgs>(e =>
        {
            service.ReadyForRendering += e;
        }, e =>
        {
            service.ReadyForRendering -= e;
        }, () =>
        {
            service.Add(batchItem2);
        });
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var batchItem1 = new RectShape() { Width = 10 };
        var batchItem2 = new RectShape() { Width = 20 };

        var service = CreateService();
        this.reactor.OnNext(new BatchSizeData(2u));
        service.Add(batchItem1);
        service.Add(batchItem2);

        // Act
        service.EmptyBatch();

        // Assert
        Assert.NotEqual(batchItem1, service.BatchItems[0]);
    }

    [Fact]
    public void EmptyBatch_WithNoItemsReadyToRender_DoesNotEmptyItems()
    {
        // Arrange
        var batchItem1 = default(RectShape);
        var batchItem2 = default(RectShape);

        var service = CreateService();
        this.reactor.OnNext(new BatchSizeData(2u));
        service.BatchItems = new List<RectShape> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        service.EmptyBatch();

        // Assert
        Assert.Equal(batchItem1, service.BatchItems[0]);
        Assert.Equal(batchItem2, service.BatchItems[1]);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="RectBatchingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private RectBatchingService CreateService() => new (this.mockBatchSizeReactable.Object);
}

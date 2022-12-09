// <copyright file="TextureBatchingServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
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
/// Tests the <see cref="TextureBatchingService"/> class.
/// </summary>
public class TextureBatchingServiceTests
{
    private readonly Mock<IReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReactor<BatchSizeData>? reactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchingServiceTests"/> class.
    /// </summary>
    public TextureBatchingServiceTests()
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
            _ = new TextureBatchingService(null);
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
        var batchItem1 = new TextureBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            new SizeF(16, 17),
            11,
            18);
        var batchItem2 = new TextureBatchItem(
            new RectangleF(24, 25, 26, 27),
            new RectangleF(20, 21, 22, 23),
            19,
            18,
            Color.FromArgb(29, 30, 31, 32),
            RenderEffects.FlipHorizontally,
            new SizeF(33, 34),
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
            SizeF.Empty,
            1,
            0);

        var batchItem2 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            0,
            Color.Empty,
            RenderEffects.None,
            SizeF.Empty,
            2,
            0);

        var sut = CreateSystemUnderTest();

        var monitor = sut.Monitor();

        this.reactor.OnNext(new BatchSizeData(1u));
        sut.Add(batchItem1);

        // Act
        sut.Add(batchItem2);

        // Assert
        monitor.Should().Raise(nameof(TextureBatchingService.ReadyForRendering));
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
            SizeF.Empty,
            1,
            0);
        var batchItem2 = new TextureBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            2,
            0,
            Color.Empty,
            RenderEffects.None,
            SizeF.Empty,
            2,
            0);

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
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

        var sut = CreateSystemUnderTest();
        this.reactor.OnNext(new BatchSizeData(2u));
        sut.BatchItems = new List<TextureBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        sut.EmptyBatch();

        // Assert
        sut.BatchItems[0].Should().BeEquivalentTo(batchItem1);
        sut.BatchItems[1].Should().BeEquivalentTo(batchItem2);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureBatchingService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureBatchingService CreateSystemUnderTest() => new (this.mockBatchSizeReactable.Object);
}

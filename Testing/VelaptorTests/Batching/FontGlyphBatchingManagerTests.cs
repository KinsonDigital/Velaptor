// <copyright file="FontGlyphBatchingManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Batching;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Carbonate.Core.UniDirectional;
using Carbonate.NonDirectional;
using Carbonate.UniDirectional;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Velaptor.ReactableData;
using Velaptor.Batching;
using Xunit;

/// <summary>
/// Tests the <see cref="FontGlyphBatchingManager"/> class.
/// </summary>
public class FontGlyphBatchingManagerTests
{
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly Mock<IPushReactable<BatchSizeData>> mockBatchSizeReactable;
    private readonly Mock<IPushReactable> mockPushReactable;
    private readonly Mock<IDisposable> mockUnsubscriber;
    private IReceiveReactor<BatchSizeData>? batchSizeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGlyphBatchingManagerTests"/> class.
    /// </summary>
    public FontGlyphBatchingManagerTests()
    {
        this.mockUnsubscriber = new Mock<IDisposable>();

        this.mockPushReactable = new Mock<IPushReactable>();

        this.mockBatchSizeReactable = new Mock<IPushReactable<BatchSizeData>>();
        this.mockBatchSizeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveReactor<BatchSizeData>>()))
            .Callback<IReceiveReactor<BatchSizeData>>(reactorObj =>
            {
                reactorObj.Should().NotBeNull();
                this.batchSizeReactor = reactorObj;
            })
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
            _ = new FontGlyphBatchingManager(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void Ctor_WhenReceivingBatchSizePushNotification_CreatesBatchItemList()
    {
        // Arrange & Act
        var batchSizeData = new BatchSizeData { BatchSize = 4 };

        var sut = CreateService();

        this.batchSizeReactor.OnReceive(batchSizeData);

        // Assert
        sut.BatchItems.Should().HaveCount(4);
    }

    [Fact]
    public void Ctor_WhenReactableUnsubscribes_InvokesUnsubscriber()
    {
        // Arrange
        _ = CreateService();

        // Act
        this.batchSizeReactor.OnUnsubscribe();
        this.batchSizeReactor.OnUnsubscribe();

        // Assert
        this.mockUnsubscriber.Verify(m => m.Dispose());
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void BatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem1 = new FontGlyphBatchItem(
            new RectangleF(7, 8, 9, 10),
            new RectangleF(3, 4, 5, 6),
            'g',
            2,
            1,
            Color.FromArgb(12, 13, 14, 15),
            RenderEffects.None,
            11);
        var batchItem2 = new FontGlyphBatchItem(
            new RectangleF(20, 21, 22, 23),
            new RectangleF(24, 25, 26, 27),
            'g',
            19,
            18,
            Color.FromArgb(29, 30, 31, 32),
            RenderEffects.FlipHorizontally,
            28);

        var batchItems = new List<FontGlyphBatchItem> { batchItem1, batchItem2 };
        var expected = new ReadOnlyCollection<FontGlyphBatchItem>(batchItems.ToReadOnlyCollection());
        var service = CreateService();

        // Act
        service.BatchItems = batchItems.ToReadOnlyCollection();
        var actual = service.BatchItems;

        // Assert
        AssertExtensions.ItemsEqual(expected.ToArray(), actual.ToArray());
        AssertExtensions.ItemsEqual(expected.ToArray(), actual.ToArray());
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Add_WhenBatchIsFull_SendsRenderPushNotification()
    {
        // Arrange
        var batchItem1 = new FontGlyphBatchItem(
            default,
            default,
            'g',
            0,
            0,
            Color.Empty,
            RenderEffects.None,
            0);
        var batchItem2 = new FontGlyphBatchItem(
            default,
            default,
            'g',
            0,
            0,
            Color.Empty,
            RenderEffects.None,
            0);

        var batchSizeData = new BatchSizeData { BatchSize = 1 };

        var service = CreateService();
        this.batchSizeReactor.OnReceive(batchSizeData);
        service.Add(batchItem1);

        // Act
        service.Add(batchItem2);

        // Assert
        this.mockPushReactable.Verify(m => m.Push(PushNotifications.RenderFontsId));
    }

    [Fact]
    public void EmptyBatch_WhenInvoked_EmptiesAllItemsReadyToRender()
    {
        // Arrange
        var batchItem1 = new FontGlyphBatchItem(
            default,
            default,
            'g',
            0,
            0,
            Color.Empty,
            RenderEffects.None,
            0);
        var batchItem2 = new FontGlyphBatchItem(
            default,
            default,
            'g',
            0,
            0,
            Color.Empty,
            RenderEffects.None,
            0);

        var batchSizeData = new BatchSizeData { BatchSize = 123 };

        var service = CreateService();
        this.batchSizeReactor.OnReceive(batchSizeData);
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
        var batchItem1 = default(FontGlyphBatchItem);
        var batchItem2 = default(FontGlyphBatchItem);

        var batchSizeData = new BatchSizeData { BatchSize = 123 };

        var service = CreateService();
        this.batchSizeReactor.OnReceive(batchSizeData);
        service.BatchItems = new List<FontGlyphBatchItem> { batchItem1, batchItem2 }.ToReadOnlyCollection();

        // Act
        service.EmptyBatch();

        // Assert
        Assert.Equal(batchItem1, service.BatchItems[0]);
        Assert.Equal(batchItem2, service.BatchItems[1]);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontGlyphBatchingManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontGlyphBatchingManager CreateService() => new (this.mockReactableFactory.Object);
}

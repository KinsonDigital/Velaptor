// <copyright file="BatchServiceManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Moq;
using Velaptor.OpenGL;
using Velaptor.Services;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="BatchServiceManager"/> class.
/// </summary>
public class BatchServiceManagerTests
{
    private readonly Mock<IBatchingService<TextureBatchItem>> mockTextureBatchingService;
    private readonly Mock<IBatchingService<FontGlyphBatchItem>> mockFontGlyphBatchingService;
    private readonly Mock<IBatchingService<RectBatchItem>> mockRectBatchingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchServiceManagerTests"/> class.
    /// </summary>
    public BatchServiceManagerTests()
    {
        this.mockTextureBatchingService = new Mock<IBatchingService<TextureBatchItem>>();
        this.mockFontGlyphBatchingService = new Mock<IBatchingService<FontGlyphBatchItem>>();
        this.mockRectBatchingService = new Mock<IBatchingService<RectBatchItem>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureBatchingServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new BatchServiceManager(
                null,
                new Mock<IBatchingService<FontGlyphBatchItem>>().Object,
                new Mock<IBatchingService<RectBatchItem>>().Object);
        }, "The parameter must not be null. (Parameter 'textureBatchingService')");
    }

    [Fact]
    public void Ctor_WithNullFontGlyphBatchingServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new BatchServiceManager(
                new Mock<IBatchingService<TextureBatchItem>>().Object,
                null,
                new Mock<IBatchingService<RectBatchItem>>().Object);
        }, "The parameter must not be null. (Parameter 'fontGlyphBatchingService')");
    }

    [Fact]
    public void Ctor_WithNullRectBatchingServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new BatchServiceManager(
                new Mock<IBatchingService<TextureBatchItem>>().Object,
                new Mock<IBatchingService<FontGlyphBatchItem>>().Object,
                null);
        }, "The parameter must not be null. (Parameter 'rectBatchingService')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToBatchFilledEvents()
    {
        // Arrange & Act
        CreateManager();

        // Assert
        this.mockTextureBatchingService.VerifyAdd(e => e.ReadyForRendering += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        this.mockFontGlyphBatchingService.VerifyAdd(e => e.ReadyForRendering += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        this.mockRectBatchingService.VerifyAdd(e => e.ReadyForRendering += It.IsAny<EventHandler<EventArgs>>(), Times.Once);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void TextureBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(TextureBatchItem);
        var batchItems = new List<TextureBatchItem>()
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<TextureBatchItem>(batchItems);

        this.mockTextureBatchingService.SetupProperty(p => p.BatchItems);

        var manager = CreateManager();

        // Act
        manager.TextureBatchItems = new ReadOnlyCollection<TextureBatchItem>(batchItems);
        var actual = manager.TextureBatchItems;

        // Assert
        this.mockTextureBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)}' was not invoked.");
        this.mockTextureBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)} was not invoked.'");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void FontGlyphBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(FontGlyphBatchItem);
        var batchItems = new List<FontGlyphBatchItem>()
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<FontGlyphBatchItem>(batchItems);

        this.mockFontGlyphBatchingService.SetupProperty(p => p.BatchItems);

        var manager = CreateManager();

        // Act
        manager.FontGlyphBatchItems = new ReadOnlyCollection<FontGlyphBatchItem>(batchItems);
        var actual = manager.FontGlyphBatchItems;

        // Assert
        this.mockFontGlyphBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)}' was not invoked.");
        this.mockFontGlyphBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)} was not invoked.'");
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void RectBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(RectBatchItem);
        var batchItems = new List<RectBatchItem>()
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<RectBatchItem>(batchItems);

        this.mockRectBatchingService.SetupProperty(p => p.BatchItems);

        var manager = CreateManager();

        // Act
        manager.RectBatchItems = new ReadOnlyCollection<RectBatchItem>(batchItems);
        var actual = manager.RectBatchItems;

        // Assert
        this.mockRectBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<RectBatchItem>)}.{nameof(IBatchingService<RectBatchItem>.BatchItems)}' was not invoked.");
        this.mockRectBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<RectBatchItem>)}.{nameof(IBatchingService<RectBatchItem>.BatchItems)} was not invoked.'");
        Assert.Equal(expected, actual);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void EmptyBatch_WhenInvoked_EmptiesCorrectBatch(int serviceTypeValue)
    {
        // Arrange
        var serviceType = (BatchServiceType)serviceTypeValue;
        var manager = CreateManager();

        // Act
        manager.EmptyBatch(serviceType);

        // Assert
        switch (serviceType)
        {
            case BatchServiceType.Texture:
                this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                break;
            case BatchServiceType.FontGlyph:
                this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                break;
            case BatchServiceType.Rectangle:
                this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockRectBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                break;
        }
    }

    [Fact]
    public void EmptyBatch_WithInvalidServiceType_ThrowsException()
    {
        // Arrange
        var manager = CreateManager();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
        {
            manager.EmptyBatch((BatchServiceType)1234);
        }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");
    }

    [Fact]
    public void AddTextureBatchItem_WhenInvoked_AddsBatchItem()
    {
        // Arrange
        var manager = CreateManager();
        var item = default(TextureBatchItem);

        // Act
        manager.AddTextureBatchItem(item);

        // Assert
        this.mockTextureBatchingService.Verify(m => m.Add(item), Times.Once);

        // Assert that the font glyph batching service is not used
        this.mockFontGlyphBatchingService.Verify(m =>
            m.Add(It.IsAny<FontGlyphBatchItem>()), Times.Never);

        // Assert that the rectangle batching service is not used
        this.mockRectBatchingService.Verify(m =>
            m.Add(It.IsAny<RectBatchItem>()), Times.Never);
    }

    [Fact]
    public void AddTextureBatchItem_WhenInvoked_AddsItemToTextureBatchingService()
    {
        // Arrange
        var batchItem = default(TextureBatchItem);
        var manager = CreateManager();

        // Act
        manager.AddTextureBatchItem(batchItem);

        // Assert
        this.mockTextureBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void AddFontGlyphBatchItem_WhenInvoked_AddsItemToFontGlyphBatchingService()
    {
        // Arrange
        var batchItem = default(FontGlyphBatchItem);
        var manager = CreateManager();

        // Act
        manager.AddFontGlyphBatchItem(batchItem);

        // Assert
        this.mockFontGlyphBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void AddRectBatchItem_WhenInvoked_AddsItemToRectBatchingService()
    {
        // Arrange
        var batchItem = default(RectBatchItem);
        var manager = CreateManager();

        // Act
        manager.AddRectBatchItem(batchItem);

        // Assert
        this.mockRectBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void EndBatch_WhenTextureBatch_RaisesTextureBatchFilledEvent()
    {
        // Arrange
        var manager = CreateManager();

        // Act & Assert
        Assert.Raises<EventArgs>(e =>
        {
            manager.TextureBatchReadyForRendering += e;
        }, e =>
        {
            manager.TextureBatchReadyForRendering -= e;
        }, () =>
        {
            manager.EndBatch(BatchServiceType.Texture);
        });
    }

    [Fact]
    public void EndBatch_WhenFontGlyphBatch_RaisesFontGlyphBatchFilledEvent()
    {
        // Arrange
        var manager = CreateManager();

        // Act & Assert
        Assert.Raises<EventArgs>(e =>
        {
            manager.FontGlyphBatchReadyForRendering += e;
        }, e =>
        {
            manager.FontGlyphBatchReadyForRendering -= e;
        }, () =>
        {
            manager.EndBatch(BatchServiceType.FontGlyph);
        });
    }

    [Fact]
    public void EndBatch_WhenRectBatch_RaisesRectBatchFilledEvent()
    {
        // Arrange
        var manager = CreateManager();

        // Act & Assert
        Assert.Raises<EventArgs>(e =>
        {
            manager.RectBatchReadyForRendering += e;
        }, e =>
        {
            manager.RectBatchReadyForRendering -= e;
        }, () =>
        {
            manager.EndBatch(BatchServiceType.Rectangle);
        });
    }

    [Fact]
    public void EndBatch_WithInvalidServiceTypeValue_ThrowsException()
    {
        // Arrange
        var manager = CreateManager();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
        {
            manager.EndBatch((BatchServiceType)1234);
        }, $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType'){Environment.NewLine}Actual value was 1234.");
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfManager()
    {
        // Arrange
        var manager = CreateManager();

        // Act
        manager.Dispose();
        manager.Dispose();

        // Assert
        this.mockTextureBatchingService.VerifyRemove(e => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        this.mockFontGlyphBatchingService.VerifyRemove(e => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
        this.mockRectBatchingService.VerifyRemove(e => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BatchServiceManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BatchServiceManager CreateManager() =>
        new (this.mockTextureBatchingService.Object,
            this.mockFontGlyphBatchingService.Object,
            this.mockRectBatchingService.Object);
}

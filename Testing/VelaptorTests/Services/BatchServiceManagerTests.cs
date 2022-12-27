// <copyright file="BatchServiceManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Services;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FluentAssertions;
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
    private readonly Mock<IBatchingService<LineBatchItem>> mockLineBatchingService;

    /// <summary>
    /// Initializes a new instance of the <see cref="BatchServiceManagerTests"/> class.
    /// </summary>
    public BatchServiceManagerTests()
    {
        this.mockTextureBatchingService = new Mock<IBatchingService<TextureBatchItem>>();
        this.mockFontGlyphBatchingService = new Mock<IBatchingService<FontGlyphBatchItem>>();
        this.mockRectBatchingService = new Mock<IBatchingService<RectBatchItem>>();
        this.mockLineBatchingService = new Mock<IBatchingService<LineBatchItem>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullTextureBatchingServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new BatchServiceManager(
                null,
                this.mockFontGlyphBatchingService.Object,
                this.mockRectBatchingService.Object,
                this.mockLineBatchingService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'textureBatchingService')");
    }

    [Fact]
    public void Ctor_WithNullFontGlyphBatchingServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new BatchServiceManager(
                this.mockTextureBatchingService.Object,
                null,
                this.mockRectBatchingService.Object,
                this.mockLineBatchingService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontGlyphBatchingService')");
    }

    [Fact]
    public void Ctor_WithNullRectBatchingServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new BatchServiceManager(
                this.mockTextureBatchingService.Object,
                this.mockFontGlyphBatchingService.Object,
                null,
                this.mockLineBatchingService.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'rectBatchingService')");
    }

    [Fact]
    public void Ctor_WhenInvoked_SubscribesToBatchFilledEvents()
    {
        // Arrange & Act
        CreateSystemUnderTest();

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
        var batchItems = new List<TextureBatchItem>
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<TextureBatchItem>(batchItems);

        this.mockTextureBatchingService.SetupProperty(p => p.BatchItems);

        var sut = CreateSystemUnderTest();

        // Act
        sut.TextureBatchItems = new ReadOnlyCollection<TextureBatchItem>(batchItems);
        var actual = sut.TextureBatchItems;

        // Assert
        this.mockTextureBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)}' was not invoked.");
        this.mockTextureBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<TextureBatchItem>)}.{nameof(IBatchingService<TextureBatchItem>.BatchItems)} was not invoked.'");
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void FontGlyphBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(FontGlyphBatchItem);
        var batchItems = new List<FontGlyphBatchItem>
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<FontGlyphBatchItem>(batchItems);

        this.mockFontGlyphBatchingService.SetupProperty(p => p.BatchItems);

        var sut = CreateSystemUnderTest();

        // Act
        sut.FontGlyphBatchItems = new ReadOnlyCollection<FontGlyphBatchItem>(batchItems);
        var actual = sut.FontGlyphBatchItems;

        // Assert
        this.mockFontGlyphBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)}' was not invoked.");
        this.mockFontGlyphBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<FontGlyphBatchItem>)}.{nameof(IBatchingService<FontGlyphBatchItem>.BatchItems)} was not invoked.'");
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void RectBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(RectBatchItem);
        var batchItems = new List<RectBatchItem>
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<RectBatchItem>(batchItems);

        this.mockRectBatchingService.SetupProperty(p => p.BatchItems);

        var sut = CreateSystemUnderTest();

        // Act
        sut.RectBatchItems = new ReadOnlyCollection<RectBatchItem>(batchItems);
        var actual = sut.RectBatchItems;

        // Assert
        this.mockRectBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<RectBatchItem>)}.{nameof(IBatchingService<RectBatchItem>.BatchItems)}' was not invoked.");
        this.mockRectBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<RectBatchItem>)}.{nameof(IBatchingService<RectBatchItem>.BatchItems)} was not invoked.'");
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void LineBatchItems_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var batchItem = default(LineBatchItem);
        var batchItems = new List<LineBatchItem>
        {
            batchItem,
        };
        var expected = new ReadOnlyCollection<LineBatchItem>(batchItems);

        this.mockLineBatchingService.SetupProperty(p => p.BatchItems);

        var sut = CreateSystemUnderTest();

        // Act
        sut.LineBatchItems = new ReadOnlyCollection<LineBatchItem>(batchItems);
        var actual = sut.LineBatchItems;

        // Assert
        this.mockLineBatchingService.VerifySet(p => p.BatchItems = expected,
            Times.Once,
            $"The setter for property '{nameof(IBatchingService<LineBatchItem>)}.{nameof(IBatchingService<LineBatchItem>.BatchItems)}' was not invoked.");
        this.mockLineBatchingService.VerifyGet(p => p.BatchItems,
            Times.Once,
            $"The getter for property '{nameof(IBatchingService<LineBatchItem>)}.{nameof(IBatchingService<LineBatchItem>.BatchItems)} was not invoked.'");
        actual.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(1)] // Texture
    [InlineData(2)] // FontGlyph
    [InlineData(3)] // Rectangle
    [InlineData(4)] // Line
    public void EmptyBatch_WhenInvoked_EmptiesCorrectBatch(int serviceTypeValue)
    {
        // Arrange
        var serviceType = (BatchServiceType)serviceTypeValue;
        var sut = CreateSystemUnderTest();

        // Act
        sut.EmptyBatch(serviceType);

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
            case BatchServiceType.Line:
                this.mockTextureBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockFontGlyphBatchingService.Verify(m => m.EmptyBatch(), Times.Never);
                this.mockLineBatchingService.Verify(m => m.EmptyBatch(), Times.Once);
                break;
            default:
                Assert.True(false, $"The value of the enum '{nameof(BatchServiceType)}' is not valid.");
                break;
        }
    }

    [Fact]
    public void EmptyBatch_WithInvalidServiceType_ThrowsException()
    {
        // Arrange
        var expected = $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType')";
        expected += $"{Environment.NewLine}Actual value was 1234.";

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.EmptyBatch((BatchServiceType)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage(expected);
    }

    [Fact]
    public void AddTextureBatchItem_WhenInvoked_AddsBatchItem()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        var item = default(TextureBatchItem);

        // Act
        sut.AddTextureBatchItem(item);

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
        var sut = CreateSystemUnderTest();

        // Act
        sut.AddTextureBatchItem(batchItem);

        // Assert
        this.mockTextureBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void AddFontGlyphBatchItem_WhenInvoked_AddsItemToFontGlyphBatchingService()
    {
        // Arrange
        var batchItem = default(FontGlyphBatchItem);
        var sut = CreateSystemUnderTest();

        // Act
        sut.AddFontGlyphBatchItem(batchItem);

        // Assert
        this.mockFontGlyphBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void AddRectBatchItem_WhenInvoked_AddsItemToRectBatchingService()
    {
        // Arrange
        var batchItem = default(RectBatchItem);
        var sut = CreateSystemUnderTest();

        // Act
        sut.AddRectBatchItem(batchItem);

        // Assert
        this.mockRectBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void AddLineBatchItem_WhenInvoked_AddsItemToRectBatchingService()
    {
        // Arrange
        var batchItem = default(LineBatchItem);
        var sut = CreateSystemUnderTest();

        // Act
        sut.AddLineBatchItem(batchItem);

        // Assert
        this.mockLineBatchingService.VerifyOnce(m => m.Add(batchItem));
    }

    [Fact]
    public void EndBatch_WhenTextureBatch_RaisesTextureBatchFilledEvent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        sut.EndBatch(BatchServiceType.Texture);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.TextureBatchReadyForRendering));
    }

    [Fact]
    public void EndBatch_WhenFontGlyphBatch_RaisesFontGlyphBatchFilledEvent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        sut.EndBatch(BatchServiceType.FontGlyph);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.FontGlyphBatchReadyForRendering));
    }

    [Fact]
    public void EndBatch_WhenRectBatch_RaisesRectBatchFilledEvent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        sut.EndBatch(BatchServiceType.Rectangle);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.RectBatchReadyForRendering));
    }

    [Fact]
    public void EndBatch_WhenLineBatch_RaisesLineBatchFilledEvent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        sut.EndBatch(BatchServiceType.Line);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.LineBatchReadyForRendering));
    }

    [Fact]
    public void EndBatch_WithInvalidServiceTypeValue_ThrowsException()
    {
        // Arrange
        var expected = $"The enum '{nameof(BatchServiceType)}' value is invalid. (Parameter 'serviceType')";
        expected += $"{Environment.NewLine}Actual value was 1234.";

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.EndBatch((BatchServiceType)1234);

        // Assert
        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage(expected);
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfManager()
    {
        // Arrange
       var sut = CreateSystemUnderTest();

        // Act
       sut.Dispose();
       sut.Dispose();

        // Assert
       this.mockTextureBatchingService.VerifyRemove(e
            => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
       this.mockFontGlyphBatchingService.VerifyRemove(e
            => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
       this.mockRectBatchingService.VerifyRemove(e
            => e.ReadyForRendering -= It.IsAny<EventHandler<EventArgs>>(), Times.Once);
    }

    [Fact]
    public void TextureBatchService_WhenReadyForRendering_InvokesBatchManagerEvent()
    {
        // Arrange
        var eventArgs = EventArgs.Empty;
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        this.mockTextureBatchingService.Raise(service
            => service.ReadyForRendering += null, eventArgs);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.TextureBatchReadyForRendering))
            .WithArgs<EventArgs>(args => args == eventArgs);
    }

    [Fact]
    public void FontGlyphBatchService_WhenReadyForRendering_InvokesBatchManagerEvent()
    {
        // Arrange
        var eventArgs = EventArgs.Empty;
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        this.mockFontGlyphBatchingService.Raise(service
            => service.ReadyForRendering += null, eventArgs);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.FontGlyphBatchReadyForRendering))
            .WithArgs<EventArgs>(args => args == eventArgs);
    }

    [Fact]
    public void RectBatchService_WhenReadyForRendering_InvokesBatchManagerEvent()
    {
        // Arrange
        var eventArgs = EventArgs.Empty;
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        this.mockRectBatchingService.Raise(service
            => service.ReadyForRendering += null, eventArgs);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.RectBatchReadyForRendering))
            .WithArgs<EventArgs>(args => args == eventArgs);
    }

    [Fact]
    public void LineBatchService_WhenReadyForRendering_InvokesBatchManagerEvent()
    {
        // Arrange
        var eventArgs = EventArgs.Empty;
        var sut = CreateSystemUnderTest();
        using var monitor = sut.Monitor();

        // Act
        this.mockLineBatchingService.Raise(service
            => service.ReadyForRendering += null, eventArgs);

        // Assert
        monitor.Should().Raise(nameof(BatchServiceManager.LineBatchReadyForRendering))
            .WithArgs<EventArgs>(args => args == eventArgs);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BatchServiceManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BatchServiceManager CreateSystemUnderTest() =>
        new (this.mockTextureBatchingService.Object,
            this.mockFontGlyphBatchingService.Object,
            this.mockRectBatchingService.Object,
            this.mockLineBatchingService.Object);
}

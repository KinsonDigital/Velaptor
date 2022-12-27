// <copyright file="BufferManagerTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Buffers;

using System;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.OpenGL;
using Velaptor.OpenGL.Buffers;
using Xunit;

/// <summary>
/// Tests the <see cref="BufferManager"/> class.
/// </summary>
public class BufferManagerTests
{
    private readonly Mock<IGPUBufferFactory> mockBufferFactory;
    private readonly Mock<IGPUBuffer<TextureBatchItem>> mockTextureBuffer;
    private readonly Mock<IGPUBuffer<FontGlyphBatchItem>> mockFontGlyphBuffer;
    private readonly Mock<IGPUBuffer<RectBatchItem>> mockRectBuffer;
    private readonly Mock<IGPUBuffer<LineBatchItem>> mockLineBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferManagerTests"/> class.
    /// </summary>
    public BufferManagerTests()
    {
        this.mockTextureBuffer = new Mock<IGPUBuffer<TextureBatchItem>>();
        this.mockFontGlyphBuffer = new Mock<IGPUBuffer<FontGlyphBatchItem>>();
        this.mockRectBuffer = new Mock<IGPUBuffer<RectBatchItem>>();
        this.mockLineBuffer = new Mock<IGPUBuffer<LineBatchItem>>();

        this.mockBufferFactory = new Mock<IGPUBufferFactory>();
        this.mockBufferFactory.Setup(m => m.CreateTextureGPUBuffer()).Returns(this.mockTextureBuffer.Object);
        this.mockBufferFactory.Setup(m => m.CreateFontGPUBuffer()).Returns(this.mockFontGlyphBuffer.Object);
        this.mockBufferFactory.Setup(m => m.CreateRectGPUBuffer()).Returns(this.mockRectBuffer.Object);
        this.mockBufferFactory.Setup(m => m.CreateLineGPUBuffer()).Returns(this.mockLineBuffer.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullBufferFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new BufferManager(null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'bufferFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void SetViewPortSize_WithTextureBufferType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedSize = new SizeU(111u, 222u);
        var sut = CreateSystemUnderTest();

        // Act
        sut.SetViewPortSize(VelaptorBufferType.Texture, new SizeU(111u, 222u));

        // Assert
        this.mockTextureBuffer.VerifySetOnce(p => p.ViewPortSize = expectedSize);
    }

    [Fact]
    public void SetViewPortSize_WithFontGlyphBufferType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedSize = new SizeU(111u, 222u);
        var sut = CreateSystemUnderTest();

        // Act
        sut.SetViewPortSize(VelaptorBufferType.Font, new SizeU(111u, 222u));

        // Assert
        this.mockFontGlyphBuffer.VerifySetOnce(p => p.ViewPortSize = expectedSize);
    }

    [Fact]
    public void SetViewPortSize_WithRectangleBufferType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedSize = new SizeU(111u, 222u);
        var sut = CreateSystemUnderTest();

        // Act
        sut.SetViewPortSize(VelaptorBufferType.Rectangle, new SizeU(111u, 222u));

        // Assert
        this.mockRectBuffer.VerifySetOnce(p => p.ViewPortSize = expectedSize);
    }

    [Fact]
    public void SetViewPortSize_WithLineBufferType_ReturnsCorrectResult()
    {
        // Arrange
        var expectedSize = new SizeU(111u, 222u);
        var sut = CreateSystemUnderTest();

        // Act
        sut.SetViewPortSize(VelaptorBufferType.Line, new SizeU(111u, 222u));

        // Assert
        this.mockLineBuffer.VerifySetOnce(p => p.ViewPortSize = expectedSize);
    }

    [Fact]
    public void SetViewPortSize_WithInvalidBufferType_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentOutOfRangeException>(() =>
        {
            sut.SetViewPortSize((VelaptorBufferType)1234, It.IsAny<SizeU>());
        }, $"The enum '{nameof(VelaptorBufferType)}' value is invalid. (Parameter 'bufferType'){Environment.NewLine}Actual value was 1234.");
    }

    [Fact]
    public void UploadTextureData_WhenInvoked_UploadsData()
    {
        // Arrange
        var data = default(TextureBatchItem);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadTextureData(data, 123u);

        // Assert
        this.mockTextureBuffer.VerifyOnce(m => m.UploadData(data, 123u));
    }

    [Fact]
    public void UploadFontGlyphData_WhenInvoked_UploadsData()
    {
        // Arrange
        var data = new FontGlyphBatchItem(
            RectangleF.Empty,
            RectangleF.Empty,
            'g',
            2.5F,
            90,
            Color.Empty,
            RenderEffects.FlipVertically,
            SizeF.Empty,
            0,
            0);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadFontGlyphData(data, 456u);

        // Assert
        this.mockFontGlyphBuffer.VerifyOnce(m => m.UploadData(data, 456u));
    }

    [Fact]
    public void UploadRectangleData_WhenInvoked_UploadsData()
    {
        // Arrange
        var data = new RectBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9f,
            new CornerRadius(10, 11, 13, 14),
            ColorGradient.Horizontal,
            Color.FromArgb(15, 16, 17, 18),
            Color.FromArgb(15, 16, 17, 18),
            19);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadRectData(data, 789u);

        // Assert
        this.mockRectBuffer.VerifyOnce(m => m.UploadData(data, 789u));
    }

    [Fact]
    public void UploadLineData_WhenInvoked_UploadsData()
    {
        // Arrange
        var data = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(3, 4),
            Color.FromArgb(5, 6, 7, 8),
            19);

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadLineData(data, 789u);

        // Assert
        this.mockLineBuffer.VerifyOnce(m => m.UploadData(data, 789u));
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="BufferManager"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private BufferManager CreateSystemUnderTest() => new (this.mockBufferFactory.Object);
}

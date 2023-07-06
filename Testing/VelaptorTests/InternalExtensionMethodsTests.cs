// <copyright file="InternalExtensionMethodsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable RedundantArgumentDefaultValue
#pragma warning disable CS8524

namespace VelaptorTests;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Velaptor;
using Velaptor.Graphics;
using Velaptor.Input;
using Velaptor.OpenGL.Batching;
using Xunit;
using NETColor = System.Drawing.Color;
using NETPoint = System.Drawing.Point;
using NETRectF = System.Drawing.RectangleF;
using NETSizeF = System.Drawing.SizeF;

/// <summary>
/// Tests the <see cref="Velaptor.InternalExtensionMethods"/> class.
/// </summary>
public class InternalExtensionMethodsTests
{
    #region Method Tests
    [Fact]
    public void ToSixLaborImage_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var imageData = new ImageData(new NETColor[2, 3], 2, 3);

        var expectedPixels = new Rgba32[2, 3];

        // Act
        var sixLaborsImage = imageData.ToSixLaborImage();
        var actualPixels = GetSixLaborPixels(sixLaborsImage);

        // Assert
        actualPixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void ToImageData_WhenInvoked_CorrectlyConvertsToSixLaborImage()
    {
        // Arrange
        var rowColors = new Dictionary<uint, NETColor>
        {
            { 0, NETColor.Red },
            { 1, NETColor.Green },
            { 2, NETColor.Blue },
        };

        var sixLaborsImage = CreateSixLaborsImage(2, 3, rowColors);
        var expectedPixels = CreateImageDataPixels(2, 3, rowColors);

        // Act
        var actual = TestHelpers.ToImageData(sixLaborsImage);

        // Assert
        actual.Pixels.Should().BeEquivalentTo(expectedPixels);
    }

    [Fact]
    public void GetPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var rect = new NETRectF(11f, 22f, 33f, 44f);

        // Act
        var actual = rect.GetPosition();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Theory]
    [InlineData(@"C:\dir1\dir2", "C:/dir1/dir2")]
    [InlineData(@"C:\dir1\dir2\", "C:/dir1/dir2/")]
    [InlineData("C:/dir1/dir2", "C:/dir1/dir2")]
    [InlineData("C:/dir1/dir2/", "C:/dir1/dir2/")]
    public void NormalizePaths_WhenInvoked_ReturnsCorrectResult(string path, string expected)
    {
        // Arrange
        var paths = new[] { path };

        // Act
        var actual = paths.NormalizePaths().ToArray();

        // Assert
        actual.Should().ContainSingle();
        actual[0].Should().Be(expected);
    }

    [Fact]
    public void ToVector2_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new NETPoint(11, 22);

        // Act
        var actual = point.ToVector2();

        // Assert
        actual.X.Should().Be(11f);
        actual.Y.Should().Be(22f);
    }

    [Fact]
    public void ToPoint_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var point = new Vector2(11, 22);

        // Act
        var actual = point.ToPoint();

        // Assert
        actual.X.Should().Be(11);
        actual.Y.Should().Be(22);
    }

    [Fact]
    public void DequeueWhile_WithNoItems_DoesNotInvokedPredicate()
    {
        // Arrange
        var queue = new Queue<int>();

        var untilPredicate = new Predicate<int>(_ =>
        {
            Assert.Fail("The 'untilPredicate' should not be invoked with 0 queue items.");
            return false;
        });

        // Act & Assert
        queue.DequeueWhile(untilPredicate);
    }

    [Fact]
    public void DequeueWhile_WhenInvoked_PerformsDequeueWhenPredicateIsTrue()
    {
        // Arrange
        var totalInvokes = 0;
        var queue = new Queue<int>();
        queue.Enqueue(11);
        queue.Enqueue(22);

        var untilPredicate = new Predicate<int>(_ =>
        {
            totalInvokes += 1;
            return true;
        });

        // Act
        queue.DequeueWhile(untilPredicate);

        // Assert
        totalInvokes.Should().Be(2);
        queue.Should().BeEmpty();
    }

    [Theory]
    [InlineData(3, 2)]
    [InlineData(40, -1)]
    public void IndexOf_WithEnumerableItemsAndPredicate_ReturnsCorrectResult(int value, int expected)
    {
        // Arrange
        var items = new[] { 1, 2, 3, 4 };

        // Act
        var actual = items.IndexOf(i => i == value);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateReturnsTrue_ReturnsCorrectIndex()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-C");

        // Assert
        actual.Should().Be(1);
    }

    [Fact]
    public void FirstItemIndex_WhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Memory<string>(new[] { "item-A", "item-C", "item-B" });

        // Act
        var actual = sut.FirstItemIndex(i => i == "item-D");

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(30);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void FirstLayerIndex_WhenLayerDoesNotExists_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 30, Item = "itemC" },
            new () { Layer = 40, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.FirstLayerIndex(300);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void TotalOnLayer_WithLayerGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(20);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void TotalOnLayer_WithNoLayersGreaterThanRequestedLayer_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.TotalOnLayer(200);

        // Assert
        actual.Should().Be(0);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            default,
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(2);
    }

    [Fact]
    public void IndexOf_WithMemoryItemsAndWhenPredicateNeverReturnsTrue_ReturnsCorrectResult()
    {
        // Arrange
        var renderItems = new RenderItem<string>[]
        {
            new () { Layer = 10, Item = "itemA" },
            new () { Layer = 20, Item = "itemB" },
            new () { Layer = 20, Item = "itemC" },
            new () { Layer = 30, Item = "itemD" },
        };

        var items = new Memory<RenderItem<string>>(renderItems);

        // Act
        var actual = items.IndexOf(string.IsNullOrEmpty);

        // Assert
        actual.Should().Be(-1);
    }

    [Fact]
    public void IncreaseBy_WhenInvoked_CorrectlyIncreasesItems()
    {
        // Arrange
        var expected = new[] { 1, 2, 3, 4, 0, 0 };
        var items = new Memory<int>(new[] { 1, 2, 3, 4, });

        // Act
        items.IncreaseBy(2);

        // Assert
        items.Span.ToArray().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithRectShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            100f,
            90f,
            NETColor.FromArgb(5, 6, 7, 8),
            true,
            30f,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.Horizontal,
            NETColor.FromArgb(14, 15, 16, 17),
            NETColor.FromArgb(18, 19, 20, 21));

        var sut = new RectShape
        {
            Position = new Vector2(1, 2),
            Width = 100,
            Height = 90,
            Color = NETColor.FromArgb(5, 6, 7, 8),
            IsSolid = true,
            BorderThickness = 30,
            CornerRadius = new CornerRadius(10, 11, 12, 13),
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(14, 15, 16, 17),
            GradientStop = NETColor.FromArgb(18, 19, 20, 21),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ToBatchItem_WithCircleShapeOverload_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new ShapeBatchItem(new Vector2(1, 2),
            100f,
            100f,
            NETColor.FromArgb(4, 5, 6, 7),
            true,
            50f,
            new CornerRadius(50f),
            ColorGradient.Horizontal,
            NETColor.FromArgb(9, 10, 11, 12),
            NETColor.FromArgb(13, 14, 15, 16));

        var sut = new CircleShape
        {
            Position = new Vector2(1, 2),
            Diameter = 100,
            Color = NETColor.FromArgb(4, 5, 6, 7),
            IsSolid = true,
            BorderThickness = 50,
            GradientType = ColorGradient.Horizontal,
            GradientStart = NETColor.FromArgb(9, 10, 11, 12),
            GradientStop = NETColor.FromArgb(13, 14, 15, 16),
        };

        // Act
        var actual = sut.ToBatchItem();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(KeyCode.Left, true, true)]
    [InlineData(KeyCode.Right, true, true)]
    [InlineData(KeyCode.Up, true, true)]
    [InlineData(KeyCode.Down, true, true)]
    [InlineData(KeyCode.Down, false, false)]
    public void AnyArrowKeysDown_WhenInvoked_ReturnsCorrectResult(
        KeyCode key,
        bool state,
        bool expected)
    {
        // Arrange
        var keyState = default(KeyboardState);
        keyState.SetKeyState(key, state);

        // Act
        var actual = keyState.AnyArrowKeysDown();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion

    /// <summary>
    /// Creates a Six Labors image type of <see cref="Image{Rgba32}"/> with the given <paramref name="width"/>
    /// and <paramref name="height"/> with each row having its own colors described by the given
    /// <paramref name="rowColors"/> dictionary.
    /// </summary>
    /// <param name="width">The width of the image.</param>
    /// <param name="height">The height of the image.</param>
    /// <param name="rowColors">The color for each row.</param>
    /// <returns>An image with the given row colors.</returns>
    /// <remarks>
    ///     The <paramref name="rowColors"/> dictionary key is the zero based row index and the
    ///     value is the color to make the entire row.
    /// </remarks>
    [ExcludeFromCodeCoverage(Justification = "Do not need to see coverage for code used for testing.")]
    private static Image<Rgba32> CreateSixLaborsImage(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        if (height != rowColors.Count)
        {
            Assert.Fail($"The height '{height}' of the image must match the total number of row colors '{rowColors.Count}'.");
        }

        var availableRows = rowColors.Keys.ToArray();

        foreach (var row in availableRows)
        {
            if (row > height - 1)
            {
                Assert.Fail($"The row '{row}' is not within the range of rows for the image height '{height}' for the definition of row colors.");
            }
        }

        var result = new Image<Rgba32>(width, height);

        for (var y = 0; y < height; y++)
        {
            var row = y;
            result.ProcessPixelRows(accessor =>
            {
                var rowSpan = accessor.GetRowSpan(row);

                for (var x = 0; x < width; x++)
                {
                    rowSpan[x] = new Rgba32(
                        rowColors[(uint)row].R,
                        rowColors[(uint)row].G,
                        rowColors[(uint)row].B,
                        rowColors[(uint)row].A);
                }
            });
        }

        return result;
    }

    private static NETColor[,] CreateImageDataPixels(int width, int height, Dictionary<uint, NETColor> rowColors)
    {
        var result = new NETColor[width, height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                result[x, y] = NETColor.FromArgb(rowColors[(uint)y].A, rowColors[(uint)y].R, rowColors[(uint)y].G, rowColors[(uint)y].B);
            }
        }

        return result;
    }

    /// <summary>
    /// Gets the <see cref="Rgba32"/> pixels from the given <paramref name="sixLaborsImage"/>.
    /// </summary>
    /// <param name="sixLaborsImage">The six labors image.</param>
    /// <returns>The two dimensional pixel colors of the image.</returns>
    private static Rgba32[,] GetSixLaborPixels(Image<Rgba32> sixLaborsImage)
    {
        var result = new Rgba32[sixLaborsImage.Width, sixLaborsImage.Height];

        for (var y = 0; y < sixLaborsImage.Height; y++)
        {
            var row = y;
            sixLaborsImage.ProcessPixelRows(accessor =>
            {
                var pixelRow = accessor.GetRowSpan(row);

                for (var x = 0; x < sixLaborsImage.Width; x++)
                {
                    result[x, row] = pixelRow[x];
                }
            });
        }

        return result;
    }
}

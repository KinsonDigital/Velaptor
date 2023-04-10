// <copyright file="ImageDataTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System;
using System.Collections.Generic;
using System.Drawing;
using FluentAssertions;
using Helpers;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="ImageData"/> struct.
/// </summary>
public class ImageDataTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WhenPixelParamIsNull_ProperlyCreatesDefaultPixelData()
    {
        // Act
        var imageData = new ImageData(null, 3, 2);

        // Assert
        Assert.Equal(2, imageData.Pixels.GetUpperBound(0));
        Assert.Equal(1, imageData.Pixels.GetUpperBound(1));

        var row0 = GetRow(imageData.Pixels, 0);

        Assert.All(row0, pixel =>
        {
            Assert.True(pixel == Color.White, $"Actual Pixel Color (Row 0): {pixel}");
        });

        Assert.All(row0, pixel =>
        {
            Assert.True(pixel == Color.White, $"Actual Pixel Color (Row 0): {pixel}");
        });
    }

    [Fact]
    public void Ctor_WhenWidthAndPixelDimensionDoesNotMatch_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
        {
            _ = new ImageData(new Color[1, 2], 11, 2);
        }, "The length of the 1st dimension of the 'pixels' parameter must match the 'width' parameter.");
    }

    [Fact]
    public void Ctor_WhenHeightAndPixelDimensionDoesNotMatch_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
        {
            _ = new ImageData(new Color[1, 2], 1, 22);
        }, "The length of the 1st dimension of the 'pixels' parameter must match the 'height' parameter.");
    }

    [Fact]
    public void Ctor_WhenInvoked_FlipStatesSetToCorrectValues()
    {
        // Arrange
        var sut = new ImageData(new Color[2, 2], 2, 2);

        // Act
        var actualHorizontalFlip = sut.IsFlippedHorizontally;
        var actualVerticalFlip = sut.IsFlippedVertically;

        // Assert
        actualHorizontalFlip.Should().BeFalse();
        actualVerticalFlip.Should().BeFalse();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Ctor_WithNullOrEmptyFilePathParam_SetsFilePathProp(string filePath)
    {
        // Arrange & Act
        var sut = new ImageData(null, 1, 1, filePath);

        // Assert
        sut.FilePath.Should().BeEmpty();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void DrawImage_WhenDrawingWithingBounds_DrawsWholeImageOntoOther()
    {
        // Arrange
        var targetImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 0, 255), 10, 10);
        var srcImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 128, 0), 6, 6);

        // Act
        var actual = targetImgData.DrawImage(srcImgData, new Point(2, 2));

        bool ClrMatches(Color clrA, Color clrB)
        {
            return clrA.A == clrB.A &&
                   clrA.R == clrB.R &&
                   clrA.G == clrB.G &&
                   clrA.B == clrB.B;
        }

        // First 2 rows
        var row0 = TestHelpers.GetRow(actual, 0);
        var row1 = TestHelpers.GetRow(actual, 1);

        // Middle rows
        var row2 = TestHelpers.GetRow(actual, 2, 2, actual.Width - (2 + 1));
        var row3 = TestHelpers.GetRow(actual, 3, 2, actual.Width - (2 + 1));
        var row4 = TestHelpers.GetRow(actual, 4, 2, actual.Width - (2 + 1));
        var row5 = TestHelpers.GetRow(actual, 5, 2, actual.Width - (2 + 1));
        var row6 = TestHelpers.GetRow(actual, 6, 2, actual.Width - (2 + 1));
        var row7 = TestHelpers.GetRow(actual, 7, 2, actual.Width - (2 + 1));

        // Last 2 rows
        var row8 = TestHelpers.GetRow(actual, 8);
        var row9 = TestHelpers.GetRow(actual, 9);

        // First 2 columns
        var col0 = TestHelpers.GetColumn(actual, 0);
        var col1 = TestHelpers.GetColumn(actual, 1);

        // Last 2 columns
        var col8 = TestHelpers.GetColumn(actual, 8);
        var col9 = TestHelpers.GetColumn(actual, 9);

        // Assert
        // First 2 rows
        Assert.All(row0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(row1, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(row2, clr => Assert.True(ClrMatches(clr, Color.Green)));

        // Middle rows
        Assert.All(row3, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row4, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row5, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row6, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row7, clr => Assert.True(ClrMatches(clr, Color.Green)));

        // Last 2 rows
        Assert.All(row8, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(row9, clr => Assert.True(ClrMatches(clr, Color.Blue)));

        // First 2 columns
        Assert.All(col0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(col1, clr => Assert.True(ClrMatches(clr, Color.Blue)));

        // Last 2 columns
        Assert.All(col8, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(col9, clr => Assert.True(ClrMatches(clr, Color.Blue)));
    }

    [Fact]
    public void FlipHorizontally_WhenInvoked_FlipsImageHorizontally()
    {
        // Arrange
        var sut = new ImageData(new Color[4, 4], 4, 4);

        // Set the first 2 columns to blue
        sut = TestHelpers.SetColumnColorTo(sut, 0, Color.Blue);
        sut = TestHelpers.SetColumnColorTo(sut, 1, Color.Blue);

        // Set the last 2 columns to yellow
        sut = TestHelpers.SetColumnColorTo(sut, 2, Color.Yellow);
        sut = TestHelpers.SetColumnColorTo(sut, 3, Color.Yellow);

        // Act
        sut.FlipHorizontally();
        var col0 = TestHelpers.GetColumn(sut, 0);
        var col1 = TestHelpers.GetColumn(sut, 1);
        var col2 = TestHelpers.GetColumn(sut, 2);
        var col3 = TestHelpers.GetColumn(sut, 3);

        // Assert
        col0.Should().AllSatisfy(clr => clr.Should().Be(Color.Yellow));
        col1.Should().AllSatisfy(clr => clr.Should().Be(Color.Yellow));
        col2.Should().AllSatisfy(clr => clr.Should().Be(Color.Blue));
        col3.Should().AllSatisfy(clr => clr.Should().Be(Color.Blue));
        sut.IsFlippedHorizontally.Should().BeTrue();
    }

    [Fact]
    public void FlipVertically_WhenInvoked_FlipsImageVertically()
    {
        // Arrange
        var sut = new ImageData(new Color[4, 4], 4, 4);

        // Set the first 2 rows to blue
        sut = TestHelpers.SetRowColorTo(sut, 0, Color.Blue);
        sut = TestHelpers.SetRowColorTo(sut, 1, Color.Blue);

        // Set the last 2 rows to yellow
        sut = TestHelpers.SetRowColorTo(sut, 2, Color.Yellow);
        sut = TestHelpers.SetRowColorTo(sut, 3, Color.Yellow);

        // Act
        sut.FlipVertically();
        var row0 = TestHelpers.GetRow(sut, 0);
        var row1 = TestHelpers.GetRow(sut, 1);
        var row2 = TestHelpers.GetRow(sut, 2);
        var row3 = TestHelpers.GetRow(sut, 3);

        // Assert
        row0.Should().AllSatisfy(clr => clr.Should().Be(Color.Yellow));
        row1.Should().AllSatisfy(clr => clr.Should().Be(Color.Yellow));
        row2.Should().AllSatisfy(clr => clr.Should().Be(Color.Blue));
        row3.Should().AllSatisfy(clr => clr.Should().Be(Color.Blue));
        sut.IsFlippedVertically.Should().BeTrue();
    }

    [Fact]
    public void DrawImage_WithWidthAndHeightLargerThanTarget_DrawsPartialSourceImageOntoTarget()
    {
        // Arrange
        var targetImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 0, 255), 6, 6);
        var srcImgData = TestHelpers.CreateImageData(Color.FromArgb(255, 0, 128, 0), 10, 10);

        // Act
        var actual = targetImgData.DrawImage(srcImgData, new Point(2, 2));

        bool ClrMatches(Color clrA, Color clrB)
        {
            return clrA.A == clrB.A &&
                   clrA.R == clrB.R &&
                   clrA.G == clrB.G &&
                   clrA.B == clrB.B;
        }

        // First top 2 blue rows
        var row0 = TestHelpers.GetRow(actual, 0);
        var row1 = TestHelpers.GetRow(actual, 1);

        // Green rows below top 2 blue rows
        var row2 = TestHelpers.GetRow(actual, 2, 2, 5);
        var row3 = TestHelpers.GetRow(actual, 3, 2, 5);
        var row4 = TestHelpers.GetRow(actual, 4, 2, 5);
        var row5 = TestHelpers.GetRow(actual, 5, 2, 5);

        // First 2 blue columns
        var col0 = TestHelpers.GetColumn(actual, 0);
        var col1 = TestHelpers.GetColumn(actual, 1);

        // Assert
        // First top 2 blue rows
        Assert.All(row0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(row1, clr => Assert.True(ClrMatches(clr, Color.Blue)));

        // Green rows below top 2 blue rows
        Assert.All(row2, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row3, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row4, clr => Assert.True(ClrMatches(clr, Color.Green)));
        Assert.All(row5, clr => Assert.True(ClrMatches(clr, Color.Green)));

        // First 2 blue columns columns
        Assert.All(col0, clr => Assert.True(ClrMatches(clr, Color.Blue)));
        Assert.All(col1, clr => Assert.True(ClrMatches(clr, Color.Blue)));
    }

    [Fact]
    public void Equals_WhenBothAreSameTypeAndPixelLengthsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenFilePathsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var sutA = new ImageData(new Color[2, 2], 2, 2, "itemA");
        var sutB = new ImageData(new Color[2, 2], 2, 2, "itemB");

        // Act
        var actual = sutA.Equals(sutB);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void Equals_WhenBothAreSameTypeAndIsEqual_ReturnsTrue()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2, "asdf");
        var imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2, "asdf");

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenBothAreSameTypeAndHasDifferentColorPixels_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        var imageDataB = TestHelpers.CreateImageData(Color.Blue,  2, 2);

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenParamIsObjectTypeAndPixelLengthsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        object imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 3, 3);

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenObjectIsNotCorrectType_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        var imageDataB = new object();

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void Equals_WhenParamIsObjectTypeAndIsEqual_ReturnsTrue()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        object imageDataB = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Equals_WhenParamIsObjectTypeAndHasDifferentColorPixels_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        object imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

        // Act
        var actual = imageDataA.Equals(imageDataB);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void EqualsOperator_WhenBothPixelLengthsAreNotEqual_ReturnsFalse()
    {
        // Arrange
        var imageDataA = new ImageData(new Color[2, 2], 2, 2);
        var imageDataB = new ImageData(new Color[3, 3], 3, 3);

        // Act
        var actual = imageDataA == imageDataB;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void EqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsFalse()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        var imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

        // Act
        var actual = imageDataA == imageDataB;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void NotEqualsOperator_WhenBothHaveDifferentColorPixels_ReturnsTrue()
    {
        // Arrange
        var imageDataA = TestHelpers.CreateImageData(Color.FromArgb(11, 22, 33, 44), 2, 2);
        var imageDataB = TestHelpers.CreateImageData(Color.Blue, 2, 2);

        // Act
        var actual = imageDataA != imageDataB;

        // Assert
        Assert.True(actual);
    }

    [Theory]
    [InlineData(0u, 0u, true)]
    [InlineData(1u, 0u, false)]
    [InlineData(0u, 1u, false)]
    public void IsEmpty_WhenEmpty_ReturnsCorrectResult(uint width, uint height, bool expected)
    {
        // Arrange
        var data = new ImageData(null, width, height);

        // Act
        var actual = data.IsEmpty();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(10, 20, null, "10 x 20")]
    [InlineData(10, 20, "", "10 x 20")]
    [InlineData(10, 20, "test-file", "10 x 20 | test-file")]
    public void ToString_WhenInvoked_ReturnsCorrectResult(uint width, uint height, string filePath, string expected)
    {
        // Arrange
        var sut = new ImageData(new Color[width, height], width, height, filePath);

        // Act
        var actual = sut.ToString();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion

    /// <summary>
    /// Gets the given <paramref name="row"/> of pixels from the 2D array of pixels.
    /// </summary>
    /// <param name="pixels">The pixel data.</param>
    /// <param name="row">The row number to retrieve.</param>
    /// <returns>The data to test.</returns>
    private static Color[] GetRow(Color[,] pixels, int row)
    {
        var result = new List<Color>();

        for (var x = 0; x < pixels.GetUpperBound(1); x++)
        {
            result.Add(pixels[x, row]);
        }

        return result.ToArray();
    }
}

// <copyright file="RectBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Tests the <see cref="RectBatchItem"/> struct.
/// </summary>
public class RectBatchItemTests
{
    /// <summary>
    /// Gets all of the test data related to testing the <see cref="IsEmptyData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyData()
    {
        yield return new object[]
        {
            // FULLY EMPTY
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            true, // Expected Result
        };
        yield return new object[]
        {
            new Vector2(10, 20), // Position - NON-EMPTY VALUE
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            10f, // Width - NON-EMPTY VALUE
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            10f, // Height - NON-EMPTY VALUE
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.FromArgb(10, 20, 30, 40), // Color - NON-EMPTY VALUE
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            true, // IsFilled - NON-EMPTY VALUE
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            10f, // BorderThickness - NON-EMPTY VALUE
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.Horizontal, // GradientType - NON-EMPTY VALUE
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.FromArgb(10, 20, 30, 40), // GradientStart - NON-EMPTY VALUE
            Color.Empty, // GradientStop
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.FromArgb(10, 20, 30, 40), // GradientStop - NON-EMPTY VALUE
            0, // Layer
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            10, // Layer - NON-EMPTY VALUE
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            10, // Layer
            false, // Expected Result
        };
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithDifferentCornerRadiusValues_SetsPropToCorrectResult()
    {
        // Arrange & Act
        var sut = new RectBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9f,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.None,
            Color.FromArgb(14, 15, 16, 17),
            Color.FromArgb(18, 19, 20, 21),
            22);

        // Assert
        sut.Position.Should().Be(new Vector2(1, 2));
        sut.Width.Should().Be(3);
        sut.Height.Should().Be(4);
        sut.Color.Should().Be(Color.FromArgb(5, 6, 7, 8));
        sut.IsFilled.Should().Be(true);
        sut.BorderThickness.Should().Be(9f);
        sut.CornerRadius.Should().Be(new CornerRadius(10, 11, 12, 13));
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.FromArgb(14, 15, 16, 17));
        sut.GradientStop.Should().Be(Color.FromArgb(18, 19, 20, 21));
        sut.Layer.Should().Be(22);
    }
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(IsEmptyData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        float width,
        float height,
        Color color,
        bool isFilled,
        float borderThickness,
        CornerRadius cornerRadius,
        ColorGradient gradientType,
        Color gradientStart,
        Color gradientStop,
        int layer,
        bool expected)
    {
        // Arrange
        var sut = new RectBatchItem(
            position,
            width,
            height,
            color,
            isFilled,
            borderThickness,
            cornerRadius,
            gradientType,
            gradientStart,
            gradientStop,
            layer);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        actual.Should().Be(expected, sut.ToString());
    }
    #endregion
}

// <copyright file="RectShapeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="RectShape"/> struct.
/// </summary>
public class RectShapeTests
{
    /// <summary>
    /// Provides test data for the <see cref="RectShape.IsEmpty"/> method unit test.
    /// </summary>
    /// <returns>The data to use during the test.</returns>
    public static IEnumerable<object[]> IsEmptyTestData()
    {
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            true, // EXPECTED
        };
        yield return new object[]
        {
            new Vector2(44, 44), // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            44f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            44f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.FromArgb(44, 44, 44, 44), // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            true, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(44f, 44f, 44f, 44f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.Horizontal, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.FromArgb(44, 44, 44, 44), // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.FromArgb(44, 44, 44, 44), // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Width
            1f, // Height
            Color.Empty, // Color
            false, // IsSolid
            1f, // Border Thickness
            new CornerRadius(0f, 0f, 0f, 0f), // Corner Radius
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            true, // EXPECTED
        };
    }

    #region Constructor Tests
    [Fact]
    [SuppressMessage(
        "StyleCop.CSharp.ReadabilityRules",
        "SA1129:Do not use default value type constructor",
        Justification = "Unit test requires use of constructor.")]
    public void Ctor_WhenInvoked_SetsDefaultValues()
    {
        // Arrange & Act
        var rect = new RectShape();

        // Assert
        Assert.Equal(Vector2.Zero, rect.Position);
        Assert.Equal(1f, rect.Width);
        Assert.Equal(1f, rect.Height);
        Assert.Equal(Color.White, rect.Color);
        Assert.True(rect.IsSolid);
        Assert.Equal(1f, rect.BorderThickness);
        Assert.Equal(new CornerRadius(1f, 1f, 1f, 1f), rect.CornerRadius);
        Assert.Equal(ColorGradient.None, rect.GradientType);
        Assert.Equal(Color.White, rect.GradientStart);
        Assert.Equal(Color.White, rect.GradientStop);
    }
    #endregion

    #region Prop Tests
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-10f, 1)]
    [InlineData(123, 123)]
    public void Width_WhenSettingValue_ReturnsCorrectResult(float value, float expected)
    {
        // Arrange
        var rect = default(RectShape);

        // Act
        rect.Width = value;
        var actual = rect.Width;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(-10f, 1)]
    [InlineData(123, 123)]
    public void Height_WhenSettingValue_ReturnsCorrectResult(float value, float expected)
    {
        // Arrange
        var rect = default(RectShape);

        // Act
        rect.Height = value;
        var actual = rect.Height;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HalfWidth_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Width = 100;

        // Act
        var actual = rect.HalfWidth;

        // Assert
        Assert.Equal(50f, actual);
    }

    [Fact]
    public void HalfHeight_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Height = 100;

        // Act
        var actual = rect.HalfHeight;

        // Assert
        Assert.Equal(50f, actual);
    }

    [Theory]
    [InlineData(75f, 100f, 10f, 10f)]
    [InlineData(100f, 75f, 20f, 20f)]
    [InlineData(100f, 75f, 90f, 75f)]
    [InlineData(100f, 100f, -30f, 1f)]
    public void BorderThickness_WhenSettingValue_ReturnsCorrectResult(
        float width,
        float height,
        float borderThickness,
        float expected)
    {
        // Arrange
        var rect = default(RectShape);
        rect.Width = width;
        rect.Height = height;

        // Act
        rect.BorderThickness = borderThickness;
        var actual = rect.BorderThickness;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CornerRadius_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);

        // Act
        var actual = rect.CornerRadius;

        // Assert
        AssertExtensions.EqualWithMessage(0f, actual.TopLeft, "The top left value is incorrect.");
        AssertExtensions.EqualWithMessage(0f, actual.BottomLeft, "The bottom left value is incorrect.");
        AssertExtensions.EqualWithMessage(0f, actual.BottomRight, "The bottom right value is incorrect.");
        AssertExtensions.EqualWithMessage(0f, actual.TopRight, "The top right value is incorrect.");
    }

    [Fact]
    public void CornerRadius_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);

        // Act
        rect.CornerRadius = new CornerRadius(11f, 22f, 33f, 44f);
        var actual = rect.CornerRadius;

        // Assert
        AssertExtensions.EqualWithMessage(11f, actual.TopLeft, "The top left value is incorrect.");
        AssertExtensions.EqualWithMessage(22f, actual.BottomLeft, "The bottom left value is incorrect.");
        AssertExtensions.EqualWithMessage(33f, actual.BottomRight, "The bottom right value is incorrect.");
        AssertExtensions.EqualWithMessage(44f, actual.TopRight, "The top right value is incorrect.");
    }

    [Fact]
    public void Top_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = new Vector2(100, 100);
        rect.Width = 100;
        rect.Height = 50;

        // Act
        rect.Top = 40f;
        var actual = rect.Top;

        // Assert
        AssertExtensions.EqualWithMessage(40f, actual, $"{nameof(RectShape.Top)} value incorrect.");
        AssertExtensions.EqualWithMessage(100, rect.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(65f, rect.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Right_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = new Vector2(200, 100);
        rect.Width = 100;
        rect.Height = 50;

        // Act
        rect.Right = 100f;
        var actual = rect.Right;

        // Assert
        AssertExtensions.EqualWithMessage(100f, actual, $"{nameof(RectShape.Right)} value incorrect.");
        AssertExtensions.EqualWithMessage(50, rect.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(100f, rect.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Bottom_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = new Vector2(100, 100);
        rect.Width = 100;
        rect.Height = 50;

        // Act
        rect.Bottom = 40f;
        var actual = rect.Bottom;

        // Assert
        AssertExtensions.EqualWithMessage(40f, actual, $"{nameof(RectShape.Bottom)} value incorrect.");
        AssertExtensions.EqualWithMessage(100, rect.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(15f, rect.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Left_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = new Vector2(200, 100);
        rect.Width = 100;
        rect.Height = 50;

        // Act
        rect.Left = 100f;
        var actual = rect.Left;

        // Assert
        AssertExtensions.EqualWithMessage(100f, actual, $"{nameof(RectShape.Left)} value incorrect.");
        AssertExtensions.EqualWithMessage(150, rect.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(100f, rect.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData(50, 60, true)] // Not touching any edges and inside
    [InlineData(40, 60, true)] // Touching left edge
    [InlineData(60, 60, true)] // Touching right edge
    [InlineData(50, 55, true)] // Touching top edge
    [InlineData(50, 75, true)] // Touching bottom edge
    [InlineData(10, 10, false)] // Vector outside of rectangle
    public void Contains_WhenInvoked_ReturnsCorrectResult(float x, float y, bool expected)
    {
        // Arrange
        var position = new Vector2(x, y);
        var sut = new RectShape
        {
            Position = new Vector2(50, 60),
            Width = 20,
            Height = 30,
        };

        // Act
        var actual = sut.Contains(position);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(IsEmptyTestData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        float width,
        float height,
        Color color,
        bool isSolid,
        float borderThickness,
        CornerRadius cornerRadius,
        ColorGradient gradientType,
        Color gradientStart,
        Color gradientStop,
        bool expected)
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = position;
        rect.Width = width;
        rect.Height = height;
        rect.Color = color;
        rect.IsSolid = isSolid;
        rect.BorderThickness = borderThickness;
        rect.CornerRadius = cornerRadius;
        rect.GradientType = gradientType;
        rect.GradientStart = gradientStart;
        rect.GradientStop = gradientStop;

        // Act
        var actual = rect.IsEmpty();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Empty_WhenInvoked_EmptiesStruct()
    {
        // Arrange
        var rect = default(RectShape);
        rect.Position = new Vector2(1, 2);
        rect.Width = 50f;
        rect.Height = 60f;
        rect.Color = Color.FromArgb(5, 6, 7, 8);
        rect.IsSolid = true;
        rect.BorderThickness = 9f;
        rect.CornerRadius = new CornerRadius(10, 11, 12, 13);
        rect.GradientType = ColorGradient.Horizontal;
        rect.GradientStart = Color.FromArgb(14, 15, 16, 17);
        rect.GradientStop = Color.FromArgb(18, 19, 20, 21);

        // Act
        rect.Empty();

        // Assert
        Assert.Equal(Vector2.Zero, rect.Position);
        Assert.Equal(1f, rect.Width);
        Assert.Equal(1f, rect.Height);
        Assert.Equal(Color.Empty, rect.Color);
        Assert.False(rect.IsSolid);
        Assert.Equal(1f, rect.BorderThickness);
        Assert.Equal(new CornerRadius(0f, 0f, 0f, 0f), rect.CornerRadius);
        Assert.Equal(ColorGradient.None, rect.GradientType);
        Assert.Equal(Color.Empty, rect.GradientStart);
        Assert.Equal(Color.Empty, rect.GradientStop);
    }
    #endregion
}

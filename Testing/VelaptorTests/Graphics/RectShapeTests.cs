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
        var sut = new RectShape();

        // Assert
        Assert.Equal(Vector2.Zero, sut.Position);
        Assert.Equal(1f, sut.Width);
        Assert.Equal(1f, sut.Height);
        Assert.Equal(Color.White, sut.Color);
        Assert.True(sut.IsSolid);
        Assert.Equal(1f, sut.BorderThickness);
        Assert.Equal(new CornerRadius(1f, 1f, 1f, 1f), sut.CornerRadius);
        Assert.Equal(ColorGradient.None, sut.GradientType);
        Assert.Equal(Color.White, sut.GradientStart);
        Assert.Equal(Color.White, sut.GradientStop);
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
        var sut = default(RectShape);

        // Act
        sut.Width = value;
        var actual = sut.Width;

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
        var sut = default(RectShape);

        // Act
        sut.Height = value;
        var actual = sut.Height;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void HalfWidth_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Width = 100;

        // Act
        var actual = sut.HalfWidth;

        // Assert
        Assert.Equal(50f, actual);
    }

    [Fact]
    public void HalfHeight_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Height = 100;

        // Act
        var actual = sut.HalfHeight;

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
        var sut = default(RectShape);
        sut.Width = width;
        sut.Height = height;

        // Act
        sut.BorderThickness = borderThickness;
        var actual = sut.BorderThickness;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void CornerRadius_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);

        // Act
        var actual = sut.CornerRadius;

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
        var sut = default(RectShape);

        // Act
        sut.CornerRadius = new CornerRadius(11f, 22f, 33f, 44f);
        var actual = sut.CornerRadius;

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
        var sut = default(RectShape);
        sut.Position = new Vector2(100, 100);
        sut.Width = 100;
        sut.Height = 50;

        // Act
        sut.Top = 40f;
        var actual = sut.Top;

        // Assert
        AssertExtensions.EqualWithMessage(40f, actual, $"{nameof(RectShape.Top)} value incorrect.");
        AssertExtensions.EqualWithMessage(100, sut.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(65f, sut.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Right_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Position = new Vector2(200, 100);
        sut.Width = 100;
        sut.Height = 50;

        // Act
        sut.Right = 100f;
        var actual = sut.Right;

        // Assert
        AssertExtensions.EqualWithMessage(100f, actual, $"{nameof(RectShape.Right)} value incorrect.");
        AssertExtensions.EqualWithMessage(50, sut.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(100f, sut.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Bottom_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Position = new Vector2(100, 100);
        sut.Width = 100;
        sut.Height = 50;

        // Act
        sut.Bottom = 40f;
        var actual = sut.Bottom;

        // Assert
        AssertExtensions.EqualWithMessage(40f, actual, $"{nameof(RectShape.Bottom)} value incorrect.");
        AssertExtensions.EqualWithMessage(100, sut.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(15f, sut.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
    }

    [Fact]
    public void Left_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Position = new Vector2(200, 100);
        sut.Width = 100;
        sut.Height = 50;

        // Act
        sut.Left = 100f;
        var actual = sut.Left;

        // Assert
        AssertExtensions.EqualWithMessage(100f, actual, $"{nameof(RectShape.Left)} value incorrect.");
        AssertExtensions.EqualWithMessage(150, sut.Position.X, $"{nameof(RectShape.Position.X)} value incorrect.");
        AssertExtensions.EqualWithMessage(100f, sut.Position.Y, $"{nameof(RectShape.Position.Y)} value incorrect.");
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
        var sut = default(RectShape);
        sut.Position = position;
        sut.Width = width;
        sut.Height = height;
        sut.Color = color;
        sut.IsSolid = isSolid;
        sut.BorderThickness = borderThickness;
        sut.CornerRadius = cornerRadius;
        sut.GradientType = gradientType;
        sut.GradientStart = gradientStart;
        sut.GradientStop = gradientStop;

        // Act
        var actual = sut.IsEmpty();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Empty_WhenInvoked_EmptiesStruct()
    {
        // Arrange
        var sut = default(RectShape);
        sut.Position = new Vector2(1, 2);
        sut.Width = 50f;
        sut.Height = 60f;
        sut.Color = Color.FromArgb(5, 6, 7, 8);
        sut.IsSolid = true;
        sut.BorderThickness = 9f;
        sut.CornerRadius = new CornerRadius(10, 11, 12, 13);
        sut.GradientType = ColorGradient.Horizontal;
        sut.GradientStart = Color.FromArgb(14, 15, 16, 17);
        sut.GradientStop = Color.FromArgb(18, 19, 20, 21);

        // Act
        sut.Empty();

        // Assert
        Assert.Equal(Vector2.Zero, sut.Position);
        Assert.Equal(1f, sut.Width);
        Assert.Equal(1f, sut.Height);
        Assert.Equal(Color.Empty, sut.Color);
        Assert.False(sut.IsSolid);
        Assert.Equal(1f, sut.BorderThickness);
        Assert.Equal(new CornerRadius(0f, 0f, 0f, 0f), sut.CornerRadius);
        Assert.Equal(ColorGradient.None, sut.GradientType);
        Assert.Equal(Color.Empty, sut.GradientStart);
        Assert.Equal(Color.Empty, sut.GradientStop);
    }
    #endregion
}

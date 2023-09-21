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
        sut.Position.Should().Be(Vector2.Zero);
        sut.Width.Should().Be(1f);
        sut.Height.Should().Be(1f);
        sut.Color.Should().Be(Color.White);
        sut.IsSolid.Should().BeTrue();
        sut.BorderThickness.Should().Be(1f);
        sut.CornerRadius.Should().Be(new CornerRadius(1f, 1f, 1f, 1f));
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.White);
        sut.GradientStop.Should().Be(Color.White);
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
        actual.Should().Be(expected);
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
        actual.Should().Be(expected);
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
        actual.Should().Be(50f);
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
        actual.Should().Be(50f);
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
        actual.Should().Be(expected);
    }

    [Fact]
    public void CornerRadius_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(RectShape);

        // Act
        var actual = sut.CornerRadius;

        // Assert
        actual.TopLeft.Should().Be(0f, "The top left value is incorrect.");
        actual.BottomLeft.Should().Be(0f, "The bottom left value is incorrect.");
        actual.BottomRight.Should().Be(0f, "The bottom right value is incorrect.");
        actual.TopRight.Should().Be(0f, "The top right value is incorrect.");
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
        actual.TopLeft.Should().Be(11f, "The top left value is incorrect.");
        actual.BottomLeft.Should().Be(22f, "The bottom left value is incorrect.");
        actual.BottomRight.Should().Be(33f, "The bottom right value is incorrect.");
        actual.TopRight.Should().Be(44f, "The top right value is incorrect.");
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
        actual.Should().Be(40f, $"{nameof(RectShape.Top)} value incorrect.");
        sut.Position.X.Should().Be(100, $"{nameof(RectShape.Position.X)} value incorrect.");
        sut.Position.Y.Should().Be(65f, $"{nameof(RectShape.Position.Y)} value incorrect.");
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
        actual.Should().Be(100f, $"{nameof(RectShape.Right)} value incorrect.");
        sut.Position.X.Should().Be(50, $"{nameof(RectShape.Position.X)} value incorrect.");
        sut.Position.Y.Should().Be(100f, $"{nameof(RectShape.Position.Y)} value incorrect.");
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
        actual.Should().Be(40f, $"{nameof(RectShape.Bottom)} value incorrect.");
        sut.Position.X.Should().Be(100, $"{nameof(RectShape.Position.X)} value incorrect.");
        sut.Position.Y.Should().Be(15f, $"{nameof(RectShape.Position.Y)} value incorrect.");
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
        actual.Should().Be(100f, $"{nameof(RectShape.Left)} value incorrect.");
        sut.Position.X.Should().Be(150, $"{nameof(RectShape.Position.X)} value incorrect.");
        sut.Position.Y.Should().Be(100f, $"{nameof(RectShape.Position.Y)} value incorrect.");
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
        actual.Should().Be(expected);
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
        sut.Position.Should().Be(Vector2.Zero);
        sut.Width.Should().Be(1f);
        sut.Height.Should().Be(1f);
        sut.Color.Should().Be(Color.Empty);
        sut.IsSolid.Should().BeFalse();
        sut.BorderThickness.Should().Be(1f);
        sut.CornerRadius.Should().Be(new CornerRadius(0f, 0f, 0f, 0f));
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.Empty);
        sut.GradientStop.Should().Be(Color.Empty);
    }
    #endregion
}

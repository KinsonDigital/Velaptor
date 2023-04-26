// <copyright file="CircleShapeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="CircleShape"/> struct.
/// </summary>
public class CircleShapeTests
{
    /// <summary>
    /// Provides test data for the <see cref="CircleShape.IsEmpty"/> method unit test.
    /// </summary>
    /// <returns>The data to use during the test.</returns>
    public static IEnumerable<object[]> IsEmptyTestData()
    {
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            true, // EXPECTED
        };
        yield return new object[]
        {
            new Vector2(44, 44), // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            44f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.FromArgb(44, 44, 44, 44), // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            true, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            44f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.Horizontal, // Gradient Type
            Color.Empty, // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.FromArgb(44, 44, 44, 44), // Gradient Start
            Color.Empty, // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
            ColorGradient.None, // Gradient Type
            Color.Empty, // Gradient Start
            Color.FromArgb(44, 44, 44, 44), // Gradient Stop
            false, // EXPECTED
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            1f, // Diameter
            Color.Empty, // Color
            false, // IsFilled
            1f, // Border Thickness
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
        var sut = new CircleShape();

        // Assert
        sut.IsFilled.Should().BeTrue();
        sut.Position.Should().Be(Vector2.Zero);
        sut.Diameter.Should().Be(1f);
        sut.Color.Should().Be(Color.White);
        sut.BorderThickness.Should().Be(1f);
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.White);
        sut.GradientStop.Should().Be(Color.White);
    }
    #endregion

    #region Prop Tests
    [Theory]
    [InlineData(0, 1, 0.5f)]
    [InlineData(-10f, 1, 0.5f)]
    [InlineData(123, 123, 61.5f)]
    public void Diameter_WhenSettingValue_ReturnsCorrectResult(float value, float expectedDiameter, float expectedRadius)
    {
        // Arrange
        var sut = default(CircleShape);
        sut.IsFilled = true;
        sut.BorderThickness = 2f;

        // Act
        sut.Diameter = value;
        var actualDiameter = sut.Diameter;
        var actualRadius = sut.Radius;

        // Assert
        actualDiameter.Should().Be(expectedDiameter);
        actualRadius.Should().Be(expectedRadius);
        sut.BorderThickness.Should().Be(2f);
    }

    [Theory]
    [InlineData(200f, 20f)]
    [InlineData(50f, 5f)]
    public void Diameter_WhenChangingDiameterWithEmptyCircle_ProportionallyUpdatesBorderThickness(
        float newDiameter,
        float expectedBorderThickness)
    {
        // Arrange
        var sut = default(CircleShape);
        sut.IsFilled = false;
        sut.Diameter = 100f;
        sut.BorderThickness = 10f;

        // Act
        sut.Diameter = newDiameter;

        // Assert
        sut.BorderThickness.Should().Be(expectedBorderThickness);
    }

    [Fact]
    public void Radius_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Radius = 50;

        // Act
        var actualRadius = sut.Radius;
        var actualDiameter = sut.Diameter;

        // Assert
        actualRadius.Should().Be(50f);
        actualDiameter.Should().Be(100f);
    }

    [Fact]
    public void BorderThickness_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);

        // Act
        sut.BorderThickness = 123f;
        var actual = sut.BorderThickness;

        // Assert
        actual.Should().Be(123f);
    }

    [Fact]
    public void Top_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Position = new Vector2(100, 100);
        sut.Diameter = 50;

        // Act
        sut.Top = 40f;
        var actual = sut.Top;

        // Assert
        actual.Should().Be(40f);
        sut.Position.X.Should().Be(100);
        sut.Position.Y.Should().Be(65f);
    }

    [Fact]
    public void Right_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Position = new Vector2(200, 100);
        sut.Diameter = 100;

        // Act
        sut.Right = 100f;
        var actual = sut.Right;

        // Assert
        actual.Should().Be(100f);
        sut.Position.X.Should().Be(50);
        sut.Position.Y.Should().Be(100f);
    }

    [Fact]
    public void Bottom_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Position = new Vector2(100, 100);
        sut.Diameter = 50;

        // Act
        sut.Bottom = 40f;
        var actual = sut.Bottom;

        // Assert
        actual.Should().Be(40f);
        sut.Position.X.Should().Be(100);
        sut.Position.Y.Should().Be(15f);
    }

    [Fact]
    public void Left_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Position = new Vector2(200, 100);
        sut.Diameter = 100;

        // Act
        sut.Left = 100f;
        var actual = sut.Left;

        // Assert
        actual.Should().Be(100f);
        sut.Position.X.Should().Be(150);
        sut.Position.Y.Should().Be(100f);
    }
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(IsEmptyTestData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        float diameter,
        Color color,
        bool isFilled,
        float borderThickness,
        ColorGradient gradientType,
        Color gradientStart,
        Color gradientStop,
        bool expected)
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Position = position;
        sut.Diameter = diameter;
        sut.Color = color;
        sut.IsFilled = isFilled;
        sut.BorderThickness = borderThickness;
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
        var sut = default(CircleShape);
        sut.Position = new Vector2(1, 2);
        sut.Diameter = 3f;
        sut.Color = Color.FromArgb(5, 6, 7, 8);
        sut.IsFilled = true;
        sut.BorderThickness = 9f;
        sut.GradientType = ColorGradient.Horizontal;
        sut.GradientStart = Color.FromArgb(14, 15, 16, 17);
        sut.GradientStop = Color.FromArgb(18, 19, 20, 21);

        // Act
        sut.Empty();

        // Assert
        sut.IsFilled.Should().BeFalse();
        sut.Position.Should().Be(Vector2.Zero);
        sut.Diameter.Should().Be(1f);
        sut.Color.Should().Be(Color.Empty);
        sut.BorderThickness.Should().Be(0f);
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.Empty);
        sut.GradientStop.Should().Be(Color.Empty);
    }
    #endregion
}

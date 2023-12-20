// <copyright file="CircleShapeTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

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
    public static TheoryData<Vector2, float, Color, bool, float, ColorGradient, Color, Color, bool> IsEmptyTestData() =>
        new ()
        {
            // Position, Diameter, Color, IsSolid, Thickness, Type, Start, Stop, EXPECTED
            { Vector2.Zero, 1f, Color.Empty, false, 1f, ColorGradient.None, Color.Empty, Color.Empty, true },
            { new Vector2(44, 44), 1f, Color.Empty, false, 1f, ColorGradient.None, Color.Empty, Color.Empty, false },
            { Vector2.Zero, 44f, Color.Empty, false, 1f, ColorGradient.None, Color.Empty, Color.Empty, false },
            { Vector2.Zero, 1f, Color.FromArgb(44, 44, 44, 44), false, 1f, ColorGradient.None, Color.Empty, Color.Empty, false },
            { Vector2.Zero, 1f, Color.Empty, true, 1f, ColorGradient.None, Color.Empty, Color.Empty, false },
            // The diameter in this test data must be greater than the border thickness multiplied by 2.
            // This is because we need to have the diameter large enough to not restrict the value of the border thickness below the value of 44.
            { Vector2.Zero, 100f, Color.Empty, false, 44f, ColorGradient.None, Color.Empty, Color.Empty, false },
            { Vector2.Zero, 1f, Color.Empty, false, 1f, ColorGradient.Horizontal, Color.Empty, Color.Empty, false },
            { Vector2.Zero, 1f, Color.Empty, false, 1f, ColorGradient.None, Color.FromArgb(44, 44, 44, 44), Color.Empty, false },
            { Vector2.Zero, 1f, Color.Empty, false, 1f, ColorGradient.None, Color.Empty, Color.FromArgb(44, 44, 44, 44), false },
        };

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
        sut.IsSolid.Should().BeTrue();
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
    [InlineData(100, 100f)]
    [InlineData(-10f, 1f)]
    public void Diameter_WhenSettingValue_ReturnsCorrectResult(float value, float expectedDiameter)
    {
        // Arrange
        var sut = default(CircleShape);

        // Act
        sut.Diameter = value;
        var actual = sut.Diameter;

        // Assert
        actual.Should().Be(expectedDiameter);
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

    [Theory]
    [InlineData(25, 25f)]
    [InlineData(50, 50f)]
    [InlineData(75, 50f)]
    [InlineData(-10, 1f)]
    public void BorderThickness_WhenSettingValue_ReturnsCorrectResult(float thickness, float expected)
    {
        // Arrange
        var sut = default(CircleShape);
        sut.Diameter = 100f;

        // Act
        sut.BorderThickness = thickness;
        var actual = sut.BorderThickness;

        // Assert
        actual.Should().Be(expected);
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
    [SuppressMessage("csharpsquid|Methods should not have too many parameters", "S107", Justification = "Intentional")]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        float diameter,
        Color color,
        bool isSolid,
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
        sut.IsSolid = isSolid;
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
        sut.Diameter = 50f;
        sut.Color = Color.FromArgb(5, 6, 7, 8);
        sut.IsSolid = true;
        sut.BorderThickness = 10f;
        sut.GradientType = ColorGradient.Horizontal;
        sut.GradientStart = Color.FromArgb(14, 15, 16, 17);
        sut.GradientStop = Color.FromArgb(18, 19, 20, 21);

        // Act
        sut.Empty();

        // Assert
        sut.IsSolid.Should().BeFalse();
        sut.Position.Should().Be(Vector2.Zero);
        sut.Diameter.Should().Be(1f);
        sut.Color.Should().Be(Color.Empty);
        sut.BorderThickness.Should().Be(1f);
        sut.GradientType.Should().Be(ColorGradient.None);
        sut.GradientStart.Should().Be(Color.Empty);
        sut.GradientStop.Should().Be(Color.Empty);
    }
    #endregion
}

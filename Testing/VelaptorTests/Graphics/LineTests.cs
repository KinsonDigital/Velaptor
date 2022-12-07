// <copyright file="LineTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="Line"/> struct.
/// </summary>
public class LineTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithP1AndP2Params_SetsPropsToCorrectValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);

        // Act
        var sut = new Line(expectedP1, expectedP2);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
    }

    [Fact]
    public void Ctor_WithP1AndP2AndColor_SetsPropsToCorrectValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);
        var expectedColor = Color.FromArgb(5, 6, 7, 8);

        // Act
        var sut = new Line(expectedP1, expectedP2, expectedColor);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
        sut.Color.Should().BeEquivalentTo(expectedColor);
    }

    [Fact]
    public void Ctor_WithP1AndP2AndThickness_SetsPropsToCorrectValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);
        var expectedThickness = 4;

        // Act
        var sut = new Line(expectedP1, expectedP2, expectedThickness);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
        sut.Thickness.Should().Be(4);
    }

    [Fact]
    public void Ctor_WithP1AndP2AndColorAndThickness_SetsPropsToCorrectValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);
        var expectedColor = Color.FromArgb(5, 6, 7, 8);
        var expectedThickness = 9;

        // Act
        var sut = new Line(expectedP1, expectedP2, expectedColor, expectedThickness);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
        sut.Color.Should().BeEquivalentTo(expectedColor);
        sut.Thickness.Should().Be(9);
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void P1_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Vector2(10, 20);
        var sut = default(Line);

        // Act
        sut.P1 = new Vector2(10, 20);
        var actual = sut.P1;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void P2_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Vector2(30, 40);
        var sut = default(Line);

        // Act
        sut.P2 = new Vector2(30, 40);
        var actual = sut.P2;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Color_WhileGettingDefaultValueWhenUsingDefaultCtor_ReturnsWhite()
    {
        // Arrange
        var sut = default(Line);

        // Act
        var actual = sut.Color;

        // Assert
        actual.Should().BeEquivalentTo(Color.White);
    }

    [Fact]
    public void Color_WhileGettingDefaultValueWhenNotUsingDefaultCtor_ReturnsWhite()
    {
        // Arrange
        var sut = new Line(Vector2.Zero, Vector2.Zero);

        // Act
        var actual = sut.Color;

        // Assert
        actual.Should().BeEquivalentTo(Color.White);
    }

    [Fact]
    public void Color_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = Color.FromArgb(50, 60, 70, 80);
        var sut = default(Line);

        // Act
        sut.Color = Color.FromArgb(50, 60, 70, 80);
        var actual = sut.Color;

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void Thickness_WhileGettingDefaultValueWhenUsingDefaultCtor_ReturnsCorrectResult()
    {
        // Arrange
        var sut = default(Line);

        // Act
        var actual = sut.Thickness;

        // Assert
        actual.Should().Be(1f);
    }

    [Fact]
    public void Thickness_WhileGettingDefaultValueWhenNotUsingDefaultCtor_ReturnsCorrectResult()
    {
        // Arrange
        var sut = new Line(Vector2.Zero, Vector2.Zero);

        // Act
        var actual = sut.Thickness;

        // Assert
        actual.Should().Be(1f);
    }

    [Fact]
    public void Thickness_WhenAssigningNegativeValue_MaintainsMinimumValue()
    {
        // Arrange
        var sut = new Line(Vector2.Zero, Vector2.Zero);
        sut.Thickness = -10;

        // Act
        var actual = sut.Thickness;

        // Assert
        actual.Should().Be(1f);
    }
    #endregion
}

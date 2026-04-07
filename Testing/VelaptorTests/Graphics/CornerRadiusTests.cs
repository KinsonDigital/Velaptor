// <copyright file="CornerRadiusTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using Shouldly;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="CornerRadius"/> struct.
/// </summary>
public class CornerRadiusTests
{
    #region Constructor Tests
    [Fact]
    public void Ctor_WithEachCornerValue_ProperlySetsProps()
    {
        // Arrange & Act
        var corners = new CornerRadius(11, 22, 33, 44);

        // Assert
        corners.TopLeft.ShouldBe(11u);
        corners.TopRight.ShouldBe(22u);
        corners.BottomRight.ShouldBe(33u);
        corners.BottomLeft.ShouldBe(44u);
    }

    [Fact]
    public void Ctor_WithSingleValue_ProperlySetsProps()
    {
        // Arrange & Act
        var corners = new CornerRadius(123f);

        // Assert
        corners.TopLeft.ShouldBe(123);
        corners.BottomLeft.ShouldBe(123);
        corners.BottomRight.ShouldBe(123);
        corners.TopRight.ShouldBe(123);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void SetTopLeft_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(55, 22, 33, 44);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetTopLeft(sut, 55);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SetBottomLeft_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(11, 22, 33, 55);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetBottomLeft(sut, 55);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SetBottomRight_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(11, 22, 55, 44);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetBottomRight(sut, 55);

        // Assert
        actual.ShouldBe(expected);
    }

    [Fact]
    public void SetTopRight_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(11, 55, 33, 44);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetTopRight(sut, 55);

        // Assert
        actual.ShouldBe(expected);
    }

    [Theory]
    [InlineData(0f, 0f, 0f, 0f, true)]
    [InlineData(2f, 0f, 0f, 0f, false)]
    [InlineData(0f, 2f, 0f, 0f, false)]
    [InlineData(0f, 0f, 2f, 0f, false)]
    [InlineData(0f, 0f, 0f, 2f, false)]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        float topLeft,
        float bottomLeft,
        float bottomRight,
        float topRight,
        bool expected)
    {
        // Arrange
        var radius = new CornerRadius(topLeft, bottomLeft, bottomRight, topRight);

        // Act
        var actual = radius.IsEmpty();

        // Assert
        actual.ShouldBe(expected);
    }
    #endregion
}

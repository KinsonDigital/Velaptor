// <copyright file="CornerRadiusTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Graphics;

using FluentAssertions;
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
        Assert.Equal(11u, corners.TopLeft);
        Assert.Equal(22u, corners.BottomLeft);
        Assert.Equal(33u, corners.BottomRight);
        Assert.Equal(44u, corners.TopRight);
    }

    [Fact]
    public void Ctor_WithSingleValue_ProperlySetsProps()
    {
        // Arrange & Act
        var corners = new CornerRadius(123f);

        // Assert
        Assert.Equal(123, corners.TopLeft);
        Assert.Equal(123, corners.BottomLeft);
        Assert.Equal(123, corners.BottomRight);
        Assert.Equal(123, corners.TopRight);
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
        actual.Should().Be(expected);
    }

    [Fact]
    public void SetBottomLeft_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(11, 55, 33, 44);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetBottomLeft(sut, 55);

        // Assert
        actual.Should().Be(expected);
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
        actual.Should().Be(expected);
    }

    [Fact]
    public void SetTopRight_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(11, 22, 33, 55);
        var sut = new CornerRadius(11, 22, 33, 44);

        // Act
        var actual = CornerRadius.SetTopRight(sut, 55);

        // Assert
        actual.Should().Be(expected);
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
        Assert.Equal(expected, actual);
    }
    #endregion
}

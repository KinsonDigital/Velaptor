// <copyright file="LineBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.OpenGL;
using Xunit;

/// <summary>
/// Tests the <see cref="LineBatchItem"/> struct.
/// </summary>
public class LineBatchItemTests
{
#pragma warning disable SA1514
    #region TestData
    /// <summary>
    /// Provides test data for the <see cref="Ctor_WhenInvoked_CorrectlySetsPropertyValues"/> test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyTestData()
    {
        yield return new object[]
        {
            Vector2.Zero, // P1
            Vector2.Zero, // P2
            Color.Empty, // Color
            0f, // Thickness
            0, // Layer
            true, // Expected
        };
        yield return new object[]
        {
            new Vector2(1, 2),
            Vector2.Zero,
            Color.Empty,
            0f,
            0,
            false,
        };
        yield return new object[]
        {
            Vector2.Zero,
            new Vector2(1, 2),
            Color.Empty,
            0f,
            0,
            false,
        };
        yield return new object[]
        {
            Vector2.Zero,
            Vector2.Zero,
            Color.FromArgb(1, 2, 3, 4),
            0f,
            0,
            false,
        };
        yield return new object[]
        {
            Vector2.Zero,
            Vector2.Zero,
            Color.Empty,
            1f,
            0,
            false,
        };
        yield return new object[]
        {
            Vector2.Zero,
            Vector2.Zero,
            Color.Empty,
            0f,
            1,
            false,
        };
    }
    #endregion
#pragma warning restore SA1514

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsPropertyValues()
    {
        // Arrange
        var expectedP1 = new Vector2(1, 2);
        var expectedP2 = new Vector2(3, 4);
        var expectedClr = Color.FromArgb(5, 6, 7, 8);
        const int expectedThickness = 9;
        const int expectedLayer = 10;

        // Act
        var sut = new LineBatchItem(expectedP1, expectedP2, expectedClr, expectedThickness, expectedLayer);

        // Assert
        sut.P1.Should().BeEquivalentTo(expectedP1);
        sut.P2.Should().BeEquivalentTo(expectedP2);
        sut.Color.Should().BeEquivalentTo(expectedClr);
        sut.Thickness.Should().Be(expectedThickness);
        sut.Layer.Should().Be(expectedLayer);
    }

    [Theory]
    [MemberData(nameof(IsEmptyTestData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 p1,
        Vector2 p2,
        Color color,
        float thickness,
        int layer,
        bool expected)
    {
        // Arrange
        var sut = new LineBatchItem(p1, p2, color, thickness, layer);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}

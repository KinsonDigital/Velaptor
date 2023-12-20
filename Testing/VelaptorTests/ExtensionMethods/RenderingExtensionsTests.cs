// <copyright file="RenderingExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1514

namespace VelaptorTests.ExtensionMethods;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.ExtensionMethods;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;

/// <summary>
/// Tests extension methods related to types used in the rendering process.
/// </summary>
public class RenderingExtensionsTests
{
    #region Test Data
    /// <summary>
    /// Gets the rectangle vertice data for the <see cref="CreateRectFromLine_WhenInvoked_ReturnsCorrectResult"/> unit test.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<Vector2, Vector2, Color, float, Vector2, Vector2, Vector2, Vector2> ExpectedRectPointData =>
        new ()
        {
            // X and Y axis aligned rectangle
            {
                new Vector2(50f, 100f), new Vector2(200f, 100f), Color.White, 20, new Vector2(50f, 90f),
                new Vector2(200f, 90f), new Vector2(200f, 110f), new Vector2(50f, 110f)
            },

            // X and Y axis aligned rectangle rotated 45 degrees clockwise
            {
                new Vector2(100f, 100f), new Vector2(200f, 200f), Color.White, 100f, new Vector2(135.35535f, 64.64465f),
                new Vector2(235.35535f, 164.64467f), new Vector2(164.64465f, 235.35533f), new Vector2(64.64465f, 135.35535f)
            },
        };
    #endregion

    [Fact]
    public void Scale_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(150, 150);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, Color.White, 0f);

        // Act
        var actual = sut.Scale(0.5f);

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void FlipEnd_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expectedP1 = new Vector2(100, 100);
        var expectedP2 = new Vector2(0, 0);

        var p1 = new Vector2(100, 100);
        var p2 = new Vector2(200, 200);

        var sut = new LineBatchItem(p1, p2, Color.White, 0f);

        // Act
        var actual = sut.FlipEnd();

        // Assert
        actual.P1.Should().Be(expectedP1);
        actual.P2.Should().Be(expectedP2);
    }

    [Fact]
    public void Clamp_WhenInvoked_ClampsRadiusValues()
    {
        // Arrange
        var sut = new CornerRadius(200f, 200, -200f, -200f);

        // Act
        sut = sut.Clamp(0f, 100f);

        // Assert
        sut.TopLeft.Should().Be(100f);
        sut.BottomLeft.Should().Be(100f);
        sut.BottomRight.Should().Be(0f);
        sut.TopRight.Should().Be(0f);
    }

    [Fact]
    public void Length_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        const float startX = 124.6f;
        const float startY = 187.5f;
        const float stopX = 257.3f;
        const float stopY = 302.4f;

        var line = new LineBatchItem(
            new Vector2(startX, startY),
            new Vector2(stopX, stopY),
            Color.White,
            0f);

        // Act
        var actual = line.Length();

        // Assert
        actual.Should().Be(175.53146f);
    }

    [Fact]
    public void SetP1_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(10, 20),
            new Vector2(2, 3),
            Color.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            Color.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP1(new Vector2(10, 20));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SetP2_WhenInvokedForLineBatchItem_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(20, 30),
            Color.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            Color.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SetP2(new Vector2(20, 30));

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void SwapEnds_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new LineBatchItem(
            new Vector2(2, 3),
            new Vector2(1, 2),
            Color.FromArgb(4, 5, 6, 7),
            8);

        var item = new LineBatchItem(
            new Vector2(1, 2),
            new Vector2(2, 3),
            Color.FromArgb(4, 5, 6, 7),
            8);

        // Act
        var actual = item.SwapEnds();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Theory]
    [MemberData(nameof(ExpectedRectPointData))]
    [SuppressMessage("csharpsquid|Methods should not have too many parameters", "S107", Justification = "Intentional")]
    internal void CreateRectFromLine_WhenInvoked_ReturnsCorrectResult(
        Vector2 p1,
        Vector2 p2,
        Color clr,
        float thickness,
        Vector2 topLeftCorner,
        Vector2 topRightCorner,
        Vector2 bottomRightCorner,
        Vector2 bottomLeftCorner)
    {
        // Arrange
        var expected = new[]
        {
            topLeftCorner,
            topRightCorner,
            bottomRightCorner,
            bottomLeftCorner,
        };

        var line = new LineBatchItem(p1, p2, clr, thickness);

        // Act
        var actual = line.CreateRectFromLine();

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}

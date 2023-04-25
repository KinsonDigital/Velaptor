// <copyright file="ShapeBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Tests the <see cref="ShapeBatchItem"/> struct.
/// </summary>
public class ShapeBatchItemTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeBatchItemTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">Provides test output.</param>
    public ShapeBatchItemTests(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

    /// <summary>
    /// Gets all of the test data related to testing the <see cref="IsEmptyData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyData()
    {
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "Fully Empty", // TEST NAME
            true, // Expected Result
        };
        yield return new object[]
        {
            new Vector2(10, 20), // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "position", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            10f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "width", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            10f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "height", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.FromArgb(10, 20, 30, 40), // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "color", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            true, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "isFilled", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            10f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "borderThickness", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            new CornerRadius(1, 2, 3, 4), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "cornerRadius", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.Horizontal, // GradientType
            Color.Empty, // GradientStart
            Color.Empty, // GradientStop
            "gradientType", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.FromArgb(10, 20, 30, 40), // GradientStart
            Color.Empty, // GradientStop
            "gradientStart", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            Vector2.Zero, // Position
            0f, // Width
            0f, // Height
            Color.Empty, // Color
            false, // IsFilled
            0f, // BorderThickness
            CornerRadius.Empty(), // CornerRadius
            ColorGradient.None, // GradientType
            Color.Empty, // GradientStart
            Color.FromArgb(10, 20, 30, 40), // GradientStop
            "gradientStop", // TEST NAME
            false, // Expected Result
        };
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsProperties()
    {
        // Arrange & Act
        var sut = new ShapeBatchItem(
            new Vector2(1, 2),
            3,
            4,
            Color.FromArgb(5, 6, 7, 8),
            true,
            9,
            new CornerRadius(10, 11, 12, 13),
            ColorGradient.Horizontal,
            Color.FromArgb(14, 15, 16, 17),
            Color.FromArgb(18, 19, 20, 21));

        // Assert
        sut.Position.Should().Be(new Vector2(1, 2));
        sut.Width.Should().Be(3);
        sut.Height.Should().Be(4);
        sut.Color.Should().Be(Color.FromArgb(5, 6, 7, 8));
        sut.IsFilled.Should().Be(true);
        sut.BorderThickness.Should().Be(9);
        sut.CornerRadius.Should().Be(new CornerRadius(10, 11, 12, 13));
        sut.GradientType.Should().Be(ColorGradient.Horizontal);
        sut.GradientStart.Should().Be(Color.FromArgb(14, 15, 16, 17));
        sut.GradientStop.Should().Be(Color.FromArgb(18, 19, 20, 21));
    }
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(IsEmptyData))]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        float width,
        float height,
        Color color,
        bool isFilled,
        float borderThickness,
        CornerRadius cornerRadius,
        ColorGradient gradientType,
        Color gradientStart,
        Color gradientStop,
        string testName, // Only used for test output
        bool expected)
    {
        // Arrange
        var sut = new ShapeBatchItem(
            position,
            width,
            height,
            color,
            isFilled,
            borderThickness,
            cornerRadius,
            gradientType,
            gradientStart,
            gradientStop);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        this.testOutputHelper.WriteLine($"Test Param: {testName}");
        actual.Should().Be(expected, sut.ToString());
    }
    #endregion
}

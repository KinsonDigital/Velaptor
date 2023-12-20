// <copyright file="ShapeBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
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
    public static TheoryData<Vector2, (float, float), Color, bool, float, CornerRadius, ColorGradient, (Color, Color), string, bool> IsEmptyData =>
        new ()
    {
         // Position, Size, Color, IsSolid, BorderThickness, CornerRadius, GradientType, Gradients, TEST NAME, Expected Result
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "Fully Empty", true },
         { new Vector2(10, 20), (0f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "position", false },
         { Vector2.Zero, (10f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "width", false },
         { Vector2.Zero, (0f, 10f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "height", false },
         { Vector2.Zero, (0f, 0f), Color.FromArgb(10, 20, 30, 40), false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "color", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, true, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "isSolid", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 10f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.Empty), "borderThickness", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 0f, new CornerRadius(1, 2, 3, 4), ColorGradient.None, (Color.Empty, Color.Empty), "cornerRadius", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.Horizontal, (Color.Empty, Color.Empty), "gradientType", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.FromArgb(10, 20, 30, 40), Color.Empty), "gradientStart", false },
         { Vector2.Zero, (0f, 0f), Color.Empty, false, 0f, CornerRadius.Empty(), ColorGradient.None, (Color.Empty, Color.FromArgb(10, 20, 30, 40)), "gradientStop", false },
    };

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
        sut.IsSolid.Should().Be(true);
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
    [SuppressMessage("csharpsquid|Methods should not have too many parameters", "S107", Justification = "Intentional")]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(
        Vector2 position,
        (float, float) size,
        Color color,
        bool isSolid,
        float borderThickness,
        CornerRadius cornerRadius,
        ColorGradient gradientType,
        (Color, Color) gradientClrs,
        string testName, // Only used for test output
        bool expected)
    {
        var (width, height) = size;
        (Color gradientStart, Color gradientStop) = gradientClrs;

        // Arrange
        var sut = new ShapeBatchItem(
            position,
            width,
            height,
            color,
            isSolid,
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

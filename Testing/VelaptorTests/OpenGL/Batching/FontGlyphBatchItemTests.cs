// <copyright file="FontGlyphBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using FluentAssertions;
using Velaptor.Graphics;
using Velaptor.OpenGL.Batching;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Tests the <see cref="FontGlyphBatchItem"/> struct.
/// </summary>
public class FontGlyphBatchItemTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGlyphBatchItemTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">Provides test output.</param>
    public FontGlyphBatchItemTests(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

    /// <summary>
    /// Gets all the test data related to testing the <see cref="IsEmptyData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<RectangleF, RectangleF, char, float, float, Color, RenderEffects, uint, string, bool> IsEmptyData =>
        new ()
        {
            // Dest Rect, Src Rect, Glyph, Size, Angle, Tint Color, ender Effects, TextureId, TEST NAME, Expected Result
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 0, Color.Empty, RenderEffects.None, 0, "Fully Empty", true },
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 0, Color.Empty, RenderEffects.None, 0, "Fully Empty", true },
            { new RectangleF(10, 20, 30, 40), RectangleF.Empty, '\0', 0, 0, Color.Empty, RenderEffects.None, 0, "srcRect", false },
            { RectangleF.Empty, new RectangleF(10, 20, 30, 40), 'V', 0, 0, Color.Empty, RenderEffects.None, 0, "destRect", false },
            { RectangleF.Empty, RectangleF.Empty, 'V', 0, 0, Color.Empty, RenderEffects.None, 0, "glyph", false },
            { RectangleF.Empty, RectangleF.Empty, '\0', 10, 0, Color.Empty, RenderEffects.None, 0, "size", false },
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 10, Color.Empty, RenderEffects.None, 0, "angle", false },
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 0, Color.FromArgb(10, 20, 30, 40), RenderEffects.None, 0, "tintColor", false },
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 0, Color.Empty, RenderEffects.FlipHorizontally, 0, "effects", false },
            { RectangleF.Empty, RectangleF.Empty, '\0', 0, 0, Color.Empty, RenderEffects.None, 10, "textureId", false },
        };

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsProperties()
    {
        // Arrange & Act
        var sut = new FontGlyphBatchItem(
            new RectangleF(1, 2, 3, 4),
            new RectangleF(5, 6, 7, 8),
            'V',
            9,
            10,
            Color.FromArgb(11, 12, 13, 14),
            RenderEffects.FlipHorizontally,
            15);

        // Assert
        sut.SrcRect.Should().Be(new RectangleF(1, 2, 3, 4));
        sut.DestRect.Should().Be(new RectangleF(5, 6, 7, 8));
        sut.Glyph.Should().Be('V');
        sut.Size.Should().Be(9);
        sut.Angle.Should().Be(10);
        sut.TintColor.Should().Be(Color.FromArgb(11, 12, 13, 14));
        sut.Effects.Should().Be(RenderEffects.FlipHorizontally);
        sut.TextureId.Should().Be(15);
    }
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(IsEmptyData))]
    [SuppressMessage("csharpsquid|Methods should not have too many parameters", "S107", Justification = "Intentional")]
    public void IsEmpty_WhenBatchItemIsEmpty_ReturnsTrue(
        RectangleF srcRect,
        RectangleF destRect,
        char glyph,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        uint textureId,
        string testName, // Only used for test output
        bool expected)
    {
        // Arrange
        var sut = new FontGlyphBatchItem(
            srcRect,
            destRect,
            glyph,
            size,
            angle,
            tintColor,
            effects,
            textureId);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        this.testOutputHelper.WriteLine($"Test Param: {testName}");
        actual.Should().Be(expected);
    }
    #endregion
}

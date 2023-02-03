// <copyright file="FontGlyphBatchItemTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Batching;

using System.Collections.Generic;
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
    /// Gets all of the test data related to testing the <see cref="IsEmptyData"/> method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyData()
    {
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "Fully Empty", // TEST NAME
            true, // Expected Result
        };
        yield return new object[]
        {
            new RectangleF(10, 20, 30, 40), // Src Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "srcRect", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            new RectangleF(10, 20, 30, 40), // Dest Rect
            'V', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "destRect", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            'V', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "glyph", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            10, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "size", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            10, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "angle", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.FromArgb(10, 20, 30, 40), // Tint Color
            RenderEffects.None, //Render Effects
            0, // TextureId
            "tintColor", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.FlipHorizontally, //Render Effects
            0, // TextureId
            "effects", // TEST NAME
            false, // Expected Result
        };
        yield return new object[]
        {
            RectangleF.Empty, // Dest Rect
            RectangleF.Empty, // Src Rect
            '\0', // Glyph
            0, // Size
            0, // Angle
            Color.Empty, // Tint Color
            RenderEffects.None, //Render Effects
            10, // TextureId
            "textureId", // TEST NAME
            false, // Expected Result
        };
    }

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

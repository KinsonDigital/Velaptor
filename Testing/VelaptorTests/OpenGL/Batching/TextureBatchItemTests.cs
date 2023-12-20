// <copyright file="TextureBatchItemTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="TextureBatchItem"/> struct.
/// </summary>
public class TextureBatchItemTests
{
    private readonly ITestOutputHelper testOutputHelper;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchItemTests"/> class.
    /// </summary>
    /// <param name="testOutputHelper">Provides test output.</param>
    public TextureBatchItemTests(ITestOutputHelper testOutputHelper) => this.testOutputHelper = testOutputHelper;

    /// <summary>
    /// Gets test data for the <see cref="IsEmptyOriginal_WhenBatchItemIsEmpty_ReturnsTrue"/> test method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static TheoryData<RectangleF, RectangleF, float, float, Color, RenderEffects, uint, string, bool> IsEmptyData =>
        new ()
        {
            // DestRect, Size, Angle, TintColor, Effects, TextureId, TEST NAME, Expected result
            { new RectangleF(4, 5, 6, 7), RectangleF.Empty, 0f, 0f, Color.Empty, RenderEffects.None, 0, "srcRect", false },
            { RectangleF.Empty, new RectangleF(8, 9, 10, 11), 0f, 0f, Color.Empty, RenderEffects.None, 0, "destRect", false },
            { RectangleF.Empty, RectangleF.Empty, 2f, 0f, Color.Empty, RenderEffects.None, 0, "size", false },
            { RectangleF.Empty, RectangleF.Empty, 0f, 3f, Color.Empty, RenderEffects.None, 0, "angle", false },
            { RectangleF.Empty, RectangleF.Empty, 0f, 0f, Color.FromArgb(12, 13, 14, 15), RenderEffects.None, 0, "tintColor", false },
            { RectangleF.Empty, RectangleF.Empty, 0f, 0f, Color.Empty, RenderEffects.None, 0, "Fully Empty", true },
            { RectangleF.Empty, RectangleF.Empty, 0f, 0f, Color.Empty, RenderEffects.None, 1, "textureId", false },
        };

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_CorrectlySetsProperties()
    {
        // Arrange & Act
        var sut = new TextureBatchItem(
            new RectangleF(1, 2, 3, 4),
            new RectangleF(5, 6, 7, 8),
            9,
            10,
            Color.FromArgb(11, 12, 13, 14),
            RenderEffects.FlipHorizontally,
            15);

        // Assert
        sut.SrcRect.Should().Be(new RectangleF(1, 2, 3, 4));
        sut.DestRect.Should().Be(new RectangleF(5, 6, 7, 8));
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
    public void IsEmptyOriginal_WhenBatchItemIsEmpty_ReturnsTrue(
        RectangleF srcRect,
        RectangleF destRect,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        uint textureId,
        string testName, // Only used for test output
        bool expected)
    {
        // Arrange
        var sut = new TextureBatchItem(
            srcRect,
            destRect,
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

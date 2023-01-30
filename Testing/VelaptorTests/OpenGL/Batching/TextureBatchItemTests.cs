// <copyright file="TextureBatchItemTests.cs" company="KinsonDigital">
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
/// Test the <see cref="TextureBatchItem"/> struct.
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
    /// Provides test data for the <see cref="IsEmptyOriginal_WhenBatchItemIsEmpty_ReturnsTrue"/> test method.
    /// </summary>
    /// <returns>The test data.</returns>
    public static IEnumerable<object[]> IsEmptyData()
    {
        yield return new object[]
        {
            new RectangleF(4, 5, 6, 7), // SrcRect
            RectangleF.Empty, // DestRect
            0f, // Size
            0f, // Angle
            Color.Empty, // TintColor
            0, // Effects
            0, // TextureId
            "srcRect", // TEST NAME
            false, // Expected result
        };
        yield return new object[]
        {
            RectangleF.Empty, // SrcRect
            new RectangleF(8, 9, 10, 11), // DestRect
            0f, // Size
            0f, // Angle
            Color.Empty, // TintColor
            0, // Effects
            0, // TextureId
            "destRect", // TEST NAME
            false, // Expected result
        };
        yield return new object[]
        {
            RectangleF.Empty, // SrcRect
            RectangleF.Empty, // DestRect
            2f, // Size
            0f, // Angle
            Color.Empty, // TintColor
            0, // Effects
            0, // TextureId
            "size", // TEST NAME
            false, // Expected result
        };
        yield return new object[]
        {
            RectangleF.Empty, // SrcRect
            RectangleF.Empty, // DestRect
            0f, // Size
            3f, // Angle
            Color.Empty, // TintColor
            0, // Effects
            0, // TextureId
            "angle", // TEST NAME
            false, // Expected result
        };
        yield return new object[]
        {
            RectangleF.Empty, // SrcRect
            RectangleF.Empty, // DestRect
            0f, // Size
            0f, // Angle
            Color.FromArgb(12, 13, 14, 15), // TintColor
            0, // Effects
            0, // TextureId
            "tintColor", // TEST NAME
            false, // Expected result
        };
        yield return new object[] // FAIL
        {
            RectangleF.Empty, // SrcRect
            RectangleF.Empty, // DestRect
            0f, // Size
            0f, // Angle
            Color.Empty, // TintColor
            RenderEffects.None, // Effects
            0, // TextureId
            "Fully Empty", // TEST NAME
            true, // Expected result
        };
        yield return new object[]
        {
            RectangleF.Empty, // SrcRect
            RectangleF.Empty, // DestRect
            0f, // Size
            0f, // Angle
            Color.Empty, // TintColor
            0, // Effects
            1, // TextureId
            "textureId", // TEST NAME
            false, // Expected result
        };
    }

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

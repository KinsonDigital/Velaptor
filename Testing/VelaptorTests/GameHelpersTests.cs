// <copyright file="GameHelpersTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Velaptor;
using Velaptor.Graphics;
using Xunit;

/// <summary>
/// Tests the <see cref="GameHelpers"/> class.
/// </summary>
[SuppressMessage("StyleCop.CSharp.LayoutRules", "SA1514:Element documentation header should be preceded by blank line", Justification = "Allowed for unit test classes.")]
public class GameHelpersTests
{
    private readonly char[] letters;
    private readonly char[] nonLetters;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameHelpersTests"/> class.
    /// </summary>
    public GameHelpersTests()
    {
        var result = new List<char>();

        // Capital letters A-Z
        for (var i = 'A'; i <= 'Z'; i++)
        {
            result.Add(i);
        }

        // Lowercase letters a-z
        for (var i = 'a'; i <= 'z'; i++)
        {
            result.Add(i);
        }

        this.letters = result.ToArray();

        result.Clear();

        for (var i = 0; i < 127; i++)
        {
            if (this.letters.Contains((char)i))
            {
                continue;
            }

            result.Add((char)i);
        }

        this.nonLetters = result.ToArray();
    }

    #region Method Tests
    [Fact]
    public void ForcePositive_WhenUsingNegativeValue_ReturnsPositiveResult()
    {
        // Act & Assert
        (-123f).ForcePositive().Should().Be(123f);
    }

    [Fact]
    public void ForcePositive_WhenUsingPositiveValue_ReturnsPositiveResult()
    {
        // Act & Assert
        123f.ForcePositive().Should().Be(123f);
    }

    [Fact]
    public void ForceNegative_WhenUsingPositiveValue_ReturnsNegativeResult()
    {
        // Act & Assert
        123f.ForceNegative().Should().Be(-123f);
    }

    [Fact]
    public void ForceNegative_WhenUsingNegativeValue_ReturnsNegativeResult()
    {
        // Act & Assert
        (-123f).ForceNegative().Should().Be(-123f);
    }

    [Fact]
    public void ToRadians_WhenInvoking_ReturnsCorrectResult()
    {
        // Act & Assert
        1234.1234f.ToDegrees().Should().Be(70710.06f);
    }

    [Fact]
    public void RotateAround_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var vectorToRotate = new Vector2(0, 0);
        var origin = new Vector2(5, 5);
        const float angle = 13f;
        var expected = new Vector2(1.25290489f, -0.996605873f);

        // Act
        var actual = vectorToRotate.RotateAround(origin, angle);

        // Assert
        actual.X.Should().Be(expected.X);
        actual.Y.Should().Be(expected.Y);
    }

    [Fact]
    public void RotateAround_WhenInvokedWithClockwiseFalse_ReturnsCorrectResult()
    {
        // Arrange
        var vectorToRotate = new Vector2(0, 0);
        var origin = new Vector2(5, 5);
        var angle = 45f;
        var expected = new Vector2(-2.07106781f, 5f);

        // Act
        var actual = vectorToRotate.RotateAround(origin, angle, false);

        // Assert
        actual.X.Should().Be(expected.X);
        actual.Y.Should().Be(expected.Y);
    }

    [Fact]
    public void ToVector4_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var color = Color.FromArgb(11, 22, 33, 44);
        var expected = new Vector4(22, 33, 44, 11);

        // Act
        var actual = color.ToVector4();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(20, 30, 40, 0.2f, 100, 24, 36, 48)] // Brightness within range
    [InlineData(20, 30, 40, -0.2f, 100, 20, 30, 40)] // Brightness below min
    [InlineData(20, 30, 40, 200f, 100, 40, 60, 80)] // Brightness above max
    [InlineData(255, 255, 255, 0.2f, 100, 255, 255, 255)] // Color components <= 255
    public void IncreaseBrightness_WhenInvoked_CorrectlySetsColor(
        byte red,
        byte green,
        byte blue,
        float brightness,
        byte expectedAlpha,
        byte expectedRed,
        byte expectedGreen,
        byte expectedBlue)
    {
        // Arrange
        var color = Color.FromArgb(100, red, green, blue);

        // Act
        var actual = color.IncreaseBrightness(brightness);

        // Assert
        actual.A.Should().Be(expectedAlpha);
        actual.R.Should().Be(expectedRed);
        actual.G.Should().Be(expectedGreen);
        actual.B.Should().Be(expectedBlue);
    }

    [Theory]
    [InlineData(20, 30, 40, 0.2f, 100, 16, 24, 32)] // Brightness within range
    [InlineData(20, 30, 40, -0.2f, 100, 20, 30, 40)] // Brightness below min
    [InlineData(20, 30, 40, 200f, 100, 0, 0, 0)] // Brightness above max
    [InlineData(0, 0, 0, 0.2f, 100, 0, 0, 0)] // Color components >= 0
    public void DecreaseBrightness_WhenInvoked_CorrectlySetsColor(
        byte red,
        byte green,
        byte blue,
        float brightness,
        byte expectedAlpha,
        byte expectedRed,
        byte expectedGreen,
        byte expectedBlue)
    {
        // Arrange
        var color = Color.FromArgb(100, red, green, blue);

        // Act
        var actual = color.DecreaseBrightness(brightness);

        // Assert
        actual.A.Should().Be(expectedAlpha);
        actual.R.Should().Be(expectedRed);
        actual.G.Should().Be(expectedGreen);
        actual.B.Should().Be(expectedBlue);
    }

    [Fact]
    public void GetPosition_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var rect = new RectangleF(1, 2, 3, 4);

        // Act
        var actual = rect.GetPosition();

        // Assert
        actual.X.Should().Be(1);
        actual.Y.Should().Be(2);
    }

    [Theory]
    [InlineData(126.5f, 0f, 255f, 0f, 1f, 0.49607843f)]
    [InlineData(500f, 0f, 1000f, 200f, 400f, 300f)]
    public void MapValue_WhenUsingFloatType_ReturnsCorrectResult(
        float testValue,
        float fromStart,
        float fromStop,
        float toStart,
        float toStop,
        float expected)
    {
        // Act
        var actual = testValue.MapValue(fromStart, fromStop, toStart, toStop);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 0, 10, 0, 100, 50)]
    [InlineData(126, 0, 255, 0, 1, 0)]
    public void MapValue_WhenUsingByteValues_ReturnsCorrectByteResult(
        byte testValue,
        byte fromStart,
        byte fromStop,
        byte toStart,
        byte toStop,
        byte expected)
    {
        // Act
        var actual = testValue.MapValue(fromStart, fromStop, toStart, toStop);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(5, 0f, 10f, 0f, 100f, 50f)]
    [InlineData(126f, 0f, 255f, 0f, 1f, 0.49411765f)]
    public void MapValue_WhenUsingByteValues_ReturnsCorrectFloatResult(
        byte testValue,
        float fromStart,
        float fromStop,
        float toStart,
        float toStop,
        float expected)
    {
        // Act
        var actual = testValue.MapValue(fromStart, fromStop, toStart, toStop);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void MapValue_WhenInvokedWithIntegerType_ReturnsCorrectResult()
    {
        // Arrange
        const int testValue = 500;

        // Act
        var actual = testValue.MapValue(0, 1_000, 0, 100_000);

        // Assert
        actual.Should().Be(50_000);
    }

    [Theory]
    [InlineData(10u, 30f)]
    [InlineData(0u, 0f)]
    public void ApplySize_WithUnsignedInt_ReturnsCorrectResult(uint value, float expected)
    {
        // Act
        var actual = value.ApplySize(2f);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(10f, 30f)]
    [InlineData(0f, 0f)]
    public void ApplySize_WithFloat_ReturnsCorrectResult(float value, float expected)
    {
        // Act
        var actual = value.ApplySize(2f);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void ApplySize_WithSizeF_ReturnsCorrectResult()
    {
        // Arrange
        var size = new SizeF(10, 20);

        // Act
        var actual = size.ApplySize(2f);

        // Assert
        actual.Width.Should().Be(30f);
        actual.Height.Should().Be(60f);
    }

    [Fact]
    public void ApplySize_WithRectangleF_ReturnsCorrectResult()
    {
        // Arrange
        var rect = new RectangleF(1, 2, 10, 20);

        // Act
        var actual = rect.ApplySize(2f);

        // Assert
        actual.X.Should().Be(3f);
        actual.Y.Should().Be(6f);
        actual.Width.Should().Be(30f);
        actual.Height.Should().Be(60f);
    }

    [Fact]
    public void ApplySize_WithGlyphMetrics_ReturnsCorrectResult()
    {
        // Arrange
        var metrics = default(GlyphMetrics);
        metrics.Glyph = 'V';
        metrics.GlyphBounds = new RectangleF(2f, 4f, 6f, 8f);
        metrics.Ascender = 2f;
        metrics.Descender = 2f;
        metrics.HorizontalAdvance = 2f;
        metrics.HoriBearingX = 2f;
        metrics.HoriBearingY = 2f;
        metrics.GlyphWidth = 2f;
        metrics.GlyphHeight = 2f;
        metrics.XMin = 2f;
        metrics.XMax = 2f;
        metrics.YMin = 2f;
        metrics.YMax = 2f;
        metrics.CharIndex = 123u;

        // Act
        var actual = metrics.ApplySize(2f);

        // Assert
        actual.GlyphBounds.Should().Be(new RectangleF(6f, 12f, 18f, 24f));
        actual.Glyph.Should().Be('V');
        actual.Ascender.Should().Be(6f);
        actual.Descender.Should().Be(6f);
        actual.HorizontalAdvance.Should().Be(6f);
        actual.HoriBearingX.Should().Be(6f);
        actual.HoriBearingY.Should().Be(6f);
        actual.GlyphWidth.Should().Be(6f);
        actual.GlyphHeight.Should().Be(6f);
        actual.XMin.Should().Be(6f);
        actual.XMax.Should().Be(6f);
        actual.YMax.Should().Be(6f);
        actual.YMin.Should().Be(6f);
        actual.CharIndex.Should().Be(123u);
    }

    [Fact]
    public void IsLetter_WithLetters_ReturnsTrue()
    {
        // Act & Assert
        this.letters.Should().AllSatisfy(l => l.IsLetter().Should().BeTrue());
    }

    [Fact]
    public void IsLetter_WithNonLetters_ReturnsFalse()
    {
        // Act & Assert
        this.nonLetters.Should().AllSatisfy(l => l.IsLetter().Should().BeFalse());
    }

    [Fact]
    public void IsNotLetter_WithNonLetters_ReturnsTrue()
    {
        // Act & Assert
        this.nonLetters.Should().AllSatisfy(l => l.IsNotLetter().Should().BeTrue());
    }

    [Theory]
    [InlineData("hello world", "world", false)]
    [InlineData("hello", "world", true)]
    [InlineData("hello", "", true)]
    [InlineData("", "world", true)]
    [InlineData("", "", false)]
    public void DoesNotContain_WhenUsingAStringParam_ReturnsCorrectResult(
        string stringToSearchIn,
        string valueToSearchFor,
        bool expected)
    {
        // Act
        var actual = stringToSearchIn.DoesNotContain(valueToSearchFor);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("hello world", 'w', false)]
    [InlineData("hello", 'w', true)]
    [InlineData("", 'w', true)]
    public void DoesNotContain_WhenUsingACharParam_ReturnsCorrectResult(
        string stringToSearchIn,
        char valueToSearchFor,
        bool expected)
    {
        // Act
        var actual = stringToSearchIn.DoesNotContain(valueToSearchFor);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", true)]
    [InlineData("Hello World", false)]
    [InlineData("Hello-World", false)]
    public void OnlyContainsLetters_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
    {
        // Act
        var actual = value.OnlyContainsLetters();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("HelloWorld", false)]
    [InlineData("Hello World", true)]
    [InlineData("Hello-World", true)]
    public void DoesNotOnlyContainsLetters_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
    {
        // Act
        var actual = value.DoesNotOnlyContainsLetters();

        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}

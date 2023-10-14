// <copyright file="TextBoxExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Velaptor.ExtensionMethods;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests all of the extension methods related to the <see cref="TextBox"/> control.
/// </summary>
public class TextBoxExtensionsTests
{
    #region Test Data

    /// <summary>
    /// Gets the data for testing the <see cref="TextBoxExtensions"/> methods.
    /// </summary>
    /// <returns>Data for tests.</returns>
    public static IEnumerable<object[]> BumpAll_TestData() =>
        new List<object[]>
        {
            // CharBounds, Expected_amount
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(0, 0, 2, 2))),
                },
                6,
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                },
                3,
            },
        };
    #endregion

    [Theory]
    [MemberData(nameof(BumpAll_TestData))]
    public void BumpAllToLeft_WhenInvoked_BumpsAmountCharsToLeft(List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        // Arrange
        var original = charBounds.ToList();
        // Act
        charBounds.BumpAllToLeft(amount);
        // Assert
        for (var i = 0; i < charBounds.Count; i++)
        {
            charBounds[i].bounds.X.Should().Be(original[i].bounds.X - amount);
        }
    }

    // MODO 2 - Implicit check, without external test data
    [Fact]
    public void BumpAllToLeft_WhenInvoked_BumpsAmountCharsToLeft2()
    {
        var charBounds = new List<(char character, RectangleF bounds)>(new[]
        {
            ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
            ('b', new RectangleF(new Vector4(4, 0, 2, 2))),
            ('c', new RectangleF(new Vector4(8, 0, 2, 2))),
        });
        var amount = 5;
        // Arrange
        var original = charBounds.ToList();
        // Act
        charBounds.BumpAllToLeft(amount);
        // Assert
        for (var i = 0; i < charBounds.Count; i++)
        {
            charBounds[i].bounds.X.Should().Be(original[i].bounds.X - amount);
        }
    }

    [Theory]
    [MemberData(nameof(BumpAll_TestData))]
    public void BumpAllToRight_WhenInvoked_BumpsAmountCharsToRight(List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        // Arrange
        var original = charBounds.ToList();
        // Act
        charBounds.BumpAllToRight(amount);
        // Assert
        for (var i = 0; i < charBounds.Count; i++)
        {
            charBounds[i].bounds.X.Should().Be(original[i].bounds.X + amount);
        }
    }

    [Theory]
    [MemberData(nameof(BumpAll_TestData))]
    public void BumpAllDown_WhenInvoked_BumpsAmountCharsToDown(List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        // Arrange
        var original = charBounds.ToList();
        // Act
        charBounds.BumpAllDown(amount);
        // Assert
        for (var i = 0; i < charBounds.Count; i++)
        {
            charBounds[i].bounds.Y.Should().Be(original[i].bounds.Y + amount);
        }
    }

    [Theory]
    [MemberData(nameof(BumpAll_TestData))]
    public void BumpAllUp_WhenInvoked_BumpsAmountCharsToUp(List<(char character, RectangleF bounds)> charBounds, float amount)
    {
        // Arrange
        var original = charBounds.ToList();
        // Act
        charBounds.BumpAllUp(amount);
        // Assert
        for (var i = 0; i < charBounds.Count; i++)
        {
            charBounds[i].bounds.Y.Should().Be(original[i].bounds.Y - amount);
        }
    }

    #region Test Data

    /// <summary>
    /// Gets the data for testing the <see cref="TextBoxExtensions"/> methods.
    /// </summary>
    /// <returns>Data for tests.</returns>
    public static IEnumerable<object[]> TextMethods_TestData() =>
        new List<object[]>
        {
            // CharBounds, Expected_amount
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(0, 0, 2, 2))),
                },
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                },
            },
        };
    #endregion

    #region Method Tests
    [Theory]
    [MemberData(nameof(TextMethods_TestData))]
    // *** InlineData is restricted, you cannot new objects there ***
    //[InlineData(new ReadOnlyCollection<(char, RectangleF)>( new[] { ('a', new RectangleF(1, 2, 3, 4)) }), 5)]
    //[InlineData("", 7)]
    //[InlineData("", 188)]
    public void TextLeft_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds)
    {
        // Arrange
        var expected = charBounds[0].bounds.Left;
        // Act
        var actual = charBounds.TextLeft();
        // Assert
        actual.Should().Be((int)expected);
    }

    [Theory]
    [MemberData(nameof(TextMethods_TestData))]
    public void TextRight_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds)
    {
        // Arrange
        var expected = charBounds[^1].bounds.Right;
        // Act
        var actual = charBounds.TextRight();
        // Assert
        actual.Should().Be((int)expected);
    }

    [Theory]
    [MemberData(nameof(TextMethods_TestData))]
    public void TextWidth_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds)
    {
        // Arrange
        var expected = charBounds.TextRight() - charBounds.TextLeft();
        // Act
        var actual = charBounds.TextWidth();
        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}

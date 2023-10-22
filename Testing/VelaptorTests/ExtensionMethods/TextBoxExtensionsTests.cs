// <copyright file="TextBoxExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Velaptor;
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
            // CharBounds, amount
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(10, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(20, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(30, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(40, 0, 2, 2))),
                },
                6,
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(100, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(200, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(300, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(400, 0, 2, 2))),
                },
                3,
            },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="TextBoxExtensions"/> methods.
    /// </summary>
    /// <returns>Data for tests.</returns>
    public static IEnumerable<object[]> TextMethods_TestData() =>
        new List<object[]>
        {
            // CharBounds
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(10, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(20, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(30, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(40, 0, 2, 2))),
                },
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(100, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(200, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(300, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(400, 0, 2, 2))),
                },
            },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="TextBoxExtensions"/> methods.
    /// </summary>
    /// <returns>Data for tests.</returns>
    public static IEnumerable<object[]> CharMethods_TestData() =>
        new List<object[]>
        {
            // CharBounds, Char index
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(10, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(20, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(30, 0, 2, 2))),
                },
                2,
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(100, 0, 20, 20))),
                    ('a', new RectangleF(new Vector4(200, 0, 20, 20))),
                    ('a', new RectangleF(new Vector4(300, 0, 20, 20))),
                    ('a', new RectangleF(new Vector4(400, 0, 20, 20))),
                },
                3,
            },
        };

    /// <summary>
    /// Gets the data for testing the <see cref="TextBoxExtensions"/> methods.
    /// </summary>
    /// <returns>Data for tests.</returns>
    public static IEnumerable<object[]> GapAtRightEnd_TestData() =>
        new List<object[]>
        {
            // CharBounds, rightEndLimitX
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('b', new RectangleF(new Vector4(2, 0, 2, 2))),
                    ('c', new RectangleF(new Vector4(4, 0, 2, 2))),
                    ('d', new RectangleF(new Vector4(6, 0, 2, 2))),
                },
                10f,
            },
            new object[]
            {
                new List<(char, RectangleF)>
                {
                    ('a', new RectangleF(new Vector4(0, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(10, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(20, 0, 2, 2))),
                    ('a', new RectangleF(new Vector4(30, 0, 2, 2))),
                },
                5f,
            },
        };
    #endregion

    #region Method Tests
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

    [Theory]
    [MemberData(nameof(TextMethods_TestData))]
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

    [Theory]
    [MemberData(nameof(CharMethods_TestData))]
    public void CharLeft_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds, int index)
    {
        // Arrange
        var expected = (index < 0 || index >= charBounds.Count) ? 0f : charBounds[index].bounds.Left;
        // Act
        var actual = charBounds.CharLeft(index);
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(CharMethods_TestData))]
    public void CharRight_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds, int index)
    {
        // Arrange
        var expected = (index < 0 || index >= charBounds.Count) ? 0f : charBounds[index].bounds.Right;
        // Act
        var actual = charBounds.CharRight(index);
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(TextMethods_TestData))]
    public void CenterPositionX_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds)
    {
        // Arrange
        var left = charBounds.Min(cb => cb.bounds.Left);
        var right = charBounds.Max(cb => cb.bounds.Right);
        var width = Math.Abs(left - right);
        var expected = (charBounds is null || charBounds.Count <= 0) ? 0f : left + width.Half();
        // Act
        var actual = charBounds.CenterPositionX();
        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(GapAtRightEnd_TestData))]
    public void GapAtRightEnd_WhenInvoked_ReturnsCorrectResult(List<(char character, RectangleF bounds)> charBounds, float rightEndLimitX)
    {
        // Arrange
        var expected = charBounds is not null && charBounds.Count > 0 && charBounds.TextRight() < rightEndLimitX;
        // Act
        var actual = charBounds.GapAtRightEnd(rightEndLimitX);
        // Assert
        actual.Should().Be(expected);
    }
    #endregion
}

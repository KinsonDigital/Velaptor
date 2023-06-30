// <copyright file="StringBuilderExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

using System.Text;
using FluentAssertions;
using Velaptor.ExtensionMethods;
using Xunit;

/// <summary>
/// Tests the extension methods <see cref="StringBuilder"/> type.
/// </summary>
public class StringBuilderExtensionsTests
{
    [Theory]
    [InlineData("test-value", "test-valu")]
    [InlineData("", "")]
    public void RemoveLastChar_WhenInvoked_ReturnsLastCharacter(string? testValue, string expected)
    {
        // Arrange
        var sut = new StringBuilder(testValue);

        // Act
        sut.RemoveLastChar();

        // ReSharper disable StringLiteralTypo
        // Assert
        sut.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("start-middle-end", 6, 6, "middle")]
    [InlineData("test-value", 60, 6, "")]
    [InlineData("start-middle-end", 6, 70, "")]
    public void Substring_WhenInvoked_ReturnsCorrectResult(string value, uint start, uint length, string expected)
    {
        // Arrange
        var sut = new StringBuilder(value);

        // Act
        var actual = sut.Substring(start, length);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("test-value-1", 4, "testvalue-1")]
    [InlineData("test-value-2", 40, "test-value-2")]
    public void RemoveChar_WhenInvoked_ReturnsCorrectResult(string value, int index, string expected)
    {
        // Arrange
        var sut = new StringBuilder(value);

        // Act
        sut.RemoveChar((uint)index);

        // Assert
        sut.ToString().Should().Be(expected);
    }

    [Theory]
    [InlineData("test-value", 9)]
    [InlineData("", -1)]
    public void LastCharIndex_WhenInvoked_ReturnsCorrectResult(string value, int expected)
    {
        // Arrange
        var sut = new StringBuilder(value);

        // Act
        var actual = sut.LastCharIndex();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData("", true)]
    [InlineData("test-value", false)]
    public void IsEmpty_WhenInvoked_ReturnsCorrectResult(string value, bool expected)
    {
        // Arrange
        var sut = new StringBuilder(value);

        // Act
        var actual = sut.IsEmpty();

        // Assert
        actual.Should().Be(expected);
    }
}

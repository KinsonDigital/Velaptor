// <copyright file="KeyCodeExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1514

namespace VelaptorTests.ExtensionMethods;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Velaptor.ExtensionMethods;
using Velaptor.Input;
using Xunit;

/// <summary>
/// Tests the extension methods for the <see cref="KeyCode"/> enum.
/// </summary>
public class KeyCodeExtensionsTests
{
    #region Test Data
    /// <summary>
    /// Gets the data for testing the <see cref="KeyCodeExtensions.ToChar(KeyCode, bool)"/> method.
    /// </summary>
    public static TheoryData<KeyCode, bool, char> ToCharTestData =>
        new ()
        {
            { KeyCode.A, true, 'A' },
            { KeyCode.A, false, 'a' },
            { KeyCode.B, true, 'B' },
            { KeyCode.B, false, 'b' },
            { KeyCode.C, true, 'C' },
            { KeyCode.C, false, 'c' },
            { KeyCode.D, true, 'D' },
            { KeyCode.D, false, 'd' },
            { KeyCode.E, true, 'E' },
            { KeyCode.E, false, 'e' },
            { KeyCode.F, true, 'F' },
            { KeyCode.F, false, 'f' },
            { KeyCode.G, true, 'G' },
            { KeyCode.G, false, 'g' },
            { KeyCode.H, true, 'H' },
            { KeyCode.H, false, 'h' },
            { KeyCode.I, true, 'I' },
            { KeyCode.I, false, 'i' },
            { KeyCode.J, true, 'J' },
            { KeyCode.J, false, 'j' },
            { KeyCode.K, true, 'K' },
            { KeyCode.K, false, 'k' },
            { KeyCode.L, true, 'L' },
            { KeyCode.L, false, 'l' },
            { KeyCode.M, true, 'M' },
            { KeyCode.M, false, 'm' },
            { KeyCode.N, true, 'N' },
            { KeyCode.N, false, 'n' },
            { KeyCode.O, true, 'O' },
            { KeyCode.O, false, 'o' },
            { KeyCode.P, true, 'P' },
            { KeyCode.P, false, 'p' },
            { KeyCode.Q, true, 'Q' },
            { KeyCode.Q, false, 'q' },
            { KeyCode.R, true, 'R' },
            { KeyCode.R, false, 'r' },
            { KeyCode.S, true, 'S' },
            { KeyCode.S, false, 's' },
            { KeyCode.T, true, 'T' },
            { KeyCode.T, false, 't' },
            { KeyCode.U, true, 'U' },
            { KeyCode.U, false, 'u' },
            { KeyCode.V, true, 'V' },
            { KeyCode.V, false, 'v' },
            { KeyCode.W, true, 'W' },
            { KeyCode.W, false, 'w' },
            { KeyCode.X, true, 'X' },
            { KeyCode.X, false, 'x' },
            { KeyCode.Y, true, 'Y' },
            { KeyCode.Y, false, 'y' },
            { KeyCode.Z, true, 'Z' },
            { KeyCode.Z, false, 'z' },
            { KeyCode.Apostrophe, true, '\"' },
            { KeyCode.Apostrophe, false, '\'' },
            { KeyCode.Semicolon, true, ':' },
            { KeyCode.Semicolon, false, ';' },
            { KeyCode.Comma, true, '<' },
            { KeyCode.Comma, false, ',' },
            { KeyCode.Period, true, '>' },
            { KeyCode.Period, false, '.' },
            { KeyCode.Slash, true, '?' },
            { KeyCode.Slash, false, '/' },
            { KeyCode.Minus, true, '_' },
            { KeyCode.Minus, false, '-' },
            { KeyCode.Equal, true, '+' },
            { KeyCode.Equal, false, '=' },
            { KeyCode.Backslash, true, '|' },
            { KeyCode.Backslash, false, '\\' },
            { KeyCode.LeftBracket, true, '{' },
            { KeyCode.LeftBracket, false, '[' },
            { KeyCode.RightBracket, true, '}' },
            { KeyCode.RightBracket, false, ']' },
            { KeyCode.D0, true, ')' },
            { KeyCode.D0, false, '0' },
            { KeyCode.D1, true, '!' },
            { KeyCode.D1, false, '1' },
            { KeyCode.D2, true, '@' },
            { KeyCode.D2, false, '2' },
            { KeyCode.D3, true, '#' },
            { KeyCode.D3, false, '3' },
            { KeyCode.D4, true, '$' },
            { KeyCode.D4, false, '4' },
            { KeyCode.D5, true, '%' },
            { KeyCode.D5, false, '5' },
            { KeyCode.D6, true, '^' },
            { KeyCode.D6, false, '6' },
            { KeyCode.D7, true, '&' },
            { KeyCode.D7, false, '7' },
            { KeyCode.D8, true, '*' },
            { KeyCode.D8, false, '8' },
            { KeyCode.D9, true, '(' },
            { KeyCode.D9, false, '9' },
            { KeyCode.GraveAccent, true, '~' },
            { KeyCode.GraveAccent, false, '`' },
            { KeyCode.KeyPad0, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad0, false, '0' },
            { KeyCode.KeyPad1, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad1, false, '1' },
            { KeyCode.KeyPad2, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad2, false, '2' },
            { KeyCode.KeyPad3, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad3, false, '3' },
            { KeyCode.KeyPad4, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad4, false, '4' },
            { KeyCode.KeyPad5, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad5, false, '5' },
            { KeyCode.KeyPad6, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad6, false, '6' },
            { KeyCode.KeyPad7, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad7, false, '7' },
            { KeyCode.KeyPad8, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad8, false, '8' },
            { KeyCode.KeyPad9, true, KeyboardKeyGroups.InvalidCharacter },
            { KeyCode.KeyPad9, false, '9' },
            { KeyCode.KeyPadDecimal, true, '.' },
            { KeyCode.KeyPadDecimal, false, '.' },
            { KeyCode.KeyPadDivide, true, '/' },
            { KeyCode.KeyPadDivide, false, '/' },
            { KeyCode.KeyPadMultiply, true, '*' },
            { KeyCode.KeyPadMultiply, false, '*' },
            { KeyCode.KeyPadSubtract, true, '-' },
            { KeyCode.KeyPadSubtract, false, '-' },
            { KeyCode.KeyPadAdd, true, '+' },
            { KeyCode.KeyPadAdd, false, '+' },
        };
    #endregion

    [Fact]
    public void IsLetterKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        var letterKeys = KeyboardKeyGroups.LetterKeys;
        SetKeysToTrue(letterKeys, keyCodes);

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsLetterKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsNumberKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        var numPadKeys = KeyboardKeyGroups.NumpadNumberKeys;
        var standardNumKeys = KeyboardKeyGroups.StandardNumberKeys;
        SetKeysToTrue(numPadKeys, keyCodes);
        SetKeysToTrue(standardNumKeys, keyCodes);

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsNumberKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsSymbolKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        var symbolKeys = KeyboardKeyGroups.SymbolKeys;
        SetKeysToTrue(symbolKeys, keyCodes);

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsSymbolKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsVisibleKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        var letterKeys = KeyboardKeyGroups.LetterKeys;
        var symbolKeys = KeyboardKeyGroups.SymbolKeys;
        var standardNumKeys = KeyboardKeyGroups.StandardNumberKeys;
        var numPadKeys = KeyboardKeyGroups.NumpadNumberKeys;
        SetKeysToTrue(letterKeys, keyCodes);
        SetKeysToTrue(symbolKeys, keyCodes);
        SetKeysToTrue(standardNumKeys, keyCodes);
        SetKeysToTrue(numPadKeys, keyCodes);

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsVisibleKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsShiftKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        keyCodes[KeyCode.LeftShift] = true;
        keyCodes[KeyCode.RightShift] = true;

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsShiftKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsArrowKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        keyCodes[KeyCode.Left] = true;
        keyCodes[KeyCode.Up] = true;
        keyCodes[KeyCode.Right] = true;
        keyCodes[KeyCode.Down] = true;

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsArrowKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsCtrlKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        keyCodes[KeyCode.LeftControl] = true;
        keyCodes[KeyCode.RightControl] = true;

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsCtrlKey().Should().Be(actualValue);
        });
    }

    [Theory]
    [MemberData(nameof(ToCharTestData))]
    public void ToChar_WhenAnyShiftKeyDownInvoked_ReturnsCorrectResult(KeyCode key, bool anyShiftKeysDown, char expected)
    {
        // Arrange & Act
        var actual = key.ToChar(anyShiftKeysDown);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void IsMoveCursorKeyTestData_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        keyCodes[KeyCode.Left] = true;
        keyCodes[KeyCode.Up] = true;
        keyCodes[KeyCode.Right] = true;
        keyCodes[KeyCode.Down] = true;
        keyCodes[KeyCode.PageUp] = true;
        keyCodes[KeyCode.PageDown] = true;
        keyCodes[KeyCode.Home] = true;
        keyCodes[KeyCode.End] = true;

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsMoveCursorKey().Should().Be(actualValue);
        });
    }

    [Fact]
    public void IsDeletionKey_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var keyCodes = CreateDefaultTestData();
        keyCodes[KeyCode.Delete] = true;
        keyCodes[KeyCode.Backspace] = true;

        // Act & Assert
        keyCodes.Should().AllSatisfy(data =>
        {
            (KeyCode actualKey, var actualValue) = data;

            actualKey.IsDeletionKey().Should().Be(actualValue);
        });
    }

    /// <summary>
    /// Creates a dictionary of <see cref="KeyCode"/>s with all values set to <see langword="false"/>.
    /// </summary>
    /// <returns>The test data.</returns>
    private static Dictionary<KeyCode, bool> CreateDefaultTestData()
    {
        var testData = new Dictionary<KeyCode, bool>();

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            testData.Add(key, false);
        }

        return testData;
    }

    /// <summary>
    /// Sets the specified <paramref name="keys"/> in the <paramref name="keyList"/> to <see langword="true"/>.
    /// </summary>
    /// <param name="keys">The keys to set.</param>
    /// <param name="keyList">The list of keys to change.</param>
    private static void SetKeysToTrue(IEnumerable<KeyCode> keys, IDictionary<KeyCode, bool> keyList)
    {
        foreach (var k in keys)
        {
            keyList[k] = true;
        }
    }
}

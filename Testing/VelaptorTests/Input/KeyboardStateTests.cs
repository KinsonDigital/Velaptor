// <copyright file="KeyboardStateTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // This needs to be disabled so the constructor can be used.
namespace VelaptorTests.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using FluentAssertions;
using Velaptor.Input;
using Xunit;

/// <summary>
/// Tests the <see cref="KeyboardState"/> struct.
/// </summary>
public class KeyboardStateTests
{
    private const char InvalidCharacter = '□';
    private readonly Dictionary<KeyCode, bool> keyStates = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardStateTests"/> class.
    /// </summary>
    public KeyboardStateTests()
    {
        var keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

        foreach (var keyCode in keyCodes)
        {
            this.keyStates.Add(keyCode, false);
        }
    }

    public static TheoryData<KeyCode, bool> StandardNumberKeyCodeData =>
        new ()
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            { KeyCode.D0,          true },
            { KeyCode.D1,          true },
            { KeyCode.D2,          true },
            { KeyCode.D3,          true },
            { KeyCode.D4,          true },
            { KeyCode.D5,          true },
            { KeyCode.D6,          true },
            { KeyCode.D7,          true },
            { KeyCode.D8,          true },
            { KeyCode.D9,          true },
            { KeyCode.KeyPad0,     false },
            { KeyCode.KeyPad1,     false },
            { KeyCode.KeyPad2,     false },
            { KeyCode.KeyPad3,     false },
            { KeyCode.KeyPad4,     false },
            { KeyCode.KeyPad5,     false },
            { KeyCode.KeyPad6,     false },
            { KeyCode.KeyPad7,     false },
            { KeyCode.KeyPad8,     false },
            { KeyCode.KeyPad9,     false },
            // ReSharper restore MultipleSpaces
        };

    public static TheoryData<KeyCode, char> StandardNumberKeysWithShiftDownCharacterData =>
        new ()
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            { KeyCode.D0,          ')' },
            { KeyCode.D1,          '!' },
            { KeyCode.D2,          '@' },
            { KeyCode.D3,          '#' },
            { KeyCode.D4,          '$' },
            { KeyCode.D5,          '%' },
            { KeyCode.D6,          '^' },
            { KeyCode.D7,          '&' },
            { KeyCode.D8,          '*' },
            { KeyCode.D9,          '(' },
            // ReSharper restore MultipleSpaces
        };

    public static TheoryData<KeyCode, char> StandardNumberKeysWithShiftUpCharacterData =>
        new ()
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            { KeyCode.D0,          '0' },
            { KeyCode.D1,          '1' },
            { KeyCode.D2,          '2' },
            { KeyCode.D3,          '3' },
            { KeyCode.D4,          '4' },
            { KeyCode.D5,          '5' },
            { KeyCode.D6,          '6' },
            { KeyCode.D7,          '7' },
            { KeyCode.D8,          '8' },
            { KeyCode.D9,          '9' },
            // ReSharper restore MultipleSpaces
        };

    public static TheoryData<KeyCode, bool> NumpadNumberKeyCodeData =>
        new ()
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            { KeyCode.D0,          false },
            { KeyCode.D1,          false },
            { KeyCode.D2,          false },
            { KeyCode.D3,          false },
            { KeyCode.D4,          false },
            { KeyCode.D5,          false },
            { KeyCode.D6,          false },
            { KeyCode.D7,          false },
            { KeyCode.D8,          false },
            { KeyCode.D9,          false },
            { KeyCode.KeyPad0,     true },
            { KeyCode.KeyPad1,     true },
            { KeyCode.KeyPad2,     true },
            { KeyCode.KeyPad3,     true },
            { KeyCode.KeyPad4,     true },
            { KeyCode.KeyPad5,     true },
            { KeyCode.KeyPad6,     true },
            { KeyCode.KeyPad7,     true },
            { KeyCode.KeyPad8,     true },
            { KeyCode.KeyPad9,     true },
            // ReSharper restore MultipleSpaces
        };

    public static TheoryData<KeyCode, char> LetterKeyUpperCaseCharacterData =>
        new ()
        {
            { KeyCode.A, 'A' },
            { KeyCode.B, 'B' },
            { KeyCode.C, 'C' },
            { KeyCode.D, 'D' },
            { KeyCode.E, 'E' },
            { KeyCode.F, 'F' },
            { KeyCode.G, 'G' },
            { KeyCode.H, 'H' },
            { KeyCode.I, 'I' },
            { KeyCode.J, 'J' },
            { KeyCode.K, 'K' },
            { KeyCode.L, 'L' },
            { KeyCode.M, 'M' },
            { KeyCode.N, 'N' },
            { KeyCode.O, 'O' },
            { KeyCode.P, 'P' },
            { KeyCode.Q, 'Q' },
            { KeyCode.R, 'R' },
            { KeyCode.S, 'S' },
            { KeyCode.T, 'T' },
            { KeyCode.U, 'U' },
            { KeyCode.V, 'V' },
            { KeyCode.W, 'W' },
            { KeyCode.X, 'X' },
            { KeyCode.Y, 'Y' },
            { KeyCode.Z, 'Z' },
            { KeyCode.Space, ' ' },
        };

    public static TheoryData<KeyCode, char> LetterKeyLowerCaseCharacterData =>
        new ()
        {
            { KeyCode.A, 'a' },
            { KeyCode.B, 'b' },
            { KeyCode.C, 'c' },
            { KeyCode.D, 'd' },
            { KeyCode.E, 'e' },
            { KeyCode.F, 'f' },
            { KeyCode.G, 'g' },
            { KeyCode.H, 'h' },
            { KeyCode.I, 'i' },
            { KeyCode.J, 'j' },
            { KeyCode.K, 'k' },
            { KeyCode.L, 'l' },
            { KeyCode.M, 'm' },
            { KeyCode.N, 'n' },
            { KeyCode.O, 'o' },
            { KeyCode.P, 'p' },
            { KeyCode.Q, 'q' },
            { KeyCode.R, 'r' },
            { KeyCode.S, 's' },
            { KeyCode.T, 't' },
            { KeyCode.U, 'u' },
            { KeyCode.V, 'v' },
            { KeyCode.W, 'w' },
            { KeyCode.X, 'x' },
            { KeyCode.Y, 'y' },
            { KeyCode.Z, 'z' },
            { KeyCode.Space, ' ' },
        };

    public static TheoryData<KeyCode, char> SymbolKeyCharacterWithShiftUpData =>
        new ()
        {
            { KeyCode.Equal, '=' },
            { KeyCode.Comma, ',' },
            { KeyCode.Minus, '-' },
            { KeyCode.Period, '.' },
            { KeyCode.Slash, '/' },
            { KeyCode.Backslash, '\\' },
            { KeyCode.LeftBracket, '[' },
            { KeyCode.RightBracket, ']' },
            { KeyCode.Apostrophe, '\'' },
            { KeyCode.Semicolon, ';' },
            { KeyCode.Period, '.' },
            { KeyCode.KeyPadDivide, '/' },
            { KeyCode.KeyPadMultiply, '*' },
            { KeyCode.KeyPadSubtract, '-' },
            { KeyCode.KeyPadAdd, '+' },
        };

    public static TheoryData<KeyCode, char> SymbolKeyCharacterWithShiftDownData =>
        new ()
        {
            { KeyCode.Equal, '+' },
            { KeyCode.Comma, '<' },
            { KeyCode.Minus, '_' },
            { KeyCode.Period, '>' },
            { KeyCode.Slash, '?' },
            { KeyCode.Backslash, '|' },
            { KeyCode.LeftBracket, '{' },
            { KeyCode.RightBracket, '}' },
            { KeyCode.Apostrophe, '"' },
            { KeyCode.Semicolon, ':' },
            { KeyCode.D1, '!' },
            { KeyCode.D2, '@' },
            { KeyCode.D3, '#' },
            { KeyCode.D4, '$' },
            { KeyCode.D5, '%' },
            { KeyCode.D6, '^' },
            { KeyCode.D7, '&' },
            { KeyCode.D8, '*' },
            { KeyCode.D9, '(' },
            { KeyCode.D0, ')' },
            { KeyCode.KeyPadDivide, '/' },
            { KeyCode.KeyPadMultiply, '*' },
            { KeyCode.KeyPadSubtract, '-' },
            { KeyCode.Equal, '+' },
        };

    #region Prop Tests
    [Fact]
    public void IsLeftShiftDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftShift, true);

        // Act
        var actual = state.IsLeftShiftKeyDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsRightShiftDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.RightShift, true);

        // Act
        var actual = state.IsRightShiftKeyDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsLeftControlDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftControl, true);

        // Act
        var actual = state.IsLeftCtrlKeyDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsRightControlDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.RightControl, true);

        // Act
        var actual = state.IsRightCtrlKeyDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsLeftAltDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftAlt, true);

        // Act
        var actual = state.IsLeftAltKeyDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsRightAltDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.RightAlt, true);

        // Act
        var actual = state.IsRightAltKeyDown();

        // Assert
        actual.Should().BeTrue();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void GetDownKeys_WhenInvoked_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new[]
        {
            KeyCode.D, KeyCode.E, KeyCode.H, KeyCode.L, KeyCode.O,
        };

        var state = new KeyboardState();

        state.SetKeyState(KeyCode.D, true);
        state.SetKeyState(KeyCode.E, true);
        state.SetKeyState(KeyCode.H, true);
        state.SetKeyState(KeyCode.L, true);
        state.SetKeyState(KeyCode.O, true);

        // Act
        var actual = state.GetDownKeys();

        // Assert
        actual.ToArray().Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void IsKeyUp_WhenKeyIsInUpPosition_ReturnsTrue()
    {
        // Arrange
        SetAllStatesTo(true);
        SetKeyState(KeyCode.C, false);
        var state = new KeyboardState();

        // Act
        var actual = state.IsKeyUp(KeyCode.C);

        // Assert
        actual.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(StandardNumberKeyCodeData))]
    public void AnyStandardNumberKeysDown_WhenInvokedWithNumberKeyDown_ReturnsTrue(KeyCode key, bool expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(key, expected);

        // Act
        var actual = state.AnyStandardNumberKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(NumpadNumberKeyCodeData))]
    public void AnyNumpadNumberKeysDown_WhenInvokedWithNumberKeyDown_ReturnsTrue(KeyCode key, bool expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(key, expected);

        // Act
        var actual = state.AnyNumpadNumberKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(KeyCode.LeftShift, true)]
    [InlineData(KeyCode.RightShift, true)]
    public void IsAnyShiftKeyDown_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(key, expected);

        // Act
        var actual = state.AnyShiftKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(KeyCode.LeftControl, true)]
    [InlineData(KeyCode.RightControl, true)]
    public void IsAnyCtrlKeyDown_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(key, expected);

        // Act
        var actual = state.AnyCtrlKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(KeyCode.LeftAlt, true)]
    [InlineData(KeyCode.RightAlt, true)]
    public void IsAnyAltKeyDown_WhenInvoked_ReturnsCorrectResult(KeyCode key, bool expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(key, expected);

        // Act
        var actual = state.AnyAltKeysDown();

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(LetterKeyUpperCaseCharacterData))]
    public void KeyToChar_WithShiftKeyDownAndLetterKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftShift, true);
        state.SetKeyState(key, true);

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(StandardNumberKeysWithShiftDownCharacterData))]
    [SuppressMessage("csharpsquid|Methods should not have identical implementations", "S4144", Justification = "Intentional")]
    public void KeyToChar_WithShiftKeyDownAndStandardNumberAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftShift, true);
        state.SetKeyState(key, true);

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(SymbolKeyCharacterWithShiftDownData))]
    [SuppressMessage("csharpsquid|Methods should not have identical implementations", "S4144", Justification = "Intentional")]
    public void KeyToChar_WithShiftKeyDownAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        var state = new KeyboardState();
        state.SetKeyState(KeyCode.LeftShift, true);
        state.SetKeyState(key, true);

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(LetterKeyLowerCaseCharacterData))]
    public void KeyToChar_WithShiftKeyUpAndLetterKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        SetKeyState(KeyCode.LeftShift, false);
        SetKeyState(key, true);
        var state = new KeyboardState();

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(SymbolKeyCharacterWithShiftUpData))]
    [SuppressMessage("csharpsquid|Methods should not have identical implementations", "S4144", Justification = "Intentional")]
    public void KeyToChar_WithShiftKeyUpAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        SetKeyState(KeyCode.LeftShift, false);
        SetKeyState(key, true);
        var state = new KeyboardState();

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [MemberData(nameof(StandardNumberKeysWithShiftUpCharacterData))]
    [SuppressMessage("csharpsquid|Methods should not have identical implementations", "S4144", Justification = "Intentional")]
    public void KeyToChar_WithShiftKeyUpAndStandardNumberKeys_ReturnsCorrectResult(KeyCode key, char expected)
    {
        // Arrange
        SetKeyState(KeyCode.LeftShift, false);
        SetKeyState(key, true);
        var state = new KeyboardState();

        // Act
        var actual = state.KeyToChar(key);

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void KeyToChar_WithInvalidKey_ReturnsCorrectResult()
    {
        // Arrange
        SetKeyState(KeyCode.LeftShift, false);
        SetKeyState(default, true);
        var state = new KeyboardState();

        // Act
        var actual = state.KeyToChar(default);

        // Assert
        actual.Should().Be(InvalidCharacter);
    }
    #endregion

    /// <summary>
    /// Sets all of the keyboard keys to the given <paramref name="state"/>.
    /// </summary>
    /// <param name="state">The state to set.</param>
    private void SetAllStatesTo(bool state)
    {
        for (var i = 0; i < this.keyStates.Keys.Count; i++)
        {
            this.keyStates[this.keyStates.Keys.ToArray()[i]] = state;
        }
    }

    /// <summary>
    /// Sets the given <paramref name="key"/> to the given <paramref name="state"/>.
    /// </summary>
    /// <param name="key">The key to set.</param>
    /// <param name="state">The state to set the key co.</param>
    private void SetKeyState(KeyCode key, bool state) => this.keyStates[key] = state;
}

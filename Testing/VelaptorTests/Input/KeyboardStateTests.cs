// <copyright file="KeyboardStateTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

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

    public static IEnumerable<object[]> StandardNumberKeyCodeData =>
        new List<object[]>
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            new object[] { KeyCode.D0,          true },
            new object[] { KeyCode.D1,          true },
            new object[] { KeyCode.D2,          true },
            new object[] { KeyCode.D3,          true },
            new object[] { KeyCode.D4,          true },
            new object[] { KeyCode.D5,          true },
            new object[] { KeyCode.D6,          true },
            new object[] { KeyCode.D7,          true },
            new object[] { KeyCode.D8,          true },
            new object[] { KeyCode.D9,          true },

            new object[] { KeyCode.KeyPad0,     false },
            new object[] { KeyCode.KeyPad1,     false },
            new object[] { KeyCode.KeyPad2,     false },
            new object[] { KeyCode.KeyPad3,     false },
            new object[] { KeyCode.KeyPad4,     false },
            new object[] { KeyCode.KeyPad5,     false },
            new object[] { KeyCode.KeyPad6,     false },
            new object[] { KeyCode.KeyPad7,     false },
            new object[] { KeyCode.KeyPad8,     false },
            new object[] { KeyCode.KeyPad9,     false },
            // ReSharper restore MultipleSpaces
        };

    public static IEnumerable<object[]> StandardNumberKeysWithShiftDownCharacterData =>
        new List<object[]>
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            new object[] { KeyCode.D0,          ')' },
            new object[] { KeyCode.D1,          '!' },
            new object[] { KeyCode.D2,          '@' },
            new object[] { KeyCode.D3,          '#' },
            new object[] { KeyCode.D4,          '$' },
            new object[] { KeyCode.D5,          '%' },
            new object[] { KeyCode.D6,          '^' },
            new object[] { KeyCode.D7,          '&' },
            new object[] { KeyCode.D8,          '*' },
            new object[] { KeyCode.D9,          '(' },
            // ReSharper restore MultipleSpaces
        };

    public static IEnumerable<object[]> StandardNumberKeysWithShiftUpCharacterData =>
        new List<object[]>
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            new object[] { KeyCode.D0,          '0' },
            new object[] { KeyCode.D1,          '1' },
            new object[] { KeyCode.D2,          '2' },
            new object[] { KeyCode.D3,          '3' },
            new object[] { KeyCode.D4,          '4' },
            new object[] { KeyCode.D5,          '5' },
            new object[] { KeyCode.D6,          '6' },
            new object[] { KeyCode.D7,          '7' },
            new object[] { KeyCode.D8,          '8' },
            new object[] { KeyCode.D9,          '9' },
            // ReSharper restore MultipleSpaces
        };

    public static IEnumerable<object[]> NumpadNumberKeyCodeData =>
        new List<object[]>
        {
            // ReSharper disable MultipleSpaces
            //              key param       expected param
            new object[] { KeyCode.D0,          false },
            new object[] { KeyCode.D1,          false },
            new object[] { KeyCode.D2,          false },
            new object[] { KeyCode.D3,          false },
            new object[] { KeyCode.D4,          false },
            new object[] { KeyCode.D5,          false },
            new object[] { KeyCode.D6,          false },
            new object[] { KeyCode.D7,          false },
            new object[] { KeyCode.D8,          false },
            new object[] { KeyCode.D9,          false },

            new object[] { KeyCode.KeyPad0,     true },
            new object[] { KeyCode.KeyPad1,     true },
            new object[] { KeyCode.KeyPad2,     true },
            new object[] { KeyCode.KeyPad3,     true },
            new object[] { KeyCode.KeyPad4,     true },
            new object[] { KeyCode.KeyPad5,     true },
            new object[] { KeyCode.KeyPad6,     true },
            new object[] { KeyCode.KeyPad7,     true },
            new object[] { KeyCode.KeyPad8,     true },
            new object[] { KeyCode.KeyPad9,     true },
            // ReSharper restore MultipleSpaces
        };

    public static IEnumerable<object[]> LetterKeyUpperCaseCharacterData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, 'A' },
            new object[] { KeyCode.B, 'B' },
            new object[] { KeyCode.C, 'C' },
            new object[] { KeyCode.D, 'D' },
            new object[] { KeyCode.E, 'E' },
            new object[] { KeyCode.F, 'F' },
            new object[] { KeyCode.G, 'G' },
            new object[] { KeyCode.H, 'H' },
            new object[] { KeyCode.I, 'I' },
            new object[] { KeyCode.J, 'J' },
            new object[] { KeyCode.K, 'K' },
            new object[] { KeyCode.L, 'L' },
            new object[] { KeyCode.M, 'M' },
            new object[] { KeyCode.N, 'N' },
            new object[] { KeyCode.O, 'O' },
            new object[] { KeyCode.P, 'P' },
            new object[] { KeyCode.Q, 'Q' },
            new object[] { KeyCode.R, 'R' },
            new object[] { KeyCode.S, 'S' },
            new object[] { KeyCode.T, 'T' },
            new object[] { KeyCode.U, 'U' },
            new object[] { KeyCode.V, 'V' },
            new object[] { KeyCode.W, 'W' },
            new object[] { KeyCode.X, 'X' },
            new object[] { KeyCode.Y, 'Y' },
            new object[] { KeyCode.Z, 'Z' },
            new object[] { KeyCode.Space, ' ' },
        };

    public static IEnumerable<object[]> LetterKeyLowerCaseCharacterData =>
        new List<object[]>
        {
            new object[] { KeyCode.A, 'a' },
            new object[] { KeyCode.B, 'b' },
            new object[] { KeyCode.C, 'c' },
            new object[] { KeyCode.D, 'd' },
            new object[] { KeyCode.E, 'e' },
            new object[] { KeyCode.F, 'f' },
            new object[] { KeyCode.G, 'g' },
            new object[] { KeyCode.H, 'h' },
            new object[] { KeyCode.I, 'i' },
            new object[] { KeyCode.J, 'j' },
            new object[] { KeyCode.K, 'k' },
            new object[] { KeyCode.L, 'l' },
            new object[] { KeyCode.M, 'm' },
            new object[] { KeyCode.N, 'n' },
            new object[] { KeyCode.O, 'o' },
            new object[] { KeyCode.P, 'p' },
            new object[] { KeyCode.Q, 'q' },
            new object[] { KeyCode.R, 'r' },
            new object[] { KeyCode.S, 's' },
            new object[] { KeyCode.T, 't' },
            new object[] { KeyCode.U, 'u' },
            new object[] { KeyCode.V, 'v' },
            new object[] { KeyCode.W, 'w' },
            new object[] { KeyCode.X, 'x' },
            new object[] { KeyCode.Y, 'y' },
            new object[] { KeyCode.Z, 'z' },
            new object[] { KeyCode.Space, ' ' },
        };

    public static IEnumerable<object[]> SymbolKeyCharacterWithShiftUpData =>
        new List<object[]>
        {
            new object[] { KeyCode.Equal, '=' },
            new object[] { KeyCode.Comma, ',' },
            new object[] { KeyCode.Minus, '-' },
            new object[] { KeyCode.Period, '.' },
            new object[] { KeyCode.Slash, '/' },
            new object[] { KeyCode.Backslash, '\\' },
            new object[] { KeyCode.LeftBracket, '[' },
            new object[] { KeyCode.RightBracket, ']' },
            new object[] { KeyCode.Apostrophe, '\'' },
            new object[] { KeyCode.Semicolon, ';' },
            new object[] { KeyCode.Period, '.' },
            new object[] { KeyCode.KeyPadDivide, '/' },
            new object[] { KeyCode.KeyPadMultiply, '*' },
            new object[] { KeyCode.KeyPadSubtract, '-' },
            new object[] { KeyCode.KeyPadAdd, '+' },
        };

    public static IEnumerable<object[]> SymbolKeyCharacterWithShiftDownData =>
        new List<object[]>
        {
            new object[] { KeyCode.Equal, '+' },
            new object[] { KeyCode.Comma, '<' },
            new object[] { KeyCode.Minus, '_' },
            new object[] { KeyCode.Period, '>' },
            new object[] { KeyCode.Slash, '?' },
            new object[] { KeyCode.Backslash, '|' },
            new object[] { KeyCode.LeftBracket, '{' },
            new object[] { KeyCode.RightBracket, '}' },
            new object[] { KeyCode.Apostrophe, '\'' },
            new object[] { KeyCode.Semicolon, ':' },
            new object[] { KeyCode.D1, '!' },
            new object[] { KeyCode.D2, '@' },
            new object[] { KeyCode.D3, '#' },
            new object[] { KeyCode.D4, '$' },
            new object[] { KeyCode.D5, '%' },
            new object[] { KeyCode.D6, '^' },
            new object[] { KeyCode.D7, '&' },
            new object[] { KeyCode.D8, '*' },
            new object[] { KeyCode.D9, '(' },
            new object[] { KeyCode.D0, ')' },
            new object[] { KeyCode.KeyPadDivide, '/' },
            new object[] { KeyCode.KeyPadMultiply, '*' },
            new object[] { KeyCode.KeyPadSubtract, '-' },
            new object[] { KeyCode.Equal, '+' },
        };

    #region Prop Tests
    [Fact]
    public void IsLeftShiftDown_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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

        var state = default(KeyboardState);

        state.SetKeyState(KeyCode.D, true);
        state.SetKeyState(KeyCode.E, true);
        state.SetKeyState(KeyCode.H, true);
        state.SetKeyState(KeyCode.L, true);
        state.SetKeyState(KeyCode.O, true);

        // Act
        var actual = state.GetDownKeys();

        // Assert
        actual.Should().Equal(expected);
    }

    [Fact]
    public void AnyKeysDown_WithNoArgumentsAndSingleDownKey_ReturnsTrue()
    {
        // Arrange
        var state = default(KeyboardState);
        state.SetKeyState(KeyCode.H, true);

        // Act
        var actual = state.AnyKeysDown();

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void AnyKeysDown_WhenCheckingCertainKeysAndAtLeastOneOfThemIsDown_ReturnsTrue()
    {
        // Arrange
        var downKeys = new[] { KeyCode.H, KeyCode.I };

        var state = default(KeyboardState);
        state.SetKeyState(KeyCode.H, true);

        // Act
        var actual = state.AnyKeysDown(downKeys);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void IsKeyUp_WhenKeyIsInUpPosition_ReturnsTrue()
    {
        // Arrange
        SetAllStatesTo(true);
        SetKeyState(KeyCode.C, false);
        var state = default(KeyboardState);

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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);
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
        var state = default(KeyboardState);

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
        var state = default(KeyboardState);

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
        var state = default(KeyboardState);

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
        var state = default(KeyboardState);

        // Act
        var actual = state.KeyToChar(default);

        // Assert
        actual.Should().Be(default);
    }

    [Fact]
    public void Equals_WhenUsingOverloadWithSameParamTypeWhileEqual_ReturnsTrue()
    {
        // Arrange
        var stateA = default(KeyboardState);
        var stateB = default(KeyboardState);

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    [SuppressMessage("csharpsquid|Methods should not have identical implementations", "S4144", Justification = "Intentional")]
    public void Equals_WhenUsingOverloadWithSameParamTypeWhileNotHavingSameKeys_ReturnsTrue()
    {
        // Arrange
        var stateA = default(KeyboardState);
        var stateB = default(KeyboardState);

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenUsingOverloadWithObjectParamWithSameEqualType_ReturnsTrue()
    {
        // Arrange
        var stateA = default(KeyboardState);
        object stateB = default(KeyboardState);

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Equals_WhenUsingOverloadWithObjectParamWithSameUnequalType_ReturnsFalse()
    {
        // Arrange
        var stateA = default(KeyboardState);
        var stateB = new object();

        // Act
        var actual = stateA.Equals(stateB);

        // Assert
        actual.Should().BeFalse();
    }

    [Fact]
    public void EqualsOperator_WhenInvokedWithEqualOperands_ReturnsTrue()
    {
        // Arrange
        var stateA = default(KeyboardState);
        var stateB = default(KeyboardState);

        // Act
        var actual = stateA == stateB;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void NotEqualsOperator_WhenInvokedWithEqualOperands_ReturnsTrue()
    {
        // Arrange
        var stateA = default(KeyboardState);
        var stateB = default(KeyboardState);
        stateB.SetKeyState(KeyCode.C, true);

        // Act
        var actual = stateA != stateB;

        // Assert
        actual.Should().BeTrue();
    }
    #endregion

    private void SetAllStatesTo(bool state)
    {
        for (var i = 0; i < this.keyStates.Keys.Count; i++)
        {
            this.keyStates[this.keyStates.Keys.ToArray()[i]] = state;
        }
    }

    private void SetKeyState(KeyCode key, bool state) => this.keyStates[key] = state;
}

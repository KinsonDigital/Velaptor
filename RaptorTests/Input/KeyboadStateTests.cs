// <copyright file="KeyboadStateTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace RaptorTests.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Raptor.Input;
    using Xunit;

    public class KeyboadStateTests
    {
        private readonly Dictionary<KeyCode, bool> keyStates = new Dictionary<KeyCode, bool>();

        public KeyboadStateTests()
        {
            var keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            for (var i = 0; i < keyCodes.Length; i++)
            {
                this.keyStates.Add(keyCodes[i], false);
            }
        }

        public static IEnumerable<object[]> StandardNumberKeyCodeData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
                new object[] { KeyCode.Number0,          true },
                new object[] { KeyCode.Number1,          true },
                new object[] { KeyCode.Number2,          true },
                new object[] { KeyCode.Number3,          true },
                new object[] { KeyCode.Number4,          true },
                new object[] { KeyCode.Number5,          true },
                new object[] { KeyCode.Number6,          true },
                new object[] { KeyCode.Number7,          true },
                new object[] { KeyCode.Number8,          true },
                new object[] { KeyCode.Number9,          true },

                new object[] { KeyCode.Numpad0,     false },
                new object[] { KeyCode.Numpad1,     false },
                new object[] { KeyCode.Numpad2,     false },
                new object[] { KeyCode.Numpad3,     false },
                new object[] { KeyCode.Numpad4,     false },
                new object[] { KeyCode.Numpad5,     false },
                new object[] { KeyCode.Numpad6,     false },
                new object[] { KeyCode.Numpad7,     false },
                new object[] { KeyCode.Numpad8,     false },
                new object[] { KeyCode.Numpad9,     false },
            };

        public static IEnumerable<object[]> StandardNumberKeysWithShiftDownCharacterData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
                new object[] { KeyCode.Number0,          ')' },
                new object[] { KeyCode.Number1,          '!' },
                new object[] { KeyCode.Number2,          '@' },
                new object[] { KeyCode.Number3,          '#' },
                new object[] { KeyCode.Number4,          '$' },
                new object[] { KeyCode.Number5,          '%' },
                new object[] { KeyCode.Number6,          '^' },
                new object[] { KeyCode.Number7,          '&' },
                new object[] { KeyCode.Number8,          '*' },
                new object[] { KeyCode.Number9,          '(' },
            };

        public static IEnumerable<object[]> StandardNumberKeysWithShiftUpCharacterData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
                new object[] { KeyCode.Number0,          '0' },
                new object[] { KeyCode.Number1,          '1' },
                new object[] { KeyCode.Number2,          '2' },
                new object[] { KeyCode.Number3,          '3' },
                new object[] { KeyCode.Number4,          '4' },
                new object[] { KeyCode.Number5,          '5' },
                new object[] { KeyCode.Number6,          '6' },
                new object[] { KeyCode.Number7,          '7' },
                new object[] { KeyCode.Number8,          '8' },
                new object[] { KeyCode.Number9,          '9' },
            };

        public static IEnumerable<object[]> NumpadNumberKeyCodeData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
                new object[] { KeyCode.Number0,          false },
                new object[] { KeyCode.Number1,          false },
                new object[] { KeyCode.Number2,          false },
                new object[] { KeyCode.Number3,          false },
                new object[] { KeyCode.Number4,          false },
                new object[] { KeyCode.Number5,          false },
                new object[] { KeyCode.Number6,          false },
                new object[] { KeyCode.Number7,          false },
                new object[] { KeyCode.Number8,          false },
                new object[] { KeyCode.Number9,          false },

                new object[] { KeyCode.Numpad0,     true },
                new object[] { KeyCode.Numpad1,     true },
                new object[] { KeyCode.Numpad2,     true },
                new object[] { KeyCode.Numpad3,     true },
                new object[] { KeyCode.Numpad4,     true },
                new object[] { KeyCode.Numpad5,     true },
                new object[] { KeyCode.Numpad6,     true },
                new object[] { KeyCode.Numpad7,     true },
                new object[] { KeyCode.Numpad8,     true },
                new object[] { KeyCode.Numpad9,     true },
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
                new object[] { KeyCode.Plus, '=' },
                new object[] { KeyCode.Comma, ',' },
                new object[] { KeyCode.Minus, '-' },
                new object[] { KeyCode.Period, '.' },
                new object[] { KeyCode.ForwardSlash, '/' },
                new object[] { KeyCode.Tilde, '`' },
                new object[] { KeyCode.BackSlash, '\\' },
                new object[] { KeyCode.LeftBracket, '[' },
                new object[] { KeyCode.RightBracket, ']' },
                new object[] { KeyCode.Quote, '\'' },
                new object[] { KeyCode.Semicolon, ';' },
                new object[] { KeyCode.Period, '.' },
                new object[] { KeyCode.NumpadDivide, '/' },
                new object[] { KeyCode.NumpadMultiply, '*' },
                new object[] { KeyCode.NumpadMinus, '-' },
                new object[] { KeyCode.NumpadPlus, '+' },
            };

        public static IEnumerable<object[]> SymbolKeyCharacterWithShiftDownData =>
            new List<object[]>
            {
                new object[] { KeyCode.Plus, '+' },
                new object[] { KeyCode.Comma, '<' },
                new object[] { KeyCode.Minus, '_' },
                new object[] { KeyCode.Period, '>' },
                new object[] { KeyCode.ForwardSlash, '?' },
                new object[] { KeyCode.Tilde, '~' },
                new object[] { KeyCode.BackSlash, '|' },
                new object[] { KeyCode.LeftBracket, '{' },
                new object[] { KeyCode.RightBracket, '}' },
                new object[] { KeyCode.Quote, '\'' },
                new object[] { KeyCode.Semicolon, ':' },
                new object[] { KeyCode.Number1, '!' },
                new object[] { KeyCode.Number2, '@' },
                new object[] { KeyCode.Number3, '#' },
                new object[] { KeyCode.Number4, '$' },
                new object[] { KeyCode.Number5, '%' },
                new object[] { KeyCode.Number6, '^' },
                new object[] { KeyCode.Number7, '&' },
                new object[] { KeyCode.Number8, '*' },
                new object[] { KeyCode.Number9, '(' },
                new object[] { KeyCode.Number0, ')' },
                new object[] { KeyCode.NumpadDivide, '/' },
                new object[] { KeyCode.NumpadMultiply, '*' },
                new object[] { KeyCode.NumpadMinus, '-' },
                new object[] { KeyCode.Plus, '+' },
            };

        #region Prop Tests
        /*TODO:
            These tests are for testing caps and num lock.  Currently OpenTK 4.0
            needs some implementation of the proper overload and enums to be able to do this.
            Refer to https://github.com/opentk/opentk/issues/1089 for more info
        [Fact]
        public void CapsLockOn_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            SetKeyState(KeyCode.CapsLock, true);
            var state = new KeyboardState();

            // Act
            var actual = state.CapsLockOn;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void NumLockOn_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.NumLock, true);
            var state = new KeyboardState();

            // Act
            var actual = state.NumLockOn;

            // Assert
            Assert.True(actual);
        }
        */

        [Fact]
        public void IsLeftShiftDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.LeftShift, true);

            // Act
            var actual = state.IsLeftShiftDown();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightShiftDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.RightShift, true);

            // Act
            var actual = state.IsRightShiftDown();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsLeftControlDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.LeftControl, true);

            // Act
            var actual = state.IsLeftCtrlDown();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightControlDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.RightControl, true);

            // Act
            var actual = state.IsRightCtrlDown();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsLeftAltDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.LeftAlt, true);

            // Act
            var actual = state.IsLeftAltDown();

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightAltDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.RightAlt, true);

            // Act
            var actual = state.IsRightAltDown();

            // Assert
            Assert.True(actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void GetDownKeys_WhenInvoked_ReturnsCorrectValue()
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
            Assert.Equal(expected, actual);
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
            Assert.True(actual);
        }

        [Fact]
        public void AnyKeysDown_WhenCheckingCertainKeysAndAtLeastOneOfThemIsDown_ReturnsTrue()
        {
            // Arrange
            var downKeys = new[] { KeyCode.H, KeyCode.I };

            var state = Keyboard.GetState();
            state.SetKeyState(KeyCode.H, true);

            // Act
            var actual = state.AnyKeysDown(downKeys);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsKeyUp_WhenKeyIsInUpPostion_ReturnsTrue()
        {
            // Arrange
            SetAllStatesTo(true);
            SetKeyState(KeyCode.C, false);
            var state = default(KeyboardState);

            // Act
            var actual = state.IsKeyUp(KeyCode.C);

            // Assert
            Assert.True(actual);
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
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(KeyCode.LeftShift, true)]
        [InlineData(KeyCode.RightShift, true)]
        public void IsAnyShiftKeyDown_WhenInvoked_ReturnsCorrectValue(KeyCode key, bool expected)
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(key, expected);

            // Act
            var actual = state.IsAnyShiftKeyDown();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(KeyCode.LeftControl, true)]
        [InlineData(KeyCode.RightControl, true)]
        public void IsAnyCtrlKeyDown_WhenInvoked_ReturnsCorrectValue(KeyCode key, bool expected)
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(key, expected);

            // Act
            var actual = state.IsAnyCtrlKeyDown();

            // Assert
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StandardNumberKeysWithShiftDownCharacterData))]
        public void KeyToChar_WithShiftKeyDownAndStandardNumberAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.LeftShift, true);
            state.SetKeyState(key, true);

            // Act
            var actual = state.KeyToChar(key);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SymbolKeyCharacterWithShiftDownData))]
        public void KeyToChar_WithShiftKeyDownAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
        {
            // Arrange
            var state = default(KeyboardState);
            state.SetKeyState(KeyCode.LeftShift, true);
            state.SetKeyState(key, true);

            // Act
            var actual = state.KeyToChar(key);

            // Assert
            Assert.Equal(expected, actual);
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
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(SymbolKeyCharacterWithShiftUpData))]
        public void KeyToChar_WithShiftKeyUpAndSymbolKeys_ReturnsCorrectResult(KeyCode key, char expected)
        {
            // Arrange
            SetKeyState(KeyCode.LeftShift, false);
            SetKeyState(key, true);
            var state = default(KeyboardState);

            // Act
            var actual = state.KeyToChar(key);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [MemberData(nameof(StandardNumberKeysWithShiftUpCharacterData))]
        public void KeyToChar_WithShiftKeyUpAndStandardNumberKeys_ReturnsCorrectResult(KeyCode key, char expected)
        {
            // Arrange
            SetKeyState(KeyCode.LeftShift, false);
            SetKeyState(key, true);
            var state = default(KeyboardState);

            // Act
            var actual = state.KeyToChar(key);

            // Assert
            Assert.Equal(expected, actual);
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
            Assert.Equal(default, actual);
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
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithSameParamTypeWhileNotHavingSameKeys_ReturnsTrue()
        {
            // Arrange
            var stateA = default(KeyboardState);
            var stateB = default(KeyboardState);

            // Act
            var actual = stateA.Equals(stateB);

            // Assert
            Assert.True(actual);
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
            Assert.True(actual);
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
            Assert.False(actual);
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
            Assert.True(actual);
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
}

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
    using RaptorTests.Helpers;
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

                new object[] { KeyCode.NumPad0,     false },
                new object[] { KeyCode.NumPad1,     false },
                new object[] { KeyCode.NumPad2,     false },
                new object[] { KeyCode.NumPad3,     false },
                new object[] { KeyCode.NumPad4,     false },
                new object[] { KeyCode.NumPad5,     false },
                new object[] { KeyCode.NumPad6,     false },
                new object[] { KeyCode.NumPad7,     false },
                new object[] { KeyCode.NumPad8,     false },
                new object[] { KeyCode.NumPad9,     false },
            };

        public static IEnumerable<object[]> StandardNumberKeysWithShiftDownCharacterData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
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
            };

        public static IEnumerable<object[]> StandardNumberKeysWithShiftUpCharacterData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
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
            };

        public static IEnumerable<object[]> NumpadNumberKeyCodeData =>
            new List<object[]>
            {
#pragma warning disable SA1005 // Single line comments should begin with single space
                //              key param       expected param
#pragma warning restore SA1005 // Single line comments should begin with single space
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

                new object[] { KeyCode.NumPad0,     true },
                new object[] { KeyCode.NumPad1,     true },
                new object[] { KeyCode.NumPad2,     true },
                new object[] { KeyCode.NumPad3,     true },
                new object[] { KeyCode.NumPad4,     true },
                new object[] { KeyCode.NumPad5,     true },
                new object[] { KeyCode.NumPad6,     true },
                new object[] { KeyCode.NumPad7,     true },
                new object[] { KeyCode.NumPad8,     true },
                new object[] { KeyCode.NumPad9,     true },
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
                new object[] { KeyCode.OemPlus, '=' },
                new object[] { KeyCode.OemComma, ',' },
                new object[] { KeyCode.OemMinus, '-' },
                new object[] { KeyCode.OemPeriod, '.' },
                new object[] { KeyCode.OemQuestion, '/' },
                new object[] { KeyCode.OemTilde, '`' },
                new object[] { KeyCode.OemPipe, '\\' },
                new object[] { KeyCode.OemOpenBrackets, '[' },
                new object[] { KeyCode.OemCloseBrackets, ']' },
                new object[] { KeyCode.OemQuotes, '\'' },
                new object[] { KeyCode.OemSemicolon, ';' },
                new object[] { KeyCode.Decimal, '.' },
                new object[] { KeyCode.Divide, '/' },
                new object[] { KeyCode.Multiply, '*' },
                new object[] { KeyCode.Subtract, '-' },
                new object[] { KeyCode.Add, '+' },
            };

        public static IEnumerable<object[]> SymbolKeyCharacterWithShiftDownData =>
            new List<object[]>
            {
                new object[] { KeyCode.OemPlus, '+' },
                new object[] { KeyCode.OemComma, '<' },
                new object[] { KeyCode.OemMinus, '_' },
                new object[] { KeyCode.OemPeriod, '>' },
                new object[] { KeyCode.OemQuestion, '?' },
                new object[] { KeyCode.OemTilde, '~' },
                new object[] { KeyCode.OemPipe, '|' },
                new object[] { KeyCode.OemOpenBrackets, '{' },
                new object[] { KeyCode.OemCloseBrackets, '}' },
                new object[] { KeyCode.OemQuotes, '\'' },
                new object[] { KeyCode.OemSemicolon, ':' },
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
                new object[] { KeyCode.Divide, '/' },
                new object[] { KeyCode.Multiply, '*' },
                new object[] { KeyCode.Subtract, '-' },
                new object[] { KeyCode.Add, '+' },
            };

        #region Constructor Tests
        [Theory]
        [InlineData(null)]
        [InlineData(new KeyCode[0])]
        public void Ctor_WhenInvokedWithNullOrEmptyKeyCodeArray_ThrowsException(KeyCode[] keys)
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var state = new KeyboardState(keys, this.keyStates.Values.ToArray());
            }, "The parameter must not be null or empty. (Parameter 'keys')");
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new bool[0])]
        public void Ctor_WhenInvokedWithNullOrEmptyKeyStatesCodeArray_ThrowsException(bool[] keyStates)
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                var state = new KeyboardState(this.keyStates.Keys.ToArray(), keyStates);
            }, "The parameter must not be null or empty. (Parameter 'keyStates')");
        }

        [Fact]
        public void Ctor_WhenInvokedWithKeysAndKeyStatesNotHavingSameQuantity_ThrowsException()
        {
            // Act & Assert
            AssertHelpers.ThrowsWithMessage<ArgumentException>(() =>
            {
                var keys = this.keyStates.Keys.ToList();
                keys.RemoveAt(0);

                var state = new KeyboardState(keys.ToArray(), this.keyStates.Values.ToArray());
            }, "The list of 'keys' and 'keyStates' must have the same number of items.");
        }
        #endregion

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(KeyCode.LeftShift, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsLeftShiftDown;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightShiftDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.RightShift, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsRightShiftDown;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsLeftControlDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.LeftControl, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsLeftCtrlDown;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightControlDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.RightControl, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsRightCtrlDown;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsLeftAltDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.LeftAlt, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsLeftAltDown;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsRightAltDown_WhenGettingValue_ReturnsCorrectValue()
        {
            // Arrange
            SetKeyState(KeyCode.RightAlt, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.IsRightAltDown;

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

            SetKeyState(KeyCode.D, true);
            SetKeyState(KeyCode.E, true);
            SetKeyState(KeyCode.H, true);
            SetKeyState(KeyCode.L, true);
            SetKeyState(KeyCode.O, true);

            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.GetDownKeys();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AnyKeysDown_WithNoArgumentsAndSingleDownKey_ReturnsTrue()
        {
            // Arrange
            SetKeyState(KeyCode.C, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(KeyCode.H, true);

            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(key, expected);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(key, expected);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(key, expected);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(key, expected);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(KeyCode.LeftShift, true);
            SetKeyState(key, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(KeyCode.LeftShift, true);
            SetKeyState(key, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            SetKeyState(KeyCode.LeftShift, true);
            SetKeyState(key, true);
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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
            var state = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = state.KeyToChar(default);

            // Assert
            Assert.Equal(default, actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithSameParamTypeWhileOperandsHaveDifferentTotals_ReturnsFalse()
        {
            // Arrange
            var stateA = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            var stateBKeys = this.keyStates.Keys.ToList();
            stateBKeys.RemoveAt(0);

            var stateBValues = this.keyStates.Values.ToList();
            stateBValues.RemoveAt(0);

            var stateB = new KeyboardState(stateBKeys.ToArray(), stateBValues.ToArray());

            // Act
            var actual = stateA.Equals(stateB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithSameParamTypeWhileOperandsHaveDifferentStates_ReturnsFalse()
        {
            // Arrange
            var stateAKeys = this.keyStates.Keys.ToList();
            stateAKeys.RemoveAt(1);

            var stateAValues = this.keyStates.Values.ToList();
            stateAValues.RemoveAt(1);

            var stateA = new KeyboardState(stateAKeys.ToArray(), stateAValues.ToArray());

            var stateBKeys = this.keyStates.Keys.ToList();
            stateBKeys.RemoveAt(0);

            var stateBValues = this.keyStates.Values.ToList();
            stateBValues.RemoveAt(0);

            var stateB = new KeyboardState(stateBKeys.ToArray(), stateBValues.ToArray());

            // Act
            var actual = stateA.Equals(stateB);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithSameParamTypeWhileEqual_ReturnsTrue()
        {
            // Arrange
            var stateA = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());
            var stateB = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = stateA.Equals(stateB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithObjectParamWithSameEqualType_ReturnsTrue()
        {
            // Arrange
            var stateA = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());
            object stateB = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

            // Act
            var actual = stateA.Equals(stateB);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Equals_WhenUsingOverloadWithObjectParamWithSameUnequalType_ReturnsFalse()
        {
            // Arrange
            var stateA = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());
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
            var stateA = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());
            var stateB = new KeyboardState(this.keyStates.Keys.ToArray(), this.keyStates.Values.ToArray());

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

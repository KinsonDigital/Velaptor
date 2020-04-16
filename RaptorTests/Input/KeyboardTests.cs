#nullable disable
using Moq;
using Xunit;
using Raptor.Input;
using Raptor.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RaptorTests.Input
{
    /// <summary>
    /// Unit tests to test the <see cref="Keyboard"/> class.
    /// </summary>
    public class KeyboardTests : IDisposable
    {
        #region Private Fields
        private Mock<IKeyboard> _mockKeyboard;
        private readonly char[] _numberAndSymbolCharacters = new []
        {
            '`', '1', '2', '3', '4', '5', '6', '7', '8', '9',
            '0', '-', '=', '[', ']', '\\',	';', '\'', ',',	'.',	
            '/', '!', '@', '#',	'$', '%', '^', '&',	'*', '(', '~',
            ')', '_', '+', '{',	'}', '|', ':', '"',	'<', '>', '?'
        };


        private readonly char[] _lowerCaseLetters;
        private readonly char[] _upperCaseLetters;
        #endregion


        #region Constructors
        public KeyboardTests()
        {
            _lowerCaseLetters = LetterKeyData.Select(l =>
            {
                var keyCode = (KeyCode)l[0];

                return keyCode == KeyCode.Space ? ' ' : char.Parse(keyCode.ToString().ToLower());
            }).ToArray();

            _upperCaseLetters = LetterKeyData.Select(l =>
            {
                var keyCode = (KeyCode)l[0];

                return keyCode == KeyCode.Space ? ' ' : char.Parse(keyCode.ToString());
            }).ToArray();

            _mockKeyboard = new Mock<IKeyboard>();
        }
        #endregion


        #region Prop Tests
        [Fact]
        public void CapsLockOn_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.CapsLockOn;

            //Assert
            _mockKeyboard.VerifyGet(p => p.CapsLockOn, Times.Once());
        }


        [Fact]
        public void IsLeftAltDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsLeftAltDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsLeftAltDown, Times.Once());
        }


        [Fact]
        public void IsLeftCtrlDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsLeftCtrlDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsLeftCtrlDown, Times.Once());
        }


        [Fact]
        public void IsLeftShiftDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsLeftShiftDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsLeftShiftDown, Times.Once());
        }


        [Fact]
        public void IsRightAltDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsRightAltDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsRightAltDown, Times.Once());
        }


        [Fact]
        public void IsRightCtrlDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsRightCtrlDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsRightCtrlDown, Times.Once());
        }


        [Fact]
        public void IsRightShiftDown_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsRightShiftDown;

            //Assert
            _mockKeyboard.VerifyGet(p => p.IsRightShiftDown, Times.Once());
        }


        [Fact]
        public void NumLockOn_WhenGettingValue_InvokesInternalMethod()
        {
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.NumLockOn;

            //Assert
            _mockKeyboard.VerifyGet(p => p.NumLockOn, Times.Once());
        }
        #endregion


        #region Method Tests
        [Fact]
        public void GetCurrentPressedKeys_WhenInvoking_ReturnsCorrectPressedKeys()
        {
            //Arrange
            var pressedKeys = new KeyCode[] { KeyCode.Left, KeyCode.Z };

            _mockKeyboard.Setup(m => m.GetCurrentPressedKeys()).Returns(pressedKeys);

            var keyboard = new Keyboard(_mockKeyboard.Object);
            var expected = new KeyCode[] { KeyCode.Left, KeyCode.Z };

            //Act
            var actual = keyboard.GetCurrentPressedKeys();

            //Assert
            Assert.Equal(expected, actual);
            _mockKeyboard.Verify(m => m.GetCurrentPressedKeys(), Times.Once());
        }


        [Fact]
        public void GetPreviousPressedKeys_WhenInvoking_ReturnsCorrectPressedKeys()
        {
            //Arrange
            var pressedKeys = new KeyCode[] { KeyCode.Down, KeyCode.U };
            _mockKeyboard.Setup(m => m.GetPreviousPressedKeys()).Returns(pressedKeys);

            var keyboard = new Keyboard(_mockKeyboard.Object);
            var expected = new KeyCode[] { KeyCode.Down, KeyCode.U };

            //Act
            var actual = keyboard.GetPreviousPressedKeys();

            //Assert
            Assert.Equal(expected, actual);
            _mockKeyboard.Verify(m => m.GetPreviousPressedKeys(), Times.Once());
        }


        [Fact]
        public void IsKeyDown_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(It.IsAny<KeyCode>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyDown(KeyCode.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyDown(It.IsAny<KeyCode>()), Times.Once());
        }


        [Fact]
        public void IsKeyUp_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyUp(It.IsAny<KeyCode>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyUp(KeyCode.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyUp(It.IsAny<KeyCode>()), Times.Once());
        }


        [Fact]
        public void IsKeyPressed_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(It.IsAny<KeyCode>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyPressed(KeyCode.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(It.IsAny<KeyCode>()), Times.Once());
        }


        [Fact]
        public void UpdateCurrentState_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            keyboard.UpdateCurrentState();

            //Assert
            _mockKeyboard.Verify(m => m.UpdateCurrentState(), Times.Once());
        }


        [Fact]
        public void UpdatePreviousState_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            keyboard.UpdatePreviousState();

            //Assert
            _mockKeyboard.Verify(m => m.UpdatePreviousState(), Times.Once());
        }


        [Fact]
        public void AreAnyKeysDown_WhenInvoking_InvokesInternalMethod()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            keyboard.AreAnyKeysDown();

            //Assert
            _mockKeyboard.Verify(m => m.AreAnyKeysDown(), Times.Once());
        }


        [Fact]
        public void IsAnyKeyDown_WhenInvoking_InvokesInternalMethod()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            keyboard.IsAnyKeyDown(new KeyCode[] { KeyCode.X, KeyCode.Z });

            //Assert
            _mockKeyboard.Verify(m => m.AreKeysDown(It.IsAny<KeyCode[]>()), Times.Once());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithNoParamsWithLetterKeyDown_ReturnsTrue(KeyCode keyToCheck)
        {
            //Act
            _mockKeyboard.Setup(m => m.IsKeyPressed(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyLettersPressed());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithNoParamsWithLetterKeyNotDown_ReturnsFalse(KeyCode keyToCheck)
        {
            //Act
            _mockKeyboard.Setup(m => m.IsKeyPressed(keyToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.False(keyboard.AnyLettersPressed());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithOneParamAndPressedLetterKey_InvokesKeyPressedAndReturnsPressedKeyValue(KeyCode letterToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(letterToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            keyboard.AnyLettersPressed(out var actual);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(letterToCheck), Times.Once());
            Assert.Equal(letterToCheck, actual);
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithOneParamAndNoPressedLetterKeys_ReturnsFalse(KeyCode letterToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(letterToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyLettersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(letterToCheck), Times.Once());
            Assert.Equal(KeyCode.None, pressedKey);
            Assert.False(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithStandardNumberKeyIsPressed_ReturnsTrue(KeyCode numToCheck)
        {
            //Arrange
            var numpadNumberKeys = (from k in NumpadNumberKeyData select (KeyCode)k[0]).ToArray();

            //Setup all of the standard number keys to return false
            numpadNumberKeys.ToList().ForEach(k => _mockKeyboard.Setup(m => m.IsKeyPressed(k)).Returns(false));

            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed();

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithNumberKeysNotPressed_ReturnsFalse(KeyCode numToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(It.IsAny<KeyCode>())).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed();

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.False(actual);
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithNumpadNumberKeysNotPressed_ReturnsTrue(KeyCode numToCheck)
        {
            //Arrange
            var standardKeys = (from k in StandardNumberKeyData select (KeyCode)k[0]).ToArray();

            //Setup all of the standard number keys to return false
            standardKeys.ToList().ForEach(k => _mockKeyboard.Setup(m => m.IsKeyPressed(k)).Returns(false));

            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed();

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(It.IsAny<KeyCode>()), Times.AtLeastOnce());
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndStandardNumberKeyIsPressed_ReturnsTrue(KeyCode numToCheck)
        {
            //Arrange
            var numpadKeys = (from k in NumpadNumberKeyData select (KeyCode)k[0]).ToArray();

            //Setup all of the standard number keys to return false
            numpadKeys.ToList().ForEach(k => _mockKeyboard.Setup(m => m.IsKeyPressed(k)).Returns(false));

            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.True(actual);
            Assert.Equal(numToCheck, pressedKey);
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithParamAndNumpadNumberKeyIsPressed_ReturnsTrue(KeyCode numToCheck)
        {
            //Arrange
            var standardKeys = (from k in StandardNumberKeyData select (KeyCode)k[0]).ToArray();

            //Setup all of the standard number keys to return false
            standardKeys.ToList().ForEach(k => _mockKeyboard.Setup(m => m.IsKeyPressed(k)).Returns(false));

            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.True(actual);
            Assert.Equal(numToCheck, pressedKey);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithParamAndNoNumbersPressed_ReturnsFalse(KeyCode numToCheck)
        {
            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.False(actual);
            Assert.Equal(KeyCode.None, pressedKey);
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void KeyToChar_WhenInvokingWithShiftDownAndLetterKeys_ReturnsCorrectLetterCharacter(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCode.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _lowerCaseLetters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void KeyToChar_WhenInvokingWithShiftUpAndLetterKeys_ReturnsCorrectLetterCharacter(KeyCode keyToCheck)
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _upperCaseLetters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(ShiftDownSymbolKeys))]
        public void KeyToChar_WhenInvokingWithShiftDownAndSymbolKeys_ReturnsCorrectSymbolCharacter(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCode.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _numberAndSymbolCharacters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(ShiftUpSymbolKeys))]
        public void KeyToChar_WhenInvokingWithShiftUpAndSymbolKeys_ReturnsCorrectSymbolCharacter(KeyCode keyToCheck)
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _numberAndSymbolCharacters.Contains(keyboard.KeyToChar(keyToCheck));
            
            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void KeyToChar_WhenInvokingWithShiftUpAndStandardNumberKeys_ReturnsCorrectNumberCharacter(KeyCode keyToCheck)
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _numberAndSymbolCharacters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(NumpadSymbolKeyData))]
        public void KeyToChar_WhenInvokingWithShiftUpAndNumpadNumberKeys_ReturnsCorrectNumberCharacter(KeyCode keyToCheck)
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _numberAndSymbolCharacters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Fact]
        public void KeyToChar_WhenInvokingWithShiftDownAndKeyWithNoCharacterValue_ReturnsTildeCharacter()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCode.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.KeyToChar(KeyCode.CapsLock);

            //Assert
            Assert.Equal((char)0, actual);
        }


        [Fact]
        public void KeyToChar_WhenInvokingWithShiftUpAndKeyWithNoCharacterValue_ReturnsTildeCharacter()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.KeyToChar(KeyCode.CapsLock);

            //Assert
            Assert.Equal((char)0, actual);
        }


        [Fact]
        public void IsDeleteKeyPressed_WhenInvokedWithDeleteKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCode.Delete)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsDeleteKeyPressed());
        }


        [Fact]
        public void IsDeleteKeyPressed_WhenInvokedWithNumpadDeleteKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCode.LeftShift)).Returns(true);
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCode.Decimal)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsDeleteKeyPressed());
        }


        [Fact]
        public void IsBackspaceKeyPressed_WhenInvokedWithBackspaceKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCode.Back)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsBackspaceKeyPressed());
        }


        [Fact]
        public void IsAnyCtrlKeyDown_WhenInvokedWithLeftCtrlKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.SetupGet(p => p.IsLeftCtrlDown).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsAnyCtrlKeyDown());
        }


        [Fact]
        public void IsAnyCtrlKeyDown_WhenInvokedWithRightCtrlKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.SetupGet(p => p.IsRightCtrlDown).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsAnyCtrlKeyDown());
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyStandardNumberKeysDown_WhenInvokedWithStandardKeyIsDown_ReturnsTrue(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyStandardNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyStandardNumberKeysDown_WhenInvokedWithStandardKeyNotDown_ReturnsFalse(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.False(keyboard.AnyStandardNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumpadNumberKeysDown_WhenInvokedWithNumpadNumberKeyIsDown_ReturnsTrue(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyNumpadNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumpadNumberKeysDown_WhenInvokedWithNumpadNumberKeyNotDown_ReturnsFalse(KeyCode keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.False(keyboard.AnyNumpadNumberKeysDown());
        }
        #endregion


        #region Test Data
        /// <summary>
        /// These are the standard number keys.  Not the keys pressed on the numpad.
        /// </summary>
        public static IEnumerable<object[]> StandardNumberKeyData => new List<object[]>
        {
            new object[] { KeyCode.D0 },
            new object[] { KeyCode.D1 },
            new object[] { KeyCode.D2 },
            new object[] { KeyCode.D3 },
            new object[] { KeyCode.D4 },
            new object[] { KeyCode.D5 },
            new object[] { KeyCode.D6 },
            new object[] { KeyCode.D7 },
            new object[] { KeyCode.D8 },
            new object[] { KeyCode.D9 }
        };

        /// <summary>
        /// These are tne number keys on the numpad.
        /// </summary>
        public static IEnumerable<object[]> NumpadNumberKeyData => new List<object[]>
        {
            new object[] { KeyCode.NumPad0 },
            new object[] { KeyCode.NumPad1 },
            new object[] { KeyCode.NumPad2 },
            new object[] { KeyCode.NumPad3 },
            new object[] { KeyCode.NumPad4 },
            new object[] { KeyCode.NumPad5 },
            new object[] { KeyCode.NumPad6 },
            new object[] { KeyCode.NumPad7 },
            new object[] { KeyCode.NumPad8 },
            new object[] { KeyCode.NumPad9 }
        };

        /// <summary>
        /// These are the letter keys only.
        /// </summary>
        public static IEnumerable<object[]> LetterKeyData => new List<object[]>
        {
            new object[] { KeyCode.A },
            new object[] { KeyCode.B },
            new object[] { KeyCode.C },
            new object[] { KeyCode.D },
            new object[] { KeyCode.E },
            new object[] { KeyCode.F },
            new object[] { KeyCode.G },
            new object[] { KeyCode.H },
            new object[] { KeyCode.I },
            new object[] { KeyCode.J },
            new object[] { KeyCode.K },
            new object[] { KeyCode.L },
            new object[] { KeyCode.M },
            new object[] { KeyCode.N },
            new object[] { KeyCode.O },
            new object[] { KeyCode.P },
            new object[] { KeyCode.Q },
            new object[] { KeyCode.R },
            new object[] { KeyCode.S },
            new object[] { KeyCode.T },
            new object[] { KeyCode.U },
            new object[] { KeyCode.V },
            new object[] { KeyCode.W },
            new object[] { KeyCode.X },
            new object[] { KeyCode.Y },
            new object[] { KeyCode.Z },
            new object[] { KeyCode.Space }
        };

        /// <summary>
        /// These are the list of keys that return a symbol character when the shift is pressed down.
        /// </summary>
        public static IEnumerable<object[]> ShiftDownSymbolKeys => new List<object[]>
        {
            new object[] { KeyCode.OemPlus },
            new object[] { KeyCode.OemComma },
            new object[] { KeyCode.OemMinus },
            new object[] { KeyCode.OemPeriod },
            new object[] { KeyCode.OemQuestion },
            new object[] { KeyCode.OemTilde },
            new object[] { KeyCode.OemPipe },
            new object[] { KeyCode.OemOpenBrackets },
            new object[] { KeyCode.OemCloseBrackets },
            new object[] { KeyCode.OemQuotes },
            new object[] { KeyCode.OemSemicolon },
            new object[] { KeyCode.Divide },
            new object[] { KeyCode.Multiply },
            new object[] { KeyCode.Subtract },
            new object[] { KeyCode.Add },
            new object[] { KeyCode.D0 },
            new object[] { KeyCode.D1 },
            new object[] { KeyCode.D2 },
            new object[] { KeyCode.D3 },
            new object[] { KeyCode.D4 },
            new object[] { KeyCode.D5 },
            new object[] { KeyCode.D6 },
            new object[] { KeyCode.D7 },
            new object[] { KeyCode.D8 },
            new object[] { KeyCode.D9 }
        };

        /// <summary>
        /// These are the list of keys that return a symbol character when the shift is not pressed down.
        /// </summary>
        public static IEnumerable<object[]> ShiftUpSymbolKeys => new List<object[]>
        {
            new object[] { KeyCode.OemPlus },
            new object[] { KeyCode.OemComma },
            new object[] { KeyCode.OemMinus },
            new object[] { KeyCode.OemPeriod },
            new object[] { KeyCode.OemQuestion },
            new object[] { KeyCode.OemTilde },
            new object[] { KeyCode.OemPipe },
            new object[] { KeyCode.OemOpenBrackets },
            new object[] { KeyCode.OemCloseBrackets },
            new object[] { KeyCode.OemQuotes },
            new object[] { KeyCode.OemSemicolon },
            new object[] { KeyCode.Divide },
            new object[] { KeyCode.Multiply },
            new object[] { KeyCode.Subtract },
            new object[] { KeyCode.Add }
        };

        /// <summary>
        /// These are the symbol keys on the numpad.
        /// </summary>
        public static IEnumerable<object[]> NumpadSymbolKeyData => new List<object[]>
        {
            new object[] { KeyCode.Add },
            new object[] { KeyCode.Subtract },
            new object[] { KeyCode.Multiply },
            new object[] { KeyCode.Divide },
            new object[] { KeyCode.Decimal }
        };
        #endregion


        #region Public Methods
        public void Dispose()
        {
            _mockKeyboard = null;
        }
        #endregion
    }
}

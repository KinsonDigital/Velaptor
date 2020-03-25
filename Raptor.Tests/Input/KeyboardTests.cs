using Moq;
using Xunit;
using RaptorCore.Input;
using RaptorCore.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KDScorpionCoreTests.Input
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
                var keyCode = (KeyCodes)l[0];

                return keyCode == KeyCodes.Space ? ' ' : char.Parse(keyCode.ToString().ToLower());
            }).ToArray();

            _upperCaseLetters = LetterKeyData.Select(l =>
            {
                var keyCode = (KeyCodes)l[0];

                return keyCode == KeyCodes.Space ? ' ' : char.Parse(keyCode.ToString());
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
            var pressedKeys = new KeyCodes[] { KeyCodes.Left, KeyCodes.Z };

            _mockKeyboard.Setup(m => m.GetCurrentPressedKeys()).Returns(pressedKeys);

            var keyboard = new Keyboard(_mockKeyboard.Object);
            var expected = new KeyCodes[] { KeyCodes.Left, KeyCodes.Z };

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
            var pressedKeys = new KeyCodes[] { KeyCodes.Down, KeyCodes.U };
            _mockKeyboard.Setup(m => m.GetPreviousPressedKeys()).Returns(pressedKeys);

            var keyboard = new Keyboard(_mockKeyboard.Object);
            var expected = new KeyCodes[] { KeyCodes.Down, KeyCodes.U };

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
            _mockKeyboard.Setup(m => m.IsKeyDown(It.IsAny<KeyCodes>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyDown(KeyCodes.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyDown(It.IsAny<KeyCodes>()), Times.Once());
        }


        [Fact]
        public void IsKeyUp_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyUp(It.IsAny<KeyCodes>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyUp(KeyCodes.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyUp(It.IsAny<KeyCodes>()), Times.Once());
        }


        [Fact]
        public void IsKeyPressed_WhenInvoking_InternalMethodInvoked()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(It.IsAny<KeyCodes>())).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.IsKeyPressed(KeyCodes.A);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(It.IsAny<KeyCodes>()), Times.Once());
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
            keyboard.IsAnyKeyDown(new KeyCodes[] { KeyCodes.X, KeyCodes.Z });

            //Assert
            _mockKeyboard.Verify(m => m.AreKeysDown(It.IsAny<KeyCodes[]>()), Times.Once());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithNoParamsWithLetterKeyDown_ReturnsTrue(KeyCodes keyToCheck)
        {
            //Act
            _mockKeyboard.Setup(m => m.IsKeyPressed(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyLettersPressed());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithNoParamsWithLetterKeyNotDown_ReturnsFalse(KeyCodes keyToCheck)
        {
            //Act
            _mockKeyboard.Setup(m => m.IsKeyPressed(keyToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.False(keyboard.AnyLettersPressed());
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void AnyLettersPressed_WhenInvokingWithOneParamAndPressedLetterKey_InvokesKeyPressedAndReturnsPressedKeyValue(KeyCodes letterToCheck)
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
        public void AnyLettersPressed_WhenInvokingWithOneParamAndNoPressedLetterKeys_ReturnsFalse(KeyCodes letterToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(letterToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyLettersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(letterToCheck), Times.Once());
            Assert.Equal(KeyCodes.None, pressedKey);
            Assert.False(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithStandardNumberKeyIsPressed_ReturnsTrue(KeyCodes numToCheck)
        {
            //Arrange
            var numpadNumberKeys = (from k in NumpadNumberKeyData select (KeyCodes)k[0]).ToArray();

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
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithNumberKeysNotPressed_ReturnsFalse(KeyCodes numToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(It.IsAny<KeyCodes>())).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed();

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.False(actual);
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndWithNumpadNumberKeysNotPressed_ReturnsTrue(KeyCodes numToCheck)
        {
            //Arrange
            var standardKeys = (from k in StandardNumberKeyData select (KeyCodes)k[0]).ToArray();

            //Setup all of the standard number keys to return false
            standardKeys.ToList().ForEach(k => _mockKeyboard.Setup(m => m.IsKeyPressed(k)).Returns(false));

            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed();

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(It.IsAny<KeyCodes>()), Times.AtLeastOnce());
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyNumbersPressed_WhenInvokingWithoutParamAndStandardNumberKeyIsPressed_ReturnsTrue(KeyCodes numToCheck)
        {
            //Arrange
            var numpadKeys = (from k in NumpadNumberKeyData select (KeyCodes)k[0]).ToArray();

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
        public void AnyNumbersPressed_WhenInvokingWithParamAndNumpadNumberKeyIsPressed_ReturnsTrue(KeyCodes numToCheck)
        {
            //Arrange
            var standardKeys = (from k in StandardNumberKeyData select (KeyCodes)k[0]).ToArray();

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
        public void AnyNumbersPressed_WhenInvokingWithParamAndNoNumbersPressed_ReturnsFalse(KeyCodes numToCheck)
        {
            _mockKeyboard.Setup(m => m.IsKeyPressed(numToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.AnyNumbersPressed(out var pressedKey);

            //Assert
            _mockKeyboard.Verify(m => m.IsKeyPressed(numToCheck), Times.Once());
            Assert.False(actual);
            Assert.Equal(KeyCodes.None, pressedKey);
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void KeyToChar_WhenInvokingWithShiftDownAndLetterKeys_ReturnsCorrectLetterCharacter(KeyCodes keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCodes.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _lowerCaseLetters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(LetterKeyData))]
        public void KeyToChar_WhenInvokingWithShiftUpAndLetterKeys_ReturnsCorrectLetterCharacter(KeyCodes keyToCheck)
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
        public void KeyToChar_WhenInvokingWithShiftDownAndSymbolKeys_ReturnsCorrectSymbolCharacter(KeyCodes keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCodes.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = _numberAndSymbolCharacters.Contains(keyboard.KeyToChar(keyToCheck));

            //Assert
            Assert.True(actual);
        }


        [Theory]
        [MemberData(nameof(ShiftUpSymbolKeys))]
        public void KeyToChar_WhenInvokingWithShiftUpAndSymbolKeys_ReturnsCorrectSymbolCharacter(KeyCodes keyToCheck)
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
        public void KeyToChar_WhenInvokingWithShiftUpAndStandardNumberKeys_ReturnsCorrectNumberCharacter(KeyCodes keyToCheck)
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
        public void KeyToChar_WhenInvokingWithShiftUpAndNumpadNumberKeys_ReturnsCorrectNumberCharacter(KeyCodes keyToCheck)
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
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCodes.LeftShift)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.KeyToChar(KeyCodes.CapsLock);

            //Assert
            Assert.Equal((char)0, actual);
        }


        [Fact]
        public void KeyToChar_WhenInvokingWithShiftUpAndKeyWithNoCharacterValue_ReturnsTildeCharacter()
        {
            //Arrange
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act
            var actual = keyboard.KeyToChar(KeyCodes.CapsLock);

            //Assert
            Assert.Equal((char)0, actual);
        }


        [Fact]
        public void IsDeleteKeyPressed_WhenInvokedWithDeleteKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCodes.Delete)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsDeleteKeyPressed());
        }


        [Fact]
        public void IsDeleteKeyPressed_WhenInvokedWithNumpadDeleteKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(KeyCodes.LeftShift)).Returns(true);
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCodes.Decimal)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);


            //Assert
            Assert.True(keyboard.IsDeleteKeyPressed());
        }


        [Fact]
        public void IsBackspaceKeyPressed_WhenInvokedWithBackspaceKeyDown_ReturnsTrue()
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyPressed(KeyCodes.Back)).Returns(true);
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
        public void AnyStandardNumberKeysDown_WhenInvokedWithStandardKeyIsDown_ReturnsTrue(KeyCodes keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyStandardNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(StandardNumberKeyData))]
        public void AnyStandardNumberKeysDown_WhenInvokedWithStandardKeyNotDown_ReturnsFalse(KeyCodes keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(false);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.False(keyboard.AnyStandardNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumpadNumberKeysDown_WhenInvokedWithNumpadNumberKeyIsDown_ReturnsTrue(KeyCodes keyToCheck)
        {
            //Arrange
            _mockKeyboard.Setup(m => m.IsKeyDown(keyToCheck)).Returns(true);
            var keyboard = new Keyboard(_mockKeyboard.Object);

            //Act & Assert
            Assert.True(keyboard.AnyNumpadNumberKeysDown());
        }


        [Theory]
        [MemberData(nameof(NumpadNumberKeyData))]
        public void AnyNumpadNumberKeysDown_WhenInvokedWithNumpadNumberKeyNotDown_ReturnsFalse(KeyCodes keyToCheck)
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
            new object[] { KeyCodes.D0 },
            new object[] { KeyCodes.D1 },
            new object[] { KeyCodes.D2 },
            new object[] { KeyCodes.D3 },
            new object[] { KeyCodes.D4 },
            new object[] { KeyCodes.D5 },
            new object[] { KeyCodes.D6 },
            new object[] { KeyCodes.D7 },
            new object[] { KeyCodes.D8 },
            new object[] { KeyCodes.D9 }
        };

        /// <summary>
        /// These are tne number keys on the numpad.
        /// </summary>
        public static IEnumerable<object[]> NumpadNumberKeyData => new List<object[]>
        {
            new object[] { KeyCodes.NumPad0 },
            new object[] { KeyCodes.NumPad1 },
            new object[] { KeyCodes.NumPad2 },
            new object[] { KeyCodes.NumPad3 },
            new object[] { KeyCodes.NumPad4 },
            new object[] { KeyCodes.NumPad5 },
            new object[] { KeyCodes.NumPad6 },
            new object[] { KeyCodes.NumPad7 },
            new object[] { KeyCodes.NumPad8 },
            new object[] { KeyCodes.NumPad9 }
        };

        /// <summary>
        /// These are the letter keys only.
        /// </summary>
        public static IEnumerable<object[]> LetterKeyData => new List<object[]>
        {
            new object[] { KeyCodes.A },
            new object[] { KeyCodes.B },
            new object[] { KeyCodes.C },
            new object[] { KeyCodes.D },
            new object[] { KeyCodes.E },
            new object[] { KeyCodes.F },
            new object[] { KeyCodes.G },
            new object[] { KeyCodes.H },
            new object[] { KeyCodes.I },
            new object[] { KeyCodes.J },
            new object[] { KeyCodes.K },
            new object[] { KeyCodes.L },
            new object[] { KeyCodes.M },
            new object[] { KeyCodes.N },
            new object[] { KeyCodes.O },
            new object[] { KeyCodes.P },
            new object[] { KeyCodes.Q },
            new object[] { KeyCodes.R },
            new object[] { KeyCodes.S },
            new object[] { KeyCodes.T },
            new object[] { KeyCodes.U },
            new object[] { KeyCodes.V },
            new object[] { KeyCodes.W },
            new object[] { KeyCodes.X },
            new object[] { KeyCodes.Y },
            new object[] { KeyCodes.Z },
            new object[] { KeyCodes.Space }
        };

        /// <summary>
        /// These are the list of keys that return a symbol character when the shift is pressed down.
        /// </summary>
        public static IEnumerable<object[]> ShiftDownSymbolKeys => new List<object[]>
        {
            new object[] { KeyCodes.OemPlus },
            new object[] { KeyCodes.OemComma },
            new object[] { KeyCodes.OemMinus },
            new object[] { KeyCodes.OemPeriod },
            new object[] { KeyCodes.OemQuestion },
            new object[] { KeyCodes.OemTilde },
            new object[] { KeyCodes.OemPipe },
            new object[] { KeyCodes.OemOpenBrackets },
            new object[] { KeyCodes.OemCloseBrackets },
            new object[] { KeyCodes.OemQuotes },
            new object[] { KeyCodes.OemSemicolon },
            new object[] { KeyCodes.Divide },
            new object[] { KeyCodes.Multiply },
            new object[] { KeyCodes.Subtract },
            new object[] { KeyCodes.Add },
            new object[] { KeyCodes.D0 },
            new object[] { KeyCodes.D1 },
            new object[] { KeyCodes.D2 },
            new object[] { KeyCodes.D3 },
            new object[] { KeyCodes.D4 },
            new object[] { KeyCodes.D5 },
            new object[] { KeyCodes.D6 },
            new object[] { KeyCodes.D7 },
            new object[] { KeyCodes.D8 },
            new object[] { KeyCodes.D9 }
        };

        /// <summary>
        /// These are the list of keys that return a symbol character when the shift is not pressed down.
        /// </summary>
        public static IEnumerable<object[]> ShiftUpSymbolKeys => new List<object[]>
        {
            new object[] { KeyCodes.OemPlus },
            new object[] { KeyCodes.OemComma },
            new object[] { KeyCodes.OemMinus },
            new object[] { KeyCodes.OemPeriod },
            new object[] { KeyCodes.OemQuestion },
            new object[] { KeyCodes.OemTilde },
            new object[] { KeyCodes.OemPipe },
            new object[] { KeyCodes.OemOpenBrackets },
            new object[] { KeyCodes.OemCloseBrackets },
            new object[] { KeyCodes.OemQuotes },
            new object[] { KeyCodes.OemSemicolon },
            new object[] { KeyCodes.Divide },
            new object[] { KeyCodes.Multiply },
            new object[] { KeyCodes.Subtract },
            new object[] { KeyCodes.Add }
        };

        /// <summary>
        /// These are the symbol keys on the numpad.
        /// </summary>
        public static IEnumerable<object[]> NumpadSymbolKeyData => new List<object[]>
        {
            new object[] { KeyCodes.Add },
            new object[] { KeyCodes.Subtract },
            new object[] { KeyCodes.Multiply },
            new object[] { KeyCodes.Divide },
            new object[] { KeyCodes.Decimal }
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

using RaptorCore.Plugins;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace RaptorCore.Input
{
    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public class Keyboard
    {
        #region Pivate Fields
        /// <summary>
        /// The letter keys including the space bar.
        /// </summary>
        private static readonly KeyCodes[] _letterKeys = new[]
        {
            KeyCodes.A, KeyCodes.B, KeyCodes.C, KeyCodes.D, KeyCodes.E,
            KeyCodes.F, KeyCodes.G, KeyCodes.H, KeyCodes.I, KeyCodes.J,
            KeyCodes.K,KeyCodes.L, KeyCodes.M, KeyCodes.N, KeyCodes.O,
            KeyCodes.P, KeyCodes.Q, KeyCodes.R, KeyCodes.S, KeyCodes.T,
            KeyCodes.U, KeyCodes.V, KeyCodes.W, KeyCodes.X, KeyCodes.Y,
            KeyCodes.Z, KeyCodes.Space
        };

        /// <summary>
        /// The standard number keys above the letter keys.
        /// </summary>
        private static readonly KeyCodes[] _standardNumberKeys = new[]
        {
            KeyCodes.D0, KeyCodes.D1, KeyCodes.D2,
            KeyCodes.D3, KeyCodes.D4, KeyCodes.D5,
            KeyCodes.D6, KeyCodes.D7, KeyCodes.D8,
            KeyCodes.D9
        };

        /// <summary>
        /// The number keys on the numpad.
        /// </summary>
        private static readonly KeyCodes[] _numpadNumberKeys = new[]
        {
            KeyCodes.NumPad0, KeyCodes.NumPad1, KeyCodes.NumPad2,
            KeyCodes.NumPad3, KeyCodes.NumPad4, KeyCodes.NumPad5,
            KeyCodes.NumPad6, KeyCodes.NumPad7, KeyCodes.NumPad8,
            KeyCodes.NumPad9,
        };

        /// <summary>
        /// The symbol keys.
        /// </summary>
        private static readonly KeyCodes[] _symbolKeys = new[]
        {
            KeyCodes.OemSemicolon, KeyCodes.OemPlus, KeyCodes.OemComma,
            KeyCodes.OemMinus, KeyCodes.OemPeriod, KeyCodes.OemQuestion,
            KeyCodes.OemTilde, KeyCodes.OemOpenBrackets, KeyCodes.OemPipe,
            KeyCodes.OemCloseBrackets, KeyCodes.OemQuotes, KeyCodes.Decimal,
            KeyCodes.Divide, KeyCodes.Multiply, KeyCodes.Subtract, KeyCodes.Add
        };

        /// <summary>
        /// The characters produced by the standard number keys when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCodes, char> _noShiftStandardNumberCharacters = new Dictionary<KeyCodes, char>()
        {
            { KeyCodes.D0 , '0'}, { KeyCodes.D1 , '1'}, { KeyCodes.D2 , '2'},
            { KeyCodes.D3 , '3'}, { KeyCodes.D4 , '4'}, { KeyCodes.D5 , '5'},
            { KeyCodes.D6 , '6'}, { KeyCodes.D7 , '7'}, { KeyCodes.D8 , '8'}, { KeyCodes.D9 , '9'},
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCodes, char> _noShiftSymbolCharacters = new Dictionary<KeyCodes, char>()
        {
            { KeyCodes.OemPlus, '=' }, { KeyCodes.OemComma, ',' }, { KeyCodes.OemMinus, '-' },
            { KeyCodes.OemPeriod, '.' }, { KeyCodes.OemQuestion, '/' }, { KeyCodes.OemTilde, '`' },
            { KeyCodes.OemPipe, '\\' }, { KeyCodes.OemOpenBrackets, '[' }, { KeyCodes.OemCloseBrackets, ']' },
            { KeyCodes.OemQuotes, '\'' }, { KeyCodes.OemSemicolon, ';' }, { KeyCodes.Decimal, '.' },
            { KeyCodes.Divide, '/' }, { KeyCodes.Multiply, '*' }, { KeyCodes.Subtract, '-' },
            { KeyCodes.Add, '+' }
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when any shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCodes, char> _withShiftSymbolCharacters = new Dictionary<KeyCodes, char>()
        {
            { KeyCodes.OemPlus, '+' }, { KeyCodes.OemComma, '<' }, { KeyCodes.OemMinus, '_' },
            { KeyCodes.OemPeriod, '>' }, { KeyCodes.OemQuestion, '?' }, { KeyCodes.OemTilde, '~' },
            { KeyCodes.OemPipe, '|' }, { KeyCodes.OemOpenBrackets, '{' }, { KeyCodes.OemCloseBrackets, '}' },
            { KeyCodes.OemQuotes, '\'' }, { KeyCodes.OemSemicolon, ':' }, { KeyCodes.D1, '!' },
            { KeyCodes.D2, '@' }, { KeyCodes.D3, '#' }, { KeyCodes.D4, '$' }, { KeyCodes.D5, '%' },
            { KeyCodes.D6, '^' }, { KeyCodes.D7, '&' }, { KeyCodes.D8, '*' }, { KeyCodes.D9, '(' },
            { KeyCodes.D0, ')' }, { KeyCodes.Divide, '/' }, { KeyCodes.Multiply, '*' }, { KeyCodes.Subtract, '-' },
            { KeyCodes.Add, '+' }
        };
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Keyboard"/>.
        /// USED FOR UNIT TESTING.
        /// </summary>
        /// <param name="mockedKeyboard">The mocked keyboard to inject.</param>
        internal Keyboard(IKeyboard mockedKeyboard) => InternalKeyboard = mockedKeyboard;


        /// <summary>
        /// Creates a new instance of <see cref="Keyboard"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        //TODO: Figure out how to get the proper implementation inside of this class
        public Keyboard() { }
        #endregion


        #region Props
        /// <summary>
        /// The internal keyboard plugin implementation.
        /// </summary>
        internal IKeyboard InternalKeyboard { get; }

        /// <summary>
        /// Gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn => InternalKeyboard.CapsLockOn;

        /// <summary>
        /// Gets a value indicating if the numlock key is on.
        /// </summary>
        public bool NumLockOn => InternalKeyboard.NumLockOn;

        /// <summary>
        /// Gets a value indicating if the left shift key is being pressed down.
        /// </summary>
        public bool IsLeftShiftDown => InternalKeyboard.IsLeftShiftDown;

        /// <summary>
        /// Gets a value indicating if the right shift key is being pressed down.
        /// </summary>
        public bool IsRightShiftDown => InternalKeyboard.IsRightShiftDown;

        /// <summary>
        /// Gets a value indicating if the left control key is being pressed down.
        /// </summary>
        public bool IsLeftCtrlDown => InternalKeyboard.IsLeftCtrlDown;

        /// <summary>
        /// Gets a value indicating if the right control key is being pressed down.
        /// </summary>
        public bool IsRightCtrlDown => InternalKeyboard.IsRightCtrlDown;

        /// <summary>
        /// Gets a value indicating if the left alt key is being pressed down.
        /// </summary>
        public bool IsLeftAltDown => InternalKeyboard.IsLeftAltDown;

        /// <summary>
        /// Gets a value indicating if the right alt key is being pressed down.
        /// </summary>
        public bool IsRightAltDown => InternalKeyboard.IsRightAltDown;
        #endregion


        #region Public Methods
        /// <summary>
        /// Returns all of the currently pressed keys on the keyboard.
        /// </summary>
        /// <returns></returns>
        public KeyCodes[] GetCurrentPressedKeys() => (from k in InternalKeyboard.GetCurrentPressedKeys()
                                                      select k).ToArray();


        /// <summary>
        /// Returns all of the previously pressed keys from the last check.
        /// </summary>
        /// <returns></returns>
        public KeyCodes[] GetPreviousPressedKeys() => (from k in InternalKeyboard.GetPreviousPressedKeys()
                                                       select k).ToArray();


        /// <summary>
        /// Gets a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyKeysDown() => InternalKeyboard.AreAnyKeysDown();


        /// <summary>
        /// Returns a value indicating if any of the given key are in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns></returns>
        public bool IsAnyKeyDown(KeyCodes[] keys) => InternalKeyboard.AreKeysDown(keys);


        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyDown(KeyCodes key) => InternalKeyboard.IsKeyDown(key);


        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyUp(KeyCodes key) => InternalKeyboard.IsKeyUp(key);


        /// <summary>
        /// Returns true if the given key has been pressed to the down position then let go
        /// to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyPressed(KeyCodes key) => InternalKeyboard.IsKeyPressed(key);


        /// <summary>
        /// Returns a value indicating if any letter keys have been fully pressed down then let go.
        /// </summary>
        /// <returns></returns>
        public bool AnyLettersPressed()
        {
            for (int i = 0; i < _letterKeys.Length; i++)
            {
                if (IsKeyPressed(_letterKeys[i]))
                    return true;
            }


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any letter keys were pressed down and then let go, and returns which letter was pressed
        /// using the out parameter.
        /// </summary>
        /// <param name="letterKey">The letter key that was pressed if found.</param>
        /// <returns></returns>
        public bool AnyLettersPressed(out KeyCodes letterKey)
        {
            for (int i = 0; i < _letterKeys.Length; i++)
            {
                if (InternalKeyboard.IsKeyPressed(_letterKeys[i]))
                {
                    letterKey = _letterKeys[i];
                    return true;
                }
            }

            letterKey = KeyCodes.None;


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any of the standard number keys
        /// above the letter keys on the keyboard are in the down position.
        /// </summary>
        /// <returns></returns>
        public bool AnyStandardNumberKeysDown()
        {
            //Check all of the standard number keys
            foreach (var key in _standardNumberKeys)
            {
                if (IsKeyDown(key))
                    return true;
            }


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any of the numpad number keys
        /// are in the down position.
        /// </summary>
        /// <returns></returns>
        public bool AnyNumpadNumberKeysDown()
        {
            //Check all of the numpad number keys
            foreach (var key in _numpadNumberKeys)
            {
                if (IsKeyDown(key))
                    return true;
            }


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any of the standard number keys
        /// above the letter keys on the keyboard have been pressed fully
        /// to the down position then let go to the up position.
        /// </summary>
        /// <returns></returns>
        public bool AnyStandardNumberKeysPressed()
        {
            //Check all of the standard number keys
            foreach (var key in _standardNumberKeys)
            {
                if (IsKeyPressed(key))
                    return true;
            }


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any of the numpad number keys
        /// are have been pressed fully to the down position then
        /// let go to the up position.
        /// </summary>
        /// <returns></returns>
        public bool AnyNumpadNumberKeysPressed()
        {
            //Check all of the numpad number keys
            foreach (var key in _numpadNumberKeys)
            {
                if (IsKeyPressed(key))
                    return true;
            }


            return false;
        }


        /// <summary>
        /// Returns a value indicating if any number keys were pressed down fully 
        /// to the down position then let go to the up position.
        /// </summary>
        /// <returns></returns>
        public bool AnyNumbersPressed() => AnyStandardNumberKeysPressed() || AnyNumpadNumberKeysPressed();


        /// <summary>
        /// Returns a value indicating if number keys were pressed down fully to the
        /// down position then let go to the up position.
        /// </summary>
        /// <param name="symbolKey">The number key that was pressed if found.</param>
        /// <returns></returns>
        public bool AnyNumbersPressed(out KeyCodes numberKey)
        {
            //Check standard number keys
            for (int i = 0; i < _standardNumberKeys.Length; i++)
            {
                if (InternalKeyboard.IsKeyPressed(_standardNumberKeys[i]))
                {
                    numberKey = _standardNumberKeys[i];

                    return true;
                }
            }

            //Check numpad number keys
            for (int i = 0; i < _numpadNumberKeys.Length; i++)
            {
                if (InternalKeyboard.IsKeyPressed(_numpadNumberKeys[i]))
                {
                    numberKey = _numpadNumberKeys[i];

                    return true;
                }
            }

            numberKey = KeyCodes.None;


            return false;
        }


        /// <summary>
        /// Returns the character equivalent of the given key if it was
        /// a letter, number or symbol key.  The value of 0 will be returned if not
        /// a letter, number or symbol.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public char KeyToChar(KeyCodes key)
        {
            if (IsAnyShiftKeyDown())
            {
                if (_letterKeys.Contains(key))
                {
                    return key == KeyCodes.Space ? ' ' : key.ToString().ToLower()[0];
                }
                else if (_standardNumberKeys.Contains(key) || _symbolKeys.Contains(key))
                {
                    //When the shift key is down, the standard number keys and symbol keys return symbols.
                    return _withShiftSymbolCharacters[key];
                }
            }
            else
            {
                if (_letterKeys.Contains(key))
                {
                    return key.ToString()[0];
                }
                else if (_symbolKeys.Contains(key))
                {
                    return _noShiftSymbolCharacters[key];
                }
                else if (_standardNumberKeys.Contains(key))
                {
                    //When the shift is is up, the standard number keys return numbers.
                    return _noShiftStandardNumberCharacters[key];
                }
            }


            return (char)0;
        }


        /// <summary>
        /// Returns a value indicating if any of the shift keys are being pressed down.
        /// </summary>
        /// <returns></returns>
        public bool IsAnyShiftKeyDown() =>
            InternalKeyboard.IsKeyDown(KeyCodes.LeftShift) || InternalKeyboard.IsKeyDown(KeyCodes.RightShift);


        /// <summary>
        /// Returns a value indicating if any of the control keys are being pressed down.
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCtrlKeyDown() => InternalKeyboard.IsLeftCtrlDown || InternalKeyboard.IsRightCtrlDown;


        /// <summary>
        /// Returns a value indicating if the any of the delete keys have been fully pressed.
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteKeyPressed() =>
            IsKeyPressed(KeyCodes.Delete) || (IsAnyShiftKeyDown() && IsKeyPressed(KeyCodes.Decimal));


        /// <summary>
        /// Returns a value indicating if the backspace key has been fully
        /// pressed to the down position then let go to the up position.
        /// </summary>
        /// <returns></returns>
        public bool IsBackspaceKeyPressed() => IsKeyPressed(KeyCodes.Back);


        /// <summary>
        /// Update the current state of the keyboard.
        /// </summary>
        public void UpdateCurrentState() => InternalKeyboard.UpdateCurrentState();


        /// <summary>
        /// Update the previous state of the keyboard.
        /// </summary>
        public void UpdatePreviousState() => InternalKeyboard.UpdatePreviousState();
        #endregion
    }
}

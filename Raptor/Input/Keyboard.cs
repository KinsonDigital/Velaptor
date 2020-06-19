using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Raptor.Input
{
    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public class Keyboard
    {
        #region Private Fields
        /// <summary>
        /// The letter keys including the space bar.
        /// </summary>
        private static readonly KeyCode[] _letterKeys = new[]
        {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K,KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z, KeyCode.Space
        };

        /// <summary>
        /// The standard number keys above the letter keys.
        /// </summary>
        private static readonly KeyCode[] _standardNumberKeys = new[]
        {
            KeyCode.D0, KeyCode.D1, KeyCode.D2,
            KeyCode.D3, KeyCode.D4, KeyCode.D5,
            KeyCode.D6, KeyCode.D7, KeyCode.D8,
            KeyCode.D9
        };

        /// <summary>
        /// The number keys on the numpad.
        /// </summary>
        private static readonly KeyCode[] _numpadNumberKeys = new[]
        {
            KeyCode.NumPad0, KeyCode.NumPad1, KeyCode.NumPad2,
            KeyCode.NumPad3, KeyCode.NumPad4, KeyCode.NumPad5,
            KeyCode.NumPad6, KeyCode.NumPad7, KeyCode.NumPad8,
            KeyCode.NumPad9,
        };

        /// <summary>
        /// The symbol keys.
        /// </summary>
        private static readonly KeyCode[] _symbolKeys = new[]
        {
            KeyCode.OemSemicolon, KeyCode.OemPlus, KeyCode.OemComma,
            KeyCode.OemMinus, KeyCode.OemPeriod, KeyCode.OemQuestion,
            KeyCode.OemTilde, KeyCode.OemOpenBrackets, KeyCode.OemPipe,
            KeyCode.OemCloseBrackets, KeyCode.OemQuotes, KeyCode.Decimal,
            KeyCode.Divide, KeyCode.Multiply, KeyCode.Subtract, KeyCode.Add
        };

        /// <summary>
        /// The characters produced by the standard number keys when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> _noShiftStandardNumberCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.D0 , '0'}, { KeyCode.D1 , '1'}, { KeyCode.D2 , '2'},
            { KeyCode.D3 , '3'}, { KeyCode.D4 , '4'}, { KeyCode.D5 , '5'},
            { KeyCode.D6 , '6'}, { KeyCode.D7 , '7'}, { KeyCode.D8 , '8'}, { KeyCode.D9 , '9'},
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> _noShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.OemPlus, '=' }, { KeyCode.OemComma, ',' }, { KeyCode.OemMinus, '-' },
            { KeyCode.OemPeriod, '.' }, { KeyCode.OemQuestion, '/' }, { KeyCode.OemTilde, '`' },
            { KeyCode.OemPipe, '\\' }, { KeyCode.OemOpenBrackets, '[' }, { KeyCode.OemCloseBrackets, ']' },
            { KeyCode.OemQuotes, '\'' }, { KeyCode.OemSemicolon, ';' }, { KeyCode.Decimal, '.' },
            { KeyCode.Divide, '/' }, { KeyCode.Multiply, '*' }, { KeyCode.Subtract, '-' },
            { KeyCode.Add, '+' }
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when any shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> _withShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.OemPlus, '+' }, { KeyCode.OemComma, '<' }, { KeyCode.OemMinus, '_' },
            { KeyCode.OemPeriod, '>' }, { KeyCode.OemQuestion, '?' }, { KeyCode.OemTilde, '~' },
            { KeyCode.OemPipe, '|' }, { KeyCode.OemOpenBrackets, '{' }, { KeyCode.OemCloseBrackets, '}' },
            { KeyCode.OemQuotes, '\'' }, { KeyCode.OemSemicolon, ':' }, { KeyCode.D1, '!' },
            { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
            { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' },
            { KeyCode.D0, ')' }, { KeyCode.Divide, '/' }, { KeyCode.Multiply, '*' }, { KeyCode.Subtract, '-' },
            { KeyCode.Add, '+' }
        };
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Keyboard"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Keyboard() { }
        #endregion


        #region Props
        /// <summary>
        /// Gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn { get; set; }

        /// <summary>
        /// Gets a value indicating if the numlock key is on.
        /// </summary>
        public bool NumLockOn { get; set; }

        /// <summary>
        /// Gets a value indicating if the left shift key is being pressed down.
        /// </summary>
        public bool IsLeftShiftDown { get; set; }

        /// <summary>
        /// Gets a value indicating if the right shift key is being pressed down.
        /// </summary>
        public bool IsRightShiftDown { get; set; }

        /// <summary>
        /// Gets a value indicating if the left control key is being pressed down.
        /// </summary>
        public bool IsLeftCtrlDown { get; set; }

        /// <summary>
        /// Gets a value indicating if the right control key is being pressed down.
        /// </summary>
        public bool IsRightCtrlDown { get; set; }

        /// <summary>
        /// Gets a value indicating if the left alt key is being pressed down.
        /// </summary>
        public bool IsLeftAltDown { get; set; }

        /// <summary>
        /// Gets a value indicating if the right alt key is being pressed down.
        /// </summary>
        public bool IsRightAltDown { get; set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Returns all of the currently pressed keys on the keyboard.
        /// </summary>
        /// <returns></returns>
        public KeyCode[] GetCurrentPressedKeys()
        {
            throw new NotImplementedException();
            //return (from k in InternalKeyboard.GetCurrentPressedKeys()
            //        select k).ToArray();
        }


        /// <summary>
        /// Returns all of the previously pressed keys from the last check.
        /// </summary>
        /// <returns></returns>
        public KeyCode[] GetPreviousPressedKeys()
        {
            throw new NotImplementedException();
            //return (from k in InternalKeyboard.GetPreviousPressedKeys()
            //        select k).ToArray();
        }


        /// <summary>
        /// Gets a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyKeysDown() { throw new NotImplementedException(); }


        /// <summary>
        /// Returns a value indicating if any of the given key are in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns></returns>
        public bool IsAnyKeyDown(KeyCode[] keys) { throw new NotImplementedException(); }


        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyDown(KeyCode key) { throw new NotImplementedException(); }


        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyUp(KeyCode key) { throw new NotImplementedException(); }


        /// <summary>
        /// Returns true if the given key has been pressed to the down position then let go
        /// to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        public bool IsKeyPressed(KeyCode key) { throw new NotImplementedException(); }


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
        public bool AnyLettersPressed(out KeyCode letterKey)
        {
            letterKey = KeyCode.None;

            //for (int i = 0; i < _letterKeys.Length; i++)
            //{
            //    if (InternalKeyboard.IsKeyPressed(_letterKeys[i]))
            //    {
            //        letterKey = _letterKeys[i];
            //        return true;
            //    }
            //}


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
        public bool AnyNumbersPressed(out KeyCode numberKey)
        {
            numberKey = KeyCode.None;

            ////Check standard number keys
            //for (int i = 0; i < _standardNumberKeys.Length; i++)
            //{
            //    if (InternalKeyboard.IsKeyPressed(_standardNumberKeys[i]))
            //    {
            //        numberKey = _standardNumberKeys[i];

            //        return true;
            //    }
            //}

            ////Check numpad number keys
            //for (int i = 0; i < _numpadNumberKeys.Length; i++)
            //{
            //    if (InternalKeyboard.IsKeyPressed(_numpadNumberKeys[i]))
            //    {
            //        numberKey = _numpadNumberKeys[i];

            //        return true;
            //    }
            //}


            return false;
        }



        /// <summary>
        /// Returns the character equivalent of the given key if it was
        /// a letter, number or symbol key.  The value of 0 will be returned if not
        /// a letter, number or symbol.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns></returns>
        [SuppressMessage("Globalization", "CA1304:Specify CultureInfo", Justification = "<Pending>")]
        public char KeyToChar(KeyCode key)
        {
            if (IsAnyShiftKeyDown())
            {
                if (_letterKeys.Contains(key))
                {
                    return key == KeyCode.Space ? ' ' : key.ToString().ToLower()[0];
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
            
        public bool IsAnyShiftKeyDown() { throw new NotImplementedException(); }


        /// <summary>
        /// Returns a value indicating if any of the control keys are being pressed down.
        /// </summary>
        /// <returns></returns>
        public bool IsAnyCtrlKeyDown() { throw new NotImplementedException(); }


        /// <summary>
        /// Returns a value indicating if the any of the delete keys have been fully pressed.
        /// </summary>
        /// <returns></returns>
        public bool IsDeleteKeyPressed() { throw new NotImplementedException(); }


        /// <summary>
        /// Returns a value indicating if the backspace key has been fully
        /// pressed to the down position then let go to the up position.
        /// </summary>
        /// <returns></returns>
        public bool IsBackspaceKeyPressed() => IsKeyPressed(KeyCode.Back);


        /// <summary>
        /// Update the current state of the keyboard.
        /// </summary>
        public void UpdateCurrentState()
        {
            //InternalKeyboard.UpdateCurrentState();
        }


        /// <summary>
        /// Update the previous state of the keyboard.
        /// </summary>
        public void UpdatePreviousState()
        {
            //InternalKeyboard.UpdatePreviousState();
        }
        #endregion
    }
}

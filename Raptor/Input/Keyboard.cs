// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public class Keyboard
    {
        /// <summary>
        /// The letter keys including the space bar.
        /// </summary>
        private static readonly KeyCode[] LetterKeys = new[]
        {
            KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E,
            KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J,
            KeyCode.K, KeyCode.L, KeyCode.M, KeyCode.N, KeyCode.O,
            KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T,
            KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y,
            KeyCode.Z, KeyCode.Space,
        };

        /// <summary>
        /// The standard number keys above the letter keys.
        /// </summary>
        private static readonly KeyCode[] StandardNumberKeys = new[]
        {
            KeyCode.D0, KeyCode.D1, KeyCode.D2,
            KeyCode.D3, KeyCode.D4, KeyCode.D5,
            KeyCode.D6, KeyCode.D7, KeyCode.D8,
            KeyCode.D9,
        };

        /// <summary>
        /// The number keys on the numpad.
        /// </summary>
        private static readonly KeyCode[] NumpadNumberKeys = new[]
        {
            KeyCode.NumPad0, KeyCode.NumPad1, KeyCode.NumPad2,
            KeyCode.NumPad3, KeyCode.NumPad4, KeyCode.NumPad5,
            KeyCode.NumPad6, KeyCode.NumPad7, KeyCode.NumPad8,
            KeyCode.NumPad9,
        };

        /// <summary>
        /// The symbol keys.
        /// </summary>
        private static readonly KeyCode[] SymbolKeys = new[]
        {
            KeyCode.OemSemicolon, KeyCode.OemPlus, KeyCode.OemComma,
            KeyCode.OemMinus, KeyCode.OemPeriod, KeyCode.OemQuestion,
            KeyCode.OemTilde, KeyCode.OemOpenBrackets, KeyCode.OemPipe,
            KeyCode.OemCloseBrackets, KeyCode.OemQuotes, KeyCode.Decimal,
            KeyCode.Divide, KeyCode.Multiply, KeyCode.Subtract, KeyCode.Add,
        };

        /// <summary>
        /// The characters produced by the standard number keys when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> NoShiftStandardNumberCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.D0, '0' }, { KeyCode.D1, '1' }, { KeyCode.D2, '2' },
            { KeyCode.D3, '3' }, { KeyCode.D4, '4' }, { KeyCode.D5, '5' },
            { KeyCode.D6, '6' }, { KeyCode.D7, '7' }, { KeyCode.D8, '8' }, { KeyCode.D9, '9' },
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when no shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> NoShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.OemPlus, '=' }, { KeyCode.OemComma, ',' }, { KeyCode.OemMinus, '-' },
            { KeyCode.OemPeriod, '.' }, { KeyCode.OemQuestion, '/' }, { KeyCode.OemTilde, '`' },
            { KeyCode.OemPipe, '\\' }, { KeyCode.OemOpenBrackets, '[' }, { KeyCode.OemCloseBrackets, ']' },
            { KeyCode.OemQuotes, '\'' }, { KeyCode.OemSemicolon, ';' }, { KeyCode.Decimal, '.' },
            { KeyCode.Divide, '/' }, { KeyCode.Multiply, '*' }, { KeyCode.Subtract, '-' },
            { KeyCode.Add, '+' },
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when any shift modifier keys are pressed.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> WithShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.OemPlus, '+' }, { KeyCode.OemComma, '<' }, { KeyCode.OemMinus, '_' },
            { KeyCode.OemPeriod, '>' }, { KeyCode.OemQuestion, '?' }, { KeyCode.OemTilde, '~' },
            { KeyCode.OemPipe, '|' }, { KeyCode.OemOpenBrackets, '{' }, { KeyCode.OemCloseBrackets, '}' },
            { KeyCode.OemQuotes, '\'' }, { KeyCode.OemSemicolon, ':' }, { KeyCode.D1, '!' },
            { KeyCode.D2, '@' }, { KeyCode.D3, '#' }, { KeyCode.D4, '$' }, { KeyCode.D5, '%' },
            { KeyCode.D6, '^' }, { KeyCode.D7, '&' }, { KeyCode.D8, '*' }, { KeyCode.D9, '(' },
            { KeyCode.D0, ')' }, { KeyCode.Divide, '/' }, { KeyCode.Multiply, '*' }, { KeyCode.Subtract, '-' },
            { KeyCode.Add, '+' },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="Keyboard"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public Keyboard()
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the numlock key is on.
        /// </summary>
        public bool NumLockOn { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the left shift key is being pressed down.
        /// </summary>
        public bool IsLeftShiftDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the right shift key is being pressed down.
        /// </summary>
        public bool IsRightShiftDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the left control key is being pressed down.
        /// </summary>
        public bool IsLeftCtrlDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the right control key is being pressed down.
        /// </summary>
        public bool IsRightCtrlDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the left alt key is being pressed down.
        /// </summary>
        public bool IsLeftAltDown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether gets a value indicating if the right alt key is being pressed down.
        /// </summary>
        public bool IsRightAltDown { get; set; }

        /// <summary>
        /// Returns all of the currently pressed keys on the keyboard.
        /// </summary>
        /// <returns>A list of the currently pressed keys.</returns>
        public KeyCode[] GetCurrentPressedKeys() => throw new NotImplementedException(); // return (from k in InternalKeyboard.GetCurrentPressedKeys()//        select k).ToArray();

        /// <summary>
        /// Returns all of the previously pressed keys from the last check.
        /// </summary>
        /// <returns>A list of previously pressed keys.</returns>
        public KeyCode[] GetPreviousPressedKeys() => throw new NotImplementedException(); // return (from k in InternalKeyboard.GetPreviousPressedKeys()//        select k).ToArray();

        /// <summary>
        /// Gets a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns>True if any keys on the keyboard in pressed in the down position.</returns>
        public bool AreAnyKeysDown() => throw new NotImplementedException();

        /// <summary>
        /// Returns a value indicating if any of the given <paramref name="keys"/> are in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns>True if any of the given <paramref name="keys"/> are pressed in the down position.</returns>
        public bool IsAnyKeyDown(KeyCode[] keys) => throw new NotImplementedException();

        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given <paramref name="key"/> is pressed in the down position.</returns>
        public bool IsKeyDown(KeyCode key) => throw new NotImplementedException();

        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the givne <paramref name="key"/> is in the up position.</returns>
        public bool IsKeyUp(KeyCode key) => throw new NotImplementedException();

        /// <summary>
        /// Returns true if the given key has been pressed to the down position then let go
        /// to the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given <paramref name="key"/> has been pressed then released.</returns>
        public bool IsKeyPressed(KeyCode key) => throw new NotImplementedException();

        /// <summary>
        /// Returns a value indicating if any letter keys have been fully pressed down then let go.
        /// </summary>
        /// <returns>True if any letters are pressed in the down position.</returns>
        public bool AnyLettersPressed()
        {
            for (var i = 0; i < LetterKeys.Length; i++)
            {
                if (IsKeyPressed(LetterKeys[i]))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns a value indicating if any of the standard number keys
        /// above the letter keys on the keyboard are in the down position.
        /// </summary>
        /// <returns>True if any of the standard number keys are in the down position.</returns>
        public bool AnyStandardNumberKeysDown()
        {
            // Check all of the standard number keys
            foreach (var key in StandardNumberKeys)
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
        /// <returns>True if any of the numpad number keys are in the down position.</returns>
        public bool AnyNumpadNumberKeysDown()
        {
            // Check all of the numpad number keys
            foreach (var key in NumpadNumberKeys)
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
        /// <returns>True if any of the standard number keys have been pressed.</returns>
        public bool AnyStandardNumberKeysPressed()
        {
            // Check all of the standard number keys
            foreach (var key in StandardNumberKeys)
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
        /// <returns>True if any of the numpad number keys have been pressed.</returns>
        public bool AnyNumpadNumberKeysPressed()
        {
            // Check all of the numpad number keys
            foreach (var key in NumpadNumberKeys)
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
        /// <returns>True if any of the standard or numpad keys have been pressed.</returns>
        public bool AnyNumbersPressed() => AnyStandardNumberKeysPressed() || AnyNumpadNumberKeysPressed();

        /// <summary>
        /// Returns the character equivalent of the given key if it was
        /// a letter, number or symbol key.  The value of 0 will be returned if not
        /// a letter, number or symbol.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>The character that matches the given key.</returns>
        public char KeyToChar(KeyCode key)
        {
            if (IsAnyShiftKeyDown())
            {
                if (LetterKeys.Contains(key))
                {
                    // NOTE!!  THIS MIGHT NOW WORK.  THE TOUPPERINVARIANT() THAT IS
                    return key == KeyCode.Space ? ' ' : key.ToString().ToUpperInvariant()[0];
                }
                else if (StandardNumberKeys.Contains(key) || SymbolKeys.Contains(key))
                {
                    // When the shift key is down, the standard number keys and symbol keys return symbols.
                    return WithShiftSymbolCharacters[key];
                }
            }
            else
            {
                if (LetterKeys.Contains(key))
                {
                    return key.ToString()[0];
                }
                else if (SymbolKeys.Contains(key))
                {
                    return NoShiftSymbolCharacters[key];
                }
                else if (StandardNumberKeys.Contains(key))
                {
                    // When the shift is is up, the standard number keys return numbers.
                    return NoShiftStandardNumberCharacters[key];
                }
            }

            return (char)0;
        }

        /// <summary>
        /// Returns a value indicating if any of the shift keys are being pressed down.
        /// </summary>
        /// <returns>True if any of the shift keys are down.</returns>
        public bool IsAnyShiftKeyDown() => throw new NotImplementedException();

        /// <summary>
        /// Returns a value indicating if any of the control keys are being pressed down.
        /// </summary>
        /// <returns>True if any of the control keys are down.</returns>
        public bool IsAnyCtrlKeyDown() => throw new NotImplementedException();

        /// <summary>
        /// Returns a value indicating if the any of the delete keys have been fully pressed.
        /// </summary>
        /// <returns>True if any of the delete keys are down.</returns>
        public bool IsDeleteKeyPressed() => throw new NotImplementedException();

        /// <summary>
        /// Returns a value indicating if the backspace key has been fully
        /// pressed to the down position then let go to the up position.
        /// </summary>
        /// <returns>True if the backspace key is down.</returns>
        public bool IsBackspaceKeyPressed() => IsKeyPressed(KeyCode.Back);

        /// <summary>
        /// Update the current state of the keyboard.
        /// </summary>
        public void UpdateCurrentState()
        {
            // InternalKeyboard.UpdateCurrentState();
        }

        /// <summary>
        /// Update the previous state of the keyboard.
        /// </summary>
        public void UpdatePreviousState()
        {
            // InternalKeyboard.UpdatePreviousState();
        }
    }
}

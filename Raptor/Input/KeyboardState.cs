// <copyright file="KeyboardState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    /// Represents a single keyboard state at a particular time.
    /// </summary>
    public struct KeyboardState : IEquatable<KeyboardState>
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
            KeyCode.Number0, KeyCode.Number1, KeyCode.Number2,
            KeyCode.Number3, KeyCode.Number4, KeyCode.Number5,
            KeyCode.Number6, KeyCode.Number7, KeyCode.Number8,
            KeyCode.Number9,
        };

        /// <summary>
        /// The number keys on the numpad.
        /// </summary>
        private static readonly KeyCode[] NumpadNumberKeys = new[]
        {
            KeyCode.Numpad0, KeyCode.Numpad1, KeyCode.Numpad2,
            KeyCode.Numpad3, KeyCode.Numpad4, KeyCode.Numpad5,
            KeyCode.Numpad6, KeyCode.Numpad7, KeyCode.Numpad8,
            KeyCode.Numpad9,
        };

        /// <summary>
        /// The symbol keys.
        /// </summary>
        private static readonly KeyCode[] SymbolKeys = new[]
        {
            KeyCode.Semicolon, KeyCode.Plus, KeyCode.Comma,
            KeyCode.Minus, KeyCode.Period, KeyCode.ForwardSlash,
            KeyCode.Tilde, KeyCode.LeftBracket, KeyCode.RightBracket,
            KeyCode.Quote, KeyCode.NumpadDivide, KeyCode.NumpadMultiply,
            KeyCode.NumpadMinus, KeyCode.NumpadPlus, KeyCode.NumpadDecimal, KeyCode.BackSlash,
        };

        /// <summary>
        /// The characters produced by the standard number keys when no shift modifier keys are in the down position.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> NoShiftStandardNumberCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.Number0, '0' }, { KeyCode.Number1, '1' }, { KeyCode.Number2, '2' },
            { KeyCode.Number3, '3' }, { KeyCode.Number4, '4' }, { KeyCode.Number5, '5' },
            { KeyCode.Number6, '6' }, { KeyCode.Number7, '7' }, { KeyCode.Number8, '8' }, { KeyCode.Number9, '9' },
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when no shift modifier keys are in the down position..
        /// </summary>
        private static readonly Dictionary<KeyCode, char> NoShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.Plus, '=' }, { KeyCode.Comma, ',' }, { KeyCode.Minus, '-' },
            { KeyCode.Period, '.' }, { KeyCode.ForwardSlash, '/' }, { KeyCode.Tilde, '`' },
            { KeyCode.BackSlash, '\\' }, { KeyCode.LeftBracket, '[' }, { KeyCode.RightBracket, ']' },
            { KeyCode.Quote, '\'' }, { KeyCode.Semicolon, ';' }, { KeyCode.NumpadPlus, '+' }, { KeyCode.NumpadDecimal, '.' },
            { KeyCode.NumpadDivide, '/' }, { KeyCode.NumpadMultiply, '*' }, { KeyCode.NumpadMinus, '-' },
        };

        /// <summary>
        /// The symbol keys produced by the keyboard when any shift modifier keys are in the down position.
        /// </summary>
        private static readonly Dictionary<KeyCode, char> WithShiftSymbolCharacters = new Dictionary<KeyCode, char>()
        {
            { KeyCode.Plus, '+' }, { KeyCode.Comma, '<' }, { KeyCode.Minus, '_' },
            { KeyCode.Period, '>' }, { KeyCode.ForwardSlash, '?' }, { KeyCode.Tilde, '~' },
            { KeyCode.BackSlash, '|' }, { KeyCode.LeftBracket, '{' }, { KeyCode.RightBracket, '}' },
            { KeyCode.Quote, '\'' }, { KeyCode.Semicolon, ':' }, { KeyCode.Number1, '!' },
            { KeyCode.Number2, '@' }, { KeyCode.Number3, '#' }, { KeyCode.Number4, '$' }, { KeyCode.Number5, '%' },
            { KeyCode.Number6, '^' }, { KeyCode.Number7, '&' }, { KeyCode.Number8, '*' }, { KeyCode.Number9, '(' },
            { KeyCode.Number0, ')' }, { KeyCode.NumpadDivide, '/' }, { KeyCode.NumpadMultiply, '*' }, { KeyCode.NumpadMinus, '-' },
            { KeyCode.NumpadPlus, '+' }, { KeyCode.NumpadDecimal, '.' },
        };

        private Dictionary<KeyCode, bool> keyStates;

        /*TODO:
            These tests are for testing caps and num lock.  Currently OpenTK 4.0
            needs some implementation of the proper overload and enums to be able to do this.
            Refer to https://github.com/opentk/opentk/issues/1089 for more info

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the caps lock key is on.
        /// </summary>
        public bool CapsLockOn => IsKeyDown(KeyCode.CapsLock);

        //// <summary>
        //// Gets or sets a value indicating whether gets a value indicating if the numlock key is on.
        //// </summary>
        public bool NumLockOn => IsKeyDown(KeyCode.NumLock);
        */

        /// <summary>
        /// Returns a value indicating whether the <paramref name="left"/> and <paramref name="right"/> operands are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if operands are equal.</returns>
        public static bool operator ==(KeyboardState left, KeyboardState right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating whether the <paramref name="left"/> and <paramref name="right"/> operands are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if operands are not equal.</returns>
        public static bool operator !=(KeyboardState left, KeyboardState right) => !left.Equals(right);

        /// <summary>
        /// Returns all of the keys and their states.
        /// </summary>
        /// <returns>The keys and given state for each key.</returns>
        public Dictionary<KeyCode, bool> GetKeyStates()
        {
            InitKeys();

            return this.keyStates;
        }

        /// <summary>
        /// Gets a value indicating whether the left shift key is in the down position.
        /// </summary>
        /// <returns>True if the left shift key is down.</returns>
        public bool IsLeftShiftDown() => IsKeyDown(KeyCode.LeftShift);

        /// <summary>
        /// Gets a value indicating whether the right shift key is in the down position.
        /// </summary>
        /// <returns>True if the right shift key is down.</returns>
        public bool IsRightShiftDown() => IsKeyDown(KeyCode.RightShift);

        /// <summary>
        /// Gets a value indicating whether the left control key is in the down position.
        /// </summary>
        /// <returns>True if the left control key is down.</returns>
        public bool IsLeftCtrlDown() => IsKeyDown(KeyCode.LeftControl);

        /// <summary>
        /// Gets a value indicating whether the right control key is in the down position.
        /// </summary>
        /// <returns>True if the right control key is down.</returns>
        public bool IsRightCtrlDown() => IsKeyDown(KeyCode.RightControl);

        /// <summary>
        /// Gets a value indicating whether the left alt key is in the down position.
        /// </summary>
        /// <returns>True if the left alt key is down.</returns>
        public bool IsLeftAltDown() => IsKeyDown(KeyCode.LeftAlt);

        /// <summary>
        /// Gets a value indicating whether the right alt key is in the down position.
        /// </summary>
        /// <returns>True if the right alt key is down.</returns>
        public bool IsRightAltDown() => IsKeyDown(KeyCode.RightAlt);

        /// <summary>
        /// Returns all of the keys that are in the down position.
        /// </summary>
        /// <returns>A list of the currently pressed keys.</returns>
        public KeyCode[] GetDownKeys()
        {
            InitKeys();

            var downKeys = new List<KeyCode>();

            foreach (var key in this.keyStates)
            {
                if (key.Value)
                    downKeys.Add(key.Key);
            }

            return downKeys.ToArray();
        }

        /// <summary>
        /// Gets a value indicating if any keys are in the down position.
        /// </summary>
        /// <returns>True if any keys on the keyboard in pressed in the down position.</returns>
        public bool AnyKeysDown()
        {
            InitKeys();

            return this.keyStates.Any(i => i.Value);
        }

        /// <summary>
        /// Returns a value indicating if any of the given <paramref name="keys"/> are in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns>True if any of the given <paramref name="keys"/> are pressed in the down position.</returns>
        public bool AnyKeysDown(KeyCode[] keys)
        {
            InitKeys();

            var downKeys = new List<KeyCode>();

            foreach (var key in this.keyStates)
            {
                if (key.Value)
                    downKeys.Add(key.Key);
            }

            return keys.Any(k => downKeys.Contains(k));
        }

        /// <summary>
        /// Returns true if the given key is in the down position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given <paramref name="key"/> is pressed in the down position.</returns>
        public bool IsKeyDown(KeyCode key)
        {
            InitKeys();

            return this.keyStates[key];
        }

        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the given <paramref name="key"/> is in the up position.</returns>
        public bool IsKeyUp(KeyCode key)
        {
            InitKeys();

            return !this.keyStates[key];
        }

        /// <summary>
        /// Returns a value indicating if any of the standard number keys
        /// above the letter keys on the keyboard are in the down position.
        /// </summary>
        /// <returns>True if any of the standard number keys are in the down position.</returns>
        public bool AnyStandardNumberKeysDown()
        {
            InitKeys();

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
        /// Returns a value indicating if any of the shift keys are in the down position.
        /// </summary>
        /// <returns>True if any of the shift keys are down.</returns>
        public bool IsAnyShiftKeyDown() => IsKeyDown(KeyCode.LeftShift) || IsKeyDown(KeyCode.RightShift);

        /// <summary>
        /// Returns a value indicating if any of the control keys are in the down position.
        /// </summary>
        /// <returns>True if any of the control keys are down.</returns>
        public bool IsAnyCtrlKeyDown() => IsKeyDown(KeyCode.LeftControl) || IsKeyDown(KeyCode.RightControl);

        /// <summary>
        /// Returns the character equivalent of the given key if it is
        /// a letter, number or symbol key.  The value of 0 will be returned
        /// if the key is not a letter, number or symbol.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>The character that matches the given key.</returns>
        public char KeyToChar(KeyCode key)
        {
            if (IsAnyShiftKeyDown())
            {
                // NOTE!!  THIS MIGHT NOT WORK.  THE ToUpperInvariant() THAT IS
                if (LetterKeys.Contains(key))
                    return key == KeyCode.Space ? ' ' : key.ToString().ToUpperInvariant()[0];

                // When the shift key is down, the standard number keys and symbol keys return symbols.
                if (StandardNumberKeys.Contains(key))
                    return WithShiftSymbolCharacters[key];

                if (SymbolKeys.Contains(key))
                    return WithShiftSymbolCharacters[key];
            }
            else
            {
                if (LetterKeys.Contains(key))
                {
#pragma warning disable CA1304 // Specify CultureInfo
                    return key == KeyCode.Space ? ' ' : key.ToString().ToLower()[0];
#pragma warning restore CA1304 // Specify CultureInfo
                }

                if (SymbolKeys.Contains(key))
                    return NoShiftSymbolCharacters[key];

                // When the shift is is up, the standard number keys return numbers.
                if (StandardNumberKeys.Contains(key))
                    return NoShiftStandardNumberCharacters[key];
            }

            return (char)0;
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => this.keyStates.GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is KeyboardState state))
                return false;

            return Equals(state);
        }

        /// <inheritdoc/>
        public bool Equals(KeyboardState other)
        {
            InitKeys();

            var otherKeyStates = other.GetKeyStates();

            return this.keyStates.All(state => otherKeyStates.Keys.Contains(state.Key) && otherKeyStates.Values.Contains(state.Value));
        }

        /// <summary>
        /// Sets the state of the given <paramref name="key"/> to the given <paramref name="state"/> value.
        /// </summary>
        /// <param name="key">The key to set the state to.</param>
        /// <param name="state">The state of the key.</param>
        internal void SetKeyState(KeyCode key, bool state)
        {
            InitKeys();

            this.keyStates[key] = state;
        }

        /// <summary>
        /// Initializes the key states.
        /// </summary>
        private void InitKeys()
        {
            if (!(this.keyStates is null) && this.keyStates.Count > 0)
                return;

            this.keyStates = new Dictionary<KeyCode, bool>();

            var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            for (var i = 0; i < keys.Length; i++)
            {
                this.keyStates.Add(keys[i], false);
            }
        }
    }
}

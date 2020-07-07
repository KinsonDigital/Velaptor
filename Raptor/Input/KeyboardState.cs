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

        private readonly Dictionary<KeyCode, bool> keyStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyboardState"/> struct.
        /// </summary>
        /// <param name="keys">The list of keys.</param>
        /// <param name="keyStates">The list of states for the given <paramref name="keys"/>.</param>
        /// <remarks>
        ///     The <paramref name="keys"/> param must have equal number of items as the <paramref name="keyStates"/> param.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if the <paramref name="keys"/> or <paramref name="keyStates"/> are null or empty.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if the <paramref name="keys"/> and <paramref name="keyStates"/> item count does not match.
        /// </exception>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "Exception message only used in method.")]
        public KeyboardState(KeyCode[] keys, bool[] keyStates)
        {
            if (keys is null || keys.Length <= 0)
                throw new ArgumentNullException(nameof(keys), "The parameter must not be null or empty.");

            if (keyStates is null || keyStates.Length <= 0)
                throw new ArgumentNullException(nameof(keyStates), "The parameter must not be null or empty.");

            if (keys.Length != keyStates.Length)
                throw new ArgumentException($"The list of '{nameof(keys)}' and '{nameof(keyStates)}' must have the same number of items.");

            this.keyStates = new Dictionary<KeyCode, bool>();

            for (var i = 0; i < keys.Length; i++)
            {
                this.keyStates.Add(keys[i], keyStates[i]);
            }
        }

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
        /// Gets a value indicating whether the left shift key is being pressed down.
        /// </summary>
        public bool IsLeftShiftDown => IsKeyDown(KeyCode.LeftShift);

        /// <summary>
        /// Gets a value indicating whether the right shift key is being pressed down.
        /// </summary>
        public bool IsRightShiftDown => IsKeyDown(KeyCode.RightShift);

        /// <summary>
        /// Gets a value indicating whether the left control key is being pressed down.
        /// </summary>
        public bool IsLeftCtrlDown => IsKeyDown(KeyCode.LeftControl);

        /// <summary>
        /// Gets a value indicating whether the right control key is being pressed down.
        /// </summary>
        public bool IsRightCtrlDown => IsKeyDown(KeyCode.RightControl);

        /// <summary>
        /// Gets a value indicating whether the left alt key is being pressed down.
        /// </summary>
        public bool IsLeftAltDown => IsKeyDown(KeyCode.LeftAlt);

        /// <summary>
        /// Gets a value indicating whether the right alt key is being pressed down.
        /// </summary>
        public bool IsRightAltDown => IsKeyDown(KeyCode.RightAlt);

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
        /// Returns all of the currently pressed keys on the keyboard.
        /// </summary>
        /// <returns>A list of the currently pressed keys.</returns>
        public KeyCode[] GetDownKeys()
        {
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
        public bool AnyKeysDown() => this.keyStates.Any(i => i.Value);

        /// <summary>
        /// Returns a value indicating if any of the given <paramref name="keys"/> are in the down position.
        /// </summary>
        /// <param name="keys">The list of key codes to check.</param>
        /// <returns>True if any of the given <paramref name="keys"/> are pressed in the down position.</returns>
        public bool AnyKeysDown(KeyCode[] keys)
        {
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
        public bool IsKeyDown(KeyCode key) => this.keyStates[key];

        /// <summary>
        /// Returns true if the given key is in the up position.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the givne <paramref name="key"/> is in the up position.</returns>
        public bool IsKeyUp(KeyCode key) => !this.keyStates[key];

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
        /// Returns a value indicating if any of the shift keys are being pressed down.
        /// </summary>
        /// <returns>True if any of the shift keys are down.</returns>
        public bool IsAnyShiftKeyDown() => IsKeyDown(KeyCode.LeftShift) || IsKeyDown(KeyCode.RightShift);

        /// <summary>
        /// Returns a value indicating if any of the control keys are being pressed down.
        /// </summary>
        /// <returns>True if any of the control keys are down.</returns>
        public bool IsAnyCtrlKeyDown() => IsKeyDown(KeyCode.LeftControl) || IsKeyDown(KeyCode.RightControl);

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
            if (this.keyStates.Count != other.keyStates.Count)
                return false;

            foreach (var keyState in this.keyStates)
            {
                if (!other.keyStates.ContainsKey(keyState.Key) || !other.keyStates.ContainsValue(keyState.Value))
                    return false;
            }

            return true;
        }
    }
}

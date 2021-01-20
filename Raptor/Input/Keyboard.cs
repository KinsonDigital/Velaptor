// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public class Keyboard : IKeyboard
    {
        /// <inheritdoc/>
        public KeyboardState GetState()
        {
            if (IKeyboard.KeyStates.Count <= 0)
            {
                InitializepKeyStates();
            }

            var keyboardState = default(KeyboardState);

            foreach (var state in IKeyboard.KeyStates)
            {
                keyboardState.SetKeyState(state.Key, state.Value);
            }

            return keyboardState;
        }

        /// <summary>
        /// Sets the state of the given <paramref name="key"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="state">The state to set the key to.</param>
        /// <remarks>Using the value of true for the <paramref name="state"/> param means the key is in the down position.</remarks>
        internal static void SetKeyState(KeyCode key, bool state) => IKeyboard.KeyStates[key] = state;

        /// <summary>
        /// Initializes all of the available keys and default states.
        /// </summary>
        private static void InitializepKeyStates()
        {
            var keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            for (var i = 0; i < keyCodes.Length; i++)
            {
                IKeyboard.KeyStates.Add(keyCodes[i], false);
            }
        }
    }
}

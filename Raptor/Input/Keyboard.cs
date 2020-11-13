// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// The state for each key on the keyboard.
        /// </summary>
        internal static readonly Dictionary<KeyCode, bool> KeyStates = new Dictionary<KeyCode, bool>();

        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        /// <returns>The current keyboard state.</returns>
        public static KeyboardState GetState()
        {
            if (KeyStates.Count <= 0)
            {
                SetupKeyStates();
            }

            var keyboardState = default(KeyboardState);

            foreach (var state in KeyStates)
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
        internal static void SetKeyState(KeyCode key, bool state) => KeyStates[key] = state;

        /// <summary>
        /// Sets up the all of the available keys and default states.
        /// </summary>
        private static void SetupKeyStates()
        {
            var keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            for (var i = 0; i < keyCodes.Length; i++)
            {
                KeyStates.Add(keyCodes[i], false);
            }
        }
    }
}

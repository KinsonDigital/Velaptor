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
    public class Keyboard : IGameInput<KeyCode, KeyboardState>
    {
        /// <summary>
        /// Gets the current state of the input.
        /// </summary>
        /// <returns>The state of the keyboard.</returns>
        public KeyboardState GetState()
        {
            if (IGameInput<KeyCode, KeyboardState>.InputStates.Count <= 0)
            {
                InitializepKeyStates();
            }

            var keyboardState = default(KeyboardState);

            foreach (var state in IGameInput<KeyCode, KeyboardState>.InputStates)
            {
                keyboardState.SetKeyState(state.Key, state.Value);
            }

            return keyboardState;
        }

        /// <summary>
        /// Sets the state of the given <paramref name="key"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="key">The key to set.</param>
        /// <param name="state">The state of the key.</param>
        /// <remarks>
        ///     True for the <paramref name="state"/> paramemter means the key is in the down position.
        /// </remarks>
        internal static void SetKeyState(KeyCode key, bool state) => IGameInput<KeyCode, KeyboardState>.InputStates[key] = state;

        /// <summary>
        /// Initializes all of the available keys and default states.
        /// </summary>
        private static void InitializepKeyStates()
        {
            var keyCodes = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            for (var i = 0; i < keyCodes.Length; i++)
            {
                IGameInput<KeyCode, KeyboardState>.InputStates.Add(keyCodes[i], false);
            }
        }
    }
}

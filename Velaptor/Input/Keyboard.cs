// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input
{
    using System;
    using System.Linq;

    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    public class Keyboard : IKeyboardInput<KeyCode, KeyboardState>
    {
        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        /// <returns>The state of the keyboard.</returns>
        public KeyboardState GetState()
        {
            if (IKeyboardInput<KeyCode, KeyboardState>.InputStates.Count <= 0)
            {
                InitializeKeyStates();
            }

            var keyboardState = default(KeyboardState);

            foreach (var state in IKeyboardInput<KeyCode, KeyboardState>.InputStates)
            {
                keyboardState.SetKeyState(state.Key, state.Value);
            }

            return keyboardState;
        }

        /// <summary>
        /// Sets the state of the given <paramref name="input"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="input">The key to set.</param>
        /// <param name="state">The state of the given key.</param>
        /// <remarks>
        ///     When <paramref name="state"/> is the value of <see langword="true"/>,
        ///     this means the keyboard key is being pressed down.
        /// </remarks>
        public void SetState(KeyCode input, bool state)
            => IKeyboardInput<KeyCode, KeyboardState>.InputStates[input] = state;

        /// <inheritdoc/>
        public void Reset()
        {
            if (IKeyboardInput<KeyCode, KeyboardState>.InputStates.Count <= 0)
            {
                InitializeKeyStates();
            }

            var keys = IKeyboardInput<KeyCode, KeyboardState>.InputStates.Keys.ToArray();

            foreach (var key in keys)
            {
                IKeyboardInput<KeyCode, KeyboardState>.InputStates[key] = false;
            }
        }

        /// <summary>
        /// Initializes all of the available keys and default states.
        /// </summary>
        private static void InitializeKeyStates()
        {
            var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            foreach (var key in keys)
            {
                IKeyboardInput<KeyCode, KeyboardState>.InputStates.Add(key, false);
            }
        }
    }
}

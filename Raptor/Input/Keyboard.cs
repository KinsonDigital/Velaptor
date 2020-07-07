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
        private static readonly Dictionary<KeyCode, bool> KeyStates = new Dictionary<KeyCode, bool>();

        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        /// <returns>The current keyboard state.</returns>
        public static KeyboardState GetState()
        {
            if (KeyStates.Count <= 0)
                SetupKeyStates();

            return new KeyboardState(KeyStates.Keys.ToArray(), KeyStates.Values.ToArray());
        }

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

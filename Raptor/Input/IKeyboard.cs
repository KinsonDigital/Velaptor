// <copyright file="IKeyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides funtionality for the keyboard.
    /// </summary>
    public interface IKeyboard
    {
        /// <summary>
        /// The state for each key on the keyboard.
        /// </summary>
        internal static readonly Dictionary<KeyCode, bool> KeyStates = new Dictionary<KeyCode, bool>();

        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        /// <returns>The current state of the keyboard.</returns>
        KeyboardState GetState();
    }
}

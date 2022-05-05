// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Velaptor.Reactables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides functionality for the keyboard.
    /// </summary>
    internal class Keyboard : IAppInput<KeyboardState>
    {
        private readonly Dictionary<KeyCode, bool> keyStates = new ();
        private readonly IDisposable keyboardStateUnsubscriber;

        /// <summary>
        /// Initializes a new instance of the <see cref="Keyboard"/> class.
        /// </summary>
        /// <param name="keyboardReactable">Subscribed for keyboard state push notifications.</param>
        public Keyboard(IReactable<(KeyCode key, bool isDown)> keyboardReactable)
        {
            this.keyboardStateUnsubscriber = keyboardReactable.Subscribe(new Reactor<(KeyCode key, bool isDown)>(
                state =>
            {
                this.keyStates[state.key] = state.isDown;
            }, () => this.keyboardStateUnsubscriber?.Dispose()));

            InitializeKeyStates();
        }

        /// <summary>
        /// Gets the current state of the keyboard.
        /// </summary>
        /// <returns>The state of the keyboard.</returns>
        public KeyboardState GetState()
        {
            var keyboardState = default(KeyboardState);

            foreach (var state in this.keyStates)
            {
                keyboardState.SetKeyState(state.Key, state.Value);
            }

            return keyboardState;
        }

        /// <summary>
        /// Initializes all of the available keys and default states.
        /// </summary>
        private void InitializeKeyStates()
        {
            if (this.keyStates.Count > 0)
            {
                return;
            }

            var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

            foreach (var key in keys)
            {
                this.keyStates[key] = false;
            }
        }
    }
}

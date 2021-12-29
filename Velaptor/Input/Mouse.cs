// <copyright file="Mouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input
{
    using System;
    using System.Linq;

    /// <summary>
    /// Gets or sets the state of the mouse.
    /// </summary>
    public class Mouse : IMouseInput<MouseButton, MouseState>
    {
        private static int xPos;
        private static int yPos;
        private static int scrollWheelValue;

        /// <summary>
        /// Gets the current state of the mouse.
        /// </summary>
        /// <returns>The state of the mouse.</returns>
        public MouseState GetState()
        {
            if (IMouseInput<MouseButton, MouseState>.InputStates.Count <= 3)
            {
                InitializeButtonStates();
            }

            var result = default(MouseState);
            result.SetPosition(xPos, yPos);
            result.SetScrollWheelValue(scrollWheelValue);

            // Set all of the states for the buttons
            foreach (var state in IMouseInput<MouseButton, MouseState>.InputStates)
            {
                result.SetButtonState(state.Key, state.Value);
            }

            return result;
        }

        /// <summary>
        /// Sets the given mouse <paramref name="input"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="input">The mouse button to set.</param>
        /// <param name="state">The state to set the button to.</param>
        /// <remarks>
        ///     When <paramref name="state"/> is the value of <see langword="true"/>,
        ///     this means the mouse button is being pressed down.
        /// </remarks>
        public void SetState(MouseButton input, bool state)
            => IMouseInput<MouseButton, MouseState>.InputStates[input] = state;

        /// <inheritdoc/>
        public void SetXPos(int x) => xPos = x;

        /// <inheritdoc/>
        public void SetYPos(int y) => yPos = y;

        /// <inheritdoc/>
        public void SetScrollWheelValue(int value) => scrollWheelValue = value;

        /// <inheritdoc/>
        public void Reset()
        {
            if (IMouseInput<MouseButton, MouseState>.InputStates.Count <= 0)
            {
                InitializeButtonStates();
            }

            for (var i = 0; i < IMouseInput<MouseButton, MouseState>.InputStates.Count; i++)
            {
                var key = IMouseInput<MouseButton, MouseState>.InputStates.Keys.ToArray()[i];

                IMouseInput<MouseButton, MouseState>.InputStates[key] = false;
            }

            xPos = 0;
            yPos = 0;
            scrollWheelValue = 0;
        }

        /// <summary>
        /// Initializes all of the available keys and default states.
        /// </summary>
        private static void InitializeButtonStates()
        {
            var keyCodes = Enum.GetValues(typeof(MouseButton)).Cast<MouseButton>().ToArray();

            foreach (var key in keyCodes)
            {
                if (IMouseInput<MouseButton, MouseState>.InputStates.ContainsKey(key))
                {
                    continue;
                }

                IMouseInput<MouseButton, MouseState>.InputStates.Add(key, false);
            }
        }
    }
}

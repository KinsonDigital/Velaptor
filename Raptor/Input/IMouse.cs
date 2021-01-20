// <copyright file="IMouse.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides functionality for the mosue.
    /// </summary>
    public interface IMouse
    {
        /// <summary>
        /// The state for each button on the mouse.
        /// </summary>
        internal static readonly Dictionary<MouseButton, bool> ButtonStates = new Dictionary<MouseButton, bool>();

        /// <summary>
        /// Gets the current state of the mouse.
        /// </summary>
        /// <returns>The current state of the mouse.</returns>
        MouseState GetMouseState();
    }
}

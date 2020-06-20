// <copyright file="KeyEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Holds information about the keys that are pressed down.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyEventArgs"/> class.
        /// </summary>
        /// <param name="keys">The list of keys that are in the down position.</param>
        public KeyEventArgs(KeyCode[] keys) => Keys = new ReadOnlyCollection<KeyCode>(keys);

        /// <summary>
        /// Gets the keys that was pressed.
        /// </summary>
        public ReadOnlyCollection<KeyCode> Keys { get; private set; }
    }
}

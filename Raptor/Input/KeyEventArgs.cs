using System;
using System.Collections.ObjectModel;

namespace Raptor.Input
{
    /// <summary>
    /// Holds information about the keys that are pressed down.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        #region Props
        /// <summary>
        /// Gets the keys that was pressed.
        /// </summary>
        public ReadOnlyCollection<KeyCode> Keys { get; private set; }
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a new instance of <see cref="KeyEventArgs"/>.
        /// </summary>
        /// <param name="keys">The list of keys that are in the down position.</param>
        public KeyEventArgs(KeyCode[] keys) => Keys = new ReadOnlyCollection<KeyCode>(keys);
        #endregion
    }
}

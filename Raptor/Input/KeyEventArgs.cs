using System;

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
        public KeyCodes[] Keys { get; set; }
        #endregion


        #region Constructor
        /// <summary>
        /// Creates a new instance of <see cref="KeyEventArgs"/>.
        /// </summary>
        /// <param name="keys">The list of keys that are in the down position.</param>
        public KeyEventArgs(KeyCodes[] keys) => Keys = keys;
        #endregion
    }
}

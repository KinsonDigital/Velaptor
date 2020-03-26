using System;

namespace Raptor.Input
{
    /// <summary>
    /// Holds information about a mouse down event.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        #region Constructor
        /// <summary>
        /// Creates a new instance of MouseEventArgs.
        /// </summary>
        /// <param name="state">The state of the mouse.</param>
        public MouseEventArgs(MouseInputState state) => State = state;
        #endregion


        #region Props
        /// <summary>
        /// Gets the state of the mouse.
        /// </summary>
        public MouseInputState State { get; }
        #endregion
    }
}

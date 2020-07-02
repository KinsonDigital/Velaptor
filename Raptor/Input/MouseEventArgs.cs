// <copyright file="MouseEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;

    /// <summary>
    /// Holds information about a mouse down event.
    /// </summary>
    public class MouseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MouseEventArgs"/> class.
        /// </summary>
        /// <param name="state">The state of the mouse.</param>
        public MouseEventArgs(MouseInputState state) => State = state;

        /// <summary>
        /// Gets the state of the mouse.
        /// </summary>
        public MouseInputState State { get; }
    }
}

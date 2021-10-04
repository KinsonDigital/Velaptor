// <copyright file="MousePositionEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.UI
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Holds information about the mouse position over a control.
    /// </summary>
    public class MousePositionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MousePositionEventArgs"/> class.
        /// </summary>
        /// <param name="mousePosition">The position of the mouse.</param>
        public MousePositionEventArgs(Point mousePosition) => MousePosition = mousePosition;

        /// <summary>
        /// Gets the position of the mouse relative to the top right corner of the control.
        /// </summary>
        public Point MousePosition { get; }
    }
}

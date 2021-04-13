// <copyright file="IGameInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;

    /// <summary>
    /// Gets the state of system mouse.
    /// </summary>
    /// <typeparam name="TInputs">The inputs available.</typeparam>
    /// <typeparam name="TInputState">The state of the input.</typeparam>
    public interface IMouseInput<TInputs, TInputState> : IGameInput<TInputs, TInputState>
        where TInputs : struct, Enum
    {
        /// <summary>
        /// The X coordinate of the mouse.
        /// </summary>
        internal static int XPos;

        /// <summary>
        /// The Y coordinate of the mouse.
        /// </summary>
        internal static int YPos;

        /// <summary>
        /// The value of the scroll wheel of the mouse.
        /// </summary>
        internal static int ScrollWheelValue;

        /// <summary>
        /// Sets the X coordinate sate of the mouse.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        void SetXPos(int x);

        /// <summary>
        /// Sets the Y coordinate sate of the mouse.
        /// </summary>
        /// <param name="y">The Y coordinate.</param>
        void SetYPos(int y);

        /// <summary>
        /// Sets the value of the scroll wheel.
        /// </summary>
        /// <param name="value">The value to set the scroll wheel to.</param>
        void SetScrollWheelValue(int value);
    }
}

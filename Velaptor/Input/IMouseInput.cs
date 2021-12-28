// <copyright file="IMouseInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// Gets the state of system mouse.
    /// </summary>
    /// <typeparam name="TInputs">The inputs available.</typeparam>
    /// <typeparam name="TInputState">The state of the input.</typeparam>
    public interface IMouseInput<in TInputs, out TInputState> : IGameInput<TInputs, TInputState>
        where TInputs : struct, Enum
    {
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
        [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Ignored until used by implementing issue #71")]
        void SetScrollWheelValue(int value);
    }
}

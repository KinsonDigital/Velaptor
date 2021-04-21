// <copyright file="IGameInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Gets the state of game-specific input such as a mouse or keyboard.
    /// </summary>
    /// <typeparam name="TInputs">The inputs available.</typeparam>
    /// <typeparam name="TInputState">The state of the input.</typeparam>
    public interface IGameInput<TInputs, TInputState>
        where TInputs : struct, Enum
    {
        /// <summary>
        /// The state of each input.
        /// </summary>
        /// <remarks>
        ///     <see langword="true"/> means that the input is being used.
        /// <para>
        ///     Example: A mouse button or keyboard key is being pressed down.
        /// </para>
        /// </remarks>
        internal static readonly Dictionary<TInputs, bool> InputStates = new Dictionary<TInputs, bool>();

        /// <summary>
        /// Returns the current state of the input.
        /// </summary>
        /// <returns>The state of the input.</returns>
        TInputState GetState();

        /// <summary>
        /// Sets the given <paramref name="input"/> to the given <paramref name="state"/>.
        /// </summary>
        /// <param name="input">The input to set.</param>
        /// <param name="state">The state to set the input to.</param>
        /// <remarks>
        ///     When the <paramref name="state"/> is the value of <see langword=""="true"/>,
        ///     this means that the input is in the down state.
        /// </remarks>
        void SetState(TInputs input, bool state);

        /// <summary>
        /// Resets the state of the input.
        /// </summary>
        void Reset();
    }
}

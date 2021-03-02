// <copyright file="IGameInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Gets the state of a game specific input such as a mouse or keyboard.
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
        ///     True means that the input is being used.
        /// <para>
        ///     Example: True could mean that a keyboard key is being held down.
        /// </para>
        /// </remarks>
        internal static readonly Dictionary<TInputs, bool> InputStates = new Dictionary<TInputs, bool>();

        /// <summary>
        /// Returns the current state of the input.
        /// </summary>
        /// <returns>The state of the input.</returns>
        TInputState GetState();
    }
}

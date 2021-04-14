// <copyright file="IKeyboardInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Input
{
    using System;

    /// <summary>
    /// Gets the state of the system keyboard.
    /// </summary>
    /// <typeparam name="TInputs">The inputs available.</typeparam>
    /// <typeparam name="TInputState">The state of the input.</typeparam>
    public interface IKeyboardInput<TInputs, TInputState> : IGameInput<TInputs, TInputState>
        where TInputs : struct, Enum
    {
    }
}

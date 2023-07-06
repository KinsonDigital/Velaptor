// <copyright file="IAppInput.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

/// <summary>
/// Gets the state of game-specific input such as a mouse or keyboard.
/// </summary>
/// <typeparam name="TState">The state of the input.</typeparam>
public interface IAppInput<out TState>
    where TState : struct
{
    /// <summary>
    /// Returns the current state of the input.
    /// </summary>
    /// <returns>The state of the input.</returns>
    TState GetState();
}

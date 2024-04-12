// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // Do not use default value type constructor
namespace Velaptor.Input;

using System;
using Services;

/// <summary>
/// Provides functionality for the keyboard.
/// </summary>
internal sealed class Keyboard : IAppInput<KeyboardState>
{
    private readonly IKeyboardDataService keyboardDataService;

    /// <summary>
    /// Initializes a new instance of the <see cref="Keyboard"/> class.
    /// </summary>
    /// <param name="keyboardDataService">Creates reactables for sending and receiving notifications with or without data.</param>
    public Keyboard(IKeyboardDataService keyboardDataService)
    {
        ArgumentNullException.ThrowIfNull(keyboardDataService);

        this.keyboardDataService = keyboardDataService;
    }

    /// <summary>
    /// Gets the current state of the keyboard.
    /// </summary>
    /// <returns>The state of the keyboard.</returns>
    public KeyboardState GetState()
    {
        var keyboardState = new KeyboardState();

        var currentKeyStates = this.keyboardDataService.GetKeyStates();

        foreach ((KeyCode key, var state) in currentKeyStates)
        {
            keyboardState.KeyStates[key] = state;
        }

        return keyboardState;
    }
}

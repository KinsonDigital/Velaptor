// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable SA1129 // Do not use default value type constructor
namespace Velaptor.Input;

using System.Linq;
using Guards;
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
        EnsureThat.ParamIsNotNull(keyboardDataService);

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

        foreach (var key in currentKeyStates.Keys.Where(key => keyboardState.KeyStates.ContainsKey(key)))
        {
            keyboardState.KeyStates[key] = currentKeyStates[key];
        }

        return keyboardState;
    }
}

// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using Carbonate.OneWay;
using Factories;
using Guards;
using ReactableData;

/// <summary>
/// Provides functionality for the keyboard.
/// </summary>
internal sealed class Keyboard : IAppInput<KeyboardState>
{
    private readonly Dictionary<KeyCode, bool> keyStates = new ();
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="Keyboard"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public Keyboard(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var reactable = reactableFactory.CreateKeyboardReactable();

        var keyboardStateChangeName = this.GetExecutionMemberName(nameof(PushNotifications.KeyboardStateChangedId));
        this.unsubscriber = reactable.Subscribe(new ReceiveSubscription<KeyboardKeyStateData>(
            id: PushNotifications.KeyboardStateChangedId,
            name: keyboardStateChangeName,
            onReceive: data =>
            {
                this.keyStates[data.Key] = data.IsDown;
            },
            onUnsubscribe: () => this.unsubscriber?.Dispose()));

        InitializeKeyStates();
    }

    /// <summary>
    /// Gets the current state of the keyboard.
    /// </summary>
    /// <returns>The state of the keyboard.</returns>
    public KeyboardState GetState()
    {
        var keyboardState = default(KeyboardState);

        foreach (var state in this.keyStates)
        {
            keyboardState.SetKeyState(state.Key, state.Value);
        }

        return keyboardState;
    }

    /// <summary>
    /// Initializes all of the available keys and default states.
    /// </summary>
    private void InitializeKeyStates()
    {
        var keys = Enum.GetValues(typeof(KeyCode)).Cast<KeyCode>().ToArray();

        foreach (var key in keys)
        {
            this.keyStates[key] = false;
        }
    }
}

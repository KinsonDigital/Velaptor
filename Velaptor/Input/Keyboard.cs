// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using Carbonate.Fluent;
using Factories;
using Guards;
using ReactableData;

/// <summary>
/// Provides functionality for the keyboard.
/// </summary>
internal sealed class Keyboard : IAppInput<KeyboardState>
{
    private readonly Dictionary<KeyCode, bool> keyStates = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="Keyboard"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public Keyboard(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        var keyboardDataReactable = reactableFactory.CreateKeyboardReactable();

        var keyboardChangedSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.KeyboardStateChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.KeyboardStateChangedId)))
            .BuildOneWayReceive<KeyboardKeyStateData>(data => this.keyStates[data.Key] = data.IsDown);

        keyboardDataReactable.Subscribe(keyboardChangedSubscription);

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

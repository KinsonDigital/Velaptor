// <copyright file="Keyboard.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using Carbonate;
using Carbonate.Core;
using Guards;
using ReactableData;
using Velaptor.Exceptions;

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
    /// <param name="reactable">Sends and receives push notifications.</param>
    public Keyboard(IReactable<IReceiveReactor> reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        var keyboardStateChangeName = this.GetExecutionMemberName(nameof(NotificationIds.KeyboardStateChangedId));
        this.unsubscriber = reactable.Subscribe(new ReceiveReactor(
            eventId: NotificationIds.KeyboardStateChangedId,
            name: keyboardStateChangeName,
            onReceiveMsg: msg =>
            {
                var data = msg.GetData<KeyboardKeyStateData>();

                if (data is null)
                {
                    throw new PushNotificationException($"{nameof(Keyboard)}.Constructor()", NotificationIds.KeyboardStateChangedId);
                }

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

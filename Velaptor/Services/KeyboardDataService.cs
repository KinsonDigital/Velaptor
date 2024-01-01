// <copyright file="KeyboardDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using Carbonate.Fluent;
using Carbonate.OneWay;
using Input;
using ReactableData;

/// <inheritdoc/>
internal sealed class KeyboardDataService : IKeyboardDataService
{
    private static readonly int Capacity = Enum.GetNames(typeof(KeyCode)).Length;
    private readonly Dictionary<KeyCode, bool> keyStates = new (Capacity);
    private readonly IDisposable unsubscriber;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyboardDataService"/> class.
    /// </summary>
    /// <param name="keyboardDataReactable">Receives notifications of keyboard key state changes.</param>
    public KeyboardDataService(IPushReactable<KeyboardKeyStateData> keyboardDataReactable)
    {
        ArgumentNullException.ThrowIfNull(keyboardDataReactable);

        var keyboardChangedSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.KeyboardStateChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.KeyboardStateChangedId)))
            .BuildOneWayReceive<KeyboardKeyStateData>(data => this.keyStates[data.Key] = data.IsDown);

        this.unsubscriber = keyboardDataReactable.Subscribe(keyboardChangedSubscription);

        InitializeKeyStates();
    }

    /// <inheritdoc/>
    public Dictionary<KeyCode, bool> GetKeyStates() => this.keyStates;

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.unsubscriber.Dispose();

        this.isDisposed = true;
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

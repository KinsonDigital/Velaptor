// <copyright file="KeyboardDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Carbonate;
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

        this.unsubscriber = keyboardDataReactable.CreateOneWayReceive(
            PushNotifications.KeyboardStateChangedId,
            (data) =>
            {
                ref var stateRef = ref CollectionsMarshal.GetValueRefOrNullRef(this.keyStates, data.Key);
                if (!Unsafe.IsNullRef(ref stateRef))
                {
                    stateRef = data.IsDown;
                }
            },
            () => this.unsubscriber?.Dispose());

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
    /// Initializes all the available keys and default states.
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

// <copyright file="IKeyboardDataService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.Immutable;
using Input;

/// <summary>
/// Holds the most updated state of the keyboard.
/// </summary>
internal interface IKeyboardDataService : IDisposable
{
    /// <summary>
    /// Gets the state of all the keys on the keyboard.
    /// </summary>
    /// <returns>The state of the keys.</returns>
    ImmutableArray<(KeyCode, bool)> GetKeyStates();
}

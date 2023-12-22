// <copyright file="KeyboardDataServiceFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace KeyboardPerf;

using System.Collections.Immutable;
using Velaptor.Input;
using Velaptor.Services;

/// <summary>
/// Used for performance testing.
/// </summary>
internal sealed class KeyboardDataServiceFake : IKeyboardDataService
{
    /// <summary>
    /// Used for performance testing.
    /// </summary>
    /// <returns>The data.</returns>
    public ImmutableArray<(KeyCode, bool)> GetKeyStates() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}

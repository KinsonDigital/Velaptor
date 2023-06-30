// <copyright file="KeyEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds information about a keyboard key event.
/// </summary>
public readonly record struct KeyEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyEventArgs"/> struct.
    /// </summary>
    /// <param name="key">The keyboard key related to the event.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "Reported incorrectly.  This is a struct, not a class")]
    public KeyEventArgs(KeyCode key) => Key = key;

    /// <summary>
    /// Gets the key that was pushed into the down position or lifted into the up position.
    /// </summary>
    public KeyCode Key { get; }
}

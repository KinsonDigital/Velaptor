// <copyright file="FreeTypeErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.FreeType;

using System;
using Guards;

/// <summary>
/// Occurs when there is an error message related to the <c>FreeType</c> font library.
/// </summary>
internal sealed class FreeTypeErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FreeTypeErrorEventArgs"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public FreeTypeErrorEventArgs(string errorMessage)
    {
        ArgumentException.ThrowIfNullOrEmpty(errorMessage);
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the error message that occured.
    /// </summary>
    public string ErrorMessage { get; }
}

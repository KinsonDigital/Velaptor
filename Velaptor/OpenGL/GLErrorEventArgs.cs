// <copyright file="GLErrorEventArgs.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Guards;

namespace Velaptor.OpenGL;

/// <summary>
/// Holds information about OpenGL errors that occur.
/// </summary>
internal sealed class GLErrorEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GLErrorEventArgs"/> class.
    /// </summary>
    /// <param name="errorMessage">The error message.</param>
    public GLErrorEventArgs(string errorMessage)
    {
        EnsureThat.StringParamIsNotNullOrEmpty(errorMessage);
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Gets the OpenGL error message.
    /// </summary>
    public string ErrorMessage { get; }
}

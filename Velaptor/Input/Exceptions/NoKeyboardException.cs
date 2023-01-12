// <copyright file="NoKeyboardException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions;

using System;

/// <summary>
/// Occurs when a keyboard has not been detected in the system.
/// </summary>
public class NoKeyboardException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
    /// </summary>
    public NoKeyboardException()
        : base("No keyboard detected.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoKeyboardException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NoKeyboardException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

// <copyright file="NoMouseException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions;

using System;

/// <summary>
/// Occurs when a mouse has not been detected in the system.
/// </summary>
public class NoMouseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoMouseException"/> class.
    /// </summary>
    public NoMouseException()
        : base($"No mouse detected.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMouseException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NoMouseException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMouseException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public NoMouseException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

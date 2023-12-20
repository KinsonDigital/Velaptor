// <copyright file="SystemDisplayException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;

/// <summary>
/// Occurs when there is an issue with one of the system displays.
/// </summary>
public sealed class SystemDisplayException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayException"/> class.
    /// </summary>
    public SystemDisplayException()
        : base("There was an issue with one of the system displays.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SystemDisplayException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public SystemDisplayException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

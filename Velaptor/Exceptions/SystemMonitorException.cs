// <copyright file="SystemMonitorException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;

namespace Velaptor.Exceptions;

/// <summary>
/// Occurs when there is an issue with one of the system monitors.
/// </summary>
public class SystemMonitorException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
    /// </summary>
    public SystemMonitorException()
        : base($"There was an issue with one of the system monitors.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SystemMonitorException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public SystemMonitorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

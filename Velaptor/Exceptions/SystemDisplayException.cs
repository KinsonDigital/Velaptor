// <copyright file="SystemDisplayException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Occurs when there is an issue with one of the system monitors.
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemDisplayException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private SystemDisplayException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

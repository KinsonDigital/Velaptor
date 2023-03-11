// <copyright file="NoMouseException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Occurs when a mouse has not been detected in the system.
/// </summary>
[Serializable]
public sealed class NoMouseException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NoMouseException"/> class.
    /// </summary>
    public NoMouseException()
        : base("No mouse detected.")
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

    /// <summary>
    /// Initializes a new instance of the <see cref="NoMouseException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private NoMouseException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

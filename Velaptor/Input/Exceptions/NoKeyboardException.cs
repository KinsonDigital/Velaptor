// <copyright file="NoKeyboardException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Occurs when a keyboard has not been detected in the system.
/// </summary>
[Serializable]
public sealed class NoKeyboardException : Exception
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

    /// <summary>
    /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    [ExcludeFromCodeCoverage(Justification = "No implementation to test")]
    private NoKeyboardException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

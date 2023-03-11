// <copyright file="BufferNotInitializedException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when a buffer has not been initialized.
/// </summary>
[Serializable]
public sealed class BufferNotInitializedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
    /// </summary>
    public BufferNotInitializedException()
        : base("The buffer has not been initialized.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="bufferName">The name of the buffer.</param>
    public BufferNotInitializedException(string message, string bufferName = "")
        : base($"{(string.IsNullOrEmpty(bufferName) ? string.Empty : $"{bufferName} ")}{message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public BufferNotInitializedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private BufferNotInitializedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

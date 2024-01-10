// <copyright file="GLException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions;

using System;

/// <summary>
/// Thrown when an error has been thrown by OpenGL.
/// </summary>
public sealed class GLException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GLException"/> class.
    /// </summary>
    public GLException()
        : base("OpenGL Error.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GLException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="bufferName">The name of the buffer.</param>
    public GLException(string message, string bufferName = "")
        : base($"{(string.IsNullOrEmpty(bufferName) ? string.Empty : $"{bufferName} ")}{message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GLException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public GLException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

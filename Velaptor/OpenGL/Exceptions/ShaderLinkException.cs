// <copyright file="ShaderLinkException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue linking a shader.
/// </summary>
public sealed class ShaderLinkException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderLinkException"/> class.
    /// </summary>
    public ShaderLinkException()
        : base("The shader could not be linked.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderLinkException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="bufferName">The name of the buffer.</param>
    public ShaderLinkException(string message, string bufferName = "")
        : base($"{(string.IsNullOrEmpty(bufferName) ? string.Empty : $"{bufferName} ")}{message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderLinkException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public ShaderLinkException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

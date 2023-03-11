// <copyright file="ShaderNotInitializedException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when a shader has not been initialized.
/// </summary>
[Serializable]
public sealed class ShaderNotInitializedException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderNotInitializedException"/> class.
    /// </summary>
    public ShaderNotInitializedException()
        : base("The shader has not been initialized.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderNotInitializedException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="shaderName">The name of the shader.</param>
    public ShaderNotInitializedException(string message, string shaderName = "")
        : base($"{(string.IsNullOrEmpty(shaderName) ? string.Empty : $"{shaderName} ")}{message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderNotInitializedException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public ShaderNotInitializedException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderNotInitializedException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private ShaderNotInitializedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

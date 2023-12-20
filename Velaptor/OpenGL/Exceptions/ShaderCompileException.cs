// <copyright file="ShaderCompileException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue compiling a shader.
/// </summary>
public sealed class ShaderCompileException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
    /// </summary>
    public ShaderCompileException()
        : base("The shader could not be compiled.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="bufferName">The name of the buffer.</param>
    public ShaderCompileException(string message, string bufferName = "")
        : base($"{(string.IsNullOrEmpty(bufferName) ? string.Empty : $"{bufferName} ")}{message}")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderCompileException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public ShaderCompileException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

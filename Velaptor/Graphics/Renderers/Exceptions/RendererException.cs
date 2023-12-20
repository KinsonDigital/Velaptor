// <copyright file="RendererException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers.Exceptions;

using System;

/// <summary>
/// Thrown when there is a renderer type of issue.
/// </summary>
public sealed class RendererException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RendererException"/> class.
    /// </summary>
    public RendererException()
        : base("There was an issue with the renderer.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public RendererException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public RendererException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

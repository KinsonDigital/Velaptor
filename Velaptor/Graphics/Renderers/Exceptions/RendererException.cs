// <copyright file="RendererException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics.Renderers.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when there is a renderer type of issue.
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="RendererException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private RendererException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

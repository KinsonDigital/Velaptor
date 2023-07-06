// <copyright file="InvalidRenderEffectsException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;
using Graphics;

/// <summary>
/// Thrown when an invalid <see cref="RenderEffects"/> value is used.
/// </summary>
[Serializable]
public sealed class InvalidRenderEffectsException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
    /// </summary>
    public InvalidRenderEffectsException()
        : base($"{nameof(RenderEffects)} value invalid.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public InvalidRenderEffectsException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public InvalidRenderEffectsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidRenderEffectsException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private InvalidRenderEffectsException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

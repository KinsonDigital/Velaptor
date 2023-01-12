// <copyright file="AtlasException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue with an atlas.
/// </summary>
public sealed class AtlasException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasException"/> class.
    /// </summary>
    public AtlasException()
        : base("There was an issue with the atlas.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public AtlasException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public AtlasException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

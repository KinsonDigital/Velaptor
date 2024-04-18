// <copyright file="FontException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue loading fonts.
/// </summary>
public sealed class FontException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FontException"/> class.
    /// </summary>
    public FontException()
        : base("There was an issue with using or processing the font.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public FontException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public FontException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

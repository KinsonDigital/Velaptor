// <copyright file="CachingException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue caching items.
/// </summary>
public class CachingException : Exception
public sealed class CachingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingException"/> class.
    /// </summary>
    public CachingException()
        : base("There was an issue caching the item.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CachingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public CachingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}

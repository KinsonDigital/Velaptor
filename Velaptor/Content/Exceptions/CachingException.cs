// <copyright file="CachingException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when there is an issue caching items.
/// </summary>
[Serializable]
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

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private CachingException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

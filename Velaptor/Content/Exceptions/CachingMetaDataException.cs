// <copyright file="CachingMetaDataException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions;

using System;
using System.Runtime.Serialization;
using System.Security;

/// <summary>
/// Thrown when there is an issue with processing metadata, when loading fonts during the caching process.
/// </summary>
[Serializable]
public sealed class CachingMetaDataException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
    /// </summary>
    public CachingMetaDataException()
        : base("There was an issue with caching the metadata.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public CachingMetaDataException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public CachingMetaDataException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> to populate the data.</param>
    /// <param name="context">The destination (see <see cref="StreamingContext"/>) for this serialization.</param>
    /// <exception cref="SecurityException">The caller does not have the required permissions.</exception>
    private CachingMetaDataException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}

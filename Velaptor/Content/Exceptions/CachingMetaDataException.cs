// <copyright file="CachingMetaDataException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;

namespace Velaptor.Content.Exceptions;

/// <summary>
/// Thrown when there is an issue with processing metadata during the caching process when loading fonts.
/// </summary>
public class CachingMetaDataException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
    /// </summary>
    public CachingMetaDataException()
        : base($"There was an issue with caching the metadata.")
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
}

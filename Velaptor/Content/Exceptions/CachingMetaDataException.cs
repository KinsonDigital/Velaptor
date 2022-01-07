// <copyright file="CachingMetaDataException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when there is an issue caching items.
    /// </summary>
    public class CachingMetaDataException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingMetaDataException"/> class.
        /// </summary>
        public CachingMetaDataException()
            : base($"There was an issue with the caching meta data.")
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
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public CachingMetaDataException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

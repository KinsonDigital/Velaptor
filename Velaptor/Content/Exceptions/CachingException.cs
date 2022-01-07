// <copyright file="CachingException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when there is an issue caching items.
    /// </summary>
    public class CachingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachingException"/> class.
        /// </summary>
        public CachingException()
            : base($"There was an issue caching the item.")
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
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public CachingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

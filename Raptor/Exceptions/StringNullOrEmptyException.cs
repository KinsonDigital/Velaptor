// <copyright file="StringNullOrEmptyException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when a string is null or empty.
    /// </summary>
    public class StringNullOrEmptyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class.
        /// </summary>
        public StringNullOrEmptyException()
            : base("The string must not be null or empty.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public StringNullOrEmptyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringNullOrEmptyException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     null reference if no inner exception is specified.
        /// </param>
        public StringNullOrEmptyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

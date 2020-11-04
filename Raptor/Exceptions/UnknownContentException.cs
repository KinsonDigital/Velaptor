// <copyright file="UnknownContentException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Exceptions
{
    using System;

    /// <summary>
    /// The exception that is thrown when a particular type of content is unknown.
    /// </summary>
    public class UnknownContentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownContentException"/> class.
        /// </summary>
        public UnknownContentException()
            : base("Content unknown.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownContentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public UnknownContentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownContentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public UnknownContentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

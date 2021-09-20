// <copyright file="NoMouseException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when no mouse has been detected in the system.
    /// </summary>
    public class NoMouseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoMouseException"/> class.
        /// </summary>
        public NoMouseException()
            : base($"No mouse detected.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoMouseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoMouseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoMouseException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public NoMouseException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

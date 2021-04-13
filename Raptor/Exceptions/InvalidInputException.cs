// <copyright file="InvalidInputException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when invalid input has occured.
    /// </summary>
    public class InvalidInputException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
        /// </summary>
        public InvalidInputException()
            : base($"Invalid Input")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidInputException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidInputException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public InvalidInputException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

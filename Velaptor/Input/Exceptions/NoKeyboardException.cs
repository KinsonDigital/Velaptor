// <copyright file="NoKeyboardException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Input.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when no keyboard has been detected in the system.
    /// </summary>
    public class NoKeyboardException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
        /// </summary>
        public NoKeyboardException()
            : base($"No keyboard detected.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public NoKeyboardException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoKeyboardException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public NoKeyboardException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

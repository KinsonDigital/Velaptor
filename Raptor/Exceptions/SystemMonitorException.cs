// <copyright file="SystemMonitorException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when there is an issue with one of the system monitors.
    /// </summary>
    public class SystemMonitorException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
        /// </summary>
        public SystemMonitorException()
            : base($"There was an issue with one of the system monitors.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SystemMonitorException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemMonitorException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public SystemMonitorException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

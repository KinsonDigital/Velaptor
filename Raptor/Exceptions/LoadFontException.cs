// <copyright file="LoadFontException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when there is an issue loading fonts.
    /// </summary>
    public class LoadFontException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadFontException"/> class.
        /// </summary>
        public LoadFontException()
            : base($"There was an issue loading the font.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadFontException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LoadFontException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadFontException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public LoadFontException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

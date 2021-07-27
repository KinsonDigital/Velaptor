// <copyright file="LoadContentException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when an issue occurs loading content.
    /// </summary>
    public class LoadContentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadContentException"/> class.
        /// </summary>
        public LoadContentException()
            : base($"There was an issue loading the content.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadContentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LoadContentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadContentException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public LoadContentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

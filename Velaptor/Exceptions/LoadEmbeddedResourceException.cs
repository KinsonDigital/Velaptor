// <copyright file="LoadEmbeddedResourceException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when something goes wrong with loading an embedded resource.
    /// </summary>
    public class LoadEmbeddedResourceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadEmbeddedResourceException"/> class.
        /// </summary>
        public LoadEmbeddedResourceException()
            : base("Issue loading the embedded resource.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadEmbeddedResourceException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public LoadEmbeddedResourceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadEmbeddedResourceException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The <see cref="Exception"/> instance that caused the current exception.
        /// </param>
        public LoadEmbeddedResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

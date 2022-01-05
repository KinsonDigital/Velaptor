// <copyright file="LoadTextureException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when there is an issue loading textures.
    /// </summary>
    public class LoadTextureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTextureException"/> class.
        /// </summary>
        public LoadTextureException()
            : base($"There was an issue loading the texture.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTextureException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public LoadTextureException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadTextureException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception,
        ///     or a null reference if no inner exception is specified.
        /// </param>
        public LoadTextureException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

// <copyright file="BufferNotInitializedException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Exceptions
{
    using System;

    /// <summary>
    /// Thrown when a buffer has not been initialized.
    /// </summary>
    public class BufferNotInitializedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
        /// </summary>
        public BufferNotInitializedException()
            : base("The buffer has not been initialized.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="bufferName">The name of the buffer.</param>
        public BufferNotInitializedException(string message, string bufferName = "")
            : base($"{(string.IsNullOrEmpty(bufferName) ? string.Empty : $"{bufferName} ")}{message}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">
        ///     The <see cref="Exception"/> instance that caused the current exception.
        /// </param>
        public BufferNotInitializedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}


// <copyright file="SoundSourceDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Audio.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when an OpenAL sound sources does not exist.
    /// </summary>
    internal class SoundSourceDoesNotExistException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SoundSourceDoesNotExistException"/> class.
        /// </summary>
        public SoundSourceDoesNotExistException()
            : base("Sound source does not exist.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundSourceDoesNotExistException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SoundSourceDoesNotExistException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundSourceDoesNotExistException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     null reference if no inner exception is specified.
        /// </param>
        public SoundSourceDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

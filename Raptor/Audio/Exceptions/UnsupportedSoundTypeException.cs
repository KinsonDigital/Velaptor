// <copyright file="UnsupportedSoundTypeException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when an unsupported type of sound is used.
    /// </summary>
    public class UnsupportedSoundTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedSoundTypeException"/> class.
        /// </summary>
        public UnsupportedSoundTypeException()
            : base("Unsupported sound type.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedSoundTypeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public UnsupportedSoundTypeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedSoundTypeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="fileExtension">The file extension of the sound type.</param>
        public UnsupportedSoundTypeException(string message, string fileExtension)
            : base($"Unsupported sound type: '{fileExtension}'\n{message}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsupportedSoundTypeException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     null reference if no inner exception is specified.
        /// </param>
        public UnsupportedSoundTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

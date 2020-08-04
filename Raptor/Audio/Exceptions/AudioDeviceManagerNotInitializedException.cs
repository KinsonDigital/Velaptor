// <copyright file="AudioDeviceManagerNotInitializedException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA1064 // Exceptions should be public
namespace Raptor.Audio.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when the <see cref="AudioDeviceManager"/> has not been initialized.
    /// </summary>
    /// <remarks>This is done by invoking the <see cref="AudioDeviceManager.InitDevice(string?)"/> method.</remarks>
    internal class AudioDeviceManagerNotInitializedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceManagerNotInitializedException"/> class.
        /// </summary>
        public AudioDeviceManagerNotInitializedException()
            : base("The audio device manager has not been initialized.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceManagerNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AudioDeviceManagerNotInitializedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceManagerNotInitializedException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     null reference if no inner exception is specified.
        /// </param>
        public AudioDeviceManagerNotInitializedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

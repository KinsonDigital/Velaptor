// <copyright file="AudioDeviceDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when an audio device does not exist.
    /// </summary>
    public class AudioDeviceDoesNotExistException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceDoesNotExistException"/> class.
        /// </summary>
        public AudioDeviceDoesNotExistException()
            : base("The audio device does not exist.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceDoesNotExistException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public AudioDeviceDoesNotExistException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceDoesNotExistException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="deviceName">The name of the device.</param>
        public AudioDeviceDoesNotExistException(string message, string deviceName)
            : base($"Device Name: {deviceName}\n{message}")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioDeviceDoesNotExistException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">
        ///     The exception that is the cause of the current exception, or a
        ///     null reference if no inner exception is specified.
        /// </param>
        public AudioDeviceDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

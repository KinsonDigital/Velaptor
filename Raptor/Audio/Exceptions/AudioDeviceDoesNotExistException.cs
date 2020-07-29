// <copyright file="AudioDeviceDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    // TODO: Improve this exception by adding constructor for custom message and also provide default message using device name
    public class AudioDeviceDoesNotExistException : Exception
    {
        public AudioDeviceDoesNotExistException()
            : base("The audio device does not exist.")
        {
        }

        public AudioDeviceDoesNotExistException(string message)
            : base(message)
        {
        }

        public AudioDeviceDoesNotExistException(string message, string deviceName)
            : base($"Device Name: {deviceName}\n{message}")
        {
        }

        public AudioDeviceDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

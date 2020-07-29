// <copyright file="AudioDeviceMangerNotInitializedException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    public class AudioDeviceManagerNotInitializedException : Exception
    {
        public AudioDeviceManagerNotInitializedException()
            : base("asdf")
        {
        }

        public AudioDeviceManagerNotInitializedException(string message)
            : base(message)
        {
        }

        public AudioDeviceManagerNotInitializedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

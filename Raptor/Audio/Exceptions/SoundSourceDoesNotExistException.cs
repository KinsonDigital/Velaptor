// <copyright file="SoundSourceDoesNotExistException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    internal class SoundSourceDoesNotExistException : Exception
    {
        public SoundSourceDoesNotExistException()
            : base("asdf")
        {
        }

        public SoundSourceDoesNotExistException(string message)
            : base(message)
        {
        }

        public SoundSourceDoesNotExistException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

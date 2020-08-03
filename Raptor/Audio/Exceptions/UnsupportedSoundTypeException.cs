using System;

namespace Raptor.Audio.Exceptions
{
    public class UnsupportedSoundTypeException : Exception
    {
        public UnsupportedSoundTypeException(string message) : base(message)
        {
        }

        public UnsupportedSoundTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UnsupportedSoundTypeException()
        {
        }
    }
}

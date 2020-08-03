using System;

namespace Raptor.Exceptions
{
    public class StringNullOrEmptyException : Exception
    {
        public StringNullOrEmptyException(string message) : base(message)
        {
        }

        public StringNullOrEmptyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public StringNullOrEmptyException()
            : base("The string must not be null or empty.")
        {
        }
    }
}

#pragma warning disable CA1303 // Do not pass literals as localized parameters
namespace Raptor.Audio.Exceptions
{
    using System;

    /// <summary>
    /// Occurs when there is an issue setting an OpenAL context as the current context.
    /// </summary>
    public class SettingContextCurrentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingContextCurrentException"/> class.
        /// </summary>
        public SettingContextCurrentException()
            : base("There was an issue setting the audio context as the current context.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingContextCurrentException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public SettingContextCurrentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingContextCurrentException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public SettingContextCurrentException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

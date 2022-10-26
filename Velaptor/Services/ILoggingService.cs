// <copyright file="ILoggingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    /// <summary>
    /// Provides logging services to the console and/or a file.
    /// </summary>
    internal interface ILoggingService
    {
        /// <summary>
        /// Logs the given <paramref name="msg"/> as an information type log.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        void Info(string msg);

        /// <summary>
        /// Logs the given <paramref name="msg"/> as a warning type log.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        void Warning(string msg);

        /// <summary>
        /// Logs the given <paramref name="msg"/> as an error type log.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        void Error(string msg);

        /// <summary>
        /// Logs the given <paramref name="msg"/> as an event type log.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="msg">The message to log.</param>
        void Event(string eventName, string msg);
    }
}

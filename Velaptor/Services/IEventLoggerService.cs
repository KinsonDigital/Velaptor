// <copyright file="IEventLoggerService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    /// <summary>
    /// Logs events to the console and to a file.
    /// </summary>
    internal interface IEventLoggerService
    {
        /// <summary>
        /// Logs an event with the given <paramref name="eventName"/> and event <paramref name="msg"/> to
        /// the console and/or file.
        /// </summary>
        /// <param name="eventName">The name of the event.</param>
        /// <param name="msg">The message to log.</param>
        void Event(string eventName, string msg);
    }
}

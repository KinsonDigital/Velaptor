// <copyright file="PushNotificationException.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Exceptions;

using System;

/// <summary>
/// Thrown when there is an issue with the push notification system.
/// </summary>
internal sealed class PushNotificationException : Exception
public sealed class PushNotificationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PushNotificationException"/> class.
    /// </summary>
    public PushNotificationException()
        : base("There was an issue with the push notification.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushNotificationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PushNotificationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushNotificationException"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">
    ///     The <see cref="Exception"/> instance that caused the current exception.
    /// </param>
    public PushNotificationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PushNotificationException"/> class.
    /// </summary>
    /// <param name="subscriberSrc">The source of where the push notification came from.</param>
    /// <param name="subscriptionId">The subscription ID.</param>
    public PushNotificationException(string subscriberSrc, Guid subscriptionId)
        : base($"There was an issue with the '{subscriberSrc}' subscription source for subscription ID '{subscriptionId}'.")
    {
    }
}

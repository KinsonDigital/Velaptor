// <copyright file="IReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.Core
{
    using System;

    /// <summary>
    /// Defines a provider for push-based notification.
    /// </summary>
    /// <typeparam name="T">The information sent with the push notification.</typeparam>
    public interface IReactable<T> : IDisposable
    {
        /// <summary>
        /// Notifies the provider that an reactor is to receive notifications.
        /// </summary>
        /// <param name="reactor">The object that is to receive notifications.</param>
        /// <returns>
        ///     A reference to an interface that allows reactors to stop receiving
        ///     notifications before the provider has finished sending them.
        /// </returns>
        IDisposable Subscribe(IReactor<T> reactor);

        /// <summary>
        /// Pushes a single notification with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data to send with the push notification.</param>
        /// <param name="unsubscribeAfterProcessing">Unsubscribes all of the reactors after the notification has been pushed.</param>
        void PushNotification(T data, bool unsubscribeAfterProcessing = false);

        /// <summary>
        /// Unsubscribes all of the currently subscribed reactors.
        /// </summary>
        void UnsubscribeAll();
    }
}

// <copyright file="IObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.Core
{
    using System;

    /// <summary>
    /// Defines a provider for push-based notification.
    /// </summary>
    /// <typeparam name="T">The information sent with the push notification.</typeparam>
    public interface IObservable<T> : System.IObservable<T>
    {
        /// <inheritdoc cref="System.IObservable{T}.Subscribe"/>
        new IDisposable Subscribe(IObserver<T> observer);

        /// <summary>
        /// Pushes a single notification with the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data to send with the push notification.</param>
        void PushNotification(T data);
    }
}

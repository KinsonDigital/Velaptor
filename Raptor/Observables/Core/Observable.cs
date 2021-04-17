// <copyright file="Observable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Observables.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines a provider for push-based notifications.
    /// </summary>
    /// <typeparam name="T">The information related to the notification.</typeparam>
    public abstract class Observable<T> : IObservable<T>, IDisposable
    {
        private readonly List<IObserver<T>> observers = new List<IObserver<T>>();
        private bool isDisposed;

        /// <summary>
        /// Gets the list of observers that are subscribed to this <see cref="Observable{T}"/>.
        /// </summary>
        public ReadOnlyCollection<IObserver<T>> Observers => new ReadOnlyCollection<IObserver<T>>(this.observers);

        /// <summary>
        /// Subscribes the given <paramref name="observer"/> to get push notifications from this <see cref="Observable{T}"/>.
        /// </summary>
        /// <param name="observer">The observer to subscribe.</param>
        /// <returns>The unsubscriber to use to unsubscribe the <see cref="IObserver{T}"/> from this <see cref="Observable{T}"/>.</returns>
        public virtual IDisposable Subscribe(IObserver<T> observer)
        {
            if (!this.observers.Contains(observer))
            {
                this.observers.Add(observer);
            }

            return new ObserverUnsubscriber<T>(this.observers, observer);
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.observers.Clear();
                }

                this.isDisposed = true;
            }
        }
    }
}

// <copyright file="Reactor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.Core
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines a provider for push-based notifications.
    /// </summary>
    /// <typeparam name="TData">The data to send with the notification.</typeparam>
    public abstract class Reactor<TData> : IReactor<TData>
    {
        private readonly List<IObserver<TData>> observers = new ();
        private bool isDisposed;

        /// <summary>
        /// Gets the list of observers that are subscribed to this <see cref="Reactor{TData}"/>.
        /// </summary>
        public ReadOnlyCollection<IObserver<TData>> Observers => new (this.observers);

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IObserver<TData> observer)
        {
            if (!this.observers.Contains(observer))
            {
                this.observers.Add(observer);
            }

            return new ObserverUnsubscriber<TData>(this.observers, observer);
        }

        /// <inheritdoc/>
        public abstract void PushNotification(TData data, bool unsubscribeAfterProcessing = false);

        /// <inheritdoc/>
        public void UnsubscribeAll() => this.observers.Clear();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.observers.Clear();
            }

            this.isDisposed = true;
        }
    }
}

// <copyright file="Reactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.Core
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Velaptor.Guards;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Defines a provider for push-based notifications.
    /// </summary>
    /// <typeparam name="TData">The data to send with the notification.</typeparam>
    public abstract class Reactable<TData> : IReactable<TData>
    {
        private readonly List<IReactor<TData>> reactors = new ();
        private bool isDisposed;
        private bool notificationsEnded;

        /// <summary>
        /// Gets the list of reactors that are subscribed to this <see cref="Reactable{TData}"/>.
        /// </summary>
        public ReadOnlyCollection<IReactor<TData>> Reactors => new (this.reactors);

        /// <inheritdoc/>
        public virtual IDisposable Subscribe(IReactor<TData> reactor)
        {
            EnsureThat.ParamIsNotNull(reactor);

            if (!this.reactors.Contains(reactor))
            {
                this.reactors.Add(reactor);
            }

            return new ReactorUnsubscriber<TData>(this.reactors, reactor);
        }

        /// <inheritdoc/>
        public abstract void PushNotification(TData data);

        /// <inheritdoc/>
        public void EndNotifications()
        {
            if (this.notificationsEnded)
            {
                return;
            }

            // ReSharper disable ForCanBeConvertedToForeach
            /* Keep this loop as a for-loop.  Do not convert to for-each.
             * This is due to the Dispose() method possibly being called during
             * iteration of the reactors list which will cause an exception.
            */
            for (var i = 0; i < this.reactors.Count; i++)
            {
                this.reactors[i].OnCompleted();
            }

            // ReSharper restore ForCanBeConvertedToForeach
            this.notificationsEnded = true;
        }

        /// <inheritdoc/>
        public void UnsubscribeAll() => this.reactors.Clear();

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.reactors.Clear();
            }

            this.isDisposed = true;
        }
    }
}

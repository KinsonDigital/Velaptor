// <copyright file="Observer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables.Core
{
    using System;

    /// <summary>
    /// Provides a mechanism for receiving push-based notifications.
    /// </summary>
    /// <typeparam name="T">The information realted to the notification.</typeparam>
    public class Observer<T> : IObserver<T>
    {
        private readonly Action<T>? onNext;
        private readonly Action? onCompleted;
        private readonly Action<Exception>? onError;

        /// <summary>
        /// Initializes a new instance of the <see cref="Observer{T}"/> class.
        /// </summary>
        /// <param name="onNext">Executed when a push notification occurs.</param>
        /// <param name="onCompleted">Executed when the provider has finished sending push-based notifications.</param>
        /// <param name="onError">Executed when the provider experiences an error condition.</param>
        public Observer(Action<T>? onNext = null, Action? onCompleted = null, Action<Exception>? onError = null)
        {
            this.onNext = onNext;
            this.onCompleted = onCompleted;
            this.onError = onError;
        }

        /// <inheritdoc/>
        public virtual void OnNext(T value)
        {
            if (this.onNext is null)
            {
                return;
            }

            this.onNext(value);
        }

        /// <inheritdoc/>
        public virtual void OnCompleted()
        {
            if (this.onCompleted is null)
            {
                return;
            }

            this.onCompleted();
        }

        /// <inheritdoc/>
        public virtual void OnError(Exception error)
        {
            if (this.onError is null)
            {
                return;
            }

            this.onError(error);
        }
    }
}

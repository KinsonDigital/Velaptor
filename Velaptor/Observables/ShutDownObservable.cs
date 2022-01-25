// <copyright file="ShutDownObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications to signal that the application is shutting down.
    /// </summary>
    internal class ShutDownObservable : Observable<bool>
    {
        /// <summary>
        /// Sends a push notification to signal application shutdown.
        /// </summary>
        public void OnShutDown()
        {
            foreach (var observer in Observers)
            {
                observer.OnNext(true);
            }
        }
    }
}

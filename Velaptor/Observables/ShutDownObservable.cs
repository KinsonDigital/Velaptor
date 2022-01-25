// <copyright file="ShutDownObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications to signal that the application is shutting down.
    /// </summary>
    public class ShutDownObservable : Observable<bool>
    {
        /// <summary>
        /// Sends a push notification to signal application shutdown.
        /// </summary>
        /// <param name="data">True to signal that the application is shutting down.</param>
        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Required for proper observable operation.")]
        public override void PushNotification(bool data)
        {
            for (var i = 0; i < Observers.Count; i++)
            {
                Observers[i].OnNext(data);
            }
        }
    }
}

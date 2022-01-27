// <copyright file="DisposeTexturesReactor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications to signal that an object needs to be disposed.
    /// </summary>
    internal class DisposeTexturesReactor : Reactor<DisposeTextureData>
    {
        /// <summary>
        /// Sends a push notification to signal that an object needs to be disposed.
        /// </summary>
        /// <param name="data">The data to send with the notification.</param>
        [SuppressMessage(
            "ReSharper",
            "ForCanBeConvertedToForeach",
            Justification = "Required for proper observable operation.")]
        public override void PushNotification(DisposeTextureData data)
        {
            /* Work from the end to the beginning of the list
               just in case the observable is disposed(removed)
               in the OnNext() method.
             */
            for (var i = Observers.Count - 1; i >= 0; i--)
            {
                Observers[i].OnNext(data);
            }
        }
    }
}

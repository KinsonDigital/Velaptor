// <copyright file="DisposeTexturesObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications to signal that an object needs to be disposed.
    /// </summary>
    internal class DisposeTexturesObservable : Observable<uint>
    {
        /// <summary>
        /// Sends a push notification to signal that an object needs to be disposed.
        /// </summary>
        /// <param name="textureId">The ID of the texture.</param>
        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach", Justification = "Required for proper observable operation.")]
        public override void PushNotification(uint textureId)
        {
            for (var i = 0; i < Observers.Count; i++)
            {
                Observers[i].OnNext(textureId);
            }
        }
    }
}

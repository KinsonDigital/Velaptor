// <copyright file="ShutDownReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.Reactables;

/// <summary>
/// Creates a reactable to send push notifications to signal that the application is shutting down.
/// </summary>
internal sealed class ShutDownReactable : Reactable<ShutDownData>
{
    /// <summary>
    /// Sends a push notification to signal application shutdown.
    /// </summary>
    /// <param name="data">The data to send with the push notification.</param>
    public override void PushNotification(ShutDownData data)
    {
        /* Work from the end to the beginning of the list
           just in case the reactable is disposed(removed)
           in the OnNext() method.
         */
        for (var i = Reactors.Count - 1; i >= 0; i--)
        {
            Reactors[i].OnNext(data);
        }
    }
}

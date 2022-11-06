// <copyright file="KeyboardStateReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Input;
using Velaptor.Reactables.Core;

namespace Velaptor.Reactables;

/// <summary>
/// Creates a reactable to send push notifications to signal that the state of the keyboard has changed.
/// </summary>
internal sealed class KeyboardStateReactable : Reactable<(KeyCode key, bool isDown)>
{
    /// <summary>
    /// Sends a push notification to signal a change to the state of a keyboard key.
    /// </summary>
    /// <param name="data">The data to send with the push notification.</param>
    public override void PushNotification((KeyCode key, bool isDown) data)
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

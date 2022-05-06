// <copyright file="MousePositionReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Reactables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates a reactable to send push notifications to signal that the position of the mouse has changed.
    /// </summary>
    internal class MousePositionReactable : Reactable<(int x, int y)>
    {
        /// <summary>
        /// Sends a push notification to signal a change to the position of the mouse.
        /// </summary>
        /// <param name="data">The data to send with the push notification.</param>
        /// <param name="unsubscribeAfterProcessing">If true, unsubscribes all of the reactors after the notification has been pushed.</param>
        public override void PushNotification((int x, int y) data, bool unsubscribeAfterProcessing = false)
        {
            /* Work from the end to the beginning of the list
               just in case the reactable is disposed(removed)
               in the OnNext() method.
             */
            for (var i = Reactors.Count - 1; i >= 0; i--)
            {
                Reactors[i].OnNext(data);
            }

            if (unsubscribeAfterProcessing)
            {
                UnsubscribeAll();
            }
        }
    }
}

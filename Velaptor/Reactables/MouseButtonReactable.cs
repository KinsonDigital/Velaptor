// <copyright file="MouseButtonReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Input;
    using Velaptor.Reactables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates a reactable to send push notifications to signal changes to the state of the mouse buttons.
    /// </summary>
    internal class MouseButtonReactable : Reactable<(MouseButton button, bool isDown)>
    {
        /// <summary>
        /// Sends a push notification to signal a change to a mouse button.
        /// </summary>
        /// <param name="data">The data to send with the push notification.</param>
        /// <param name="unsubscribeAfterProcessing">If true, unsubscribes all of the reactors after the notification has been pushed.</param>
        public override void PushNotification((MouseButton button, bool isDown) data, bool unsubscribeAfterProcessing = false)
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

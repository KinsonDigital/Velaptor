// <copyright file="OpenGLContextReactor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Observables.Core;
    using Velaptor.Observables.ObservableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications of OpenGL events.
    /// </summary>
    internal class OpenGLContextReactor : Reactor<GLContextData>
    {
        /// <summary>
        /// Sends a push notification that the OpenGL context has been created.
        /// </summary>
        /// <param name="data">The data to send with the notification.</param>
        public override void PushNotification(GLContextData data)
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

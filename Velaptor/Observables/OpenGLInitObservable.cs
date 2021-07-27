// <copyright file="OpenGLInitObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    using Velaptor.Observables.Core;

    /// <summary>
    /// Creates an observable to send push notifications of OpenGL events.
    /// </summary>
    internal class OpenGLInitObservable : Observable<bool>
    {
        /// <summary>
        /// Sends a push notification that OpenGL has been initialized.
        /// </summary>
        public virtual void OnOpenGLInitialized()
        {
            foreach (var observer in Observers)
            {
                observer.OnNext(true);
            }
        }
    }
}

// <copyright file="OpenGLContextObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Observables
{
    using Raptor.Observables.Core;

    /// <summary>
    /// Creates an observable to send push notifications of OpenGL events.
    /// </summary>
    internal class OpenGLContextObservable : Observable<object>
    {
        /// <summary>
        /// Sends a push notification that the OpenGL context has been created.
        /// </summary>
        /// <param name="data">The data to send with the notification.</param>
        public virtual void OnGLContextCreated(object data)
        {
            foreach (var observer in Observers)
            {
                observer.OnNext(data);
            }
        }
    }
}

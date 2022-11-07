// <copyright file="OpenGLInitReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;

namespace Velaptor.Reactables;

/// <summary>
/// Creates an reactable to send push notification that OpenGL is initialized.
/// </summary>
internal sealed class OpenGLInitReactable : Reactable<GLInitData>
{
    /// <summary>
    /// Sends a push notification that OpenGL has been initialized.
    /// </summary>
    /// <param name="data">The data to send with the push notification.</param>
    [SuppressMessage(
        "ReSharper",
        "ForCanBeConvertedToForeach",
        Justification = "Required for proper reactable operation.")]
    public override void PushNotification(GLInitData data)
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

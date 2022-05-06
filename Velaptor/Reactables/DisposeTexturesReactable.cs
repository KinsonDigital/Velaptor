// <copyright file="DisposeTexturesReactable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables
{
    // ReSharper disable RedundantNameQualifier
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates a reactable to send push notifications to signal a texture needs to be disposed.
    /// </summary>
    internal class DisposeTexturesReactable : Reactable<DisposeTextureData>
    {
        /// <summary>
        /// Sends a push notification to dispose of a texture.
        /// </summary>
        /// <param name="data">The data to send with the push notification.</param>
        [SuppressMessage(
            "ReSharper",
            "ForCanBeConvertedToForeach",
            Justification = "Required for proper reactable operation.")]
        public override void PushNotification(DisposeTextureData data)
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
}

// <copyright file="RemoveBatchItemObservable.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Observables
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates an observable to send push notifications to remove items from a batching service.
    /// </summary>
    internal class RemoveBatchItemObservable : Observable<uint>
    {
        /// <summary>
        /// Sends a push notification to remove a batch item from a batch service.
        /// </summary>
        /// <param name="batchItemId">The ID of the batch item to remove.</param>
        public void OnRemoveBatchItem(uint batchItemId)
        {
            foreach (var observer in Observers)
            {
                observer.OnNext(batchItemId);
            }
        }
    }
}

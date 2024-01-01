// <copyright file="PushReactableFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace MousePerf;

using System.Collections.ObjectModel;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Velaptor.ReactableData;

/// <summary>
/// Used for performance testing.
/// </summary>
internal sealed class PushReactableFake : IPushReactable<MouseStateData>
{
    /// <summary>
    /// Gets a value for performance testing.
    /// </summary>
    public ReadOnlyCollection<IReceiveSubscription<MouseStateData>> Subscriptions { get; } =
        Array.Empty<IReceiveSubscription<MouseStateData>>().AsReadOnly();

    /// <summary>
    /// Gets a value for performance testing.
    /// </summary>
    public ReadOnlyCollection<Guid> SubscriptionIds { get; } = Array.Empty<Guid>().AsReadOnly();

    /// <summary>
    /// Used for performance testing.
    /// </summary>
    /// <param name="subscription">Sample subscription.</param>
    /// <returns>The unsubscriber.</returns>
    public IDisposable Subscribe(IReceiveSubscription<MouseStateData> subscription) => new UnsubscriberFake();

    /// <summary>
    /// Used for performance testing.
    /// </summary>
    /// <param name="data">Sample data.</param>
    /// <param name="eventId">Sample id.</param>
    public void Push(in MouseStateData data, Guid eventId) => throw new NotImplementedException();

    /// <summary>
    /// Used for performance testing.
    /// </summary>
    /// <param name="id">Sample id.</param>
    public void Unsubscribe(Guid id) => throw new NotImplementedException();

    /// <summary>
    /// Used for performance testing.
    /// </summary>
    public void UnsubscribeAll() => throw new NotImplementedException();

    /// <inheritdoc/>
    public void Dispose() => throw new NotImplementedException();
}

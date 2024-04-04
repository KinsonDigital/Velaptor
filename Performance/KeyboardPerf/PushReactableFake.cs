// <copyright file="PushReactableFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable UnassignedGetOnlyAutoProperty
namespace KeyboardPerf;

using System.Collections.Immutable;
using Carbonate.Core;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using Velaptor.ReactableData;

/// <summary>
/// Used for performance testing.
/// </summary>
internal sealed class PushReactableFake : IPushReactable<KeyboardKeyStateData>
{
    /// <summary>
    /// Gets a value for performance testing.
    /// </summary>
    public ImmutableArray<IReceiveSubscription<KeyboardKeyStateData>> Subscriptions { get; } =
        Array.Empty<IReceiveSubscription<KeyboardKeyStateData>>().ToImmutableArray();

    /// <summary>
    /// Gets a value for performance testing.
    /// </summary>
    public ImmutableArray<Guid> SubscriptionIds { get; } = Array.Empty<Guid>().ToImmutableArray();

    /// <inheritdoc cref="IReactable{TSubscription}.SubscriptionNames"/>
    public ImmutableArray<string> SubscriptionNames { get; }

    /// <summary>
    /// Used for performance testing.
    /// </summary>
    /// <param name="subscription">Sample subscription.</param>
    /// <returns>The unsubscriber.</returns>
    public IDisposable Subscribe(IReceiveSubscription<KeyboardKeyStateData> subscription) => new UnsubscriberFake();

    /// <inheritdoc cref="IPushable{TIn}.Push"/>
    public void Push(Guid id, in KeyboardKeyStateData data) => throw new NotImplementedException();

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

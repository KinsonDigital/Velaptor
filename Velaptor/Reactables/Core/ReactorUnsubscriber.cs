// <copyright file="ReactorUnsubscriber.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.Core;

using System;
using System.Collections.Generic;
using Guards;

/// <summary>
/// A reactor unsubscriber for unsubscribing from an <see cref="Reactable{TData}"/>.
/// </summary>
/// <typeparam name="T">
///     The type of data that is pushed to all of the subscribed <see cref="Reactor{T}"/>s.
/// </typeparam>
internal sealed class ReactorUnsubscriber<T> : IDisposable
{
    private readonly List<IReactor<T>> reactors;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReactorUnsubscriber{T}"/> class.
    /// </summary>
    /// <param name="reactors">The list of reactor subscriptions.</param>
    /// <param name="reactor">The reactor that has been subscribed.</param>
    internal ReactorUnsubscriber(List<IReactor<T>> reactors, IReactor<T> reactor)
    {
        EnsureThat.ParamIsNotNull(reactors);
        EnsureThat.ParamIsNotNull(reactor);
        this.reactors = reactors;
        Reactor = reactor;
    }

    /// <summary>
    /// Gets the reactors of this unsubscription.
    /// </summary>
    public IReactor<T> Reactor { get; }

    /// <summary>
    /// Gets the total number of current subscriptions that an <see cref="Reactable{TData}"/> has.
    /// </summary>
    public int TotalReactors => this.reactors.Count;

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// </summary>
    /// <param name="disposing">Disposes managed resources when <c>true</c>.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            if (this.reactors.Contains(Reactor))
            {
                this.reactors.Remove(Reactor);
            }
        }

        this.isDisposed = true;
    }
}

// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonate;
using Guards;
using Reactables.Core;
using Reactables.ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class SoundFactory : ISoundFactory
{
    private static readonly Dictionary<uint, string> Sounds = new ();
    private readonly IReactable reactable;
    private readonly IDisposable disposeSoundUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFactory"/> class.
    /// </summary>
    /// <param name="reactable">Sends and receives push notifications.</param>
    /// <param name="shutDownReactable">Sends a push notifications that the application is shutting down.</param>
    public SoundFactory(
        IReactable reactable,
        IReactable<ShutDownData> shutDownReactable)
    {
        EnsureThat.ParamIsNotNull(reactable);
        EnsureThat.ParamIsNotNull(shutDownReactable);

        this.reactable = reactable;
        this.disposeSoundUnsubscriber =
            reactable.Subscribe(new Reactor(
                eventId: NotificationIds.DisposeSoundId,
                onNextMsg: msg =>
                {
                    var data = msg.GetData<DisposeSoundData>();

                    if (data is null)
                    {
                        throw new PushNotificationException($"{nameof(SoundFactory)}.Constructor()", NotificationIds.DisposeSoundId);
                    }

                    Sounds.Remove(data.SoundId);
                },
                onCompleted: () => this.disposeSoundUnsubscriber?.Dispose()));

        this.shutDownUnsubscriber =
            shutDownReactable.Subscribe(new Reactor<ShutDownData>(_ => ShutDown()));
    }

    /// <summary>
    /// Gets a new unique sound ID.
    /// </summary>
    /// <param name="filePath">The file path to the sound.</param>
    /// <returns>The new ID for a sound.</returns>
    public static uint GetNewId(string filePath)
    {
        var newId = Sounds.Count <= 0
            ? 1
            : Sounds.Keys.Max() + 1;

        Sounds.Add(newId, filePath);

        return newId;
    }

    /// <inheritdoc/>
    public ISound Create(string filePath)
    {
        var newId = Sounds.Count <= 0
            ? 1
            : Sounds.Keys.Max() + 1;

        Sounds.Add(newId, filePath);

        return new Sound(this.reactable, filePath, newId);
    }

    /// <summary>
    /// Unsubscribes from the <see cref="IReactable{T}"/> reactors.
    /// </summary>
    private void ShutDown()
    {
        this.disposeSoundUnsubscriber.Dispose();
        this.shutDownUnsubscriber.Dispose();
    }
}

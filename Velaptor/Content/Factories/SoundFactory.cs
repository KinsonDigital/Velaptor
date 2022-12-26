// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonate;
using Guards;
using ReactableData;
using Velaptor.Exceptions;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
internal sealed class SoundFactory : ISoundFactory
{
    private readonly Dictionary<uint, string> sounds = new ();
    private readonly IReactable reactable;
    private readonly IDisposable disposeSoundUnsubscriber;
    private readonly IDisposable shutDownUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFactory"/> class.
    /// </summary>
    /// <param name="reactable">Sends and receives push notifications.</param>
    public SoundFactory(IReactable reactable)
    {
        EnsureThat.ParamIsNotNull(reactable);

        this.reactable = reactable;
        this.disposeSoundUnsubscriber =
            reactable.Subscribe(new Reactor(
                eventId: NotificationIds.SoundDisposedId,
                onNextMsg: msg =>
                {
                    var data = msg.GetData<DisposeSoundData>();

                    if (data is null)
                    {
                        throw new PushNotificationException($"{nameof(SoundFactory)}.Constructor()", NotificationIds.SoundDisposedId);
                    }

                    this.sounds.Remove(data.SoundId);
                }));

        this.shutDownUnsubscriber = reactable.Subscribe(new Reactor(
            eventId: NotificationIds.SystemShuttingDownId,
            onNext: ShutDown));
    }

    /// <inheritdoc/>
    public ReadOnlyDictionary<uint, string> Sounds => new (this.sounds);

    /// <inheritdoc />
    public uint GetNewId(string filePath)
    {
        var newId = this.sounds.Count <= 0
            ? 1
            : this.sounds.Keys.Max() + 1;

        this.sounds.Add(newId, filePath);

        return newId;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the CASL library.")]
    public ISound Create(string filePath)
    {
        var newId = this.sounds.Count <= 0
            ? 1
            : this.sounds.Keys.Max() + 1;

        this.sounds.Add(newId, filePath);

        return new Sound(this.reactable, filePath, newId);
    }

    /// <summary>
    /// Disposes of all sounds.
    /// </summary>
    private void ShutDown()
    {
        this.disposeSoundUnsubscriber.Dispose();
        this.shutDownUnsubscriber.Dispose();
    }
}

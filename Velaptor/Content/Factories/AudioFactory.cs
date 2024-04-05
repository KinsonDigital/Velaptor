// <copyright file="AudioFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonate;
using Carbonate.OneWay;
using Guards;
using ReactableData;
using Velaptor.Factories;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
internal sealed class AudioFactory : IAudioFactory
{
    private readonly Dictionary<uint, string> sounds = new ();
    private readonly IPushReactable<DisposeSoundData> disposeReactable;
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFactory"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public AudioFactory(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.disposeReactable = reactableFactory.CreateDisposeSoundReactable();

        this.unsubscriber = this.disposeReactable.CreateOneWayReceive(
            PushNotifications.SoundDisposedId,
            data => this.sounds.Remove(data.SoundId),
            () => this.unsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test this until the Create() method can be tested.  Waiting for CASL improvements.")]
    public ReadOnlyDictionary<uint, string> Sounds => new (this.sounds);

    /// <inheritdoc/>
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

        return new Sound(this.disposeReactable, filePath, newId);
    }
}

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
/// Creates audio based on the audio file at a location.
/// </summary>
internal sealed class AudioFactory : IAudioFactory
{
    private readonly Dictionary<uint, string> allAudio = new ();
    private readonly IPushReactable<DisposeAudioData> disposeReactable;
    private readonly IDisposable unsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioFactory"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public AudioFactory(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.disposeReactable = reactableFactory.CreateDisposeAudioReactable();

        this.unsubscriber = this.disposeReactable.CreateOneWayReceive(
            PushNotifications.AudioDisposedId,
            data => this.allAudio.Remove(data.AudioId),
            () => this.unsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test this until the Create() method can be tested.  Waiting for CASL improvements.")]
    public ReadOnlyDictionary<uint, string> LoadedAudio => new (this.allAudio);

    /// <inheritdoc/>
    public uint GetNewId(string filePath)
    {
        var newId = this.allAudio.Count <= 0
            ? 1
            : this.allAudio.Keys.Max() + 1;

        this.allAudio.Add(newId, filePath);

        return newId;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with the CASL library.")]
    public IAudio Create(string filePath, AudioBuffer bufferType)

    {
        var cacheId = this.allAudio.Count <= 0
            ? 1
            : this.allAudio.Keys.Max() + 1;

        this.allAudio.Add(cacheId, filePath);

        return new Audio(this.disposeReactable, filePath, cacheId, bufferType);
    }
}

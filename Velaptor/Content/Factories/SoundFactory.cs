// <copyright file="SoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Carbonate.Fluent;
using Carbonate.OneWay;
using Guards;
using ReactableData;
using Velaptor.Factories;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
internal sealed class SoundFactory : ISoundFactory
{
    private readonly Dictionary<uint, string> sounds = new ();
    private readonly IPushReactable<DisposeSoundData> disposeReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundFactory"/> class.
    /// </summary>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public SoundFactory(IReactableFactory reactableFactory)
    {
        EnsureThat.ParamIsNotNull(reactableFactory);

        this.disposeReactable = reactableFactory.CreateDisposeSoundReactable();

        var disposeSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.SoundDisposedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.SoundDisposedId)))
            .BuildOneWayReceive<DisposeSoundData>(data => this.sounds.Remove(data.SoundId));

        this.disposeReactable.Subscribe(disposeSubscription);
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

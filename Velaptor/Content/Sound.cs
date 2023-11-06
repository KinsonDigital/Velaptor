// <copyright file="Sound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using Carbonate.Fluent;
using Carbonate.OneWay;
using CASL;
using Factories;
using ReactableData;
using CASLSound = CASL.Sound;

/// <summary>
/// A single sound that can be played, paused etc.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Waiting for {nameof(CASL)}.{nameof(CASLSound)} implementation changes.")]
public sealed class Sound : ISound
{
    private CASLSound sound = null!;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Sound"/> class.
    /// </summary>
    /// <param name="filePath">The path to the sound file.</param>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public Sound(string filePath)
    {
        ArgumentException.ThrowIfNullOrEmpty(filePath);

        var soundFactory = IoC.Container.GetInstance<ISoundFactory>();
        var disposeReactable = IoC.Container.GetInstance<IPushReactable<DisposeSoundData>>();

        Init(disposeReactable, filePath, soundFactory.GetNewId(filePath));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Sound"/> class.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="filePath">The path to the sound file.</param>
    /// <param name="soundId">The unique ID of the sound.</param>
    internal Sound(IPushReactable<DisposeSoundData> disposeReactable, string filePath, uint soundId) => Init(disposeReactable, filePath, soundId);

    /// <inheritdoc/>
    public uint Id { get; private set; }

    /// <summary>
    /// Gets or sets the volume of the sound.
    /// </summary>
    /// <remarks>
    ///     The only valid value accepted is 0-100. If a value outside of
    ///     this range is used, it will be set within that range.
    /// </remarks>
    public float Volume
    {
        get => this.sound.Volume;
        set => this.sound.Volume = value;
    }

    /// <summary>
    /// Gets the current time position of the sound.
    /// </summary>
    public SoundTime Position => this.sound.Position;

    /// <summary>
    /// Gets the length of the sound.
    /// </summary>
    public SoundTime Length => this.sound.Length;

    /// <summary>
    /// Gets or sets a value indicating whether or not the sound loops back to the beginning once the end has been reached.
    /// </summary>
    public bool IsLooping
    {
        get => this.sound.IsLooping;
        set => this.sound.IsLooping = value;
    }

    /// <summary>
    /// Gets the state of the sound.
    /// </summary>
    public SoundState State => this.sound.State;

    /// <summary>
    /// Gets or sets the play speed to the given value.
    /// </summary>
    /// <param name="value">The speed that the sound should play.</param>
    /// <remarks>
    ///     The valid range of <paramref name="value"/> is between 0.25 and 2.0
    ///     with a <paramref name="value"/> less than 0.25 defaulting to 0.25 and
    ///     with a <paramref name="value"/> greater than 2.0 defaulting to 2.0.
    /// </remarks>
    public float PlaySpeed
    {
        get => this.sound.PlaySpeed;
        set => this.sound.PlaySpeed = value;
    }

    /// <summary>
    /// Gets the name of the sound.
    /// </summary>
    public string Name => this.sound.Name;

    /// <summary>
    /// Gets the fully qualified path to the sound file.
    /// </summary>
    public string FilePath => this.sound.FilePath;

    /// <summary>
    /// Advances the sound forward by the given amount of <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">The amount of seconds to fast forward the sound.</param>
    public void FastForward(float seconds) => this.sound.FastForward(seconds);

    /// <summary>
    /// Pauses the sound.
    /// </summary>
    public void Pause() => this.sound.Pause();

    /// <summary>
    /// Plays the sound.
    /// </summary>
    public void Play() => this.sound.Play();

    /// <summary>
    /// Resets the sound.
    /// </summary>
    /// <remarks>
    ///     This will stop the sound and set the time position back to the beginning.
    /// </remarks>
    public void Reset() => this.sound.Reset();

    /// <summary>
    /// Rewinds the sound by the given amount of <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">The amount of seconds to rewind the sound.</param>
    public void Rewind(float seconds) => this.sound.Rewind(seconds);

    /// <summary>
    /// Sets the time position of the sound to the given value.
    /// </summary>
    /// <param name="seconds">The time position in seconds of where to set the sound.</param>
    public void SetTimePosition(float seconds) => this.sound.SetTimePosition(seconds);

    /// <summary>
    /// Stops the sound.
    /// </summary>
    /// <remarks>This will set the time position back to the beginning.</remarks>
    public void Stop()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.sound.Stop();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose() => Dispose(new DisposeSoundData { SoundId = Id });

    /// <summary>
    /// Initializes the sound.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="filePath">The path to the sound file.</param>
    /// <param name="soundId">The unique ID of the sound.</param>
    private void Init(IPushReactable<DisposeSoundData> disposeReactable, string filePath, uint soundId)
    {
        var disposeSubscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.SoundDisposedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.SoundDisposedId)))
            .BuildOneWayReceive<DisposeSoundData>(Dispose);

        disposeReactable.Subscribe(disposeSubscription);

        this.sound = new CASLSound(filePath);
        Id = soundId;
    }

    /// <summary>
    /// Disposes of the sounds if this sounds <see cref="Id"/> matches the sound ID in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data of the sound to dispose.</param>
    private void Dispose(DisposeSoundData data)
    {
        if (this.isDisposed && Id != data.SoundId)
        {
            return;
        }

        this.sound.Dispose();

        this.isDisposed = true;
    }
}

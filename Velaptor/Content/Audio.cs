// <copyright file="Audio.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using Carbonate;
using Carbonate.OneWay;
using CASL;
using ReactableData;

// TODO: Look into throwing exceptions for prop setters and methods if invoked while disposed

/// <summary>
/// A single sound that can be played, paused etc.
/// </summary>
public sealed class Audio : IAudio
{
    private IDisposable? unsubscriber;
    private CASL.IAudio sound = null!;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Audio"/> class.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="filePath">The path to the sound file.</param>
    /// <param name="soundId">The unique ID of the sound.</param>
    internal Audio(IPushReactable<DisposeSoundData> disposeReactable, string filePath, uint soundId) => Init(disposeReactable, filePath, soundId);

    /// <inheritdoc/>
    public uint Id { get; private set; }

    /// <inheritdoc cref="IAudio"/>
    public float Volume
    {
        get => this.sound.Volume;
        set => this.sound.Volume = value;
    }

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Position => new (0, 0, 0, 0, (int)this.sound.Position.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Length => new (0, 0, 0, 0, (int)this.sound.Length.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public bool IsLooping
    {
        get => this.sound.IsLooping;
        set => this.sound.IsLooping = value;
    }

    /// <inheritdoc cref="IAudio"/>
    public bool IsPlaying => this.sound.State == AudioState.Playing;

    /// <inheritdoc cref="IAudio"/>
    public bool IsPaused => this.sound.State == AudioState.Paused;

    /// <inheritdoc cref="IAudio"/>
    public bool IsStopped => this.sound.State == AudioState.Stopped;

    /// <inheritdoc cref="IAudio"/>
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
    public AudioBuffer BufferType => this.sound.BufferType switch
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
    {
        CASL.BufferType.Full => AudioBuffer.Full,
        CASL.BufferType.Stream => AudioBuffer.Stream,
    };

    /// <inheritdoc cref="IAudio"/>
    public float PlaySpeed
    {
        get => this.sound.PlaySpeed;
        set => this.sound.PlaySpeed = value;
    }

    /// <inheritdoc cref="IAudio"/>
    public string Name => this.sound.Name;

    /// <inheritdoc cref="IAudio"/>
    public string FilePath => this.sound.FilePath;

    /// <inheritdoc cref="IAudio"/>
    public void FastForward(float seconds) => this.sound.FastForward(seconds);

    /// <inheritdoc cref="IAudio"/>
    public void Pause() => this.sound.Pause();

    /// <inheritdoc cref="IAudio"/>
    public void Play() => this.sound.Play();

    /// <inheritdoc cref="IAudio"/>
    public void Rewind(float seconds) => this.sound.Rewind(seconds);

    /// <inheritdoc cref="IAudio"/>
    public void SetTimePosition(float seconds) => this.sound.SetTimePosition(seconds);

    /// <inheritdoc cref="IAudio"/>
    public void Stop()
    {
        // TODO: This is a temporary fix until the next bug release of CASL is done.
        if (this.isDisposed || this.sound.State == AudioState.Stopped)
        {
            return;
        }

        this.sound.Reset();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(new DisposeSoundData { SoundId = Id });

    /// <summary>
    /// Initializes the sound.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="filePath">The path to the sound file.</param>
    /// <param name="soundId">The unique ID of the sound.</param>
    private void Init(IPushReactable<DisposeSoundData> disposeReactable, string filePath, uint soundId)
    {
        this.unsubscriber = disposeReactable.CreateOneWayReceive(
            PushNotifications.SoundDisposedId,
            Dispose,
            () => this.unsubscriber?.Dispose());

        // TODO: Setup the AudioFactory to crate audio files using IoC so this class can finally be tested

        this.sound = new CASL.Audio(filePath, CASL.BufferType.Stream);
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

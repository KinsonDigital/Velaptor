// <copyright file="Audio.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using Carbonate;
using Carbonate.OneWay;
using CASL;
using ReactableData;
using ICASLAudio = CASL.IAudio;

/// <summary>
/// A single audio that can be played, paused etc.
/// </summary>
public sealed class Audio : IAudio
{
    private readonly IDisposable? unsubscriber;
    private readonly ICASLAudio caslAudio;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Audio"/> class.
    /// </summary>
    /// <param name="disposeReactable">Sends and receives push notifications.</param>
    /// <param name="internalAudio">Internal audio.</param>
    /// <param name="audioId">The unique ID of the audio.</param>
    internal Audio(
        IPushReactable<DisposeAudioData> disposeReactable,
        ICASLAudio internalAudio,
        uint audioId)
    {
        ArgumentNullException.ThrowIfNull(disposeReactable);
        ArgumentNullException.ThrowIfNull(internalAudio);

        this.unsubscriber = disposeReactable.CreateOneWayReceive(
            PushNotifications.AudioDisposedId,
            Dispose,
            () => this.unsubscriber?.Dispose());

        this.caslAudio = internalAudio;
        Id = audioId;
    }

    /// <inheritdoc/>
    public uint Id { get; private set; }

    /// <inheritdoc cref="IAudio"/>
    public float Volume
    {
        get => this.caslAudio.Volume;
        set
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("Audio is disposed.");
            }

            this.caslAudio.Volume = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Position => new (0, 0, 0, 0, (int)this.caslAudio.Position.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Length => new (0, 0, 0, 0, (int)this.caslAudio.Length.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public bool IsLooping
    {
        get => this.caslAudio.IsLooping;
        set
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("Audio is disposed.");
            }

            this.caslAudio.IsLooping = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public bool IsPlaying => this.caslAudio.State == AudioState.Playing;

    /// <inheritdoc cref="IAudio"/>
    public bool IsPaused => this.caslAudio.State == AudioState.Paused;

    /// <inheritdoc cref="IAudio"/>
    public bool IsStopped => this.caslAudio.State == AudioState.Stopped;

    /// <inheritdoc cref="IAudio"/>
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
    public AudioBuffer BufferType => this.caslAudio.BufferType switch
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
    {
        CASL.BufferType.Full => AudioBuffer.Full,
        CASL.BufferType.Stream => AudioBuffer.Stream,
    };

    /// <inheritdoc cref="IAudio"/>
    public float PlaySpeed
    {
        get => this.caslAudio.PlaySpeed;
        set
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException("Audio is disposed.");
            }

            this.caslAudio.PlaySpeed = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public string Name => this.caslAudio.Name;

    /// <inheritdoc cref="IAudio"/>
    public string FilePath => this.caslAudio.FilePath;

    /// <inheritdoc cref="IAudio"/>
    public void FastForward(float seconds)
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.FastForward(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void Pause()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.Pause();
    }

    /// <inheritdoc cref="IAudio"/>
    public void Play()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.Play();
    }

    /// <inheritdoc cref="IAudio"/>
    public void Rewind(float seconds)
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.Rewind(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void SetTimePosition(float seconds)
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.SetTimePosition(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void Stop()
    {
        if (this.isDisposed)
        {
            throw new ObjectDisposedException("Audio is disposed.");
        }

        this.caslAudio.Reset();
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    public void Dispose() => Dispose(new DisposeAudioData { AudioId = Id });

    /// <summary>
    /// Disposes of the audio if this audio <see cref="Id"/> matches the audio ID in the given <paramref name="data"/>.
    /// </summary>
    /// <param name="data">The data of the audio to dispose.</param>
    private void Dispose(DisposeAudioData data)
    {
        if (this.isDisposed && Id != data.AudioId)
        {
            return;
        }

        this.caslAudio.Dispose();

        this.isDisposed = true;
    }
}

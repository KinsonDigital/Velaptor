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
        get => this.isDisposed ? 0 : this.caslAudio.Volume;
        set
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
            this.caslAudio.Volume = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Position =>
        this.isDisposed ? default : new TimeSpan(0, 0, 0, 0, (int)this.caslAudio.Position.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public TimeSpan Length =>
        this.isDisposed ? default : new TimeSpan(0, 0, 0, 0, (int)this.caslAudio.Length.Milliseconds);

    /// <inheritdoc cref="IAudio"/>
    public bool IsLooping
    {
        get => !this.isDisposed && this.caslAudio.IsLooping;
        set
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
            this.caslAudio.IsLooping = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public bool IsPlaying => !this.isDisposed && this.caslAudio.State == AudioState.Playing;

    /// <inheritdoc cref="IAudio"/>
    public bool IsPaused => !this.isDisposed && this.caslAudio.State == AudioState.Paused;

    /// <inheritdoc cref="IAudio"/>
    public bool IsStopped => this.isDisposed || this.caslAudio.State == AudioState.Stopped;

    /// <inheritdoc cref="IAudio"/>
    public AudioBuffer BufferType
    {
        get
        {
            if (this.isDisposed)
            {
                return AudioBuffer.Full;
            }

#pragma warning disable CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
            return this.caslAudio.BufferType switch
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type involving an unnamed enum value.
            {
                CASL.BufferType.Full => AudioBuffer.Full,
                CASL.BufferType.Stream => AudioBuffer.Stream,
            };
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public float PlaySpeed
    {
        get => this.isDisposed ? 0 : this.caslAudio.PlaySpeed;
        set
        {
            ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
            this.caslAudio.PlaySpeed = value;
        }
    }

    /// <inheritdoc cref="IAudio"/>
    public string Name => this.isDisposed ? string.Empty : this.caslAudio.Name;

    /// <inheritdoc cref="IAudio"/>
    public string FilePath => this.isDisposed ? string.Empty : this.caslAudio.FilePath;

    /// <inheritdoc cref="IAudio"/>
    public void FastForward(float seconds)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
        this.caslAudio.FastForward(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void Pause()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
        this.caslAudio.Pause();
    }

    /// <inheritdoc cref="IAudio"/>
    public void Play()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
        this.caslAudio.Play();
    }

    /// <inheritdoc cref="IAudio"/>
    public void Rewind(float seconds)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
        this.caslAudio.Rewind(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void SetTimePosition(float seconds)
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
        this.caslAudio.SetTimePosition(seconds);
    }

    /// <inheritdoc cref="IAudio"/>
    public void Stop()
    {
        ObjectDisposedException.ThrowIf(this.isDisposed, typeof(Audio));
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
        if (this.isDisposed || Id != data.AudioId)
        {
            return;
        }

        this.caslAudio.Dispose();

        this.isDisposed = true;
    }
}

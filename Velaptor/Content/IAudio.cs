// <copyright file="IAudio.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;

/// <summary>
/// A single sound that can be played, paused etc.
/// </summary>
public interface IAudio : IContent, IDisposable
{
    /// <summary>
    /// Gets the unique ID of the sound.
    /// </summary>
    uint Id { get; }

    /// <summary>
    /// Gets the name of the content.
    /// </summary>
    new string Name { get; }

    /// <summary>
    /// Gets the fully qualified path to the content file.
    /// </summary>
    new string FilePath { get; }

    /// <summary>
    /// Gets or sets the volume of the audio.
    /// </summary>
    /// <remarks>
    ///     The only valid value accepted is 0-100. If a value outside of
    ///     this range is used, it will be set within that range.
    /// </remarks>
    float Volume { get; set; }

    /// <summary>
    /// Gets the current time position of the audio.
    /// </summary>
    TimeSpan Position { get; }

    /// <summary>
    /// Gets the length of the song.
    /// </summary>
    TimeSpan Length { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the audio loops back to the beginning once the end has been reached.
    /// </summary>
    bool IsLooping { get; set; }

    /// <summary>
    /// Gets a value indicating whether or not the audio is playing.
    /// </summary>
    bool IsPlaying { get; }

    /// <summary>
    /// Gets a value indicating whether or not the audio is paused.
    /// </summary>
    bool IsPaused { get; }

    /// <summary>
    /// Gets a value indicating whether or not the audio is stopped.
    /// </summary>
    bool IsStopped { get; }

    /// <summary>
    /// Gets the type of buffer.
    /// </summary>
    AudioBuffer BufferType { get; }

    /// <summary>
    /// Gets or sets the play speed to the given value.
    /// </summary>
    /// <param name="value">The speed that the audio should play at.</param>
    /// <remarks>
    ///     The valid range of <paramref name="value"/> is between 0.25 and 2.0
    ///     with a <paramref name="value"/> less then 0.25 defaulting to 0.25 and
    ///     with a <paramref name="value"/> greater then 2.0 defaulting to 2.0.
    /// </remarks>
    float PlaySpeed { get; set; }

    /// <summary>
    /// Plays the audio.
    /// </summary>
    void Play();

    /// <summary>
    /// Pauses the audio.
    /// </summary>
    void Pause();

    /// <summary>
    /// Stops the audio playback and resets back to the beginning.
    /// </summary>
    void Stop();

    /// <summary>
    /// Sets the time position of the audio to the given value.
    /// </summary>
    /// <param name="seconds">The time position in seconds of where to set the audio.</param>
    void SetTimePosition(float seconds);

    /// <summary>
    /// Rewinds the audio by the given amount of <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">The amount of seconds to rewind the song.</param>
    void Rewind(float seconds);

    /// <summary>
    /// Fast forwards the audio by the given amount of <paramref name="seconds"/>.
    /// </summary>
    /// <param name="seconds">The amount of seconds to fast forward the song.</param>
    void FastForward(float seconds);
}

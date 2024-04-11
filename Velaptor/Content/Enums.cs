// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// The kinds of buffering used for audio.
/// </summary>
public enum AudioBuffer
{
    /// <summary>
    /// All of the audio data has been loaded into memory.
    /// </summary>
    /// <remarks>
    /// <br/>
    /// Good For:
    ///     <list type="bullet">
    ///         <item>Sounds that need to be played in quick succession such as audio effects.</item>
    ///         <item>Greatly improves the performance with loading audio content.</item>
    ///     </list>
    /// Not Good For:
    ///     <list type="bullet">
    ///         <item>Large audio files that take up much more memory.</item>
    ///         <item>Large audio files take longer when loading as content.</item>
    ///     </list>
    /// <br/>
    /// It is recommend to use <see cref="Full"/> for very short audio effects such as lasers, weapons, etc. and
    /// to use <see cref="Stream"/> for large files such as game music.
    /// </remarks>
    Full,

    /// <summary>
    /// Audio data is streamed from a source during playback.
    /// </summary>
    /// <remarks>
    /// <br/>
    /// Good For:
    ///     <list type="bullet">
    ///         <item>Large audio files that would take up too much memory if loaded all at once.</item>
    ///         <item>Greatly improves the performance with loading audio content.</item>
    ///     </list>
    /// Not Good For:
    ///     <list type="bullet">
    ///         <item>Sounds that need to be played in quick succession such as audio effects.</item>
    ///     </list>
    /// <br/>
    /// It is recommend to use <see cref="Stream"/> for large files such as game music and to use <see cref="Full"/>
    /// for very short audio effects such as lasers, weapons, etc.
    /// </remarks>
    Stream,
}

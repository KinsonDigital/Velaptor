// <copyright file="IAudioFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Collections.ObjectModel;

/// <summary>
/// Creates audio based on the audio file at a location.
/// </summary>
internal interface IAudioFactory
{
    /// <summary>
    /// Gets the list of all the currently loaded audio.
    /// </summary>
    /// <remarks>
    ///     The key is the audio ID assigned by the <see cref="CASL"/> library.
    ///     <br/>
    ///     The value is the fully qualified file path to the audio file.
    /// </remarks>
    ReadOnlyDictionary<uint, string> LoadedAudio { get; }

    /// <summary>
    /// Gets a new unique audio ID.
    /// </summary>
    /// <param name="filePath">The file path to the audio.</param>
    /// <returns>The new ID for the audio.</returns>
    uint GetNewId(string filePath);

    /// <summary>
    /// Creates new audio from an audio file at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the audio file.</param>
    /// <returns>The audio.</returns>
    IAudio Create(string filePath);
}

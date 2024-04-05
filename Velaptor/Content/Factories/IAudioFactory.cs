// <copyright file="IAudioFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Collections.ObjectModel;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
internal interface IAudioFactory
{
    /// <summary>
    /// Gets the list of sounds currently loaded.
    /// </summary>
    /// <remarks>
    ///     The key is the sound ID assigned by the <see cref="CASL"/> library.
    ///     <br/>
    ///     The value is the fully qualified file path to the sound file.
    /// </remarks>
    ReadOnlyDictionary<uint, string> Sounds { get; }

    /// <summary>
    /// Gets a new unique sound ID.
    /// </summary>
    /// <param name="filePath">The file path to the sound.</param>
    /// <returns>The new ID for a sound.</returns>
    uint GetNewId(string filePath);

    /// <summary>
    /// Creates a new sound from a sound file at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the sound file.</param>
    /// <returns>The sound.</returns>
    ISound Create(string filePath);
}

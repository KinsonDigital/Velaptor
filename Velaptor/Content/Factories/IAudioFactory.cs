// <copyright file="IAudioFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

/// <summary>
/// Creates audio based on the audio file at a location.
/// </summary>
internal interface IAudioFactory
{
    /// <summary>
    /// Creates new audio from an audio file at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the audio file.</param>
    /// <param name="bufferType">The type of buffering to use.</param>
    /// <returns>The audio.</returns>
    IAudio Create(string filePath, AudioBuffer bufferType);
}

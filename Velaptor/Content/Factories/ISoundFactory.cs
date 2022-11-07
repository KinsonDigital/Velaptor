// <copyright file="ISoundFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Content;

namespace Velaptor.Content.Factories;

/// <summary>
/// Creates sounds based on the sound file at a location.
/// </summary>
internal interface ISoundFactory
{
    /// <summary>
    /// Creates a new sound from a sound file at the given <paramref name="filePath"/>.
    /// </summary>
    /// <param name="filePath">The path to the sound file.</param>
    /// <returns>The sound.</returns>
    ISound Create(string filePath);
}

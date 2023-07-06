// <copyright file="ISound.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using ICASLSound = CASL.ISound;

/// <summary>
/// A single sound that can be played, paused etc.
/// </summary>
public interface ISound : ICASLSound, IContent
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
}

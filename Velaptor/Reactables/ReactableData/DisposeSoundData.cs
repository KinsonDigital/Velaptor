// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.ReactableData;

/// <summary>
/// Holds data for the <see cref="DisposeSoundsReactable"/> reactable.
/// </summary>
internal readonly struct DisposeSoundData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeSoundData"/> struct.
    /// </summary>
    /// <param name="soundId">The ID of the sound.</param>
    public DisposeSoundData(in uint soundId) => SoundId = soundId;

    /// <summary>
    /// Gets the sound ID.
    /// </summary>
    public uint SoundId { get; }
}

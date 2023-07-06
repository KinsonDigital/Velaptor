// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the disposing of a sound.
/// </summary>
internal readonly record struct DisposeSoundData
{
    /// <summary>
    /// Gets the sound ID.
    /// </summary>
    public uint SoundId { get; init; }
}

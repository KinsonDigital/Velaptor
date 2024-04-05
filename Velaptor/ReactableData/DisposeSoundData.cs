// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the disposing of the audio.
/// </summary>
internal readonly record struct DisposeSoundData
{
    /// <summary>
    /// Gets the audio ID.
    /// </summary>
    public uint SoundId { get; init; }
}

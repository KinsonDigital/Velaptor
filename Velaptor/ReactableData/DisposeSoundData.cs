// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the disposing of a sound.
/// </summary>
internal sealed record DisposeSoundData
{
    /// <summary>
    /// Gets or sets the sound ID.
    /// </summary>
    public uint SoundId { get; set; }
}

// <copyright file="DisposeAudioData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the disposing of the audio.
/// </summary>
internal readonly record struct DisposeAudioData
{
    /// <summary>
    /// Gets the audio ID.
    /// </summary>
    public uint AudioId { get; init; }
}

// <copyright file="DisposeSoundData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Reactables.Core;

namespace Velaptor.Reactables.ReactableData;

/// <summary>
/// Holds data for the <see cref="IReactable{T}"/>.
/// </summary>
internal readonly struct DisposeSoundData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeSoundData"/> struct.
    /// </summary>
    /// <param name="soundId">The ID of the sound.</param>
    public DisposeSoundData(uint soundId) => SoundId = soundId;

    /// <summary>
    /// Gets the sound ID.
    /// </summary>
    public uint SoundId { get; }
}

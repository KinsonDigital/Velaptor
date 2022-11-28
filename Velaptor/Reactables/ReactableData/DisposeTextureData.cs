// <copyright file="DisposeTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.ReactableData;

/// <summary>
/// Holds data for the <see cref="DisposeTexturesReactable"/> reactable.
/// </summary>
internal readonly struct DisposeTextureData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DisposeTextureData"/> struct.
    /// </summary>
    /// <param name="textureId">The texture ID of the texture to dispose.</param>
    public DisposeTextureData(in uint textureId) => TextureId = textureId;

    /// <summary>
    /// Gets the texture ID.
    /// </summary>
    public uint TextureId { get; }
}

// <copyright file="DisposeTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the disposing of a texture.
/// </summary>
internal readonly record struct DisposeTextureData
{
    /// <summary>
    /// Gets the texture ID.
    /// </summary>
    public uint TextureId { get; init; }
}

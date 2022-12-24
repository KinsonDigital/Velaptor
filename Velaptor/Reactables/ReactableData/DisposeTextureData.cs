// <copyright file="DisposeTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.ReactableData;

/// <summary>
/// Holds data for the disposing of a texture.
/// </summary>
internal sealed record DisposeTextureData
{
    /// <summary>
    /// Gets or sets the texture ID.
    /// </summary>
    public uint TextureId { get; set; }
}

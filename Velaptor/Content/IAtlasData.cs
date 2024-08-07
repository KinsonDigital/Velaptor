﻿// <copyright file="IAtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System.Collections.Generic;
using Graphics;

/// <summary>
/// Holds data for a texture atlas.
/// </summary>
public interface IAtlasData : IContent
{
    /// <summary>
    /// Gets the list of frame names.
    /// </summary>
    // ReSharper disable once UnusedMemberInSuper.Global
    // ReSharper disable once ReturnTypeCanBeEnumerable.Global
    IReadOnlyCollection<string> SubTextureNames { get; }

    /// <summary>
    /// Gets the texture of the atlas.
    /// </summary>
    ITexture Texture { get; }

    /// <summary>
    /// Gets the width of the entire texture atlas texture.
    /// </summary>
    uint Width { get; }

    /// <summary>
    /// Gets the height of the entire texture atlas texture.
    /// </summary>
    uint Height { get; }

    /// <summary>
    /// Gets the file path to the atlas data.
    /// </summary>
    string AtlasDataFilePath { get; }

    /// <summary>
    /// The iterator for the atlas sub texture data.
    /// </summary>
    /// <param name="index">The index of the item to retrieve.</param>
    /// <returns>The sub texture data.</returns>
    AtlasSubTextureData this[int index] { get; }

    /// <summary>
    /// Gets the all the frames that have the given sub texture id.
    /// </summary>
    /// <param name="subTextureId">The sub texture ID of the frames to return.</param>
    /// <returns>The list of frame rectangles.</returns>
    AtlasSubTextureData[] GetFrames(string subTextureId);
}

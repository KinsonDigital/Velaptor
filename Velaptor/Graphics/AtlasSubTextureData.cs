// <copyright file="AtlasSubTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;

/// <summary>
/// Holds data about a texture atlas sub texture.
/// </summary>
public readonly struct AtlasSubTextureData
{
    /// <summary>
    /// Gets the bounds of the sub texture data.
    /// </summary>
    public Rectangle Bounds { get; init; }

    /// <summary>
    /// Gets the name of the sub texture.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Gets the index of the sub texture frame.
    /// </summary>
    /// <remarks>
    ///     This is used for frames of animation.  A negative value indicates
    ///     whether the sub texture is part of any animation frames.
    /// </remarks>
    public int FrameIndex { get; init; }
}

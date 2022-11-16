// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;

namespace Velaptor.Graphics;

/// <summary>
/// Adds basic effects to a texture when rendered.
/// </summary>
[Flags]
public enum RenderEffects
{
    /// <summary>
    /// No effects are applied.
    /// </summary>
    None = 1,

    /// <summary>
    /// The texture is flipped horizontally.
    /// </summary>
    FlipHorizontally = 2,

    /// <summary>
    /// The texture is flipped vertically.
    /// </summary>
    FlipVertically = 4,

    /// <summary>
    /// The texture is flipped horizontally and vertically.
    /// </summary>
    FlipBothDirections = FlipHorizontally | FlipVertically,
}

/// <summary>
/// Represents the type of gradient a color can.
/// </summary>
public enum ColorGradient
{
    /// <summary>
    /// No gradient is applied.
    /// </summary>
    None = 0,

    /// <summary>
    /// A horizontal gradient is applied.
    /// </summary>
    Horizontal = 1,

    /// <summary>
    /// A vertical gradient is applied.
    /// </summary>
    Vertical = 2,
}

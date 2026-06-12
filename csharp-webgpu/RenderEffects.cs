// <copyright file="RenderEffects.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

/// <summary>
/// The rendering effects that can be applied to a texture when rendering.
/// </summary>
/// <remarks>
/// Mirrors <c>Velaptor.Graphics.RenderEffects</c>.
/// </remarks>
public enum RenderEffects
{
    /// <summary>No rendering effects applied.</summary>
    None = 0,

    /// <summary>Flips the texture horizontally (left ↔ right).</summary>
    FlipHorizontally = 1,

    /// <summary>Flips the texture vertically (top ↔ bottom).</summary>
    FlipVertically = 2,

    /// <summary>Flips the texture both horizontally and vertically.</summary>
    FlipBothDirections = 3,
}

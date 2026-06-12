// <copyright file="TextureQuad.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Drawing;
using System.Numerics;

/// <summary>
/// Describes a single textured quad to render: where it appears, how it is oriented,
/// how it is tinted, and which region of the source texture to use.
/// </summary>
/// <remarks>
/// <para>
/// Mirrors the parameters accepted by <c>Velaptor.Graphics.Renderers.ITextureRenderer.Render</c>.
/// </para>
/// <para>
/// <b>Coordinate system:</b> screen pixel coordinates with the origin at the top-left
/// corner of the window; X increases rightward, Y increases downward.
/// <see cref="Position"/> is the <em>centre</em> of the rendered quad.
/// </para>
/// <para>
/// <see cref="Width"/> and <see cref="Height"/> represent the full source-texture dimensions
/// in pixels. They are used to normalise <see cref="SrcRect"/> into UV coordinates [0, 1].
/// When <see cref="SrcRect"/> is left at its default (<see cref="RectangleF.Empty"/>), the
/// entire texture is rendered.
/// </para>
/// </remarks>
public record struct TextureQuad
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureQuad"/> struct.
    /// </summary>
    public TextureQuad()
    {
    }

    /// <summary>
    /// Gets or sets the centre of the rendered quad in screen pixel coordinates.
    /// (0, 0) is the top-left corner of the window; Y increases downward.
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the full width of the source texture in pixels.
    /// Used to normalise <see cref="SrcRect"/> UV coordinates.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the full height of the source texture in pixels.
    /// Used to normalise <see cref="SrcRect"/> UV coordinates.
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Gets or sets the clockwise rotation angle in degrees.
    /// The texture rotates around its centre (<see cref="Position"/>).
    /// Default is <c>0</c> (no rotation).
    /// </summary>
    public float Angle { get; set; } = 0f;

    /// <summary>
    /// Gets or sets the uniform scale factor.
    /// <c>1.0</c> renders at native pixel size; values above <c>1.0</c> enlarge the quad,
    /// values below shrink it.
    /// Default is <c>1.0</c>.
    /// </summary>
    public float Size { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the tint color multiplied with each sampled texel.
    /// Use <see cref="Color.White"/> for no tint (the default).
    /// </summary>
    public Color TintColor { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the flip effects to apply. Default is <see cref="RenderEffects.None"/>.
    /// </summary>
    public RenderEffects Effects { get; set; } = RenderEffects.None;

    /// <summary>
    /// Gets or sets the rectangular region of the source texture to render, in texture pixels
    /// relative to the top-left corner. When <see cref="RectangleF.IsEmpty"/> is <c>true</c>
    /// (the default), the entire texture is rendered.
    /// </summary>
    public RectangleF SrcRect { get; set; } = RectangleF.Empty;
}

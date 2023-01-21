// <copyright file="TextureBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using Graphics;

/// <summary>
/// A single texture batch item that can be rendered to the screen.
/// </summary>
internal readonly record struct TextureBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureBatchItem"/> struct.
    /// </summary>
    /// <param name="srcRect">The rectangular section inside of a texture to render.</param>
    /// <param name="destRect">The destination rectangular area of where to render the texture on the screen.</param>
    /// <param name="size">The size of the rendered texture.</param>
    /// <param name="angle">The angle of the texture in degrees.</param>
    /// <param name="tintColor">The color to apply to the entire texture.</param>
    /// <param name="effects">The type of effects to apply to a texture.</param>
    /// <param name="textureId">The ID of the texture.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public TextureBatchItem(
        in
        RectangleF srcRect,
        RectangleF destRect,
        float size,
        float angle,
        Color tintColor,
        RenderEffects effects,
        uint textureId)
    {
        SrcRect = srcRect;
        DestRect = destRect;
        Size = size;
        Angle = angle;
        TintColor = tintColor;
        Effects = effects;
        TextureId = textureId;
    }

    /// <summary>
    /// Gets the rectangular section inside of a texture to render.
    /// </summary>
    public RectangleF SrcRect { get; }

    /// <summary>
    /// Gets the destination rectangular area of where to render the texture on the screen.
    /// </summary>
    public RectangleF DestRect { get; }

    /// <summary>
    /// Gets the size of the rendered texture.
    /// </summary>
    public float Size { get; }

    /// <summary>
    /// Gets the angle of the texture in degrees.
    /// </summary>
    public float Angle { get; }

    /// <summary>
    /// Gets the color to apply to an entire texture.
    /// </summary>
    public Color TintColor { get; }

    /// <summary>
    /// Gets the type of effects to apply to a texture.
    /// </summary>
    public RenderEffects Effects { get; } = RenderEffects.None;

    /// <summary>
    /// Gets the ID of the texture.
    /// </summary>
    public uint TextureId { get; }

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="TextureBatchItem"/> is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        !(Angle != 0f ||
          Size != 0f ||
          Effects != RenderEffects.None ||
          TintColor.IsEmpty != true ||
          SrcRect.IsEmpty != true ||
          DestRect.IsEmpty != true ||
          TextureId != 0);
}

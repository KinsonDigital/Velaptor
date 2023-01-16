// <copyright file="TextureBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
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
    /// <param name="layer">The layer where a texture will be rendered.</param>
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
        uint textureId,
        int layer)
    {
        SrcRect = srcRect;
        DestRect = destRect;
        Size = size;
        Angle = angle;
        TintColor = tintColor;
        Effects = effects;
        TextureId = textureId;
        Layer = layer;
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
    /// Gets the layer that the a texture will be rendered on.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Lower layer values will render before higher layer values.
    ///         If two separate textures have the same layer value, they will
    ///         rendered in the order that the render method was invoked.
    ///     </para>
    ///     <para>Example below:</para>
    ///
    ///     <b>Render Method Invoked Order:</b>
    ///     <list type="number">
    ///         <item>Texture 1 (Layer -10)</item>
    ///         <item>Texture 2 (Layer -20)</item>
    ///         <item>Texture 3 (Layer 0)</item>
    ///         <item>Texture 4 (Layer 0)</item>
    ///         <item>Texture 5 (Layer 4)</item>
    ///         <item>Texture 6 (Layer 3)</item>
    ///     </list>
    ///
    ///     <b>Texture Render Order:</b>
    ///     <list type="bullet">
    ///         <item>Texture 2</item>
    ///         <item>Texture 1</item>
    ///         <item>Texture 3</item>
    ///         <item>Texture 4</item>
    ///         <item>Texture 6</item>
    ///         <item>Texture 5</item>
    ///     </list>
    /// </remarks>
    public int Layer { get; }

    /// <summary>
    /// Gets a value indicating whether or not the current <see cref="TextureBatchItem"/> is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        TextureId == 0 &&
        Size == 0f &&
        Angle == 0f &&
        Effects is 0 or RenderEffects.None &&
        Layer == 0 &&
        SrcRect.IsEmpty &&
        DestRect.IsEmpty &&
        TintColor.IsEmpty;

    /// <inheritdoc/>
    public override string ToString()
    {
        var result = new StringBuilder();

        result.AppendLine("Texture Batch Item Values:");
        result.AppendLine($"Src Rect: {SrcRect.ToString()}");
        result.AppendLine($"Dest Rect: {DestRect.ToString()}");
        result.AppendLine($"Size: {Size.ToString(CultureInfo.InvariantCulture)}");
        result.AppendLine($"Angle: {Angle.ToString(CultureInfo.InvariantCulture)}");
        result.AppendLine(
            $"Tint Clr: {{A={TintColor.A},R={TintColor.R},G={TintColor.G},B={TintColor.B}}}");
        result.AppendLine($"Effects: {Effects.ToString()}");
        result.AppendLine($"Texture ID: {TextureId.ToString()}");
        result.Append($"Layer: {Layer}");

        return result.ToString();
    }
}

// <copyright file="LineBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a line with various attributes.
/// </summary>
internal readonly struct LineBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchItem"/> struct.
    /// </summary>
    /// <param name="p1">The start vector of the line.</param>
    /// <param name="p2">The end vector of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    /// <param name="layer">The layer where the shape will be rendered.</param>
    public LineBatchItem(
        Vector2 p1,
        Vector2 p2,
        Color color,
        float thickness,
        int layer = 0)
    {
        P1 = p1;
        P2 = p2;
        Color = color;
        Thickness = thickness;
        Layer = layer;
    }

    /// <summary>
    /// Gets the start vector of the line.
    /// </summary>
    public Vector2 P1 { get; }

    /// <summary>
    /// Gets the end vector of the line.
    /// </summary>
    public Vector2 P2 { get; }

    /// <summary>
    /// Gets the color of the line.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Gets the thickness of the line.
    /// </summary>
    public float Thickness { get; }

    /// <summary>
    /// Gets the layer where the shape will be rendered.
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
    /// Gets a value indicating whether or not the <see cref="LineBatchItem"/> is empty.
    /// </summary>
    /// <returns><c>true</c> if the item is empty.</returns>
    public bool IsEmpty() =>
        P1 == Vector2.Zero &&
        P2 == Vector2.Zero &&
        Color == Color.Empty &&
        Thickness == 0f &&
        Layer == 0;
}

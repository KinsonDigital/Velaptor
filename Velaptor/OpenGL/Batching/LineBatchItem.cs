// <copyright file="LineBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a line with various attributes.
/// </summary>
internal readonly record struct LineBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LineBatchItem"/> struct.
    /// </summary>
    /// <param name="p1">The start vector of the line.</param>
    /// <param name="p2">The end vector of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public LineBatchItem(
        Vector2 p1,
        Vector2 p2,
        Color color,
        float thickness)
    {
        P1 = p1;
        P2 = p2;
        Color = color;
        Thickness = thickness;
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
    /// Gets a value indicating whether or not the <see cref="LineBatchItem"/> is empty.
    /// </summary>
    /// <returns><c>true</c> if the item is empty.</returns>
    public bool IsEmpty() =>
        !(P1 != Vector2.Zero ||
          P2 != Vector2.Zero ||
          Color.IsEmpty != true ||
          Thickness != 0f);

}

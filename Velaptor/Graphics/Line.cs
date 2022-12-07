// <copyright file="Line.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a single line segment.
/// </summary>
public struct Line
{
    private Color color = Color.White;
    private float thickness = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="Line"/> struct.
    /// </summary>
    /// <param name="p1">The starting point of the line.</param>
    /// <param name="p2">The ending point of the line.</param>
    public Line(Vector2 p1, Vector2 p2)
    {
        P1 = p1;
        P2 = p2;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Line"/> struct.
    /// </summary>
    /// <param name="p1">The starting point of the line.</param>
    /// <param name="p2">The ending point of the line.</param>
    /// <param name="color">The color of the line.</param>
    public Line(Vector2 p1, Vector2 p2, Color color)
    {
        P1 = p1;
        P2 = p2;
        Color = color;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Line"/> struct.
    /// </summary>
    /// <param name="p1">The starting point of the line.</param>
    /// <param name="p2">The ending point of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    public Line(Vector2 p1, Vector2 p2, float thickness)
    {
        P1 = p1;
        P2 = p2;
        Thickness = thickness;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Line"/> struct.
    /// </summary>
    /// <param name="p1">The starting point of the line.</param>
    /// <param name="p2">The ending point of the line.</param>
    /// <param name="color">The color of the line.</param>
    /// <param name="thickness">The thickness of the line.</param>
    public Line(Vector2 p1, Vector2 p2, Color color, float thickness)
    {
        P1 = p1;
        P2 = p2;
        Color = color;
        Thickness = thickness < 0 ? 1f : thickness;
    }

    /// <summary>
    /// Gets or sets the starting point of the line.
    /// </summary>
    public Vector2 P1 { get; set; }

    /// <summary>
    /// Gets or sets the ending point of the line.
    /// </summary>
    public Vector2 P2 { get; set; }

    /// <summary>
    /// Gets or sets the color of the line.
    /// </summary>
    public Color Color
    {
        get
        {
            if (this.color == Color.Empty)
            {
                this.color = Color.White;
            }

            return this.color;
        }
        set => this.color = value;
    }

    /// <summary>
    /// Gets or sets the thickness of the line.
    /// </summary>
    /// <remarks>
    ///     Restricts the thickness to a minimum value of 1.
    /// </remarks>
    public float Thickness
    {
        get
        {
            if (this.thickness == 0f)
            {
                this.thickness = 1f;
            }

            return this.thickness;
        }
        set => this.thickness = value < 1f ? 1f : value;
    }
}

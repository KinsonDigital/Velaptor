// <copyright file="RectShape.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a rectangle shape with position, size, color, corner rounding,
/// fill/border mode, and gradient settings.
/// </summary>
/// <remarks>
/// This mirrors the public API of <c>Velaptor.Graphics.RectShape</c>.
/// Position is the <b>centre</b> of the rectangle in world pixels (Y-up).
/// Width and Height are clamped to a minimum of 1 pixel.
/// </remarks>
public record struct RectShape
{
    private float width = 1f;
    private float height = 1f;
    private float borderThickness = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectShape"/> class.
    /// </summary>
    public RectShape()
    {
    }

    /// <summary>
    /// Gets or sets the centre position of the rectangle in world pixels.
    /// </summary>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the width of the rectangle in pixels. Minimum value is 1.
    /// </summary>
    public float Width
    {
        readonly get => this.width;
        set => this.width = value < 1f ? 1f : value;
    }

    /// <summary>
    /// Gets or sets the height of the rectangle in pixels. Minimum value is 1.
    /// </summary>
    public float Height
    {
        readonly get => this.height;
        set => this.height = value < 1f ? 1f : value;
    }

    /// <summary>
    /// Gets the half-width of the rectangle.
    /// </summary>
    public readonly float HalfWidth => Width / 2f;

    /// <summary>
    /// Gets the half-height of the rectangle.
    /// </summary>
    public readonly float HalfHeight => Height / 2f;

    /// <summary>
    /// Gets or sets the color of the rectangle. Ignored when <see cref="GradientType"/> is not <c>None</c>.
    /// </summary>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets a value indicating whether the rectangle is solid (<c>true</c>) or a bordered outline (<c>false</c>).
    /// </summary>
    public bool IsSolid { get; set; } = true;

    /// <summary>
    /// Gets or sets the border thickness in pixels. Ignored when <see cref="IsSolid"/> is <c>true</c>.
    /// Clamped to <c>[1, min(Width, Height)]</c>.
    /// </summary>
    public float BorderThickness
    {
        readonly get => this.borderThickness;
        set
        {
            var smallestDimension = this.width < this.height ? this.width : this.height;
            value = value > smallestDimension ? smallestDimension : value;
            value = value < 1f ? 1f : value;
            this.borderThickness = value;
        }
    }

    /// <summary>
    /// Gets or sets the corner radius of the rectangle. Default is all zero (square corners).
    /// </summary>
    public CornerRadius CornerRadius { get; set; } = new (0f, 0f, 0f, 0f);

    /// <summary>
    /// Gets or sets the gradient type. <c>None</c> uses <see cref="Color"/> as a solid fill.
    /// </summary>
    public ColorGradient GradientType { get; set; } = ColorGradient.None;

    /// <summary>
    /// Gets or sets the gradient start color. Ignored when <see cref="GradientType"/> is <c>None</c>.
    /// </summary>
    public Color GradientStart { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the gradient stop color. Ignored when <see cref="GradientType"/> is <c>None</c>.
    /// </summary>
    public Color GradientStop { get; set; } = Color.White;

    /// <summary>
    /// Returns a value indicating whether the rectangle contains the given point.
    /// </summary>
    /// <param name="point">The point to test.</param>
    /// <returns><c>true</c> if the point is inside the rectangle (edges inclusive).</returns>
    public readonly bool Contains(Vector2 point) =>
        point.X >= Position.X - HalfWidth &&
        point.X <= Position.X + HalfWidth &&
        point.Y >= Position.Y - HalfHeight &&
        point.Y <= Position.Y + HalfHeight;

    /// <summary>
    /// Returns a value indicating whether the <see cref="RectShape"/> is empty (all default values).
    /// </summary>
    /// <returns><c>true</c> if empty.</returns>
    public readonly bool IsEmpty() =>
        Position == Vector2.Zero &&
        Width <= 1f &&
        Height <= 1f &&
        Color.IsEmpty &&
        !IsSolid &&
        BorderThickness <= 1f &&
        CornerRadius.IsEmpty() &&
        GradientType == ColorGradient.None &&
        GradientStart.IsEmpty &&
        GradientStop.IsEmpty;

    /// <summary>
    /// Resets the struct to its empty/default state.
    /// </summary>
    public void Empty()
    {
        Position = Vector2.Zero;
        Width = 0;
        Height = 0;
        Color = Color.Empty;
        IsSolid = false;
        BorderThickness = 0u;
        CornerRadius = new CornerRadius(0f, 0f, 0f, 0f);
        GradientType = ColorGradient.None;
        GradientStart = Color.Empty;
        GradientStop = Color.Empty;
    }
}

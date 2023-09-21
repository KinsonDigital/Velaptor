// <copyright file="RectShape.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a rectangular shape with various attributes.
/// </summary>
public record struct RectShape
{
    private float width = 1f;
    private float height = 1f;
    private float borderThickness = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectShape"/> struct.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "Standard text is correct.  Reported incorrectly.")]
    public RectShape()
    {
    }

    /// <summary>
    /// Gets or sets the position of the rectangle.
    /// </summary>
    /// <remarks>
    ///     This position is the center of the rectangle.
    /// </remarks>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the width of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The width is restricted to a minimum value of one.
    /// </remarks>
    public float Width
    {
        get => this.width;
        set
        {
            value = value < 1f ? 1f : value;

            this.width = value;
        }
    }

    /// <summary>
    /// Gets or sets the height of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The height is restricted to a minimum value of one.
    /// </remarks>
    public float Height
    {
        get => this.height;
        set
        {
            value = value < 1f ? 1f : value;

            this.height = value;
        }
    }

    /// <summary>
    /// Gets the half width of the rectangle.
    /// </summary>
    public float HalfWidth => Width / 2f;

    /// <summary>
    /// Gets the half height of the rectangle.
    /// </summary>
    public float HalfHeight => Height / 2f;

    /// <summary>
    /// Gets or sets the location of the top of the rectangle on the Y axis.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the rectangle.
    /// </remarks>
    public float Top
    {
        get => Position.Y - HalfHeight;
        set => Position = new Vector2(Position.X, value + HalfHeight);
    }

    /// <summary>
    /// Gets or sets the location of the right side of the rectangle on the X axis.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the rectangle.
    /// </remarks>
    public float Right
    {
        get => Position.X + HalfWidth;
        set => Position = new Vector2(value - HalfWidth, Position.Y);
    }

    /// <summary>
    /// Gets or sets the location of the bottom of the rectangle on the Y axis.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the rectangle.
    /// </remarks>
    public float Bottom
    {
        get => Position.Y + HalfHeight;
        set => Position = new Vector2(Position.X, value - HalfHeight);
    }

    /// <summary>
    /// Gets or sets the location of the left side of the rectangle on the X axis.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the rectangle.
    /// </remarks>
    public float Left
    {
        get => Position.X - HalfWidth;
        set => Position = new Vector2(value + HalfWidth, Position.Y);
    }

    /// <summary>
    /// Gets or sets the color of the rectangle.
    /// </summary>
    /// <remarks>
    ///     Ignored if the <see cref="GradientType"/> is set to any value other than <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets a value indicating whether or not the rectangle is solid.
    /// </summary>
    public bool IsSolid { get; set; } = true;

    /// <summary>
    /// Gets or sets the thickness of the rectangle's border.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Ignored if the <see cref="IsSolid"/> property is set to <c>true</c>.
    /// </para>
    ///
    /// <para>
    ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </para>
    /// </remarks>
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
    /// Gets or sets the radius of each corner of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </remarks>
    public CornerRadius CornerRadius { get; set; } = new (1f, 1f, 1f, 1f);

    /// <summary>
    /// Gets or sets the type of color gradient that will be applied to the rectangle.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     A value of <see cref="ColorGradient.None"/> will use the <see cref="Color"/>
    ///     property and render the rectangle with a solid color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Horizontal"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> <see cref="GradientStop"/> properties.
    ///     This will render the rectangle with <see cref="GradientStart"/> color on the left side and gradually
    ///     render it to the right side as the <see cref="GradientStop"/> color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Vertical"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> and <see cref="GradientStop"/> properties.
    ///     This will render the rectangle with <see cref="GradientStart"/> color on the top and gradually
    ///     render it to the bottom as the <see cref="GradientStop"/> color.
    /// </para>
    /// </remarks>
    public ColorGradient GradientType { get; set; } = ColorGradient.None;

    /// <summary>
    /// Gets or sets the starting color of the gradient.
    /// </summary>
    /// <remarks>
    ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color GradientStart { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets the ending color of the gradient.
    /// </summary>
    /// <remarks>
    ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color GradientStop { get; set; } = Color.White;

    /// <summary>
    /// Returns a value indicating whether or not the given <see cref="Vector2"/> is contained within the rectangle shape.
    /// </summary>
    /// <param name="vector">The possibly contained <see cref="Vector2"/>.</param>
    /// <returns><c>true</c> if the <paramref name="vector"/> is contained.</returns>
    /// <remarks>
    ///     The <see cref="Left"/> or <see cref="Right"/> or <see cref="Top"/> or <see cref="Bottom"/> are inclusive.
    /// </remarks>
    public bool Contains(Vector2 vector) =>
        vector.X >= Left &&
        vector.X <= Right &&
        vector.Y >= Top &&
        vector.Y <= Bottom;

    /// <summary>
    /// Returns a value indicating whether or not the <see cref="RectShape"/> struct is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
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
    /// Empties the struct.
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

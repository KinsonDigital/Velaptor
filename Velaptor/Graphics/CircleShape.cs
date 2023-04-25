// <copyright file="CircleShape.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents an circle shape with various attributes.
/// </summary>
public struct CircleShape
{
    private float diameter = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleShape"/> struct.
    /// </summary>
    public CircleShape()
    {
    }

    /// <summary>
    /// Gets or sets the position of the circle.
    /// </summary>
    /// <remarks>
    ///     This position is the center of the circle.
    /// </remarks>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the width of the circle.
    /// </summary>
    /// <remarks>
    ///     The diameter is restricted to a minimum value of one.
    /// </remarks>
    public float Diameter
    {
        get => this.diameter;
        set
        {
            value = value < 1f ? 1f : value;

            this.diameter = value;
        }
    }

    /// <summary>
    /// Gets or sets the radius of the circle.
    /// </summary>
    /// <remarks>
    /// This is half of the <see cref="Diameter"/>.
    /// </remarks>
    public float Radius
    {
        get => Diameter / 2f;
        set => Diameter = value * 2f;
    }

    /// <summary>
    /// Gets or sets the top location of the circle.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the circle.
    /// </remarks>
    public float Top
    {
        get => Position.Y - Radius;
        set => Position = new Vector2(Position.X, value + Radius);
    }

    /// <summary>
    /// Gets or sets the right location of the circle.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the circle.
    /// </remarks>
    public float Right
    {
        get => Position.X + Radius;
        set => Position = new Vector2(value - Radius, Position.Y);
    }

    /// <summary>
    /// Gets or sets the bottom location of the circle.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the circle.
    /// </remarks>
    public float Bottom
    {
        get => Position.Y + Radius;
        set => Position = new Vector2(Position.X, value - Radius);
    }

    /// <summary>
    /// Gets or sets the Left location of the circle.
    /// </summary>
    /// <remarks>
    ///     Will automatically update the <see cref="Position"/> of the circle.
    /// </remarks>
    public float Left
    {
        get => Position.X - Radius;
        set => Position = new Vector2(value + Radius, Position.Y);
    }

    /// <summary>
    /// Gets or sets the color of the circle.
    /// </summary>
    /// <remarks>
    ///     Ignored if the <see cref="GradientType"/> is set to any value other than <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or sets a value indicating whether or not the circle is filled or empty.
    /// </summary>
    public bool IsFilled { get; set; } = true;

    /// <summary>
    /// Gets or sets the thickness of the circle's border.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Ignored if the <see cref="IsFilled"/> property is set to <c>true</c>.
    /// </para>
    /// </remarks>
    public float BorderThickness { get; set; } = 1f;

    /// <summary>
    /// Gets or sets the type of color gradient that will be applied to the circle.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     A value of <see cref="ColorGradient.None"/> will use the <see cref="Color"/>
    ///     property and render the circle with a solid color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Horizontal"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> <see cref="GradientStop"/> properties.
    ///     This will render the circle with <see cref="GradientStart"/> color on the left side and gradually
    ///     render it to the right side as the <see cref="GradientStop"/> color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Vertical"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> and <see cref="GradientStop"/> properties.
    ///     This will render the circle with <see cref="GradientStart"/> color on the top and gradually
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
    /// Returns a value indicating whether or not the <see cref="RectShape"/> struct is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        Position == Vector2.Zero &&
        Diameter <= 1f &&
        Color.IsEmpty &&
        IsFilled is false &&
        BorderThickness <= 1f &&
        GradientType == ColorGradient.None &&
        GradientStart.IsEmpty &&
        GradientStop.IsEmpty;

    /// <summary>
    /// Empties the struct.
    /// </summary>
    public void Empty()
    {
        Position = Vector2.Zero;
        Diameter = 0;
        Color = Color.Empty;
        IsFilled = false;
        BorderThickness = 0u;
        GradientType = ColorGradient.None;
        GradientStart = Color.Empty;
        GradientStop = Color.Empty;
    }
}

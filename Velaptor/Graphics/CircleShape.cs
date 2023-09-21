// <copyright file="CircleShape.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a circle shape with various attributes.
/// </summary>
public record struct CircleShape
{
    private float diameter = 1f;
    private float borderThickness = 1f;

    /// <summary>
    /// Initializes a new instance of the <see cref="CircleShape"/> struct.
    /// </summary>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "Standard text is correct.  Reported incorrectly.")]
    public CircleShape()
    {
    }

    /// <summary>
    /// Gets or sets the position of the circle.
    /// </summary>
    /// <remarks>
    ///     This position is relative to the center of the circle.
    /// </remarks>
    public Vector2 Position { get; set; } = Vector2.Zero;

    /// <summary>
    /// Gets or sets the diameter of the circle.
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
    /// <br/>
    /// Changing the radius will automatically update the <see cref="Diameter"/>.
    /// </remarks>
    public float Radius
    {
        get => Diameter / 2f;
        set => Diameter = value * 2f;
    }

    /// <summary>
    /// Gets or sets the top location of the top of the circle on the Y axis.
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
    /// Gets or sets the location of the right side of the circle on the X axis.
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
    /// Gets or sets the bottom location of the bottom of the circle on the Y axis.
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
    /// Gets or sets the location of the left side of the circle on the X axis.
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
    /// Gets or sets a value indicating whether or not the circle is solid.
    /// </summary>
    public bool IsSolid { get; set; } = true;

    /// <summary>
    /// Gets or sets the thickness of the circle's border.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Only visible if the <see cref="IsSolid"/> property is set to <c>false</c>.
    ///     <br/>
    ///     The border thickness is automatically restricted to a value no greater than the <see cref="Radius"/>.
    /// </para>
    /// </remarks>
    public float BorderThickness
    {
        get => this.borderThickness;
        set
        {
            value = value > Radius ? Radius : value;
            value = value < 1f ? 1f : value;

            this.borderThickness = value;
        }
    }

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
        BorderThickness <= 1f &&
        Diameter <= 1f &&
        Color.IsEmpty &&
        !IsSolid &&
        GradientType == ColorGradient.None &&
        GradientStart.IsEmpty &&
        GradientStop.IsEmpty;

    /// <summary>
    /// Empties the struct.
    /// </summary>
    public void Empty()
    {
        Position = Vector2.Zero;
        Diameter = 1f;
        Color = Color.Empty;
        IsSolid = false;
        BorderThickness = 1u;
        GradientType = ColorGradient.None;
        GradientStart = Color.Empty;
        GradientStop = Color.Empty;
    }
}

// <copyright file="ShapeBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using Graphics;

/// <summary>
/// Represents a rectangular shape with various attributes.
/// </summary>
internal readonly record struct ShapeBatchItem
{
    private readonly float halfWidth;
    private readonly float halfHeight;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeBatchItem"/> struct.
    /// </summary>
    /// <param name="position">The position of the shape.</param>
    /// <param name="width">The width of the shape.</param>
    /// <param name="height">The height of the shape.</param>
    /// <param name="color">The color of the shape.</param>
    /// <param name="isSolid">If true, a solid shape.</param>
    /// <param name="borderThickness">The thickness of the shape's border.</param>
    /// <param name="cornerRadius">The radius of each corner of the shape.</param>
    /// <param name="gradientType">The type of color gradient that will be applied to the shape.</param>
    /// <param name="gradientStart">The starting color of the gradient.</param>
    /// <param name="gradientStop">The ending color of the gradient.</param>
    /// <remarks>
    /// <para>
    ///     The <see cref="BorderThickness"/> property is ignored if the <paramref name="isSolid"/> parameter is set to <c>true</c>.
    /// </para>
    /// <para>
    ///     The value of each corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </para>
    /// </remarks>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public ShapeBatchItem(
        Vector2 position,
        float width,
        float height,
        Color color,
        bool isSolid,
        float borderThickness,
        CornerRadius cornerRadius,
        ColorGradient gradientType,
        Color gradientStart,
        Color gradientStop)
    {
        Position = position;
        Width = width;
        this.halfWidth = Width / 2f;
        Height = height;
        this.halfHeight = Height / 2f;
        Color = color;
        IsSolid = isSolid;
        BorderThickness = borderThickness;
        CornerRadius = cornerRadius;
        GradientType = gradientType;
        GradientStart = gradientStart;
        GradientStop = gradientStop;
    }

    /// <summary>
    /// Gets the position of the shape.
    /// </summary>
    /// <remarks>
    ///     This is the center of the shape.
    /// </remarks>
    public Vector2 Position { get; }

    /// <summary>
    /// Gets the width of the shape.
    /// </summary>
    /// <remarks>
    ///     The width is restricted to a minimum value of 1.
    /// </remarks>
    public float Width { get; }

    /// <summary>
    /// Gets the height of the shape.
    /// </summary>
    /// <remarks>
    ///     The height is restricted to a minimum value of 1.
    /// </remarks>
    public float Height { get; }

    /// <summary>
    /// Gets the half width of the shape.
    /// </summary>
    public float HalfWidth => this.halfWidth;

    /// <summary>
    /// Gets the half height of the shape.
    /// </summary>
    public float HalfHeight => this.halfHeight;

    /// <summary>
    /// Gets the color of the shape.
    /// </summary>
    /// <remarks>
    ///     Ignored if the <see cref="GradientType"/> is set to any value other than <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color Color { get; }

    /// <summary>
    /// Gets a value indicating whether the shape is a solid color.
    /// </summary>
    public bool IsSolid { get; }

    /// <summary>
    /// Gets the thickness of the shape's border.
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
    public float BorderThickness { get; }

    /// <summary>
    /// Gets the radius of each corner of the shape.
    /// </summary>
    /// <remarks>
    ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </remarks>
    public CornerRadius CornerRadius { get; }

    /// <summary>
    /// Gets the type of color gradient that will be applied to the shape.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     A value of <see cref="ColorGradient.None"/> will use the <see cref="Color"/>
    ///     property and render the shape with a solid color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Horizontal"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> <see cref="GradientStop"/> properties.
    ///     This will render the shape with <see cref="GradientStart"/> color on the left side and gradually
    ///     render it to the right side as the <see cref="GradientStop"/> color.
    /// </para>
    ///
    /// <para>
    ///     A value of <see cref="ColorGradient.Vertical"/> will ignore the <see cref="Color"/>
    ///     property and use the <see cref="GradientStart"/> and <see cref="GradientStop"/> properties.
    ///     This will render the shape with <see cref="GradientStart"/> color on the top and gradually
    ///     render it to the bottom as the <see cref="GradientStop"/> color.
    /// </para>
    /// </remarks>
    public ColorGradient GradientType { get; }

    /// <summary>
    /// Gets the starting color of the gradient.
    /// </summary>
    /// <remarks>
    ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color GradientStart { get; }

    /// <summary>
    /// Gets the ending color of the gradient.
    /// </summary>
    /// <remarks>
    ///     This property is ignored if the <see cref="GradientType"/> is set to a value of <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color GradientStop { get; }

    /// <summary>
    /// Returns a value indicating whether the <see cref="RectShape"/> struct is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        !(Position != Vector2.Zero ||
        Width > 0f ||
        Height > 0f ||
        !Color.IsEmpty ||
        IsSolid ||
        !CornerRadius.IsEmpty() ||
        GradientType != ColorGradient.None ||
        !GradientStart.IsEmpty ||
        !GradientStop.IsEmpty ||
        BorderThickness > 0f);
}

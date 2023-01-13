// <copyright file="RectBatchItem.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Batching;

using System.Drawing;
using System.Numerics;
using Graphics;

/// <summary>
/// Represents a rectangular shape with various attributes.
/// </summary>
internal readonly struct RectBatchItem
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RectBatchItem"/> struct.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <param name="isFilled">If true, a solid rectangle.</param>
    /// <param name="borderThickness">The thickness of the rectangle's border.</param>
    /// <param name="cornerRadius">The radius of each corner of the rectangle.</param>
    /// <param name="gradientType">The type of color gradient that will be applied to the rectangle.</param>
    /// <param name="gradientStart">The starting color of the gradient.</param>
    /// <param name="gradientStop">The ending color of the gradient.</param>
    /// <param name="layer">The layer where the shape will be rendered.</param>
    /// <remarks>
    /// <para>
    ///     The <see cref="BorderThickness"/> property is ignored if the <paramref name="isFilled"/> parameter is set to <c>true</c>.
    /// </para>
    /// <para>
    ///     The value of each corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </para>
    /// </remarks>
    public RectBatchItem(
        Vector2 position = default,
        float width = 1f,
        float height = 1f,
        Color color = default,
        bool isFilled = true,
        float borderThickness = 1f,
        CornerRadius cornerRadius = default,
        ColorGradient gradientType = ColorGradient.None,
        Color gradientStart = default,
        Color gradientStop = default,
        int layer = 0)
    {
        Position = position;
        Width = width;
        Height = height;
        Color = color == Color.Empty ? Color.White : color;
        IsFilled = isFilled;
        BorderThickness = borderThickness;

        if (cornerRadius == CornerRadius.Empty())
        {
            CornerRadius = new CornerRadius(1, 1, 1, 1);
        }
        else
        {
            var minValue = Width > height ? width : height;

            cornerRadius = cornerRadius.TopLeft > minValue
                ? CornerRadius.SetTopLeft(cornerRadius, minValue)
                : cornerRadius;

            cornerRadius = cornerRadius.TopRight > minValue
                ? CornerRadius.SetTopRight(cornerRadius, minValue)
                : cornerRadius;

            cornerRadius = cornerRadius.BottomRight > minValue
                ? CornerRadius.SetBottomRight(cornerRadius, minValue)
                : cornerRadius;

            cornerRadius = cornerRadius.BottomLeft > minValue
                ? CornerRadius.SetBottomLeft(cornerRadius, minValue)
                : cornerRadius;

            CornerRadius = cornerRadius;
        }

        GradientType = gradientType;
        GradientStart = gradientStart == Color.Empty ? Color.White : gradientStart;
        GradientStop = gradientStop == Color.Empty ? Color.White : gradientStop;
        Layer = layer;
    }

    /// <summary>
    /// Gets the position of the rectangle.
    /// </summary>
    /// <remarks>
    ///     This is the center of the rectangle.
    /// </remarks>
    public Vector2 Position { get; }

    /// <summary>
    /// Gets the width of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The width is restricted to a minimum value of 1.
    /// </remarks>
    public float Width { get; }

    /// <summary>
    /// Gets the height of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The height is restricted to a minimum value of 1.
    /// </remarks>
    public float Height { get; }

    /// <summary>
    /// Gets the color of the rectangle.
    /// </summary>
    /// <remarks>
    ///     Ignored if the <see cref="GradientType"/> is set to any value other than <see cref="ColorGradient.None"/>.
    /// </remarks>
    public Color Color { get; }

    /// <summary>
    /// Gets a value indicating whether or not the rectangle is filled with a solid color.
    /// </summary>
    public bool IsFilled { get; } = true;

    /// <summary>
    /// Gets the thickness of the rectangle's border.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Ignored if the <see cref="IsFilled"/> property is set to <c>true</c>.
    /// </para>
    ///
    /// <para>
    ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </para>
    /// </remarks>
    public float BorderThickness { get; }

    /// <summary>
    /// Gets the radius of each corner of the rectangle.
    /// </summary>
    /// <remarks>
    ///     The value of a corner will never be larger than the smallest half <see cref="Width"/> or half <see cref="Height"/>.
    /// </remarks>
    public CornerRadius CornerRadius { get; }

    /// <summary>
    /// Gets the type of color gradient that will be applied to the rectangle.
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
    /// Returns a value indicating whether or not the <see cref="RectShape"/> struct is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() =>
        Position == Vector2.Zero &&
        Width <= 1f &&
        Height <= 1f &&
        Color.IsEmpty &&
        IsFilled is false &&
        BorderThickness <= 1f &&
        CornerRadius.IsEmpty() &&
        GradientType == ColorGradient.None &&
        GradientStart.IsEmpty &&
        GradientStop.IsEmpty &&
        Layer == 0;
}

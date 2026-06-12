// <copyright file="CornerRadius.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

/// <summary>
/// Represents the corner radius values for a rectangle. All four corners can
/// be set independently.
/// </summary>
/// <remarks>
/// Mirrors <c>Velaptor.Graphics.CornerRadius</c>.
/// </remarks>
public readonly record struct CornerRadius
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CornerRadius"/> struct
    /// with all four corners set to <paramref name="value"/>.
    /// </summary>
    /// <param name="value">The uniform radius for all corners.</param>
    public CornerRadius(float value)
        : this(value, value, value, value)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CornerRadius"/> struct
    /// with individual radii for each corner.
    /// </summary>
    /// <param name="topLeft">Top-left radius.</param>
    /// <param name="topRight">Top-right radius.</param>
    /// <param name="bottomRight">Bottom-right radius.</param>
    /// <param name="bottomLeft">Bottom-left radius.</param>
    public CornerRadius(float topLeft, float topRight, float bottomRight, float bottomLeft)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
    }

    /// <summary>
    /// Gets the top-left corner radius.
    /// </summary>
    public float TopLeft { get; init; }

    /// <summary>
    /// Gets the top-right corner radius.
    /// </summary>
    public float TopRight { get; init; }

    /// <summary>
    /// Gets the bottom-right corner radius.
    /// </summary>
    public float BottomRight { get; init; }

    /// <summary>
    /// Gets the bottom-left corner radius.
    /// </summary>
    public float BottomLeft { get; init; }

    /// <summary>
    /// Returns a <see cref="CornerRadius"/> with all corners set to zero.
    /// </summary>
    /// <returns>The empty instance.</returns>
    public static CornerRadius Empty() => new (0f, 0f, 0f, 0f);

    /// <summary>
    /// Returns a new <see cref="CornerRadius"/> with only the top-left corner changed.
    /// </summary>
    public static CornerRadius SetTopLeft(CornerRadius radius, float value) =>
        radius with { TopLeft = value };

    /// <summary>
    /// Returns a new <see cref="CornerRadius"/> with only the top-right corner changed.
    /// </summary>
    public static CornerRadius SetTopRight(CornerRadius radius, float value) =>
        radius with { TopRight = value };

    /// <summary>
    /// Returns a new <see cref="CornerRadius"/> with only the bottom-right corner changed.
    /// </summary>
    public static CornerRadius SetBottomRight(CornerRadius radius, float value) =>
        radius with { BottomRight = value };

    /// <summary>
    /// Returns a new <see cref="CornerRadius"/> with only the bottom-left corner changed.
    /// </summary>
    public static CornerRadius SetBottomLeft(CornerRadius radius, float value) =>
        radius with { BottomLeft = value };

    /// <summary>
    /// Returns a value indicating whether all four corners are zero.
    /// </summary>
    /// <returns><c>true</c> if all corners are zero.</returns>
    public readonly bool IsEmpty() => TopLeft == 0f && TopRight == 0f && BottomRight == 0f && BottomLeft == 0f;
}

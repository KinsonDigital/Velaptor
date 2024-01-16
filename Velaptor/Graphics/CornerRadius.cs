// <copyright file="CornerRadius.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds all of the radius values for each corner of a rectangle.
/// </summary>
public readonly record struct CornerRadius
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CornerRadius"/> struct.
    /// </summary>
    /// <param name="topLeft">The top left corner radius.</param>
    /// <param name="bottomLeft">The bottom left corner radius.</param>
    /// <param name="bottomRight">The bottom right corner radius.</param>
    /// <param name="topRight">The top right corner radius.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public CornerRadius(float topLeft, float bottomLeft, float bottomRight, float topRight)
    {
        TopLeft = topLeft;
        TopRight = topRight;
        BottomRight = bottomRight;
        BottomLeft = bottomLeft;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CornerRadius"/> struct.
    /// </summary>
    /// <param name="value">The value to set all corner radius values.</param>
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1642:Constructor summary documentation should begin with standard text",
        Justification = "The standard text is incorrect and says class instead of struct.")]
    public CornerRadius(float value)
    {
        TopLeft = value;
        TopRight = value;
        BottomRight = value;
        BottomLeft = value;
    }

    /// <summary>
    /// Gets the top left corner radius.
    /// </summary>
    public float TopLeft { get; init; }

    /// <summary>
    /// Gets the bottom left corner radius.
    /// </summary>
    public float BottomLeft { get; init; }

    /// <summary>
    /// Gets the bottom right corner radius.
    /// </summary>
    public float BottomRight { get; init; }

    /// <summary>
    /// Gets the top right corner radius.
    /// </summary>
    public float TopRight { get; init; }

    /// <summary>
    /// Returns an empty <see cref="CornerRadius"/> instance.
    /// </summary>
    /// <returns>The empty instance.</returns>
    public static CornerRadius Empty() => new (0f);

    /// <summary>
    /// Sets the top left corner value of the given <paramref name="cornerRadius"/> to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to change.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The corner radius with the updated value.</returns>
    public static CornerRadius SetTopLeft(CornerRadius cornerRadius, float value)
        => new (value, cornerRadius.BottomLeft, cornerRadius.BottomRight, cornerRadius.TopRight);

    /// <summary>
    /// Sets the bottom left corner value of the given <paramref name="cornerRadius"/> to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to change.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The corner radius with the updated value.</returns>
    public static CornerRadius SetBottomLeft(CornerRadius cornerRadius, float value)
        => new (cornerRadius.TopLeft, value, cornerRadius.BottomRight, cornerRadius.TopRight);

    /// <summary>
    /// Sets the bottom right corner value of the given <paramref name="cornerRadius"/> to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to change.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The corner radius with the updated value.</returns>
    public static CornerRadius SetBottomRight(CornerRadius cornerRadius, float value)
        => new (cornerRadius.TopLeft, cornerRadius.BottomLeft, value, cornerRadius.TopRight);

    /// <summary>
    /// Sets the top right corner value of the given <paramref name="cornerRadius"/> to the given <paramref name="value"/>.
    /// </summary>
    /// <param name="cornerRadius">The corner radius to change.</param>
    /// <param name="value">The value to set.</param>
    /// <returns>The corner radius with the updated value.</returns>
    public static CornerRadius SetTopRight(CornerRadius cornerRadius, float value)
        => new (cornerRadius.TopLeft, cornerRadius.BottomLeft, cornerRadius.BottomRight, value);

    /// <summary>
    /// Returns a value indicating if the <see cref="CornerRadius"/> is empty.
    /// </summary>
    /// <returns>True if empty.</returns>
    public bool IsEmpty() => TopLeft == 0f &&
                             BottomLeft == 0f &&
                             BottomRight == 0f &&
                             TopRight == 0f;
}

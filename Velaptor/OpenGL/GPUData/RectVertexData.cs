// <copyright file="RectVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a single vertex of the <see cref="RectGPUData"/> sent to the GPU.
/// </summary>
internal readonly struct RectVertexData
{
    private const uint TotalElements = 16u;
    private static readonly uint Stride;

    /// <summary>
    /// Initializes static members of the <see cref="RectVertexData"/> struct.
    /// </summary>
    static RectVertexData() => Stride = TotalElements * sizeof(float);

    /// <summary>
    /// Initializes a new instance of the <see cref="RectVertexData"/> struct.
    /// </summary>
    /// <param name="vertexPos">The position of the vertex.</param>
    /// <param name="rectangle">The rectangle components.</param>
    /// <param name="color">The fill or border color.</param>
    /// <param name="isSolid">True if the rectangle is a solid color.</param>
    /// <param name="borderThickness">The thickness of the border.</param>
    /// <param name="topLeftCornerRadius">The top left corner radius.</param>
    /// <param name="bottomLeftCornerRadius">The bottom left corner radius.</param>
    /// <param name="bottomRightCornerRadius">The bottom right corner radius.</param>
    /// <param name="topRightCornerRadius">The top right corner radius.</param>
    public RectVertexData(
        Vector2 vertexPos,
        Vector4 rectangle,
        Color color,
        bool isSolid,
        float borderThickness,
        float topLeftCornerRadius,
        float bottomLeftCornerRadius,
        float bottomRightCornerRadius,
        float topRightCornerRadius)
    {
        VertexPos = vertexPos;
        Rectangle = rectangle;
        Color = color;
        IsSolid = isSolid;
        BorderThickness = borderThickness;
        TopLeftCornerRadius = topLeftCornerRadius;
        BottomLeftCornerRadius = bottomLeftCornerRadius;
        BottomRightCornerRadius = bottomRightCornerRadius;
        TopRightCornerRadius = topRightCornerRadius;
    }

    /// <summary>
    /// Gets the position of a single rectangle vertex.
    /// </summary>
    public Vector2 VertexPos { get; }

    /// <summary>
    /// Gets the components that make up the rectangle.
    /// </summary>
    /// <remarks>
    /// The components below represent the rectangle:
    /// <list type="bullet">
    ///     <item><c>X:</c> The position of the center of the rectangle on the X axis.</item>
    ///     <item><c>Y:</c> The position of the center of the rectangle on the Y axis.</item>
    ///     <item><c>Z:</c> The width of the rectangle.</item>
    ///     <item><c>W:</c> The height of the rectangle.</item>
    /// </list>
    /// </remarks>
    public Vector4 Rectangle { get; }

    /// <summary>
    /// Gets the color of the rectangle.
    /// </summary>
    /// <remarks>
    ///     This is the solid color if the entire rectangle <see cref="IsSolid"/> is set to <c>true</c>
    ///     and is the solid color of the rectangle border if <see cref="IsSolid"/> is set to <c>false</c>.
    /// </remarks>
    public Color Color { get; }

    /// <summary>
    /// Gets a value indicating whether or not the rectangle will be rendered as a solid rectangle.
    /// </summary>
    public bool IsSolid { get; }

    /// <summary>
    /// Gets the thickness of the rectangle border if the <see cref="IsSolid"/> is set to <c>false</c>.
    /// </summary>
    public float BorderThickness { get; }

    /// <summary>
    /// Gets the radius of the top left corner of the rectangle.
    /// </summary>
    public float TopLeftCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the bottom left corner of the rectangle.
    /// </summary>
    public float BottomLeftCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the bottom right corner of the rectangle.
    /// </summary>
    public float BottomRightCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the top right corner of the rectangle.
    /// </summary>
    public float TopRightCornerRadius { get; }

    /// <summary>
    /// Returns an empty <see cref="RectVertexData"/> instance.
    /// </summary>
    /// <returns>The empty instance.</returns>
    public static RectVertexData Empty() =>
        new (
            Vector2.Zero,
            Vector4.Zero,
            Color.Empty,
            false,
            0f,
            0f,
            0f,
            0f,
            0f);

    /// <summary>
    /// Gets the stride of the entire vertex data chunk in bytes.
    /// </summary>
    /// <returns>The total bytes of the vertex's stride.</returns>
    public static uint GetStride() => Stride;

    /// <summary>
    /// Creates a new instance of a <see cref="RectVertexData"/> struct with the given <paramref name="position"/>.
    /// </summary>
    /// <param name="x">The X component of the position.</param>
    /// <param name="y">The Y component of the position.</param>
    /// <returns>The new instance.</returns>
    public static RectVertexData New(float x, float y) =>
        new (new Vector2(x, y),
            Vector4.Zero,
            Color.Empty,
            false,
            1f,
            0f,
            0f,
            0f,
            0f);

    /// <summary>
    /// Returns all of the vertex data as an array or ordered values.
    /// </summary>
    /// <returns>All of the vertex data values.</returns>
    public IEnumerable<float> ToArray()
    {
        /* NOTE:
            The order of the array elements is extremely important.
            It determines the layout of each stride of vertex data and that layout
            has to match the layout told to OpenGL.
        */

        var result = new List<float>();

        result.AddRange(VertexPos.ToArray());
        result.AddRange(Rectangle.ToArray());
        result.AddRange(Color.ToArray());

        result.Add(IsSolid ? 1f : 0f);
        result.Add(BorderThickness);
        result.Add(TopLeftCornerRadius);
        result.Add(BottomLeftCornerRadius);
        result.Add(BottomRightCornerRadius);
        result.Add(TopRightCornerRadius);

        return result.ToArray();
    }
}

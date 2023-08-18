// <copyright file="ShapeVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a single vertex of the <see cref="ShapeGpuData"/> sent to the GPU.
/// </summary>
internal readonly struct ShapeVertexData
{
    private const uint TotalElements = 16u;
    private static readonly uint Stride;

    /// <summary>
    /// Initializes static members of the <see cref="ShapeVertexData"/> struct.
    /// </summary>
    static ShapeVertexData() => Stride = TotalElements * sizeof(float);

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeVertexData"/> struct.
    /// </summary>
    /// <param name="vertexPos">The position of the vertex.</param>
    /// <param name="boundingBox">The corner vectors of the bounding box that contain the shape.</param>
    /// <param name="color">The fill or border color.</param>
    /// <param name="isSolid">True if the shape is a solid color.</param>
    /// <param name="borderThickness">The thickness of the border.</param>
    /// <param name="topLeftCornerRadius">The top left corner radius.</param>
    /// <param name="bottomLeftCornerRadius">The bottom left corner radius.</param>
    /// <param name="bottomRightCornerRadius">The bottom right corner radius.</param>
    /// <param name="topRightCornerRadius">The top right corner radius.</param>
    public ShapeVertexData(
        Vector2 vertexPos,
        Vector4 boundingBox,
        Color color,
        bool isSolid,
        float borderThickness,
        float topLeftCornerRadius,
        float bottomLeftCornerRadius,
        float bottomRightCornerRadius,
        float topRightCornerRadius)
    {
        VertexPos = vertexPos;
        BoundingBox = boundingBox;
        Color = color;
        IsSolid = isSolid;
        BorderThickness = borderThickness;
        TopLeftCornerRadius = topLeftCornerRadius;
        BottomLeftCornerRadius = bottomLeftCornerRadius;
        BottomRightCornerRadius = bottomRightCornerRadius;
        TopRightCornerRadius = topRightCornerRadius;
    }

    /// <summary>
    /// Gets the position of a single shape vertex.
    /// </summary>
    public Vector2 VertexPos { get; }

    /// <summary>
    /// Gets the components that make up the bounding box.
    /// </summary>
    /// <remarks>
    /// The components below represent the bounding box:
    /// <list type="bullet">
    ///     <item><c>X:</c> The position of the center of the bounding box on the X axis.</item>
    ///     <item><c>Y:</c> The position of the center of the bounding box on the Y axis.</item>
    ///     <item><c>Z:</c> The width of the bound box.</item>
    ///     <item><c>W:</c> The height of the bound box.</item>
    /// </list>
    /// </remarks>
    public Vector4 BoundingBox { get; }

    /// <summary>
    /// Gets the color of the shape.
    /// </summary>
    /// <remarks>
    ///     This is the solid color if the entire shape <see cref="IsSolid"/> is set to <c>true</c>
    ///     and is the solid color of the shape border if <see cref="IsSolid"/> is set to <c>false</c>.
    /// </remarks>
    public Color Color { get; }

    /// <summary>
    /// Gets a value indicating whether or not the shape will be rendered as a solid shape.
    /// </summary>
    public bool IsSolid { get; }

    /// <summary>
    /// Gets the thickness of the shape border if the <see cref="IsSolid"/> is set to <c>false</c>.
    /// </summary>
    public float BorderThickness { get; }

    /// <summary>
    /// Gets the radius of the top left corner of the bound box.
    /// </summary>
    public float TopLeftCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the bottom left corner of the bound box.
    /// </summary>
    public float BottomLeftCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the bottom right corner of the bound box.
    /// </summary>
    public float BottomRightCornerRadius { get; }

    /// <summary>
    /// Gets the radius of the top right corner of the bound box.
    /// </summary>
    public float TopRightCornerRadius { get; }

    /// <summary>
    /// Returns an empty <see cref="ShapeVertexData"/> instance.
    /// </summary>
    /// <returns>The empty instance.</returns>
    public static ShapeVertexData Empty() =>
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
    /// Creates a new instance of a <see cref="ShapeVertexData"/> struct with the given <paramref name="x"/> and <paramref name="y"/> components..
    /// </summary>
    /// <param name="x">The X component of the position.</param>
    /// <param name="y">The Y component of the position.</param>
    /// <returns>The new instance.</returns>
    public static ShapeVertexData New(float x, float y) =>
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
        result.AddRange(BoundingBox.ToArray());
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

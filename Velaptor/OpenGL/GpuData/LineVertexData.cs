// <copyright file="LineVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a single vertex of the <see cref="LineGpuData"/> sent to the GPU.
/// </summary>
internal readonly struct LineVertexData
{
    private const uint TotalElements = 6u;
    private static readonly uint Stride;

    /// <summary>
    /// Initializes static members of the <see cref="LineVertexData"/> struct.
    /// </summary>
    static LineVertexData() => Stride = TotalElements * sizeof(float);

    /// <summary>
    /// Initializes a new instance of the <see cref="LineVertexData"/> struct.
    /// </summary>
    /// <param name="vertexPos">The position of a the vertice for the render quad area.</param>
    /// <param name="color">The fill or border color.</param>
    public LineVertexData(
        Vector2 vertexPos,
        Color color)
    {
        VertexPos = vertexPos;
        Color = color;
    }

    /// <summary>
    /// Gets the top left corner of the line render area.
    /// </summary>
    public Vector2 VertexPos { get; }

    /// <summary>
    /// Gets the color of the line.
    /// </summary>
    public Color Color { get; }

    /// <summary>
    /// Returns an empty <see cref="LineVertexData"/> instance.
    /// </summary>
    /// <returns>The empty instance.</returns>
    public static LineVertexData Empty() =>
        new (Vector2.Zero,
            Color.Empty);

    /// <summary>
    /// Gets the stride of the entire vertex data chunk in bytes.
    /// </summary>
    /// <returns>The total bytes of the vertex's stride.</returns>
    public static uint GetStride() => Stride;

    /// <summary>
    /// Returns all the vertex data as an array or ordered values.
    /// </summary>
    /// <returns>All the vertex data values.</returns>
    public IEnumerable<float> ToArray()
    {
        /* NOTE:
            The order of the array elements is extremely important.
            It determines the layout of each stride of vertex data and that layout
            has to match the layout told to OpenGL.
        */

        var result = new List<float>();

        result.AddRange(VertexPos.ToArray());
        result.AddRange(Color.ToArray());

        return result.ToArray();
    }
}

// <copyright file="RectGPUData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData;

using System.Collections.Generic;

/// <summary>
/// Holds all the necessary data for a rectangle to sent to the GPU for rendering.
/// </summary>
internal readonly struct RectGPUData
{
    private const uint TotalVertexItems = 4u;
    private static readonly uint TotalBytes;

    /// <summary>
    /// Initializes static members of the <see cref="RectGPUData"/> struct.
    /// </summary>
    static RectGPUData() => TotalBytes = RectVertexData.GetStride() * TotalVertexItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="RectGPUData"/> struct.
    /// </summary>
    /// <param name="vertex1">The first vertex of the rectangle.</param>
    /// <param name="vertex2">The second vertex of the rectangle.</param>
    /// <param name="vertex3">The third vertex of the rectangle.</param>
    /// <param name="vertex4">The fourth vertex of the rectangle.</param>
    public RectGPUData(RectVertexData vertex1, RectVertexData vertex2, RectVertexData vertex3, RectVertexData vertex4)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
        Vertex4 = vertex4;
    }

    /// <summary>
    /// Gets the vertex data for the top left vertex of a rectangle.
    /// </summary>
    /// <remarks>
    ///     This is first vertex of the top left triangle that makes up the rectangle.
    /// </remarks>
    public RectVertexData Vertex1 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom left vertex of a rectangle.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is second vertex of the bottom left triangle that makes up the rectangle.
    /// </para>
    /// <para>
    ///     This vertex is shared with the second vertex of the bottom right triangle of the rectangle.
    /// </para>
    /// </remarks>
    public RectVertexData Vertex2 { get; }

    /// <summary>
    /// Gets the vertex data for the top right vertex of a rectangle.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is third vertex of the top left triangle that makes up the rectangle.
    /// </para>
    /// <para>
    ///     This vertex is shared with the first vertex of the bottom right triangle of the rectangle.
    /// </para>
    /// </remarks>
    public RectVertexData Vertex3 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom right vertex of a rectangle.
    /// </summary>
    /// <remarks>
    ///     This is the third vertex of the bottom right triangle of the rectangle.
    /// </remarks>
    public RectVertexData Vertex4 { get; }

    /// <summary>
    /// Creates an empty instance of <see cref="RectGPUData"/>.
    /// </summary>
    /// <returns>An empty instance.</returns>
    public static RectGPUData Empty() => new (RectVertexData.Empty(), RectVertexData.Empty(), RectVertexData.Empty(), RectVertexData.Empty());

    /// <summary>
    /// The total number of bytes that the <see cref="RectGPUData"/> data contains.
    /// </summary>
    /// <returns>The total number of bytes.</returns>
    public static uint GetTotalBytes() => TotalBytes;

    /// <summary>
    /// Returns all of the vertex data in an array of <see cref="float"/> values.
    /// </summary>
    /// <returns>The array of vertex data.</returns>
    public float[] ToArray()
    {
        var result = new List<float>();

        result.AddRange(Vertex1.ToArray());
        result.AddRange(Vertex2.ToArray());
        result.AddRange(Vertex3.ToArray());
        result.AddRange(Vertex4.ToArray());

        return result.ToArray();
    }
}

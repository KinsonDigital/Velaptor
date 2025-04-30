// <copyright file="LineGpuData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

/// <summary>
/// Holds all the necessary data for a line to send to the GPU for rendering.
/// </summary>
internal readonly struct LineGpuData
{
    private const uint TotalVertexItems = 4u;
    private static readonly uint Stride;

    /// <summary>
    /// Initializes static members of the <see cref="LineGpuData"/> struct.
    /// </summary>
    static LineGpuData() => Stride = LineVertexData.GetStride() * TotalVertexItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuData"/> struct.
    /// </summary>
    /// <param name="vertex1">The first vertex of the line.</param>
    /// <param name="vertex2">The second vertex of the line.</param>
    /// <param name="vertex3">The third vertex of the line.</param>
    /// <param name="vertex4">The fourth vertex of the line.</param>
    public LineGpuData(LineVertexData vertex1, LineVertexData vertex2, LineVertexData vertex3, LineVertexData vertex4)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
        Vertex4 = vertex4;
    }

    /// <summary>
    /// Gets the vertex data for the top left vertex of a render area.
    /// </summary>
    /// <remarks>
    ///     This is first vertex of the top left triangle that makes up the render area.
    /// </remarks>
    public LineVertexData Vertex1 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom left vertex of a render area.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is second vertex of the bottom left triangle that makes up the render area.
    /// </para>
    /// <para>
    ///     This vertex is shared with the second vertex of the bottom right triangle of the render area.
    /// </para>
    /// </remarks>
    public LineVertexData Vertex2 { get; }

    /// <summary>
    /// Gets the vertex data for the top right vertex of a render area.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is third vertex of the top left triangle that makes up the render area.
    /// </para>
    /// <para>
    ///     This vertex is shared with the first vertex of the bottom right triangle of the render area.
    /// </para>
    /// </remarks>
    public LineVertexData Vertex3 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom right vertex of a render area.
    /// </summary>
    /// <remarks>
    ///     This is the third vertex of the bottom right triangle of the render area.
    /// </remarks>
    public LineVertexData Vertex4 { get; }

    /// <summary>
    /// The total number of bytes that the <see cref="LineGpuData"/> data contains.
    /// </summary>
    /// <returns>The total number of bytes.</returns>
    public static uint GetTotalBytes() => Stride;

    /// <summary>
    /// Generates default data to be sent to the GPU.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>The default data.</returns>
    public static float[] GenerateDefaultData(uint batchSize)
    {
        var result = new float[batchSize * 24];

        for (var i = 0u; i < batchSize; i++)
        {
            result[i] = 0f;
        }

        return result;
    }

    /// <summary>
    /// Returns all the vertex data in an array of <see cref="float"/> value.
    /// </summary>
    /// <returns>The array of vertex data.</returns>
    public float[] ToArray()
    {
        /* NOTE:
            The order of the array elements is extremely important.
            It determines the layout of each stride of vertex data and that layout
            has to match the layout told to OpenGL.
        */

        var result = new float[24];

        // Vector 1
        result[0] = Vertex1.VertexPos.X;
        result[1] = Vertex1.VertexPos.Y;
        result[2] = Vertex1.Color.R;
        result[3] = Vertex1.Color.G;
        result[4] = Vertex1.Color.B;
        result[5] = Vertex1.Color.A;

        // Vector 2
        result[6] = Vertex2.VertexPos.X;
        result[7] = Vertex2.VertexPos.Y;
        result[8] = Vertex2.Color.R;
        result[9] = Vertex2.Color.G;
        result[10] = Vertex2.Color.B;
        result[11] = Vertex2.Color.A;

        // Vector 3
        result[12] = Vertex3.VertexPos.X;
        result[13] = Vertex3.VertexPos.Y;
        result[14] = Vertex3.Color.R;
        result[15] = Vertex3.Color.G;
        result[16] = Vertex3.Color.B;
        result[17] = Vertex3.Color.A;

        // Vector 4
        result[18] = Vertex4.VertexPos.X;
        result[19] = Vertex4.VertexPos.Y;
        result[20] = Vertex4.Color.R;
        result[21] = Vertex4.Color.G;
        result[22] = Vertex4.Color.B;
        result[23] = Vertex4.Color.A;

        return result;
    }
}

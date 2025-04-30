// <copyright file="ShapeGpuData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

/// <summary>
/// Holds all the necessary data for a shape to send to the GPU for rendering.
/// </summary>
internal readonly struct ShapeGpuData
{
    private const uint TotalVertexItems = 4u;
    private static readonly uint TotalBytes;

    /// <summary>
    /// Initializes static members of the <see cref="ShapeGpuData"/> struct.
    /// </summary>
    static ShapeGpuData() => TotalBytes = ShapeVertexData.GetStride() * TotalVertexItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuData"/> struct.
    /// </summary>
    /// <param name="vertex1">The first vertex of the shape.</param>
    /// <param name="vertex2">The second vertex of the shape.</param>
    /// <param name="vertex3">The third vertex of the shape.</param>
    /// <param name="vertex4">The fourth vertex of the shape.</param>
    public ShapeGpuData(ShapeVertexData vertex1, ShapeVertexData vertex2, ShapeVertexData vertex3, ShapeVertexData vertex4)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
        Vertex4 = vertex4;
    }

    /// <summary>
    /// Gets the vertex data for the top left vertex of a bounding box.
    /// </summary>
    /// <remarks>
    ///     This is first vertex of the top left triangle that makes up the bounding box.
    /// </remarks>
    public ShapeVertexData Vertex1 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom left vertex of a bounding box.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is second vertex of the bottom left triangle that makes up the bounding box.
    /// </para>
    /// <para>
    ///     This vertex is shared with the second vertex of the bottom right triangle of the bounding box.
    /// </para>
    /// </remarks>
    public ShapeVertexData Vertex2 { get; }

    /// <summary>
    /// Gets the vertex data for the top right vertex of a bounding box.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     This is third vertex of the top left triangle that makes up the bounding box.
    /// </para>
    /// <para>
    ///     This vertex is shared with the first vertex of the bottom right triangle of the bounding box.
    /// </para>
    /// </remarks>
    public ShapeVertexData Vertex3 { get; }

    /// <summary>
    /// Gets the vertex data for the bottom right vertex of a bounding box.
    /// </summary>
    /// <remarks>
    ///     This is the third vertex of the bottom right triangle of the bounding box.
    /// </remarks>
    public ShapeVertexData Vertex4 { get; }

    /// <summary>
    /// Creates an empty instance of <see cref="ShapeGpuData"/>.
    /// </summary>
    /// <returns>An empty instance.</returns>
    public static ShapeGpuData Empty() => new (ShapeVertexData.Empty(), ShapeVertexData.Empty(), ShapeVertexData.Empty(), ShapeVertexData.Empty());

    /// <summary>
    /// The total number of bytes that the <see cref="ShapeGpuData"/> data contains.
    /// </summary>
    /// <returns>The total number of bytes.</returns>
    public static uint GetTotalBytes() => TotalBytes;

    /// <summary>
    /// Generates default data to be sent to the GPU.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>The default data.</returns>
    public static float[] GenerateDefaultData(uint batchSize)
    {
        var result = new float[batchSize * 64];

        for (var i = 0u; i < batchSize; i++)
        {
            result[i] = 0f;
        }

        return result;
    }

    /// <summary>
    /// Returns all the vertex data in an array of <see cref="float"/> values.
    /// </summary>
    /// <returns>The array of vertex data.</returns>
    public float[] ToArray()
    {
        var result = new float[64];

        // Vector 1
        result[0] = Vertex1.VertexPos.X;
        result[1] = Vertex1.VertexPos.Y;
        result[2] = Vertex1.BoundingBox.X;
        result[3] = Vertex1.BoundingBox.Y;
        result[4] = Vertex1.BoundingBox.Z;
        result[5] = Vertex1.BoundingBox.W;
        result[6] = Vertex1.Color.R;
        result[7] = Vertex1.Color.G;
        result[8] = Vertex1.Color.B;
        result[9] = Vertex1.Color.A;
        result[10] = Vertex1.IsSolid ? 1f : 0f;
        result[11] = Vertex1.BorderThickness;
        result[12] = Vertex1.TopLeftCornerRadius;
        result[13] = Vertex1.BottomLeftCornerRadius;
        result[14] = Vertex1.BottomRightCornerRadius;
        result[15] = Vertex1.TopRightCornerRadius;

        // Vector 2
        result[16] = Vertex2.VertexPos.X;
        result[17] = Vertex2.VertexPos.Y;
        result[18] = Vertex2.BoundingBox.X;
        result[19] = Vertex2.BoundingBox.Y;
        result[20] = Vertex2.BoundingBox.Z;
        result[21] = Vertex2.BoundingBox.W;
        result[22] = Vertex2.Color.R;
        result[23] = Vertex2.Color.G;
        result[24] = Vertex2.Color.B;
        result[25] = Vertex2.Color.A;
        result[26] = Vertex2.IsSolid ? 1f : 0f;
        result[27] = Vertex2.BorderThickness;
        result[28] = Vertex2.TopLeftCornerRadius;
        result[29] = Vertex2.BottomLeftCornerRadius;
        result[30] = Vertex2.BottomRightCornerRadius;
        result[31] = Vertex2.TopRightCornerRadius;

        // Vector 3
        result[32] = Vertex3.VertexPos.X;
        result[33] = Vertex3.VertexPos.Y;
        result[34] = Vertex3.BoundingBox.X;
        result[35] = Vertex3.BoundingBox.Y;
        result[36] = Vertex3.BoundingBox.Z;
        result[37] = Vertex3.BoundingBox.W;
        result[38] = Vertex3.Color.R;
        result[39] = Vertex3.Color.G;
        result[40] = Vertex3.Color.B;
        result[41] = Vertex3.Color.A;
        result[42] = Vertex3.IsSolid ? 1f : 0f;
        result[43] = Vertex3.BorderThickness;
        result[44] = Vertex3.TopLeftCornerRadius;
        result[45] = Vertex3.BottomLeftCornerRadius;
        result[46] = Vertex3.BottomRightCornerRadius;
        result[47] = Vertex3.TopRightCornerRadius;

        // Vector 4
        result[48] = Vertex4.VertexPos.X;
        result[49] = Vertex4.VertexPos.Y;
        result[50] = Vertex4.BoundingBox.X;
        result[51] = Vertex4.BoundingBox.Y;
        result[52] = Vertex4.BoundingBox.Z;
        result[53] = Vertex4.BoundingBox.W;
        result[54] = Vertex4.Color.R;
        result[55] = Vertex4.Color.G;
        result[56] = Vertex4.Color.B;
        result[57] = Vertex4.Color.A;
        result[58] = Vertex4.IsSolid ? 1f : 0f;
        result[59] = Vertex4.BorderThickness;
        result[60] = Vertex4.TopLeftCornerRadius;
        result[61] = Vertex4.BottomLeftCornerRadius;
        result[62] = Vertex4.BottomRightCornerRadius;
        result[63] = Vertex4.TopRightCornerRadius;

        return result;
    }
}

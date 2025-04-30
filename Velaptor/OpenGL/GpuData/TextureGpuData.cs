// <copyright file="TextureGpuData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds data for a single quad in the GPU vertex buffer.
/// </summary>
internal readonly struct TextureGpuData : IEquatable<TextureGpuData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGpuData"/> struct.
    /// </summary>
    /// <param name="vertex1">The top left corner vertex of the quad.</param>
    /// <param name="vertex2">The top right corner vertex of the quad.</param>
    /// <param name="vertex3">The bottom right corner vertex of the quad.</param>
    /// <param name="vertex4">The bottom left corner vertex of the quad.</param>
    public TextureGpuData(in TextureVertexData vertex1, TextureVertexData vertex2, TextureVertexData vertex3, TextureVertexData vertex4)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
        Vertex4 = vertex4;
    }

    /// <summary>
    /// Gets the top left corner vertex of the quad.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public TextureVertexData Vertex1 { get; }

    /// <summary>
    /// Gets the top right corner vertex of the quad.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public TextureVertexData Vertex2 { get; }

    /// <summary>
    /// Gets the bottom right corner vertex of the quad.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public TextureVertexData Vertex3 { get; }

    /// <summary>
    /// Gets the bottom left corner vertex of the quad.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public TextureVertexData Vertex4 { get; }

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the two operands are equal.</returns>
    public static bool operator ==(TextureGpuData left, TextureGpuData right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the two operands are equal.</returns>
    public static bool operator !=(TextureGpuData left, TextureGpuData right) => !(left == right);

    /// <summary>
    /// Returns the total number of bytes for this struct.
    /// </summary>
    /// <returns>The total number of bytes in size.</returns>
    public static uint GetTotalBytes() => TextureVertexData.GetStride() * 4u;

    /// <summary>
    /// Generates default data to be sent to the GPU.
    /// </summary>
    /// <param name="batchSize">The batch size.</param>
    /// <returns>The default data.</returns>
    public static float[] GenerateDefaultData(uint batchSize)
    {
        var result = new float[batchSize * 32];

        for (var i = 0u; i < batchSize; i++)
        {
            result[i] = 0f;
        }

        return result;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not TextureGpuData data)
        {
            return false;
        }

        return data == this;
    }

    /// <inheritdoc/>
    public bool Equals(TextureGpuData other)
        => Vertex1 == other.Vertex1 &&
           Vertex2 == other.Vertex2 &&
           Vertex3 == other.Vertex3 &&
           Vertex4 == other.Vertex4;

    /// <summary>
    /// Converts the gpu data into an array of <see cref="float"/> values.
    /// </summary>
    /// <returns>An array of float values.</returns>
    public float[] ToArray()
    {
        var result = new float[32];

        // Vertex 1
        result[0] = Vertex1.VertexPos.X;
        result[1] = Vertex1.VertexPos.Y;
        result[2] = Vertex1.TextureCoord.X;
        result[3] = Vertex1.TextureCoord.Y;
        result[4] = Vertex1.TintColor.R;
        result[5] = Vertex1.TintColor.G;
        result[6] = Vertex1.TintColor.B;
        result[7] = Vertex1.TintColor.A;

        // Vertex 2
        result[8] = Vertex2.VertexPos.X;
        result[9] = Vertex2.VertexPos.Y;
        result[10] = Vertex2.TextureCoord.X;
        result[11] = Vertex2.TextureCoord.Y;
        result[12] = Vertex2.TintColor.R;
        result[13] = Vertex2.TintColor.G;
        result[14] = Vertex2.TintColor.B;
        result[15] = Vertex2.TintColor.A;

        // Vertex 3
        result[16] = Vertex3.VertexPos.X;
        result[17] = Vertex3.VertexPos.Y;
        result[18] = Vertex3.TextureCoord.X;
        result[19] = Vertex3.TextureCoord.Y;
        result[20] = Vertex3.TintColor.R;
        result[21] = Vertex3.TintColor.G;
        result[22] = Vertex3.TintColor.B;
        result[23] = Vertex3.TintColor.A;

        // Vertex 4
        result[24] = Vertex4.VertexPos.X;
        result[25] = Vertex4.VertexPos.Y;
        result[26] = Vertex4.TextureCoord.X;
        result[27] = Vertex4.TextureCoord.Y;
        result[28] = Vertex4.TintColor.R;
        result[29] = Vertex4.TintColor.G;
        result[30] = Vertex4.TintColor.B;
        result[31] = Vertex4.TintColor.A;

        return result;
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => Vertex1.GetHashCode() + Vertex2.GetHashCode() + Vertex3.GetHashCode() + Vertex4.GetHashCode();
}

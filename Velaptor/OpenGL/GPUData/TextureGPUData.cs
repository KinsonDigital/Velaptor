// <copyright file="TextureGPUData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds data for a single quad in the GPU vertex buffer.
/// </summary>
internal readonly struct TextureGPUData : IEquatable<TextureGPUData>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGPUData"/> struct.
    /// </summary>
    /// <param name="vertex1">The top left corner vertex of the quad.</param>
    /// <param name="vertex2">The top right corner vertex of the quad.</param>
    /// <param name="vertex3">The bottom right corner vertex of the quad.</param>
    /// <param name="vertex4">The bottom left corner vertex of the quad.</param>
    public TextureGPUData(in TextureVertexData vertex1, TextureVertexData vertex2, TextureVertexData vertex3, TextureVertexData vertex4)
    {
        Vertex1 = vertex1;
        Vertex2 = vertex2;
        Vertex3 = vertex3;
        Vertex4 = vertex4;
    }

    /// <summary>
    /// Gets the top left corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex1 { get; }

    /// <summary>
    /// Gets the top right corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex2 { get; }

    /// <summary>
    /// Gets the bottom right corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex3 { get; }

    /// <summary>
    /// Gets the bottom left corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex4 { get; }

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// operation is equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the two operands are equal.</returns>
    public static bool operator ==(TextureGPUData left, TextureGPUData right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// operation is not equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the two operands are equal.</returns>
    public static bool operator !=(TextureGPUData left, TextureGPUData right) => !(left == right);

    /// <summary>
    /// Returns the total number of bytes for this struct.
    /// </summary>
    /// <returns>The total number of bytes in size.</returns>
    public static uint GetTotalBytes() => TextureVertexData.GetStride() * 4u;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not TextureGPUData data)
        {
            return false;
        }

        return data == this;
    }

    /// <inheritdoc/>
    public bool Equals(TextureGPUData other)
        => Vertex1 == other.Vertex1 &&
           Vertex2 == other.Vertex2 &&
           Vertex3 == other.Vertex3 &&
           Vertex4 == other.Vertex4;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => Vertex1.GetHashCode() + Vertex2.GetHashCode() + Vertex3.GetHashCode() + Vertex4.GetHashCode();
}

// <copyright file="TextureQuadData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Holds data for a single quad in the GPU vertex buffer.
/// </summary>
internal struct TextureQuadData : IEquatable<TextureQuadData>
{
    /// <summary>
    /// The top left corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex1;

    /// <summary>
    /// The top right corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex2;

    /// <summary>
    /// The bottom right corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex3;

    /// <summary>
    /// The bottom left corner vertex of the quad.
    /// </summary>
    public TextureVertexData Vertex4;

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// operation is equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the 2 operands are equal.</returns>
    public static bool operator ==(TextureQuadData left, TextureQuadData right) => left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// operation is not equal.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if the 2 operands are equal.</returns>
    public static bool operator !=(TextureQuadData left, TextureQuadData right) => !(left == right);

    /// <summary>
    /// Returns the total number of bytes for this struct.
    /// </summary>
    /// <returns>The total number of bytes in size.</returns>
    public static uint GetTotalBytes() => TextureVertexData.Stride() * 4u;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not TextureQuadData data)
        {
            return false;
        }

        return data == this;
    }

    /// <inheritdoc/>
    public bool Equals(TextureQuadData other)
        => this.Vertex1 == other.Vertex1 &&
           this.Vertex2 == other.Vertex2 &&
           this.Vertex3 == other.Vertex3 &&
           this.Vertex4 == other.Vertex4;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public override int GetHashCode() => this.Vertex1.GetHashCode() + this.Vertex2.GetHashCode() + this.Vertex3.GetHashCode() + this.Vertex4.GetHashCode();
}

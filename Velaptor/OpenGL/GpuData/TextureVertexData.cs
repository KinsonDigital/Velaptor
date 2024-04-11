// <copyright file="TextureVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GpuData;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;

/// <summary>
/// Represents a single vertex of the <see cref="TextureGpuData"/> sent to the GPU.
/// </summary>
internal readonly struct TextureVertexData : IEquatable<TextureVertexData>
{
    private const uint TotalElements = 8u;
    private static readonly uint Stride;

    /// <summary>
    /// Initializes static members of the <see cref="TextureVertexData"/> struct.
    /// </summary>
    static TextureVertexData() => Stride = TotalElements * sizeof(float);

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureVertexData"/> struct.
    /// </summary>
    /// <param name="vertexPos">The position of the vertex in NDC (normal device coordinate) coordinates.</param>
    /// <param name="textureCoord">The point in a texture that corresponds to this structure's <see cref="VertexPos"/>.</param>
    /// <param name="tintColor">The color of the vertex.</param>
    public TextureVertexData(in Vector2 vertexPos, Vector2 textureCoord, Color tintColor)
    {
        VertexPos = vertexPos;
        TextureCoord = textureCoord;
        TintColor = tintColor;
    }

    /// <summary>
    /// Gets the position of the vertex in NDC (normal device coordinate) coordinates.
    /// </summary>
    public Vector2 VertexPos { get; } // Location 0 | aPosition

    /// <summary>
    /// Gets the point in a texture that corresponds to this structure's <see cref="VertexPos"/>.
    /// </summary>
    public Vector2 TextureCoord { get; } // Location 1 | aTexCoord

    /// <summary>
    /// Gets the color of the current vertex.
    /// </summary>
    public Color TintColor { get; } // Location 2 | aTintColor

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if both <see cref="TextureVertexData"/> objects are not equal.</returns>
    public static bool operator !=(TextureVertexData left, TextureVertexData right) => !left.Equals(right);

    /// <summary>
    /// Returns a value indicating whether the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
    /// </summary>
    /// <param name="left">The left operand.</param>
    /// <param name="right">The right operand.</param>
    /// <returns><c>true</c> if both <see cref="TextureVertexData"/> objects are equal.</returns>
    public static bool operator ==(TextureVertexData left, TextureVertexData right) => left.Equals(right);

    /// <summary>
    /// Returns the data stride for the vertex data.
    /// </summary>
    /// <returns>The size of the stride in bytes.</returns>
    public static uint GetStride() => Stride;

    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is not TextureVertexData data)
        {
            return false;
        }

        return this == data;
    }

    /// <inheritdoc/>
    public bool Equals(TextureVertexData other)
        => other.VertexPos == VertexPos &&
           other.TextureCoord == TextureCoord &&
           other.TintColor == TintColor;

    /// <summary>
    /// Returns a value indicating if this struct is empty.
    /// </summary>
    /// <returns>True if the struct is empty.</returns>
    public bool IsEmpty() => VertexPos == Vector2.Zero &&
                             TextureCoord == Vector2.Zero &&
                             TintColor == Color.Empty;

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage(Justification = "Cannot test because hash codes do not return repeatable results.")]
    public override int GetHashCode() => VertexPos.GetHashCode() + TextureCoord.GetHashCode() + TintColor.GetHashCode();
}

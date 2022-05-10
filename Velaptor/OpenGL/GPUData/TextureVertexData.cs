// <copyright file="TextureVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;

    /// <summary>
    /// Represents a single vertex of data for a quad.
    /// </summary>
    internal struct TextureVertexData : IEquatable<TextureVertexData>
    {
        /// <summary>
        /// The position of the vertex in NDC (normal device coordinate) coordinates.
        /// </summary>
        public Vector2 VertexPos; // Location 0 | aPosition

        /// <summary>
        /// The point in a texture that corresponds to this structure's <see cref="VertexPos"/>.
        /// </summary>
        public Vector2 TextureCoord; // Location 1 | aTexCoord

        /// <summary>
        /// The color of the current vertex.
        /// </summary>
        public Color TintColor; // Location 2 | aTintColor

        /// <summary>
        /// Returns a value indicating whether or not the <paramref name="left"/> operand is not equal to the <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both <see cref="TextureVertexData"/> objects are not equal.</returns>
        public static bool operator !=(TextureVertexData left, TextureVertexData right) => !left.Equals(right);

        /// <summary>
        /// Returns a value indicating whether or not the <paramref name="left"/> operand is equal to the <paramref name="right"/> operand.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><c>true</c> if both <see cref="TextureVertexData"/> objects are equal.</returns>
        public static bool operator ==(TextureVertexData left, TextureVertexData right) => left.Equals(right);

        /// <summary>
        /// Returns the total number of data elements of this struct.
        /// </summary>
        /// <returns>The total number of data elements.</returns>
        public static uint TotalElements() => 8u;

        /// <summary>
        /// Returns the data stride for the vertex data.
        /// </summary>
        /// <returns>The size of the stride in bytes.</returns>
        public static uint Stride() => TotalElements() * sizeof(float);

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
            => other.VertexPos == this.VertexPos &&
               other.TextureCoord == this.TextureCoord &&
               other.TintColor == this.TintColor;

        /// <summary>
        /// Returns a value indicating if this struct is empty.
        /// </summary>
        /// <returns>True if the struct is empty.</returns>
        public bool IsEmpty() => this.VertexPos == Vector2.Zero &&
                this.TextureCoord == Vector2.Zero &&
                this.TintColor == Color.Empty;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => this.VertexPos.GetHashCode() + this.TextureCoord.GetHashCode() + this.TintColor.GetHashCode();
    }
}

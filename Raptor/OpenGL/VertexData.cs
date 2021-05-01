// <copyright file="VertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using OpenTK.Mathematics;

    /// <summary>
    /// Represents a single vertex of data for a quad.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct VertexData : IEquatable<VertexData>
    {
        /// <summary>
        /// The position of the vertex in NDC (normal device coordinate) coordinates.
        /// </summary>
        [FieldData(sizeof(float), 3)]
        public Vector3 Vertex; // Location 0 | aPosition

        /// <summary>
        /// The point in a texture that corresponds to this structures <see cref="Vertex"/>.
        /// </summary>
        [FieldData(sizeof(float), 2)]
        public Vector2 TextureCoord; // Location 1 | aTexCoord

        /// <summary>
        /// The color of the current vertex.
        /// </summary>
        [FieldData(sizeof(float), 4)]
        public Vector4 TintColor; // Location 2 | aTintColor

        /// <summary>
        /// The index of the transform to use to apply to this vertex.
        /// </summary>
        [FieldData(sizeof(float), 1)]
        public float TransformIndex; // Location 3 | aTransformIndex

        /// <summary>
        /// Returns a value indicating if the left and right side of the not equals operator are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both <see cref="VertexData"/> objects are not equal.</returns>
        public static bool operator !=(VertexData left, VertexData right) => !left.Equals(right);

        /// <summary>
        /// Returns a value indicating if the left and right side of the not equals operator are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns><see langword="true"/> if both <see cref="VertexData"/> objects are equal.</returns>
        public static bool operator ==(VertexData left, VertexData right) => left.Equals(right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj is not VertexData data)
            {
                return false;
            }

            return this == data;
        }

        /// <inheritdoc/>
        public bool Equals(VertexData other)
            => other.Vertex == this.Vertex &&
               other.TextureCoord == this.TextureCoord &&
               other.TintColor == this.TintColor &&
               other.TransformIndex == this.TransformIndex;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => this.Vertex.GetHashCode() + this.TextureCoord.GetHashCode() + this.TintColor.GetHashCode() + this.TransformIndex.GetHashCode();
    }
}

// <copyright file="QuadData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Holds data for a single quad in the GPU vertex buffer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QuadData : IEquatable<QuadData>
    {
        /// <summary>
        /// The top left corner vertex of the quad.
        /// </summary>
        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex1;

        /// <summary>
        /// The top right corner vertex of the quad.
        /// </summary>
        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex2;

        /// <summary>
        /// The bottom right corner vertex of the quad.
        /// </summary>
        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex3;

        /// <summary>
        /// The bottom left corner vertex of the quad.
        /// </summary>
        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex4;

        /// <summary>
        /// Returns a value indicating if the left and right operands of an equalds comparison
        /// operation is equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the 2 operands are equal.</returns>
        public static bool operator ==(QuadData left, QuadData right) => left.Equals(right);

        /// <summary>
        /// Returns a value indicating if the left and right operands of a not equalds comparison
        /// operation is not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if the 2 operands are equal.</returns>
        public static bool operator !=(QuadData left, QuadData right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is QuadData data))
                return false;

            return data == this;
        }

        /// <inheritdoc/>
        public bool Equals(QuadData other)
            => this.Vertex1 == other.Vertex1 &&
               this.Vertex2 == other.Vertex2 &&
               this.Vertex3 == other.Vertex3 &&
               this.Vertex4 == other.Vertex4;

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => this.Vertex1.GetHashCode() + this.Vertex2.GetHashCode() + this.Vertex3.GetHashCode() + this.Vertex4.GetHashCode();
    }
}

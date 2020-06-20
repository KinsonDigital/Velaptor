// <copyright file="QuadData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QuadData : IEquatable<QuadData>
    {
        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex1;

        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex2;

        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex3;

        [SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex4;

        public static bool operator ==(QuadData left, QuadData right) => left.Equals(right);

        public static bool operator !=(QuadData left, QuadData right) => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (!(obj is QuadData data))
                return false;

            return data == this;
        }

        /// <inheritdoc/>
        public bool Equals(QuadData other) => other == this;

        /// <inheritdoc/>
        public override int GetHashCode() => this.Vertex1.GetHashCode() + this.Vertex2.GetHashCode() + this.Vertex3.GetHashCode() + this.Vertex4.GetHashCode();
    }
}

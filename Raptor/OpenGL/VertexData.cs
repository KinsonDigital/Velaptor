namespace Raptor.OpenGL
{
    using System;
    using System.Runtime.InteropServices;
    using OpenToolkit.Mathematics;

    [StructLayout(LayoutKind.Sequential)]
    internal struct VertexData : IEquatable<VertexData>
    {
        /// <summary>
        /// The position of the vertex in NDC (normal device coordinate) coordinates.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Needed for vertex buffer layout")]
        public Vector3 Vertex; // Location 0 | aPosition

        /// <summary>
        /// The point in a texture that corresponds to this structures <see cref="Vertex"/>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Needed for vertex buffer layout")]
        public Vector2 TextureCoord; // Location 1 | aTexCoord

        /// <summary>
        /// The color of the current vertex.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Needed for vertex buffer layout")]
        public Vector4 TintColor; // Location 2 | aTintColor

        /// <summary>
        /// The index of the transform to use to apply to this vertex.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Needed for vertex buffer layout")]
        public float TransformIndex; // Location 3 | aTransformIndex

        /// <summary>
        /// Returns a value indicating if the left and right side of the not equals operator are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both <see cref="VertexData"/> objects are not equal.</returns>
        public static bool operator !=(VertexData left, VertexData right) => !(left == right);

        /// <summary>
        /// Returns a value indicating if the left and right side of the not equals operator are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>True if both <see cref="VertexData"/> objects are equal.</returns>
        public static bool operator ==(VertexData left, VertexData right) => left.Equals(right);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>True if the specified object is equal to the current object; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is VertexData data))
                return false;

            return data == this;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>true if the current object is equal to the other parameter; otherwise, false.</returns>
        public bool Equals(VertexData other) => other == this;

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode() => this.Vertex.GetHashCode() + this.TextureCoord.GetHashCode() + this.TintColor.GetHashCode() + this.TransformIndex.GetHashCode();
    }
}

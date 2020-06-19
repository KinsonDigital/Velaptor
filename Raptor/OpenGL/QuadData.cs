namespace Raptor.OpenGL
{
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct QuadData : System.IEquatable<QuadData>
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex1;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex2;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex3;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1051:Do not declare visible instance fields", Justification = "Used for data layout for OpenGL vertex buffers.")]
        public VertexData Vertex4;

        public static bool operator ==(QuadData left, QuadData right) => left.Equals(right);

        public static bool operator !=(QuadData left, QuadData right) => !(left == right);

        public override bool Equals(object obj)
        {
            if (!(obj is QuadData data))
                return false;

            return data == this;
        }

        public bool Equals(QuadData other) => other == this;

        public override int GetHashCode() => this.Vertex1.GetHashCode() + this.Vertex2.GetHashCode() + this.Vertex3.GetHashCode() + this.Vertex4.GetHashCode();
    }
}

using OpenToolkit.Mathematics;
using System.Runtime.InteropServices;

namespace Raptor.GLHelperClasses
{
    /// <summary>
    /// The vertex buffer data to be uploaded to the GPU for OpenGL
    /// to use for rendering in the vertex shader.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct QuadBufferData
    {
        public Vector3 CornerVertice { get; set; }

        public Vector2 TextureCoords { get; set; }

        public Vector4 TintColor { get; set; }
    }
}

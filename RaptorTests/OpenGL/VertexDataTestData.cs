using OpenToolkit.Mathematics;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace RaptorTests.OpenGL
{
    /// <summary>
    /// Provides test data for testing the VertexData struct.
    /// </summary>
    public class VertexDataTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            //                                  Vertex          Texture Coord           Tint Color          Texture Index   Expected Result
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9),       10,             true};
            yield return new object[] { new Vector3(11, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9),      10,             false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(44, 5), new Vector4(6, 7, 8, 9),      10,             false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(66, 7, 8, 9),      10,             false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9),       100,            false };
        }
        
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

// <copyright file="VertexDataTestData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.OpenGL
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Mathematics;
    using Raptor.OpenGL;

    /// <summary>
    /// Initializes a new instance of <see cref="VertexDataTestData"/>.
    /// </summary>
    /// <remarks>This is used to provide test data for some of the tests for the <see cref="VertexDataAnalyzer"/> class.</remarks>
    public class VertexDataTestData : IEnumerable<object[]>
    {
        /// <inheritdoc/>
        public IEnumerator<object[]> GetEnumerator()
        {
            // Vertex          Texture Coord           Tint Color          Texture Index   Expected Result
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9), 10, true };
            yield return new object[] { new Vector3(11, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9), 10, false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(44, 5), new Vector4(6, 7, 8, 9), 10, false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(66, 7, 8, 9), 10, false };
            yield return new object[] { new Vector3(1, 2, 3), new Vector2(4, 5), new Vector4(6, 7, 8, 9), 100, false };
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

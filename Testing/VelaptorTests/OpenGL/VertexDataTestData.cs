// <copyright file="VertexDataTestData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Numerics;

    /// <summary>
    /// Initializes a new instance of <see cref="VertexDataTestData"/>.
    /// </summary>
    public class VertexDataTestData : IEnumerable<object[]>
    {
        /// <inheritdoc/>
        public IEnumerator<object[]> GetEnumerator()
        {
            //                                Vertex           Texture Coord          Tint Color                  Expected Result
            yield return new object[] { new Vector2(1, 2),   new Vector2(4, 5),  Color.FromArgb(6, 7, 8, 9),         true };
            yield return new object[] { new Vector2(11, 2),  new Vector2(4, 5),  Color.FromArgb(6, 7, 8, 9),         false };
            yield return new object[] { new Vector2(1, 2),   new Vector2(44, 5), Color.FromArgb(6, 7, 8, 9),         false };
            yield return new object[] { new Vector2(1, 2),   new Vector2(4, 5),  Color.FromArgb(66, 7, 8, 9),        false };
        }

        /// <inheritdoc/>
        [ExcludeFromCodeCoverage]
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

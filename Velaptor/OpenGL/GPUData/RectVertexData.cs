// <copyright file="RectVertexData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.GPUData
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Numerics;

    /// <summary>
    /// Represents the data for a simple GPU vertex for a rectangle.
    /// </summary>
    internal readonly struct RectVertexData
    {
        private const uint TotalElements = 16u;
        private static readonly uint Stride;

        /// <summary>
        /// Initializes static members of the <see cref="RectVertexData"/> struct.
        /// </summary>
        static RectVertexData() => Stride = TotalElements * sizeof(float);

        /// <summary>
        /// Initializes a new instance of the <see cref="RectVertexData"/> struct.
        /// </summary>
        /// <param name="vertexPos">The position of this single vertex.</param>
        /// <param name="rectangle">The rectangle components.</param>
        /// <param name="color">The fill or border color.</param>
        /// <param name="isFilled">True if the rectangle is filled.</param>
        /// <param name="borderThickness">The thickness of the border.</param>
        /// <param name="topLeftCornerRadius">The top left corner radius.</param>
        /// <param name="bottomLeftCornerRadius">The bottom left corner radius.</param>
        /// <param name="bottomRightCornerRadius">The bottom right corner radius.</param>
        /// <param name="topRightCornerRadius">The top right corner radius.</param>
        public RectVertexData(
            Vector2 vertexPos,
            Vector4 rectangle,
            Color color,
            bool isFilled,
            float borderThickness,
            float topLeftCornerRadius,
            float bottomLeftCornerRadius,
            float bottomRightCornerRadius,
            float topRightCornerRadius)
        {
            VertexPos = vertexPos;
            Rectangle = rectangle;
            Color = color;
            IsFilled = isFilled;
            BorderThickness = borderThickness;
            TopLeftCornerRadius = topLeftCornerRadius;
            BottomLeftCornerRadius = bottomLeftCornerRadius;
            BottomRightCornerRadius = bottomRightCornerRadius;
            TopRightCornerRadius = topRightCornerRadius;
        }

        /// <summary>
        /// Gets the position of a single rectangle vertex.
        /// </summary>
        public Vector2 VertexPos { get; }

        /// <summary>
        /// Gets the components that make up the rectangle.
        /// </summary>
        /// <remarks>
        /// The components below represent the rectangle:
        /// <list type="bullet">
        ///     <item><c>X:</c> The position of the center of the rectangle on the X axis.</item>
        ///     <item><c>Y:</c> The position of the center of the rectangle on the Y axis.</item>
        ///     <item><c>Z:</c> The width of the rectangle.</item>
        ///     <item><c>W:</c> The height of the rectangle.</item>
        /// </list>
        /// </remarks>
        public Vector4 Rectangle { get; }

        /// <summary>
        /// Gets the color of the rectangle.
        /// </summary>
        /// <remarks>
        ///     This is the solid color if the entire rectangle <see cref="IsFilled"/> is set to <c>true</c>
        ///     and is the solid color of the rectangle border if <see cref="IsFilled"/> is set to <c>false</c>.
        /// </remarks>
        public Color Color { get; }

        /// <summary>
        /// Gets a value indicating whether if <c>true</c>, then the rectangle will be rendered as a filled rectangle and an empty rectangle if <c>false</c>.
        /// </summary>
        public bool IsFilled { get; }

        /// <summary>
        /// Gets the thickness of the rectangle border if the <see cref="IsFilled"/> is set to <c>false</c>.
        /// </summary>
        public float BorderThickness { get; }

        /// <summary>
        /// Gets the radius of the top left corner of the rectangle.
        /// </summary>
        public float TopLeftCornerRadius { get; }

        /// <summary>
        /// Gets the radius of the bottom left corner of the rectangle.
        /// </summary>
        public float BottomLeftCornerRadius { get; }

        /// <summary>
        /// Gets the radius of the bottom right corner of the rectangle.
        /// </summary>
        public float BottomRightCornerRadius { get; }

        /// <summary>
        /// Gets the radius of the top right corner of the rectangle.
        /// </summary>
        public float TopRightCornerRadius { get; }

        /// <summary>
        /// Returns an empty <see cref="RectVertexData"/> instance.
        /// </summary>
        /// <returns>The empty instance.</returns>
        public static RectVertexData Empty() =>
            new (
                Vector2.Zero,
                Vector4.Zero,
                Color.Empty,
                false,
                0f,
                0f,
                0f,
                0f,
                0f);

        /// <summary>
        /// Gets the stride of the entire vertex data chunk in bytes.
        /// </summary>
        /// <returns>The total bytes of the stride of the vertex.</returns>
        public static uint GetTotalBytes() => Stride;

        /// <summary>
        /// Returns all of the vertex data as an array or ordered values.
        /// </summary>
        /// <returns>All of the vertex data values.</returns>
        public IEnumerable<float> ToArray()
        {
            /* NOTE:
                The order of the array elements are extremely important.
                They determine the layout of each stride of vertex data and the layout
                here has to match the layout told to OpenGL using the VertexAttribLocation() calls
            */

            var result = new List<float>();

            result.AddRange(VertexPos.ToArray());
            result.AddRange(Rectangle.ToArray());
            result.AddRange(Color.ToArray());

            result.Add(IsFilled ? 1f : 0f);
            result.Add(BorderThickness);
            result.Add(TopLeftCornerRadius);
            result.Add(BottomLeftCornerRadius);
            result.Add(BottomRightCornerRadius);
            result.Add(TopRightCornerRadius);

            return result.ToArray();
        }
    }
}

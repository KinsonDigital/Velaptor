// <copyright file="AtlasRegionRectangle.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Represents a rectangular region of a texture atlas.
    /// </summary>
    public class AtlasRegionRectangle : IAtlasRegionRectangle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasRegionRectangle"/> class.
        /// </summary>
        /// <param name="name">The name of the rectangle.</param>
        /// <param name="x">The X position of the rectangular region..</param>
        /// <param name="y">The Y position of the rectangular region.</param>
        /// <param name="width">The width of the rectangular region.</param>
        /// <param name="height">The height of the rectangular region.</param>
        public AtlasRegionRectangle(string name, int x, int y, int width, int height)
        {
            Name = name;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public int X { get; set; }

        /// <inheritdoc/>
        public int Y { get; set; }

        /// <inheritdoc/>
        public int Width { get; set; }

        /// <inheritdoc/>
        public int Height { get; set; }
    }
}

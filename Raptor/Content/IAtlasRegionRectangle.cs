// <copyright file="IAtlasRegionRectangle .cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Represents a rectangular region in a a texture atlas.
    /// </summary>
    public interface IAtlasRegionRectangle : IContent
    {
        /// <summary>
        /// Gets or sets the name of the atlas rectangle region.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the width of the rectangular region.
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of the rectangular region.
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Gets or sets the X coordinate position of the rectangular region.
        /// </summary>
        int X { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate position of the rectangular region.
        /// </summary>
        int Y { get; set; }
    }
}

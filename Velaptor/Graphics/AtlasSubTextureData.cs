// <copyright file="AtlasSubTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System.Drawing;

    /// <summary>
    /// Holds data about a texture atlas sub texture.
    /// </summary>
    public class AtlasSubTextureData
    {
        /// <summary>
        /// Gets or sets the bounds of the sub texture data.
        /// </summary>
        public Rectangle Bounds { get; set; }

        /// <summary>
        /// Gets or sets the name of the sub texture.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets the index of the sub texture frame.
        /// </summary>
        /// <remarks>
        ///     This is used for frames of animation.  A negative value indicates
        ///     that the sub texture is not part of any animation frames.
        /// </remarks>
        public int FrameIndex { get; init; } = -1;
    }
}

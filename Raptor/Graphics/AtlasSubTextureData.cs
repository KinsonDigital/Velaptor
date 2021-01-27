// <copyright file="AtlasSubTextureData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;

    /// <summary>
    /// Holds dat about a texture atlas sube texture.
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
        /// Gets or sets the index of the sub texture frame.
        /// </summary>
        /// <remarks>
        ///     This is used for frames of animation.  A negative value indicates
        ///     that the sub texture is not part of any animation frames.
        /// </remarks>
        public int FrameIndex { get; set; } = -1;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///     True if the specified object is equal to the current object; otherwise, false.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is AtlasSubTextureData data))
            {
                return false;
            }

            return data.Name == Name && data.Bounds == Bounds && data.FrameIndex == FrameIndex;
        }

        /// <summary>
        /// Serves as the default hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        [ExcludeFromCodeCoverage]
        public override int GetHashCode() => HashCode.Combine(Name, Bounds);
    }
}

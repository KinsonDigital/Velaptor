// <copyright file="IGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Drawing;

    /// <summary>
    /// Represents GPU vertex buffer memory.
    /// </summary>
    /// <typeparam name="T">The type of data to send to the GPU.</typeparam>
    public interface IGPUBuffer : IDisposable
    {
        /// <summary>
        /// Gets or sets the number of quads that the buffer will deal with.
        /// </summary>
        uint TotalQuads { get; set; }

        /// <summary>
        /// Updates the given quad using the given information for a particular quad item in the GPU.
        /// </summary>
        /// <param name="quadID">The ID of the quad to update.</param>
        /// <param name="srcRect">The area within the texture to update.</param>
        /// <param name="textureWidth">The width of the texture.</param>
        /// <param name="textureHeight">The height of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture area being rendered.</param>
        void UpdateQuad(uint quadID, Rectangle srcRect, int textureWidth, int textureHeight, Color tintColor);
    }
}

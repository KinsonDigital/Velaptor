// <copyright file="IAtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.ObjectModel;
    using Raptor.Content;

    /// <summary>
    /// Holds data for a texture atlas.
    /// </summary>
    public interface IAtlasData : IContent, IDisposable
    {
        /// <summary>
        /// Gets the list of frame names.
        /// </summary>
        ReadOnlyCollection<string> SubTextureNames { get; }

        /// <summary>
        /// Gets the texture.
        /// </summary>
        ITexture Texture { get; set; }

        /// <summary>
        /// Gets the width of the entire texture atlas texture.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the entire texture atlas texture.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// The iterator for the atlas sub texture data.
        /// </summary>
        /// <param name="index">The index of the item to retrieve.</param>
        /// <returns>The sub texture data.</returns>
        AtlasSubTextureData this[int index] { get; }

        /// <summary>
        /// Gets the single frame in the atlas data that matches the given <paramref name="subTextureID"/>.
        /// </summary>
        /// <param name="subTextureID">The name of the sub texture frame.</param>
        /// <returns>The sub texture data of the frame.</returns>
        AtlasSubTextureData GetFrame(string subTextureID);

        /// <summary>
        /// Gets the all of the frames that have the given sub texture id.
        /// </summary>
        /// <param name="subTextureID">The sub texture ID of the frames to return.</param>
        /// <returns>The list of frame rectangles.</returns>
        AtlasSubTextureData[] GetFrames(string subTextureID);
    }
}

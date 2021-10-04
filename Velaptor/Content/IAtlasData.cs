// <copyright file="IAtlasData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// Holds data for a texture atlas.
    /// </summary>
    public interface IAtlasData : IContent
    {
        /// <summary>
        /// Gets the list of frame names.
        /// </summary>
        ReadOnlyCollection<string> SubTextureNames { get; }

        /// <summary>
        /// Gets the the texture of the atlas.
        /// </summary>
        ITexture Texture { get; }

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
        /// Gets the single frame in the atlas data that matches the given <paramref name="subTextureId"/>.
        /// </summary>
        /// <param name="subTextureId">The name of the sub texture frame.</param>
        /// <returns>The sub texture data of the frame.</returns>
        AtlasSubTextureData GetFrame(string subTextureId);

        /// <summary>
        /// Gets the all of the frames that have the given sub texture id.
        /// </summary>
        /// <param name="subTextureId">The sub texture ID of the frames to return.</param>
        /// <returns>The list of frame rectangles.</returns>
        AtlasSubTextureData[] GetFrames(string subTextureId);
    }
}

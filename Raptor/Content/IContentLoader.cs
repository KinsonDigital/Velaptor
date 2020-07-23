// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using Raptor.Graphics;
    using Raptor.Audio;

    /// <summary>
    /// Represents a loader that can load content for rendering or sound.
    /// </summary>
    public interface IContentLoader
    {
        /// <summary>
        /// Gets or sets the root directory for the game's content.
        /// </summary>
        string ContentRootDirectory { get; set; }

        /// <summary>
        /// Loads a texture with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns>A texture to render.</returns>
        ITexture? LoadTexture(string name);

        /// <summary>
        /// Loads atlas data with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the atlas data file to load.</param>
        /// <returns>The atlas data.</returns>
        AtlasRegionRectangle[] LoadAtlasData(string name);

        ISound LoadSound(string name);
    }
}

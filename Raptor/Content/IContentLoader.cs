// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using Raptor.Graphics;

    /// <summary>
    /// Represents a loader that can load content for rendering or sound.
    /// </summary>
    public interface IContentLoader
    {
        /// <summary>
        /// Gets the root directory for the game's content.
        /// </summary>
        string ContentRootDirectory { get; }

        Texture? LoadTexture(string name);
    }
}

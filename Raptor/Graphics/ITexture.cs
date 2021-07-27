// <copyright file="ITexture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using Velaptor.Content;

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public interface ITexture : IContent
    {
        /// <summary>
        /// Gets he ID of the texture.
        /// </summary>
        uint ID { get; }

        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        int Height { get; }
    }
}

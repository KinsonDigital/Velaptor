// <copyright file="ITexture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public interface ITexture : IContent
    {
        /// <summary>
        /// Gets he ID of the texture.
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        uint Width { get; }

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        uint Height { get; }
    }
}

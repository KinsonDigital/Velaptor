// <copyright file="ITexture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using Raptor.Content;

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public interface ITexture : IContent, IDisposable
    {
        /// <summary>
        /// Gets he ID of the texture.
        /// </summary>
        uint ID { get; }

        /// <summary>
        /// Gets the name of the texture.
        /// </summary>
        string Name { get; }

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

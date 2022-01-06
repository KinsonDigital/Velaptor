// <copyright file="ITextureFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates <see cref="ITexture"/> objects for rendering.
    /// </summary>
    internal interface ITextureFactory
    {
        /// <summary>
        /// Creates a <see cref="ITexture"/> object for use for rendering tot he screen.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="filePath">The file path to the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        /// <param name="isPooled">True to have the object pooled.</param>
        /// <returns>The texture instance.</returns>
        ITexture Create(string name, string filePath, ImageData imageData, bool isPooled);
    }
}

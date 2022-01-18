// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using Velaptor.Content.Fonts;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads various kinds of content.
    /// </summary>
    public interface IContentLoader : IDisposable
    {
        /// <summary>
        /// Loads texture content using the given <paramref name="nameOrFilePath"/>.
        /// </summary>
        /// <param name="nameOrFilePath">The name content in the application content directory or direct file path to the content.</param>
        /// <returns>The loaded texture content.</returns>
        ITexture LoadTexture(string nameOrFilePath);

        /// <summary>
        /// Loads sound content using the given <paramref name="nameOrFilePath"/>.
        /// </summary>
        /// <param name="nameOrFilePath">The name content in the application content directory or direct file path to the content.</param>
        /// <returns>The loaded sound content.</returns>
        ISound LoadSound(string nameOrFilePath);

        /// <summary>
        /// Loads the texture atlas data using the given <paramref name="nameOrFilePath"/>.
        /// </summary>
        /// <param name="nameOrFilePath">The name content in the application content directory or direct file path to the content.</param>
        /// <returns>The loaded texture atlas data.</returns>
        IAtlasData LoadAtlas(string nameOrFilePath);

        /// <summary>
        /// Loads font content using the given <paramref name="nameOrFilePath"/> and <paramref name="size"/>.
        /// </summary>
        /// <param name="nameOrFilePath">The name content in the application content directory or direct file path to the content.</param>
        /// <param name="size">The size of the font.</param>
        /// <returns>The loaded font content.</returns>
        IFont LoadFont(string nameOrFilePath, int size);

        /// <summary>
        /// Unloads the texture content.
        /// </summary>
        /// <param name="content">The content to unload.</param>
        void UnloadTexture(ITexture content);

        /// <summary>
        /// Unloads the sound content.
        /// </summary>
        /// <param name="content">The content to unload.</param>
        void UnloadSound(ISound content);

        /// <summary>
        /// Unloads the atlas data content.
        /// </summary>
        /// <param name="content">The content to unload.</param>
        void UnloadAtlas(IAtlasData content);

        /// <summary>
        /// Unloads the font.
        /// </summary>
        /// <param name="content">The content to unload.</param>
        void UnloadFont(IFont content);
    }
}

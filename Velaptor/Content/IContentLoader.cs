// <copyright file="IContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;

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
        /// Unloads content with the given <paramref name="nameOrFilePath"/>.
        /// </summary>
        /// <typeparam name="T">The type of content to unload.</typeparam>
        /// <param name="nameOrFilePath">The name content in the application content directory or direct file path to the content.</param>
        void Unload<T>(string nameOrFilePath)
            where T : class, IContent;
    }
}

// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Content.Exceptions;
    using Velaptor.Factories;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads textures.
    /// </summary>
    public sealed class TextureLoader : ILoader<ITexture>
    {
        private const string TextureFileExtension = ".png";
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly IPathResolver pathResolver;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public TextureLoader()
        {
            this.textureCache = IoC.Container.GetInstance<IDisposableItemCache<string, ITexture>>();
            this.pathResolver = PathResolverFactory.CreateTexturePathResolver();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal TextureLoader(
            IDisposableItemCache<string, ITexture> textureCache,
            IPathResolver texturePathResolver,
            IPath path)
        {
            this.textureCache = textureCache;
            this.pathResolver = texturePathResolver;
            this.path = path;
        }

        /// <summary>
        /// Loads a texture with the given <paramref name="contentPathOrName"/>.
        /// </summary>
        /// <param name="contentPathOrName">The name of the texture to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="LoadContentException">Thrown if the resulting content file path is invalid.</exception>
        /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
        /// <exception cref="LoadFontException">Thrown if the resulting font content file path is invalid.</exception>
        public ITexture Load(string contentPathOrName)
        {
            var isFullFilePath = contentPathOrName.IsValidFilePath();
            string filePath;

            if (isFullFilePath)
            {
                filePath = contentPathOrName;
            }
            else
            {
                contentPathOrName = this.path.GetFileNameWithoutExtension(contentPathOrName);

                filePath = this.pathResolver.ResolveFilePath(contentPathOrName);
            }

            if (filePath.IsValidFilePath() is false && this.path.GetExtension(filePath) == TextureFileExtension)
            {
                throw new LoadTextureException($"The texture file path '{filePath}' is not a valid path.");
            }

            return this.textureCache.GetItem(filePath);
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string name) => this.textureCache.Unload(name);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureCache.Dispose();
            }

            this.isDisposed = true;
        }
    }
}

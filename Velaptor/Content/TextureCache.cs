// <copyright file="TextureCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Caches <see cref="ITexture"/> objects for performant retrieval at a later time.
    /// </summary>
    internal sealed class TextureCache : IDisposableItemCache<string, ITexture>
    {
        private readonly ConcurrentDictionary<string, ITexture> textures = new ();
        private readonly IImageService imageService;
        private readonly ITextureFactory textureFactory;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureCache"/> class.
        /// </summary>
        /// <param name="imageService">Provides image related services.</param>
        /// <param name="textureFactory">Creates <see cref="ITexture"/> objects.</param>
        /// <param name="path">Provides path related services.</param>
        public TextureCache(
            IImageService imageService,
            ITextureFactory textureFactory,
            IPath path)
        {
            this.imageService = imageService;
            this.textureFactory = textureFactory;
            this.path = path;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TextureCache"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~TextureCache() => Dispose(false);

        /// <inheritdoc/>
        public int TotalCachedItems => this.textures.Count;

        /// <inheritdoc/>
        public ITexture GetItem(string filePath)
        {
            if (filePath.IsValidFilePath() is false)
            {
                throw new LoadTextureException($"The texture file path '{filePath}' is not a valid path.");
            }

            // If the requested texture is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var texture in this.textures)
            {
                if (texture.Key != filePath || !texture.Value.IsDisposed)
                {
                    continue;
                }

                this.textures.TryRemove(texture);
                break;
            }

            return this.textures.GetOrAdd(filePath, textureFilePath =>
            {
                var imageData = this.imageService.Load(textureFilePath);

                var name = this.path.GetFileNameWithoutExtension(textureFilePath);

                return this.textureFactory.Create(name, textureFilePath, imageData, true);
            });
        }

        /// <inheritdoc/>
        public void Unload(string cacheKey)
        {
            this.textures.TryRemove(cacheKey, out var texture);

            if (texture is null)
            {
                return;
            }

            texture.IsPooled = false;
            texture.Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var texture in this.textures.Values)
                {
                    texture.IsPooled = false;
                    texture.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}

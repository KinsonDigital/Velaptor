// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Factories;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads textures.
    /// </summary>
    public sealed class TextureLoader : ILoader<ITexture>
    {
        private const string TextureFileExtension = ".png";
        private readonly IItemCache<string, ITexture> textureCache;
        private readonly IPathResolver texturePathResolver;
        private readonly IFile file;
        private readonly IPath path;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public TextureLoader()
        {
            this.textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
            this.texturePathResolver = PathResolverFactory.CreateTexturePathResolver();
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        internal TextureLoader(
            IItemCache<string, ITexture> textureCache,
            IPathResolver texturePathResolver,
            IFile file,
            IPath path)
        {
            this.textureCache = textureCache ?? throw new ArgumentNullException(nameof(textureCache), "The parameter must not be null.");
            this.texturePathResolver = texturePathResolver ?? throw new ArgumentNullException(nameof(texturePathResolver), "The parameter must not be null.");
            this.file = file ?? throw new ArgumentNullException(nameof(file), "The parameter must not be null.");
            this.path = path ?? throw new ArgumentNullException(nameof(path), "The parameter must not be null.");
        }

        /// <summary>
        /// Loads a texture with the given <paramref name="contentPathOrName"/>.
        /// </summary>
        /// <param name="contentPathOrName">The full file path or name of the texture to load.</param>
        /// <returns>The loaded texture.</returns>
        /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
        public ITexture Load(string contentPathOrName)
        {
            var isFullFilePath = contentPathOrName.HasValidFullFilePathSyntax();
            string filePath;
            string cacheKey;

            if (isFullFilePath)
            {
                filePath = contentPathOrName;
                cacheKey = filePath;
            }
            else
            {
                contentPathOrName = this.path.GetFileNameWithoutExtension(contentPathOrName);
                filePath = this.texturePathResolver.ResolveFilePath(contentPathOrName);
                cacheKey = filePath;
            }

            if (this.file.Exists(filePath))
            {
                if (this.path.GetExtension(filePath) is not TextureFileExtension)
                {
                    throw new LoadTextureException(
                        $"The file '{filePath}' must be a texture file with the extension '{TextureFileExtension}'.");
                }
            }
            else
            {
                throw new FileNotFoundException($"The texture file '{filePath}' does not exist.", filePath);
            }

            return this.textureCache.GetItem(cacheKey);
        }

        /// <inheritdoc/>
        public void Unload(string contentNameOrPath) => this.textureCache.Unload(contentNameOrPath);
    }
}

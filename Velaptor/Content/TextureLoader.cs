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
    using Velaptor.Observables;
    using Velaptor.Observables.Core;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads textures.
    /// </summary>
    public sealed class TextureLoader : ILoader<ITexture>
    {
        private const string TextureFileExtension = ".png";
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly IPathResolver pathResolver;
        private readonly IFile file;
        private readonly IPath path;
        private readonly IDisposable shutDownObservableUnsubscriber;
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
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();

            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();
            this.shutDownObservableUnsubscriber = shutDownObservable.Subscribe(new Observer<bool>(_ => ShutDown()));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        /// <param name="shutDownObservable">Sends out a notification that the application is shutting down.</param>
        internal TextureLoader(
            IDisposableItemCache<string, ITexture> textureCache,
            IPathResolver texturePathResolver,
            IFile file,
            IPath path,
            IObservable<bool> shutDownObservable)
        {
            this.textureCache = textureCache;
            this.pathResolver = texturePathResolver;
            this.file = file;
            this.path = path;

            this.shutDownObservableUnsubscriber = shutDownObservable.Subscribe(new Observer<bool>(_ => ShutDown()));
        }

        /// <summary>
        /// Loads a texture with the given <paramref name="contentPathOrName"/>.
        /// </summary>
        /// <param name="contentPathOrName">The name of the texture to load.</param>
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
                filePath = this.pathResolver.ResolveFilePath(contentPathOrName);
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
        public void Unload(string nameOrPath) => this.textureCache.Unload(nameOrPath);

        /// <summary>
        /// Shuts down the application by unloading all of the textures.
        /// </summary>
        private void ShutDown()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.textureCache.Dispose();
            this.shutDownObservableUnsubscriber.Dispose();

            this.isDisposed = true;
        }
    }
}

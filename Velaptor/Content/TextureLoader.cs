// <copyright file="TextureLoader.cs" company="KinsonDigital">
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
    using Velaptor.Factories;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads textures.
    /// </summary>
    public sealed class TextureLoader : ILoader<ITexture>
    {
        private const string TextureFileExtension = ".png";
        private const string FontFileExtension = ".ttf";
        private readonly ConcurrentDictionary<string, ITexture> textures = new ();
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IImageService imageService;
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
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.glExtensions = IoC.Container.GetInstance<IGLInvokerExtensions>();
            this.imageService = IoC.Container.GetInstance<ImageService>();
            this.pathResolver = PathResolverFactory.CreateTexturePathResolver();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="imageService">Loads an image file.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal TextureLoader(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IImageService imageService,
            IPathResolver texturePathResolver,
            IPath path)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.imageService = imageService;
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
            var isFullFilePath = contentPathOrName.IsValidFullFilePath();
            var name = string.Empty;
            string filePath;

            if (isFullFilePath)
            {
                name = this.path.GetFileNameWithoutExtension(contentPathOrName);
                filePath = contentPathOrName;
            }
            else
            {
                contentPathOrName = this.path.GetFileNameWithoutExtension(contentPathOrName);

                filePath = this.pathResolver.ResolveFilePath(contentPathOrName);
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

            // If the file path is valid
            if (filePath.IsValidFullFilePath() is false && this.path.HasExtension(filePath) is false)
            {
                throw new LoadContentException($"The content file path '{filePath}' is not a valid path.");
            }

            if (filePath.IsValidFullFilePath() is false && this.path.GetExtension(filePath) == TextureFileExtension)
            {
                throw new LoadTextureException($"The texture file path '{filePath}' is not a valid path.");
            }

            if (filePath.IsValidFullFilePath() is false && this.path.GetExtension(filePath) == FontFileExtension)
            {
                throw new LoadFontException($"The font file path '{filePath}' is not a valid path.");
            }

            return this.textures.GetOrAdd(filePath, (filePathToLoad) =>
            {
                var imageData = this.imageService.Load(filePathToLoad);

                return new Texture(this.gl, this.glExtensions, name, filePathToLoad, imageData) { IsPooled = true };
            });
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string name)
        {
            var filePath = this.pathResolver.ResolveFilePath(name);

            if (this.textures.TryRemove(filePath, out var texture))
            {
                texture.IsPooled = false;
                texture.Dispose();
            }
        }

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
                foreach (var texture in this.textures.Values)
                {
                    texture.IsPooled = false;
                    texture.Dispose();
                }

                this.textures.Clear();
            }

            this.isDisposed = true;
        }
    }
}

// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Graphics;
    using Raptor.NativeInterop;
    using Raptor.Services;

    /// <summary>
    /// Loads textures.
    /// </summary>
    public class TextureLoader : ILoader<ITexture>
    {
        private readonly ConcurrentDictionary<string, ITexture> textures = new ();
        private readonly IGLInvoker gl;
        private readonly IImageService imageService;
        private readonly IPathResolver pathResolver;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageService">Loads an image file.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        [ExcludeFromCodeCoverage]
        public TextureLoader(IImageService imageService, IPathResolver texturePathResolver)
        {
            this.gl = new GLInvoker();
            this.imageService = imageService;
            this.pathResolver = texturePathResolver;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="imageService">Loads an image file.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        internal TextureLoader(IGLInvoker gl, IImageService imageService, IPathResolver texturePathResolver)
        {
            this.gl = gl;
            this.imageService = imageService;
            this.pathResolver = texturePathResolver;
        }

        /// <summary>
        /// Loads a texture with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns>The loaded texture.</returns>
        public ITexture Load(string name)
        {
            var filePath = this.pathResolver.ResolveFilePath(name);

            return this.textures.GetOrAdd(filePath, (path) =>
            {
                var imageData = this.imageService.Load(path);

                return new Texture(this.gl, name, path, imageData);
            });
        }

        /// <inheritdoc/>
        public void Unload(string name)
        {
            var filePath = this.pathResolver.ResolveFilePath(name);

            if (this.textures.TryRemove(filePath, out var texture))
            {
                texture.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var texture in this.textures.Values)
                {
                    texture.Dispose();
                }

                this.textures.Clear();
            }

            this.isDisposed = true;
        }
    }
}

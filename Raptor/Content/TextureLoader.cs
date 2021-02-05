// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using Raptor.Services;

    /// <summary>
    /// Loads textures.
    /// </summary>
    public class TextureLoader : ILoader<ITexture>
    {
        private readonly IGLInvoker gl;
        private readonly IImageFileService imageFileService;
        private readonly IPathResolver pathResolver;
        private readonly ConcurrentDictionary<string, ITexture> textures = new ConcurrentDictionary<string, ITexture>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageFileService">Loads an image file.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        [ExcludeFromCodeCoverage]
        public TextureLoader(IImageFileService imageFileService, IPathResolver texturePathResolver)
        {
            this.gl = new GLInvoker();
            this.imageFileService = imageFileService;
            this.pathResolver = texturePathResolver;
        }

        // TODO: Check if this is needed or being used, and if not, remove it
        // The IoC container might use it
        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="imageFileService">Loads an image file.</param>
        /// <param name="texturePathResolver">Resolves paths to texture content.</param>
        internal TextureLoader(IGLInvoker gl, IImageFileService imageFileService, IPathResolver texturePathResolver)
        {
            this.gl = gl;
            this.imageFileService = imageFileService;
            this.pathResolver = texturePathResolver;
        }

        /// <inheritdoc/>
        public ITexture Load(string name)
        {
            var filePath = this.pathResolver.ResolveFilePath(name);

            return this.textures.GetOrAdd(filePath, (key) =>
            {
                var (pixels, width, height) = this.imageFileService.Load(key);

                return new Texture(this.gl, name, filePath, pixels, width, height);
            });
        }
    }
}

// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
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
        private readonly IContentSource contentSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageFileService">Loads an image file.</param>
        /// <param name="contentSource">Provides access to the content source.</param>
        [ExcludeFromCodeCoverage]
        public TextureLoader(IImageFileService imageFileService, IContentSource contentSource)
        {
            this.gl = new GLInvoker();
            this.imageFileService = imageFileService;
            this.contentSource = contentSource;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="imageFileService">Loads an image file.</param>
        /// <param name="contentSource">Provides access to the content source.</param>
        internal TextureLoader(IGLInvoker gl, IImageFileService imageFileService, IContentSource contentSource)
        {
            this.gl = gl;
            this.imageFileService = imageFileService;
            this.contentSource = contentSource;
        }

        /// <inheritdoc/>
        public ITexture Load(string name)
        {
            var filePath = this.contentSource.GetContentPath(name);
            var (pixels, width, height) = this.imageFileService.Load(filePath);

            return new Texture(this.gl, name, pixels, width, height);
        }
    }
}

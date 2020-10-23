// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Raptor.Graphics;
    using Raptor.OpenGL;
    using Raptor.Services;

    /// <summary>
    /// Loads textures.
    /// </summary>
    public class TextureLoader : ILoader<ITexture>
    {
        private readonly IGLInvoker gl;
        private readonly IImageFileService imageFile;
        private readonly IContentSource contentSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageFile">Loads an image file.</param>
        [ExcludeFromCodeCoverage]
        public TextureLoader(IImageFileService imageFile)
        {
            this.gl = new GLInvoker();
            this.imageFile = imageFile;
            this.contentSource = new ContentSource(IoC.Container.GetInstance<IDirectory>());
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="imageFile">Loads an image file.</param>
        /// <param name="contentSource">Provides access to the content source.</param>
        internal TextureLoader(IGLInvoker gl, IImageFileService imageFile, IContentSource contentSource)
        {
            this.gl = gl;
            this.imageFile = imageFile;
            this.contentSource = contentSource;
        }

        /// <inheritdoc/>
        public ITexture Load(string name)
        {
            var filePath = this.contentSource.GetContentPath(ContentType.Graphics, name);
            var (pixels, width, height) = this.imageFile.Load(filePath);

            return new Texture(this.gl, name, pixels, width, height);
        }
    }
}

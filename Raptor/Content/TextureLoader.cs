// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using FileIO.Core;
    using Raptor.Graphics;
    using Raptor.OpenGL;

    /// <summary>
    /// Loads textures.
    /// </summary>
    public class TextureLoader : ILoader<ITexture>
    {
        private readonly IGLInvoker gl;
        private readonly IImageFile imageFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="imageFile">Loads an image file.</param>
        [ExcludeFromCodeCoverage]
        public TextureLoader(IImageFile imageFile)
        {
            this.gl = new GLInvoker();
            this.imageFile = imageFile;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureLoader"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="imageFile">Loads an image file.</param>
        internal TextureLoader(IGLInvoker gl, IImageFile imageFile)
        {
            this.gl = gl;
            this.imageFile = imageFile;
        }

        /// <inheritdoc/>
        public ITexture Load(string filePath)
        {
            var (pixels, width, height) = this.imageFile.Load(filePath);

            return new Texture(this.gl, Path.GetFileNameWithoutExtension(filePath), pixels, width, height);
        }
    }
}

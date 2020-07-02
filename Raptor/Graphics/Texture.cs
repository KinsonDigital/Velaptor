// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using OpenToolkit.Graphics.OpenGL4;
    using Raptor.OpenGL;

    /// <summary>
    /// The texture to render to the screen.
    /// </summary>
    public class Texture : ITexture
    {
        private readonly IGLInvoker gl;
        private bool isDisposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="pixelData">The pixel data of the texture.</param>
        /// <param name="name">The name of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        [ExcludeFromCodeCoverage]
        public Texture(string name, byte[] pixelData, int width, int height)
        {
            this.gl = new GLInvoker();
            Init(name, pixelData, width, height);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="name">The name of the texture.</param>
        /// <param name="pixelData">The pixel data of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        internal Texture(IGLInvoker gl, string name, byte[] pixelData, int width, int height)
        {
            this.gl = gl;
            Init(name, pixelData, width, height);
        }

        /// <inheritdoc/>
        public uint ID { get; protected set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public int Width { get; protected set; }

        /// <inheritdoc/>
        public int Height { get; protected set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
                return;

            this.gl.DeleteTexture(ID);

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="pixelData">The pixel data of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        private void Init(string name, byte[] pixelData, int width, int height)
        {
            ID = this.gl.GenTexture();

            this.gl.BindTexture(TextureTarget.Texture2D, ID);

            Width = width;
            Height = height;

            Name = Path.GetFileNameWithoutExtension(name);

            UploadDataToGPU(name, pixelData, width, height);

            // Unbind
            this.gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Uploads the given pixel data to the GPU.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="pixelData">The pixel data of the texture.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        private void UploadDataToGPU(string name, byte[] pixelData, int width, int height)
        {
            this.gl.ObjectLabel(ObjectLabelIdentifier.Texture, ID, -1, name);

            // Set the min and mag filters to linear
            this.gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            this.gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Sett the x(S) and y(T) axis wrap mode to repeat
            this.gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            this.gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Load the texture data to the GPU for the currently active texture slot
            this.gl.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        }
    }
}

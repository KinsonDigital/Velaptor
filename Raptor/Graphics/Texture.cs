// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using OpenTK.Graphics.OpenGL4;
    using Raptor.OpenGL;

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public class Texture : ITexture
    {
        private readonly IGLInvoker gl;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="path">The path to the image file.</param>
        /// <param name="imageData">The image data of the texture.</param>
        [ExcludeFromCodeCoverage]
        public Texture(string name, string path, ImageData imageData)
        {
            this.gl = new GLInvoker();
            Path = path;
            Init(name, imageData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="name">The name of the texture.</param>
        /// <param name="path">The path to the image file.</param>
        /// <param name="imageData">The image data of the texture.</param>
        internal Texture(IGLInvoker gl, string name, string path, ImageData imageData)
        {
            this.gl = gl;
            Path = path;
            Init(name, imageData);
        }

        /// <inheritdoc/>
        public uint ID { get; protected set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public string Path { get; set; }

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
            {
                return;
            }

            this.gl.DeleteTexture(ID);

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        private void Init(string name, ImageData imageData)
        {
            ID = this.gl.GenTexture();

            this.gl.BindTexture(TextureTarget.Texture2D, ID);

            Width = imageData.Width;
            Height = imageData.Height;

            Name = name;

            UploadDataToGPU(name, imageData);

            // Unbind
            this.gl.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <summary>
        /// Uploads the given pixel data to the GPU.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        private void UploadDataToGPU(string name, ImageData imageData)
        {
            /*NOTE:
             * The incoming image data is in the ARGB byte layout.
             *
             * The data layout required by OpenGL is RGBA.
             */
            var pixelData = new List<byte>();

            for (var y = 0; y < imageData.Height; y++)
            {
                var rowBytes = new List<byte>();

                for (var x = 0; x < imageData.Width; x++)
                {
                    rowBytes.Add(imageData.Pixels[x, y].R);
                    rowBytes.Add(imageData.Pixels[x, y].G);
                    rowBytes.Add(imageData.Pixels[x, y].B);
                    rowBytes.Add(imageData.Pixels[x, y].A);
                }

                pixelData.AddRange(rowBytes);
                rowBytes.Clear();
            }

            this.gl.ObjectLabel(ObjectLabelIdentifier.Texture, ID, -1, name);

            // Set the min and mag filters to linear
            this.gl.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMinFilter,
                param: (int)TextureMinFilter.Linear);

            this.gl.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureMagFilter,
                param: (int)TextureMagFilter.Linear);

            // Sett the x(S) and y(T) axis wrap mode to repeat
            this.gl.TexParameter(
                target: TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapS,
                param: (int)TextureWrapMode.ClampToEdge);

            this.gl.TexParameter(
                target:TextureTarget.Texture2D,
                pname: TextureParameterName.TextureWrapT,
                param: (int)TextureWrapMode.ClampToEdge);

            // Load the texture data to the GPU for the currently active texture slot
            this.gl.TexImage2D(
                target: TextureTarget.Texture2D,
                level: 0,
                PixelInternalFormat.Rgba,
                width: imageData.Width,
                height: imageData.Height,
                border: 0,
                format: PixelFormat.Rgba,
                type: PixelType.UnsignedByte,
                pixels: pixelData.ToArray());
        }
    }
}

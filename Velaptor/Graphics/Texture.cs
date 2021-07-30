// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public class Texture : ITexture
    {
        private readonly IGLInvoker gl;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="path">The path to the image file.</param>
        /// <param name="imageData">The image data of the texture.</param>
        [ExcludeFromCodeCoverage]
        public Texture(string name, string path, ImageData imageData)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            Path = path;
            Init(name, imageData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
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

        /// <summary>
        /// Finalizes an instance of the <see cref="Texture"/> class.
        /// </summary>
        ~Texture()
        {
            Dispose(false);
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

        /// <inheritdoc/>
        public bool Unloaded { get; private set; }

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
        /// <param name="disposing"><see langword="true"/> if managed resources should be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Unloaded)
            {
                return;
            }

            this.gl.DeleteTexture(ID);

            Unloaded = true;
        }

        /// <summary>
        /// Initializes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        private void Init(string name, ImageData imageData)
        {
            ID = this.gl.GenTexture();

            this.gl.BindTexture(GLTextureTarget.Texture2D, ID);

            // TODO: Make the Texture.Width and Texture.Height uint data type
            Width = (int)imageData.Width;
            Height = (int)imageData.Height;

            Name = name;

            UploadDataToGPU(name, imageData);

            // Unbind
            this.gl.BindTexture(GLTextureTarget.Texture2D, 0);
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

            this.gl.ObjectLabel(GLObjectIdentifier.Texture, ID, 1u, name);

            // Set the min and mag filters to linear
            this.gl.TexParameter(
                target: GLTextureTarget.Texture2D,
                pname: GLTextureParameterName.TextureMinFilter,
                param: GLTextureMinFilter.Linear);

            this.gl.TexParameter(
                target: GLTextureTarget.Texture2D,
                pname: GLTextureParameterName.TextureMagFilter,
                param: GLTextureMagFilter.Linear);

            // Set the x(S) and y(T) axis wrap mode to repeat
            this.gl.TexParameter(
                target: GLTextureTarget.Texture2D,
                pname: GLTextureParameterName.TextureWrapS,
                param: GLTextureWrapMode.ClampToEdge);

            this.gl.TexParameter(
                target: GLTextureTarget.Texture2D,
                pname: GLTextureParameterName.TextureWrapT,
                param: GLTextureWrapMode.ClampToEdge);

            // Load the texture data to the GPU for the currently active texture slot
            this.gl.TexImage2D<byte>(
                target: GLTextureTarget.Texture2D,
                level: 0,
                GLInternalFormat.Rgba,
                width: imageData.Width,
                height: imageData.Height,
                border: 0,
                format: GLPixelFormat.Rgba,
                type: GLPixelType.UnsignedByte,
                pixels: pixelData.ToArray());
        }
    }
}

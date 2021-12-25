// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public sealed class Texture : ITexture
    {
        private readonly IGLInvoker gl;

        /*NOTE:
         * This flag is for the purpose of unit testing.  This flag gets set in one location and that is
         * the finalizer.  This then gets checked to see if it is false in the Dispose() method.
         *
         * If the finalizer is false and the Texture is pooled, it will then allow the PooledDisposalException
         * to be thrown.
         *
         * An issue arises when running enough unit tests together with this class being included in those tests
         * When a lot of tests are ran, the finalizers is being invoked which then in turn invokes the Dispose()
         * method. This in turn then throws the exception which is not expected in the unit tests which fails the
         * unit test.
         *
         * This happens when the pooling status of an object is true, and the object was disposed of in the test,
         * but the finalizer gets invoked before the GC.SuppressFinalize(this) is invoked.
         */
        private bool invokedFromFinalizer;

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
        [ExcludeFromCodeCoverage]
        ~Texture()
        {
            this.invokedFromFinalizer = true;
            Dispose();
        }

        /// <inheritdoc/>
        public uint Id { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public string Path { get; }

        /// <inheritdoc/>
        public uint Width { get; private set; }

        /// <inheritdoc/>
        public uint Height { get; private set; }

        /// <inheritdoc/>
        public bool IsDisposed { get; private set; }

        /// <inheritdoc/>
        public bool IsPooled { get; set; }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            // TODO: Make sure once you improve this control, that the lower level IContent items
            // are checked to see if they are not being pooled before disposing them
            if (IsDisposed)
            {
                return;
            }

            if (IsPooled && this.invokedFromFinalizer is false)
            {
                throw new PooledDisposalException();
            }

            this.gl.DeleteTexture(Id);

            IsDisposed = true;

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        private void Init(string name, ImageData imageData)
        {
            Id = this.gl.GenTexture();

            this.gl.BindTexture(GLTextureTarget.Texture2D, Id);

            Width = imageData.Width;
            Height = imageData.Height;

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

            this.gl.LabelTexture(Id, name);

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

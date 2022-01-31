// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// The texture to render to a screen.
    /// </summary>
    public sealed class Texture : ITexture
    {
        private readonly IGLInvoker gl;
        private readonly IOpenGLService openGLService;
        private readonly IDisposable disposeUnsubscriber;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="filePath">The file path to the image file.</param>
        /// <param name="imageData">The image data of the texture.</param>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public Texture(string name, string filePath, ImageData imageData)
        {
            this.gl = IoC.Container.GetInstance<IGLInvoker>();
            this.openGLService = IoC.Container.GetInstance<IOpenGLService>();

            var disposeReactor = IoC.Container.GetInstance<IReactable<DisposeTextureData>>();

            this.disposeUnsubscriber =
                disposeReactor.Subscribe(new Reactor<DisposeTextureData>(_ =>
                {
                    var disposeData = new DisposeTextureData(Id);
                    Dispose(disposeData);
                }));

            FilePath = filePath;
            Init(name, imageData);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Texture"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="openGLService">Provides OpenGL related helper methods.</param>
        /// <param name="disposeTexturesReactable">Sends a push notification to dispose of a texture.</param>
        /// <param name="name">The name of the texture.</param>
        /// <param name="filePath">The file path to the image file.</param>
        /// <param name="imageData">The image data of the texture.</param>
        internal Texture(
            IGLInvoker gl,
            IOpenGLService openGLService,
            IReactable<DisposeTextureData> disposeTexturesReactable,
            string name,
            string filePath,
            ImageData imageData)
        {
            this.gl = gl ?? throw new ArgumentNullException(nameof(gl), "The parameter must not be null.");
            this.openGLService = openGLService ?? throw new ArgumentNullException(nameof(openGLService), "The parameter must not be null.");

            if (disposeTexturesReactable is null)
            {
                throw new ArgumentNullException(nameof(disposeTexturesReactable), "The parameter must not be null.");
            }

            this.disposeUnsubscriber =
                disposeTexturesReactable.Subscribe(new Reactor<DisposeTextureData>(Dispose));

            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath), "The parameter must not be null or empty.");
            }

            FilePath = filePath;
            Init(name, imageData);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Texture"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~Texture()
        {
            if (UnitTestDetector.IsRunningFromUnitTest)
            {
                return;
            }

            Dispose(new DisposeTextureData(Id));
        }

        /// <inheritdoc/>
        public uint Id { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <inheritdoc/>
        public string FilePath { get; }

        /// <inheritdoc/>
        public uint Width { get; private set; }

        /// <inheritdoc/>
        public uint Height { get; private set; }

        /// <summary>
        /// Disposes of the texture if this textures <see cref="Id"/> matches the texture ID  in given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The data of the texture to dispose.</param>
        private void Dispose(DisposeTextureData data)
        {
            if (this.isDisposed || Id != data.TextureId)
            {
                return;
            }

            this.gl.DeleteTexture(Id);
            this.disposeUnsubscriber.Dispose();

            this.isDisposed = true;
        }

        /// <summary>
        /// Initializes the <see cref="Texture"/>.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <param name="imageData">The image data of the texture.</param>
        private void Init(string name, ImageData imageData)
        {
            if (imageData.IsEmpty())
            {
                throw new ArgumentException("The image data must not be empty.", nameof(imageData));
            }

            Id = this.gl.GenTexture();

            this.gl.BindTexture(GLTextureTarget.Texture2D, Id);
            this.openGLService.BindTexture2D(Id);

            Width = imageData.Width;
            Height = imageData.Height;

            Name = name;

            UploadDataToGPU(name, imageData);

            this.openGLService.UnbindTexture2D();
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

            this.openGLService.LabelTexture(Id, name);

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

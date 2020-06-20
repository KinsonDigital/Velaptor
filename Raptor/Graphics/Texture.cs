// <copyright file="Texture.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Graphics
{
    using System;
    using System.IO;
    using OpenToolkit.Graphics.OpenGL4;

    /// <summary>
    /// The texture to render to the screen.
    /// </summary>
    public class Texture : ITexture
    {
        private bool disposedValue = false;

        public Texture(byte[] pixelData, int width, int height, string name)
        {
            ID = GL.GenTexture();

            GL.BindTexture(TextureTarget.Texture2D, ID);

            Width = width;
            Height = height;

            Name = Path.GetFileNameWithoutExtension(name);

            UploadDataToGPU(pixelData, width, height, name);

            // Unbind
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        /// <inheritdoc/>
        public int ID { get; protected set; }

        /// <inheritdoc/>
        public string Name { get; private set; }

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
            if (this.disposedValue)
                return;

            // NOTE: Finalizers cannot call this method and then invoke GL calls.
            // GL calls are not on the same thread as the finalizer and they will not work.
            // To avoid this problem, you have to make sure that all dispose methods are called
            // manually for anything using these objects where they contain GL calls in there
            // Dispose() methods
            GL.DeleteTexture(ID);

            this.disposedValue = true;
        }

        private void UploadDataToGPU(byte[] pixelData, int width, int height, string name)
        {
            GL.ObjectLabel(ObjectLabelIdentifier.Texture, ID, -1, Path.GetFileName(name));

            // Set the min and mag filters to linear
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            // Sett the x(S) and y(T) axis wrap mode to repeat
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            // Load the texture data to the GPU for the currently active texture slot
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        }
    }
}

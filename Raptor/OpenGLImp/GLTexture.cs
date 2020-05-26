using System;
using System.Collections.Generic;
using Raptor.Graphics;
using OpenToolkit.Graphics.OpenGL4;
using OpenToolkit;

namespace Raptor.OpenGLImp
{
    /// <summary>
    /// Here just for reference.  This is how you render to another window.
    /// //This will be for particle maker
    /// </summary>
    public class MyBindings : IBindingsContext
    {
        public IntPtr GetProcAddress(string procName)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// An OpenGL texture that can be rendered to a render target.
    /// </summary>
    internal class GLTexture : ITexture
    {
        #region Private Fields
        private static readonly List<int> _boundTextures = new List<int>();
        private bool _disposedValue = false;
        private readonly int _id;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="GLTexture"/>.
        /// </summary>
        /// <param name="imageData">The texture data.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        public GLTexture(byte[] imageData, int width, int height)
        {
            //TODO: Maybe expose this ID as property for the other GL helper classes to use?
            //This will depend on how the render can be implemented
            _id = GL.GenTexture();

            Width = width;
            Height = height;

            Bind();

            LoadTextureDataToGPU(imageData, width, height);

            Unbind();
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width { get; }

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height { get; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Bind the texture for performing operations on it.
        /// </summary>
        public void Bind()
        {
            if (_boundTextures.Contains(_id))
                return;

            GL.BindTexture(TextureTarget.Texture2D, _id);
            _boundTextures.Add(_id);
        }


        /// <summary>
        /// Unbind the texture.
        /// </summary>
        public void Unbind()
        {
            if (!_boundTextures.Contains(_id))
                return;

            GL.BindTexture(TextureTarget.Texture2D, 0);
            _boundTextures.Remove(_id);
        }


        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// <paramref name="disposing">True if other managed resources need to be disposed of.</paramref>
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            //NOTE: Finalizers cannot call this method and then invoke GL calls.
            //GL calls are not on the same thread as the finalizer and they will not work.
            //To avoid this problem, you have to make sure that all dispose methods are called
            //manually for anything using these objects where they contain GL calls in there
            //Dispose() methods
            Unbind();
            GL.DeleteTexture(_id);

            _disposedValue = true;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Loads the pixel data to the GPU.
        /// </summary>
        /// <param name="pixelData">The texure pixel data to load.</param>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        private static void LoadTextureDataToGPU(byte[] pixelData, int width, int height)
        {
            //Set the min and mag filters to linear
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            //Sett the x(S) and y(T) axis wrap mode to repeat
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            //Load the texture data to the GPU for the currently active texture slot
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pixelData);
        }
        #endregion
    }
}

using Raptor.Graphics;
using SDLCore;
using System;

namespace Raptor.SDLImp
{
    /// <summary>
    /// Represents a SDL texture that can be renderered to a graphics surface.
    /// </summary>
    internal class SDLTexture : ITexture
    {
        #region Private Fields
        private readonly SDL? _sdl = null;
        private readonly IntPtr _texturePtr;
        private readonly int _width;
        private readonly int _height;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="SDLTexture"/>.
        /// </summary>
        /// <param name="texturePtr">The pointer to the SDL texture.</param>
        public SDLTexture(IntPtr texturePtr)
        {
            //TODO: Load the SDL libraries using a library loader
            _texturePtr = texturePtr;

            //Query the texture data which gets the width and height of the texture
            if (!(_sdl is null))
                _sdl.QueryTexture(_texturePtr, out uint _, out _, out _width, out _height);
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width => _width;

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height => _height;
        #endregion


        #region Public Methods
        /// <summary>
        /// Injects any arbitrary data into the plugin for use.  Must be a class.
        /// </summary>
        /// <typeparam name="T">The type of data to inject.</typeparam>
        /// <param name="data">The data to inject.</param>
        public void InjectData<T>(T data) where T : class => throw new NotImplementedException();


        /// <summary>
        /// Gets the data as the given type <typeparamref name="T"/>.
        /// </summary>
        /// <param name="option">Use the value of '1' if the data to return is of type <see cref="PointerContainer"/>.</param>
        /// <typeparam name="T">The type of data to get.</typeparam>
        /// <exception cref="Exception">Thrown if the <paramref name="option"/> value is not the value of
        /// type '1' for the type <see cref="PointerContainer"/>.</exception>
        public T? GetData<T>(int option) where T : class
        {
            if (option == 1)
            {
                var ptrContainer = new PointerContainer();
                ptrContainer.PackPointer(_texturePtr);


                return ptrContainer as T;
            }

            throw new Exception($"The option '{option}' is not a valid option.");
        }
        #endregion
    }
}

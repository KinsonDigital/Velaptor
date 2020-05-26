using System;

namespace Raptor.Graphics
{
    /// <summary>
    /// Represents a texture that can be renderered to a graphics surface.
    /// </summary>
    public interface ITexture : IDisposable
    {
        #region Props
        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        int Height { get; }
        #endregion


        #region Methods
        /// <summary>
        /// Binds the texture.
        /// </summary>
        void Bind();


        /// <summary>
        /// Unbinds the texture.
        /// </summary>
        void Unbind();
        #endregion
    }
}
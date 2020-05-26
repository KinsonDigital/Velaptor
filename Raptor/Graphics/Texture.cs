using Raptor.OpenGLImp;

namespace Raptor.Graphics
{
    /// <summary>
    /// The texture to render to the screen.
    /// </summary>
    public class Texture
    {
        #region Constructors
        public Texture(ITexture texture)
        {
            InternalTexture = texture;
        }


        public Texture(byte[] imageData, int width, int height)
        {
            InternalTexture = new GLTexture(imageData, width, height);
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width => InternalTexture.Width;

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height => InternalTexture.Height;


        internal ITexture InternalTexture { get; private set; }
        #endregion
    }
}

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
        /// Gets or sets the ID of the texture.
        /// </summary>
        public int ID => InternalTexture.ID;

        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width => InternalTexture.Width;

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height => InternalTexture.Height;


        public int Layer { get; set; }


        internal ITexture InternalTexture { get; private set; }
        #endregion
    }
}

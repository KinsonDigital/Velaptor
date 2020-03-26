namespace Raptor.Graphics
{
    /// <summary>
    /// The texture to render to the screen.
    /// </summary>
    public class Texture
    {
        #region Props
        /// <summary>
        /// Gets the internal texture plugin.
        /// </summary>
        public ITexture InternalTexture { get; set; }

        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width => InternalTexture.Width;

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height => InternalTexture.Height;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="Texture"/>.
        /// </summary>
        /// <param name="mockedTexture">The mocked texture to inject.</param>
        public Texture(ITexture mockedTexture) => InternalTexture = mockedTexture;
        #endregion
    }
}

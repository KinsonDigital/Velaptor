using Raptor.OpenGL;

namespace Raptor.Graphics
{
    /// <summary>
    /// The texture to render to the screen.
    /// </summary>
    public class Texture
    {
        #region Constructors
        public Texture(byte[] imageData, int width, int height)
        {
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the ID of the texture.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets the width of the texture.
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Gets the height of the texture.
        /// </summary>
        public int Height { get; set; }


        public int Layer { get; set; }
        #endregion
    }
}

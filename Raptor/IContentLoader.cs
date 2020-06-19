using Raptor.Graphics;

namespace Raptor
{
    /// <summary>
    /// Represents a loader that can load content for rendering or sound.
    /// </summary>
    public interface IContentLoader
    {
        #region Props
        /// <summary>
        /// Gets or sets the root directory for the game's content.
        /// </summary>
        string ContentRootDirectory { get; }
        #endregion


        #region Methods
        Texture? LoadTexture(string name);
        #endregion
    }
}

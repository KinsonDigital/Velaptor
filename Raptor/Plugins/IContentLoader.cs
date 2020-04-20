using Raptor.Graphics;
namespace Raptor.Plugins
{
    /// <summary>
    /// Represents a loader that can load content for rendering or sound.
    /// </summary>
    public interface IContentLoader
    {
        #region Props
        /// <summary>
        /// The path to the executable game consuming the content being loaded.
        /// </summary>
        string GamePath { get; }

        /// <summary>
        /// Gets or sets the root directory for the game's content.
        /// </summary>
        string ContentRootDirectory { get; set; }
        #endregion


        #region Methods
        /// <summary>
        /// Loads a texture that has the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of texture to render.</typeparam>
        /// <param name="name">The name of the texture object to render.</param>
        /// <returns></returns>
        T? LoadTexture<T>(string name) where T : class, ITexture;


        /// <summary>
        /// Loads a text objec to render that has the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of text object to render.</typeparam>
        /// <param name="name">The name of the text object to render.</param>
        /// <returns></returns>
        T? LoadText<T>(string name) where T : class, IText;
        #endregion
    }
}

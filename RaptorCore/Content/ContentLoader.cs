using RaptorCore.Graphics;
using RaptorCore.Plugins;
using System.Diagnostics.CodeAnalysis;

namespace RaptorCore.Content
{
    /// <summary>
    /// Loads graphical and soud content for rendering and playback.
    /// </summary>
    public class ContentLoader
    {
        #region Private Fields
        private readonly IContentLoader _internalLoader;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ContentLoader"/>.
        /// USED FOR UNIT TESTING.
        /// </summary>
        /// <param name="mockedContentLoader">The mocked content loader to inject.</param>
        internal ContentLoader(IContentLoader mockedContentLoader) => _internalLoader = mockedContentLoader;


        /// <summary>
        /// Creates a new instace of <see cref="ContentLoader"/>.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ContentLoader()
        {
            //TODO: Figure out how to get the proper implementation inside of this class
        }
        #endregion


        #region Props
        /// <summary>
        /// The path to the executable game consuming the content being loaded.
        /// </summary>
        public string GamePath => _internalLoader.GamePath;


        /// <summary>
        /// Gets or sets the root directory for the game's content.
        /// </summary>
        public string ContentRootDirectory
        {
            get => _internalLoader.ContentRootDirectory;
            set => _internalLoader.ContentRootDirectory = value;
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Loads a texture that has the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of texture to render.</typeparam>
        /// <param name="name">The name of the texture object to render.</param>
        /// <returns></returns>
        public Texture LoadTexture(string textureName) => new Texture(_internalLoader.LoadTexture<ITexture>(textureName));


        /// <summary>
        /// Loads a text objec to render that has the given <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">The type of text object to render.</typeparam>
        /// <param name="name">The name of the text object to render.</param>
        /// <returns></returns>
        public GameText LoadText(string textName) => new GameText() { InternalText = _internalLoader.LoadText<IText>(textName) };
        #endregion
    }
}

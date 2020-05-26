using System.IO;
using System.Reflection;
using Raptor.Graphics;
using Raptor.Plugins;
using FileIO.Core;

namespace Raptor.Content
{
    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        #region Private Fields
        private readonly string _baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly string _graphicsDir;
        private readonly IImageFile _file;
        private string _contentDir;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instace of <see cref="ContentLoader"/>.
        /// </summary>
        public ContentLoader(IImageFile file)
        {
            _file = file;
            _contentDir = $@"{_baseDir}Content\";
            _graphicsDir = @$"{_contentDir}Graphics\";
        }
        #endregion


        #region Props
        /// <summary>
        /// Gets or sets the root directory for the content.
        /// </summary>
        public string ContentRootDirectory
        {
            get => _contentDir;
            set => _contentDir = $@"{value}Content\";
        }
        #endregion


        #region Public Methods
        /// <summary>
        /// Loads a texture that has the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the texture to load.</param>
        /// <returns></returns>
        public Texture? LoadTexture(string name)
        {
            var textureImagePath = $"{_graphicsDir}{name}";


            var (pixels, width, height) = _file.Load(textureImagePath);

            return new Texture(pixels, width, height);
        }
        #endregion
    }
}

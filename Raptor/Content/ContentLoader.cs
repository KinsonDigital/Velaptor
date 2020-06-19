using System.IO;
using System.Reflection;
using Raptor.Graphics;
using FileIO.Core;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using System.Text.Json;
using System;
using FileIO.File;

namespace Raptor.Content
{
    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        #region Private Fields
        private readonly string _baseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly IImageFile _file;
        private readonly ITextFile _textFile;
        private string _graphicsDir;
        private string _contentDir;
        #endregion


        #region Constructors
        public ContentLoader()
        {
            _file = new ImageFile();
            _textFile = new TextFile();
            SetupPaths();
        }


        /// <summary>
        /// Creates a new instace of <see cref="ContentLoader"/>.
        /// </summary>
        public ContentLoader(IImageFile imageFile, ITextFile textFile)
        {
            _file = imageFile;
            _textFile = textFile;
            SetupPaths();
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

            return new Texture(pixels, width, height, name);
        }


        public Dictionary<string, AtlasSubRect> LoadAtlasData(string fileName)
        {
            var result = new Dictionary<string, AtlasSubRect>();

            var contentDir = $@"{_baseDir}Content\";
            var graphicsContent = $@"{contentDir}Graphics\";

            var rawData = _textFile.Load($"{graphicsContent}{fileName}");

            var rectItems = JsonSerializer.Deserialize<AtlasSubRect[]>(rawData);

            foreach (var item in rectItems)
            {
                result.Add(item.Name, item);
            }

            return result;
        }
        #endregion

        /// <summary>
        /// Sets up the pathing variables for the content location on disk.
        /// </summary>
        private void SetupPaths()
        {
            _contentDir = $@"{_baseDir}Content\";
            _graphicsDir = @$"{_contentDir}Graphics\";
        }
    }
}

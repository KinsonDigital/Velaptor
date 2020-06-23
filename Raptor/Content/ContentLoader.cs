// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using FileIO.Core;
    using FileIO.File;
    using Raptor.Graphics;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private readonly IImageFile? imageFile;
        private readonly ITextFile? textFile;
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<AtlasRegionRectangle[]> atlasDataLoader;
        private string? contentRootDirectory = @$"{BaseDir}Content\";
        private string? graphicsDir;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public ContentLoader()
        {
            this.imageFile = new ImageFile();
            this.textFile = new TextFile();
            this.textureLoader = new TextureLoader(this.imageFile);
            this.atlasDataLoader = new AtlasDataLoader<AtlasRegionRectangle>(this.textFile);
            SetupPaths();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="atlasDataLoader">The loader used to load atlas data.</param>
        public ContentLoader(ILoader<ITexture> textureLoader, ILoader<AtlasRegionRectangle[]> atlasDataLoader)
        {
            this.textureLoader = textureLoader;
            this.atlasDataLoader = atlasDataLoader;
            SetupPaths();
        }

        /// <inheritdoc/>
        public string ContentRootDirectory
        {
            get => this.contentRootDirectory;
            set
            {
                value = value is null ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith('\\') ? value : $@"{value}\";

                this.contentRootDirectory = $@"{value}Content\";
            }
        }

        /// <inheritdoc/>
        public ITexture? LoadTexture(string name)
        {
            var textureImagePath = $"{this.graphicsDir}{name}";

            return this.textureLoader.Load(textureImagePath);
        }

        /// <inheritdoc/>
        public AtlasRegionRectangle[] LoadAtlasData(string name)
            => this.atlasDataLoader.Load($@"{this.graphicsDir}{name}");

        /// <summary>
        /// Sets up the pathing variables for the content location on disk.
        /// </summary>
        private void SetupPaths() => this.graphicsDir = @$"{this.contentRootDirectory}Graphics\";
    }
}

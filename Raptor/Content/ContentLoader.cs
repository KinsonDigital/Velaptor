// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CA1303 // Do not pass literals as localized parameters
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
namespace Raptor.Content
{
    using FileIO.Core;
    using Raptor.Audio;
    using Raptor.Graphics;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private readonly IContentSource contentSource;
        private readonly IImageFile? imageFile;
        private readonly ITextFile? textFile;
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<AtlasRegionRectangle[]> atlasDataLoader;
        private readonly ILoader<ISound> soundLoader;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="atlasDataLoader">The loader used to load atlas data.</param>
        public ContentLoader(IContentSource contentSource,
                             ILoader<ITexture> textureLoader,
                             ILoader<AtlasRegionRectangle[]> atlasDataLoader,
                             ILoader<ISound> soundLoader,
                             IImageFile imageFile,
                             ITextFile textFile)
        {
            this.contentSource = contentSource;
            this.imageFile = imageFile;
            this.textFile = textFile;
            this.textureLoader = textureLoader;
            this.atlasDataLoader = atlasDataLoader;
            this.soundLoader = soundLoader;
        }

        /// <inheritdoc/>
        public ITexture? LoadTexture(string name) => this.textureLoader.Load(name);

        /// <inheritdoc/>
        public ISound LoadSound(string name) => this.soundLoader.Load(name);

        /// <inheritdoc/>
        public AtlasRegionRectangle[] LoadAtlasData(string name)
        {
            var atlasFilePath = this.contentSource.GetContentPath(ContentType.Atlas, name);

            return this.atlasDataLoader.Load(atlasFilePath);
        }
    }
}

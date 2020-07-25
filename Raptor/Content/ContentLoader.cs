// <copyright file="ContentLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Reflection;
    using FileIO.Core;
    using FileIO.File;
    using Raptor.Audio;
    using Raptor.Graphics;

    /// <summary>
    /// Loads content.
    /// </summary>
    public class ContentLoader : IContentLoader
    {
        private static readonly string BaseDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\";
        private static string contentRootDirectory = @$"{BaseDir}Content\";
        private static string graphicsDirName = "Graphics";
        private static string soundsDirName = "Sounds";
        private static string atlasDirName = "AtlasData";
        private readonly IImageFile? imageFile;
        private readonly ITextFile? textFile;
        private readonly ILoader<ITexture> textureLoader;
        private readonly ILoader<AtlasRegionRectangle[]> atlasDataLoader;
        private readonly ILoader<ISound> soundLoader;

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
            this.soundLoader = new SoundLoader();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentLoader"/> class.
        /// </summary>
        /// <param name="textureLoader">The loader used to load textures.</param>
        /// <param name="atlasDataLoader">The loader used to load atlas data.</param>
        public ContentLoader(ILoader<ITexture> textureLoader, ILoader<AtlasRegionRectangle[]> atlasDataLoader, ILoader<ISound> soundLoader)
        {
            this.textureLoader = textureLoader;
            this.atlasDataLoader = atlasDataLoader;
            this.soundLoader = soundLoader;
        }

        /// <inheritdoc/>
        public static string ContentRootDirectory
        {
            get => contentRootDirectory;
            set
            {
                value = value is null ? BaseDir : value;

                // If the value ends with a backslash, leave as is, else add one
                value = value.EndsWith('\\') ? value : $@"{value}\";

                contentRootDirectory = $@"{value}Content\";
            }
        }

        public static string GraphicsDirectoryName
        {
            get => graphicsDirName;
            set
            {
                // TODO: Check if value is null or empty and throw exception
                // TODO: If the value starts or ends with a \, remove it
                graphicsDirName = value;
            }
        }

        public static string SoundsDirectoryName
        {
            get => soundsDirName;
            set
            {
                // TODO: Check if value is null or empty and throw exception
                // TODO: If the value starts or ends with a \, remove it
                soundsDirName = value;
            }
        }

        public static string AtlasDirectoryName
        {
            get => atlasDirName;
            set
            {
                // TODO: Check if value is null or empty and throw exception
                // TODO: If the value starts or ends with a \, remove it
                atlasDirName = value;
            }
        }

        /// <inheritdoc/>
        public ITexture? LoadTexture(string name)
        {
            var textureImagePath = $"{GetGraphicsPath()}{name}";

            return this.textureLoader.Load(textureImagePath);
        }

        /// <inheritdoc/>
        public ISound LoadSound(string name)
        {
            var soundPath = @$"{GetSoundsPath()}{name}";

            return this.soundLoader.Load(soundPath);
        }

        /// <inheritdoc/>
        public AtlasRegionRectangle[] LoadAtlasData(string name)
        {
            var atlasPath = $@"{GetAtlasPath()}{name}";

            return this.atlasDataLoader.Load(atlasPath);
        }

        private string GetGraphicsPath()
        {
            return $@"{contentRootDirectory}{graphicsDirName}\";
        }

        private string GetSoundsPath()
        {
            return $@"{contentRootDirectory}{soundsDirName}\";
        }

        private string GetAtlasPath()
        {
            return $@"{contentRootDirectory}{atlasDirName}\";
        }
    }
}

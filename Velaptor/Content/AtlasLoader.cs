// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Factories;
    using Velaptor.Graphics;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads atlas data.
    /// </summary>
    public sealed class AtlasLoader : ILoader<IAtlasData>
    {
        private const string TextureExtension = ".png";
        private const string AtlasDataExtension = ".json";
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly IAtlasDataFactory atlasDataFactory;
        private readonly IPathResolver atlasDataPathResolver;
        private readonly IJSONService jsonService;
        private readonly IFile file;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public AtlasLoader()
        {
            this.textureCache = IoC.Container.GetInstance<IDisposableItemCache<string, ITexture>>();
            this.atlasDataFactory = IoC.Container.GetInstance<IAtlasDataFactory>();
            this.atlasDataPathResolver = PathResolverFactory.CreateTextureAtlasPathResolver();
            this.jsonService = IoC.Container.GetInstance<IJSONService>();
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        /// <param name="textureCache">Provides texture caching services.</param>
        /// <param name="atlasDataFactory">Generates <see cref="IAtlasData"/> instances.</param>
        /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
        /// <param name="jsonService">Provides JSON related services.</param>
        /// <param name="file">Used to load the texture atlas.</param>
        /// <param name="path">Processes directory and file paths.</param>
        internal AtlasLoader(
            IDisposableItemCache<string, ITexture> textureCache,
            IAtlasDataFactory atlasDataFactory,
            IPathResolver atlasDataPathResolver,
            IJSONService jsonService,
            IFile file,
            IPath path)
        {
            this.textureCache = textureCache ?? throw new ArgumentNullException(nameof(textureCache), "The parameter must not be null.");
            this.atlasDataFactory = atlasDataFactory ?? throw new ArgumentNullException(nameof(atlasDataFactory), "The parameter must not be null.");
            this.atlasDataPathResolver = atlasDataPathResolver ?? throw new ArgumentNullException(nameof(atlasDataPathResolver), "The parameter must not be null.");
            this.jsonService = jsonService ?? throw new ArgumentNullException(nameof(jsonService), "The parameter must not be null.");
            this.file = file ?? throw new ArgumentNullException(nameof(file), "The parameter must not be null.");
            this.path = path ?? throw new ArgumentNullException(nameof(path), "The parameter must not be null.");
        }

        /// <inheritdoc/>
        public IAtlasData Load(string contentPathOrName)
        {
            var isFullFilePath = contentPathOrName.IsValidFilePath();
            string name;
            string dirPath;

            if (isFullFilePath)
            {
                var validExtensions = new[] { TextureExtension, AtlasDataExtension };
                var extension = this.path.GetExtension(contentPathOrName);

                if (validExtensions.All(e => e != extension))
                {
                    var exceptionMsg = $"When performing full content file path loads,";
                    exceptionMsg += $" the files must be a '{TextureExtension}' or '{AtlasDataExtension}' extension.";

                    throw new LoadAtlasException(exceptionMsg);
                }

                name = this.path.GetFileNameWithoutExtension(contentPathOrName);

                dirPath = $@"{this.path.GetDirectoryName(contentPathOrName).TrimEnd('\\')}\";
            }
            else
            {
                if (contentPathOrName.IsFullyQualifiedDirPath() || contentPathOrName.IsUNCPath())
                {
                    var exceptionMsg = "Directory paths not allowed when loading texture atlas data.\n";
                    exceptionMsg += "Relative and fully qualified directory paths not valid.\n";
                    exceptionMsg += "The path must be a fully qualified file path or content item name\n";
                    exceptionMsg += @"located in the application's './Content/Atlas' directory.";

                    throw new LoadAtlasException(exceptionMsg);
                }

                // Remove a possible file extension and return just the 'name' of the content.
                // The name of the content should always match the name of the file without the extension
                name = this.path.GetFileNameWithoutExtension(contentPathOrName);

                // Resolve to the application's content directory where atlas data is located
                dirPath = this.atlasDataPathResolver.ResolveDirPath();
            }

            var atlasDataFilePath = $"{dirPath}{name}{AtlasDataExtension}";

            if (this.file.Exists(atlasDataFilePath) is false)
            {
                var exceptionMsg = $"The atlas data directory '{dirPath}' does not contain the";
                exceptionMsg += $" required '{dirPath}{name}{AtlasDataExtension}' atlas data file.";

                throw new LoadAtlasException(exceptionMsg);
            }

            var atlasImageFilePath = $"{dirPath}{name}{TextureExtension}";

            if (this.file.Exists(atlasImageFilePath) is false)
            {
                var exceptionMsg = $"The atlas data directory '{dirPath}' does not contain the";
                exceptionMsg += $" required '{dirPath}{name}{TextureExtension}' atlas image file.";

                throw new LoadAtlasException(exceptionMsg);
            }

            var rawData = this.file.ReadAllText(atlasDataFilePath);
            var atlasSpriteData = this.jsonService.Deserialize<AtlasSubTextureData[]>(rawData);

            if (atlasSpriteData is null)
            {
                throw new LoadContentException($"There was an issue deserializing the JSON atlas data file at '{atlasDataFilePath}'.");
            }

            var atlasName = isFullFilePath
                ? name
                : contentPathOrName;

            return this.atlasDataFactory.Create(atlasSpriteData, dirPath, atlasName);
        }

        /// <inheritdoc/>
        public void Unload(string contentPathOrName) => this.textureCache.Unload(contentPathOrName);

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.textureCache.Dispose();
            }

            this.isDisposed = true;
        }
    }
}

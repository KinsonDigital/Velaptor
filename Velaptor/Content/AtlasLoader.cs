// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Guards;

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
        private readonly IItemCache<string, ITexture> textureCache;
        private readonly IAtlasDataFactory atlasDataFactory;
        private readonly IPathResolver atlasDataPathResolver;
        private readonly IJSONService jsonService;
        private readonly IFile file;
        private readonly IPath path;

        /// <summary>
        /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public AtlasLoader()
        {
            this.textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
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
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        internal AtlasLoader(
            IItemCache<string, ITexture> textureCache,
            IAtlasDataFactory atlasDataFactory,
            IPathResolver atlasDataPathResolver,
            IJSONService jsonService,
            IFile file,
            IPath path)
        {
            EnsureThat.ParamIsNotNull(textureCache);
            EnsureThat.ParamIsNotNull(atlasDataFactory);
            EnsureThat.ParamIsNotNull(atlasDataPathResolver);
            EnsureThat.ParamIsNotNull(jsonService);
            EnsureThat.ParamIsNotNull(file);
            EnsureThat.ParamIsNotNull(path);

            this.textureCache = textureCache;
            this.atlasDataFactory = atlasDataFactory;
            this.atlasDataPathResolver = atlasDataPathResolver;
            this.jsonService = jsonService;
            this.file = file;
            this.path = path;
        }

        /// <summary>
        /// Loads texture atlas data using the given <param name="contentNameOrPath"></param>.
        /// </summary>
        /// <param name="contentNameOrPath">The content name or file path to the atlas data.</param>
        /// <returns>The loaded atlas data.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Occurs if <paramref name="contentNameOrPath"/> is null or empty.
        /// </exception>
        /// <exception cref="LoadAtlasException">
        ///     If the given full file path is not a <c>Texture(.png)</c> or <c>Atlas Data(.json)</c> file.
        /// </exception>
        /// <exception cref="LoadContentException">
        ///     Occurs if directory path is used.  A non path content name or fully qualified file path is required.
        /// </exception>
        /// <remarks>
        /// Valid Values:
        /// <list type="bullet">
        ///     <item>MyAtlas</item>
        ///     <item>C:\Atlas\MyAtlas.png</item>
        ///     <item>C:\Atlas\MyAtlas.json</item>
        /// </list>
        ///
        /// Invalid Values:
        /// <list type="bullet">
        ///     <item>C:\Atlas\MyAtlas</item>
        ///     <item>C:\Atlas\MyAtlas.txt</item>
        /// </list>
        /// </remarks>
        public IAtlasData Load(string contentNameOrPath)
        {
            if (string.IsNullOrEmpty(contentNameOrPath))
            {
                throw new ArgumentNullException(
                    nameof(contentNameOrPath),
                    "The parameter must not be null or empty.");
            }

            var isFullFilePath = contentNameOrPath.HasValidFullFilePathSyntax();
            string name;
            string dirPath;

            if (isFullFilePath)
            {
                var validExtensions = new[] { TextureExtension, AtlasDataExtension };
                var extension = this.path.GetExtension(contentNameOrPath);

                if (validExtensions.All(e => e != extension))
                {
                    var exceptionMsg = $"When performing full content file path loads,";
                    exceptionMsg += $" the files must be a '{TextureExtension}' or '{AtlasDataExtension}' extension.";

                    throw new LoadAtlasException(exceptionMsg);
                }

                name = this.path.GetFileNameWithoutExtension(contentNameOrPath);

                dirPath = $@"{this.path.GetDirectoryName(contentNameOrPath).TrimEnd('\\')}\";
            }
            else
            {
                if (contentNameOrPath.HasValidFullDirPathSyntax() || contentNameOrPath.HasValidUNCPathSyntax())
                {
                    var exceptionMsg = "Directory paths not allowed when loading texture atlas data.\n";
                    exceptionMsg += "Relative and fully qualified directory paths not valid.\n";
                    exceptionMsg += "The path must be a fully qualified file path or content item name\n";
                    exceptionMsg += @"located in the application's './Content/Atlas' directory.";

                    throw new LoadAtlasException(exceptionMsg);
                }

                // Remove a possible file extension and return just the 'name' of the content.
                // The name of the content should always match the name of the file without the extension
                name = this.path.GetFileNameWithoutExtension(contentNameOrPath);

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
                : contentNameOrPath;

            return this.atlasDataFactory.Create(atlasSpriteData, dirPath, atlasName);
        }

        /// <inheritdoc/>
        public void Unload(string contentPathOrName) => this.textureCache.Unload(contentPathOrName);
    }
}

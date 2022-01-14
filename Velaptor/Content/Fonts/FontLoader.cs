// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
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
    using Velaptor.Services;

    // ReS harper restore RedundantNameQualifier

    /// <summary>
    /// Loads font content for rendering text.
    /// </summary>
    public sealed class FontLoader : ILoader<IFont>
    {
        private const string ExpectedMetaDataSyntax = "size:<font-size>";
        private readonly IFontAtlasService fontAtlasService;
        private readonly IPathResolver fontPathResolver;
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly IFontFactory fontFactory;
        private readonly IFontMetaDataParser fontMetaDataParser;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        public FontLoader()
        {
            this.fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
            this.fontPathResolver = PathResolverFactory.CreateFontPathResolver();
            this.textureCache = IoC.Container.GetInstance<IDisposableItemCache<string, ITexture>>();
            this.fontFactory = IoC.Container.GetInstance<IFontFactory>();
            this.fontMetaDataParser = IoC.Container.GetInstance<IFontMetaDataParser>();
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="fontFactory">Generates new <see cref="IFont"/> instances.</param>
        /// <param name="fontMetaDataParser">Parses metadata from strings.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal FontLoader(
            IFontAtlasService fontAtlasService,
            IPathResolver fontPathResolver,
            IDisposableItemCache<string, ITexture> textureCache,
            IFontFactory fontFactory,
            IFontMetaDataParser fontMetaDataParser,
            IPath path)
        {
            this.fontAtlasService = fontAtlasService;
            this.fontPathResolver = fontPathResolver;
            this.textureCache = textureCache;
            this.fontFactory = fontFactory;
            this.fontMetaDataParser = fontMetaDataParser;
            this.path = path;
        }

        /// <summary>
        /// Loads font content from the application's content directory or directly using a full file path.
        /// </summary>
        /// <param name="contentWithMetaData">The name or full file path to the font with metadata.</param>
        /// <returns>The loaded font.</returns>
        /// <exception cref="ArgumentNullException">
        ///     Occurs when the <paramref name="contentWithMetaData"/> argument is null or empty.
        /// </exception>
        /// <exception cref="CachingMetaDataException">
        ///     Occurs if the meta data is missing or invalid.
        /// </exception>
        /// <remarks>
        ///     If a path is used, it must be a fully qualified file path.
        ///     <para>Directory paths are not valid.</para>
        /// </remarks>
        /// <example>
        ///     <code>
        ///     // Valid Value
        ///     ContentLoader.Load("my-font|size:12");
        ///
        ///     // Valid Value
        ///     ContentLoader.Load(@"C:\fonts\my-font.ttf|size:12");
        ///
        ///     // Invalid Value
        ///     ContentLoader.Load("my-font|size:12");
        ///
        ///     ContentLoader.Load("my-font|size:12");
        ///     </code>
        /// </example>
        public IFont Load(string contentWithMetaData)
        {
            if (string.IsNullOrEmpty(contentWithMetaData))
            {
                throw new ArgumentNullException(nameof(contentWithMetaData), "The parameter must not be null.");
            }

            var parseResult = this.fontMetaDataParser.Parse(contentWithMetaData);
            string fullFontFilePath;

            if (parseResult.ContainsMetaData)
            {
                if (parseResult.IsValid)
                {
                    fullFontFilePath = parseResult.MetaDataPrefix;

                    // If the file path is a full file path, leave it be.
                    // If it is not, then it is a content name and could be a file name with an extension.
                    // If this is the case, remove the extension
                    if (parseResult.MetaDataPrefix.IsValidFilePath() is false)
                    {
                        var newMetaDataPrefix = this.path.GetFileNameWithoutExtension(parseResult.MetaDataPrefix);

                        parseResult = new FontMetaDataParseResult(
                            parseResult.ContainsMetaData,
                            parseResult.IsValid,
                            newMetaDataPrefix,
                            parseResult.MetaData,
                            parseResult.FontSize);
                    }
                }
                else
                {
                    var exceptionMsg = $"The metadata '{parseResult.MetaData}' is invalid when loading '{contentWithMetaData}'.";
                    exceptionMsg += $"\n\tExpected MetaData Syntax: {ExpectedMetaDataSyntax}";
                    exceptionMsg += "\n\tExample: size:12";
                    throw new CachingMetaDataException(exceptionMsg);
                }
            }
            else
            {
                var exceptionMsg = "The font content item 'missing-metadata' must have metadata post fixed to the";
                exceptionMsg += " end of a content name or full file path";

                throw new CachingMetaDataException(exceptionMsg);
            }

            fullFontFilePath = fullFontFilePath.IsValidFilePath()
                ? parseResult.MetaDataPrefix
                : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

            // If the full font file path is empty, then the font does not exist. Throw an exception
            if (string.IsNullOrEmpty(fullFontFilePath))
            {
                var exceptionMsg = $"The font content item '{parseResult.MetaDataPrefix}' does not exist.";
                exceptionMsg += $"\nCheck the applications font content directory '{this.fontPathResolver.ResolveDirPath()}' to see if it exists.";

                throw new LoadFontException(exceptionMsg);
            }

            var contentName = this.path.GetFileNameWithoutExtension(fullFontFilePath);

            var (_, atlasData) = this.fontAtlasService.CreateFontAtlas(fullFontFilePath, parseResult.FontSize);

            var cacheKey = $"{fullFontFilePath}|{parseResult.MetaData}";
            var fontAtlasTexture = this.textureCache.GetItem(cacheKey);

            return this.fontFactory.Create(fontAtlasTexture, contentName, fullFontFilePath, parseResult.FontSize, atlasData);
        }

        /// <inheritdoc/>
        public void Unload(string contentWithMetaData)
        {
            var parseResult = this.fontMetaDataParser.Parse(contentWithMetaData);

            if (parseResult.ContainsMetaData)
            {
                if (parseResult.IsValid)
                {
                    var fullFilePath = parseResult.MetaDataPrefix.IsValidFilePath()
                        ? parseResult.MetaDataPrefix
                        : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

                    var cacheKey = $"{fullFilePath}|{parseResult.MetaData}";

                    this.textureCache.Unload(cacheKey);
                }
                else
                {
                    var exceptionMsg = $"The metadata '{parseResult.MetaData}' is invalid when unloading '{contentWithMetaData}'.";
                    exceptionMsg += $"\n\tExpected MetaData Syntax: {ExpectedMetaDataSyntax}";
                    exceptionMsg += "\n\tExample: size:12";
                    throw new CachingMetaDataException(exceptionMsg);
                }
            }
            else
            {
                var exceptionMsg = "When unloading fonts, the name of or the full file path of the font";
                exceptionMsg += " must be supplied with valid metadata syntax.";
                exceptionMsg += $"\n\tExpected MetaData Syntax: {ExpectedMetaDataSyntax}";
                exceptionMsg += "\n\tExample: size:12";

                throw new CachingMetaDataException(exceptionMsg);
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing"><c>true</c> to dispose of managed resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                var fontAtlasTextureKeys = (from k in this.textureCache.CacheKeys
                                                where this.fontMetaDataParser.Parse(k).ContainsMetaData
                                                select k).ToArray();

                foreach (var key in fontAtlasTextureKeys)
                {
                    this.textureCache.Unload(key);
                }
            }

            this.isDisposed = true;
        }
    }
}

// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Factories;
    using Velaptor.Observables;
    using Velaptor.Observables.Core;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads font content for rendering text.
    /// </summary>
    public sealed class FontLoader : ILoader<IFont>
    {
        private const string ExpectedMetaDataSyntax = "size:<font-size>";
        private const string FontExtension = ".ttf";
        private readonly string defaultRegularFontName = $"TimesNewRoman-Regular{FontExtension}";
        private readonly string defaultBoldFontName = $"TimesNewRoman-Bold{FontExtension}";
        private readonly string defaultItalicFontName = $"TimesNewRoman-Italic{FontExtension}";
        private readonly string defaultBoldItalicFontName = $"TimesNewRoman-BoldItalic{FontExtension}";
        private readonly IFontAtlasService fontAtlasService;
        private readonly IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService;
        private readonly IPathResolver fontPathResolver;
        private readonly IDisposableItemCache<string, ITexture> textureCache;
        private readonly IFontFactory fontFactory;
        private readonly IFontMetaDataParser fontMetaDataParser;
        private readonly IDirectory directory;
        private readonly IFileStreamFactory fileStream;
        private readonly IFile file;
        private readonly IPath path;
        private readonly IDisposable shutDownObservableUnsubscriber;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public FontLoader()
        {
            this.fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
            this.embeddedFontResourceService = IoC.Container.GetInstance<IEmbeddedResourceLoaderService<Stream?>>();
            this.fontPathResolver = PathResolverFactory.CreateFontPathResolver();
            this.textureCache = IoC.Container.GetInstance<IDisposableItemCache<string, ITexture>>();
            this.fontFactory = IoC.Container.GetInstance<IFontFactory>();
            this.fontMetaDataParser = IoC.Container.GetInstance<IFontMetaDataParser>();
            this.directory = IoC.Container.GetInstance<IDirectory>();
            this.file = IoC.Container.GetInstance<IFile>();
            this.fileStream = IoC.Container.GetInstance<IFileStreamFactory>();
            this.path = IoC.Container.GetInstance<IPath>();

            var shutDownObservable = IoC.Container.GetInstance<ShutDownObservable>();
            this.shutDownObservableUnsubscriber = shutDownObservable.Subscribe(new Observer<bool>(_ => ShutDown()));

            SetupDefaultFonts();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FontLoader"/> class.
        /// </summary>
        /// <param name="fontAtlasService">Loads font files and builds atlas textures from them.</param>
        /// <param name="embeddedFontResourceService">Gives access to embedded font file resources.</param>
        /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
        /// <param name="textureCache">Caches textures for later use to improve performance.</param>
        /// <param name="fontFactory">Generates new <see cref="IFont"/> instances.</param>
        /// <param name="fontMetaDataParser">Parses metadata from strings.</param>
        /// <param name="directory">Manages and deals with directories.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="fileStream">Provides a stream to a file for file operations.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        /// <param name="shutDownObservable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        internal FontLoader(
            IFontAtlasService fontAtlasService,
            IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService,
            IPathResolver fontPathResolver,
            IDisposableItemCache<string, ITexture> textureCache,
            IFontFactory fontFactory,
            IFontMetaDataParser fontMetaDataParser,
            IDirectory directory,
            IFile file,
            IFileStreamFactory fileStream,
            IPath path,
            IObservable<bool> shutDownObservable)
        {
            this.fontAtlasService = fontAtlasService ?? throw new ArgumentNullException(nameof(fontAtlasService), "The parameter must not be null.");
            this.embeddedFontResourceService = embeddedFontResourceService ?? throw new ArgumentNullException(nameof(embeddedFontResourceService), "The parameter must not be null.");
            this.fontPathResolver = fontPathResolver ?? throw new ArgumentNullException(nameof(fontPathResolver), "The parameter must not be null.");
            this.textureCache = textureCache ?? throw new ArgumentNullException(nameof(textureCache), "The parameter must not be null.");
            this.fontFactory = fontFactory ?? throw new ArgumentNullException(nameof(fontFactory), "The parameter must not be null.");
            this.fontMetaDataParser = fontMetaDataParser ?? throw new ArgumentNullException(nameof(fontMetaDataParser), "The parameter must not be null.");
            this.directory = directory ?? throw new ArgumentNullException(nameof(directory), "The parameter must not be null.");
            this.file = file ?? throw new ArgumentNullException(nameof(file), "The parameter must not be null.");
            this.fileStream = fileStream ?? throw new ArgumentNullException(nameof(fileStream), "The parameter must not be null.");
            this.path = path ?? throw new ArgumentNullException(nameof(path), "The parameter must not be null.");

            if (shutDownObservable is null)
            {
                throw new ArgumentNullException(nameof(shutDownObservable), "The parameter must not be null.");
            }

            this.shutDownObservableUnsubscriber = shutDownObservable.Subscribe(new Observer<bool>(_ => ShutDown()));

            SetupDefaultFonts();
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
        ///     Occurs if the metadata is missing or invalid.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        ///     Occurs if the font file does not exist.
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
                    if (parseResult.MetaDataPrefix.HasValidFullFilePathSyntax() is false)
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

            fullFontFilePath = fullFontFilePath.HasValidFullFilePathSyntax()
                ? parseResult.MetaDataPrefix
                : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

            // If the full font file path is empty, then the font does not exist. Throw an exception
            if (this.file.Exists(fullFontFilePath) is false)
            {
                var exceptionMsg = $"The font content item '{fullFontFilePath}' does not exist.";

                throw new FileNotFoundException(exceptionMsg, fullFontFilePath);
            }

            var contentName = this.path.GetFileNameWithoutExtension(fullFontFilePath);

            var (_, glyphMetrics) = this.fontAtlasService.CreateFontAtlas(fullFontFilePath, parseResult.FontSize);

            var cacheKey = $"{fullFontFilePath}|{parseResult.MetaData}";
            var fontAtlasTexture = this.textureCache.GetItem(cacheKey);

            return this.fontFactory.Create(fontAtlasTexture, contentName, fullFontFilePath, parseResult.FontSize, glyphMetrics);
        }

        /// <inheritdoc/>
        public void Unload(string contentWithMetaData)
        {
            var parseResult = this.fontMetaDataParser.Parse(contentWithMetaData);

            if (parseResult.ContainsMetaData)
            {
                if (parseResult.IsValid)
                {
                    var fullFilePath = parseResult.MetaDataPrefix.HasValidFullFilePathSyntax()
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

        /// <summary>
        /// Shuts down the application by unloading all of the font atlas textures.
        /// </summary>
        private void ShutDown()
        {
            if (this.isDisposed)
            {
                return;
            }

            var fontAtlasTextureKeys = (from k in this.textureCache.CacheKeys
                                            where this.fontMetaDataParser.Parse(k).ContainsMetaData
                                            select k).ToArray();

            foreach (var key in fontAtlasTextureKeys)
            {
                this.textureCache.Unload(key);
            }

            this.shutDownObservableUnsubscriber.Dispose();

            this.isDisposed = true;
        }

        /// <summary>
        /// Checks for and sets up the default fonts.
        /// </summary>
        private void SetupDefaultFonts()
        {
            var rootDirPath = $"{this.fontPathResolver.RootDirectoryPath.TrimEnd('\\').TrimEnd('/')}{this.path.AltDirectorySeparatorChar}";
            var contentDirName = this.fontPathResolver.ContentDirectoryName;
            var fontContentDirPath = $"{rootDirPath}{contentDirName}{this.path.AltDirectorySeparatorChar}";

            var fontNames = new[]
            {
                this.defaultRegularFontName,
                this.defaultBoldFontName,
                this.defaultItalicFontName,
                this.defaultBoldItalicFontName,
            };

            // Create the content directory if it does not exist
            if (this.directory.Exists(rootDirPath) is false)
            {
                this.directory.CreateDirectory(rootDirPath);
            }

            // Create the font content directory if it does not exist
            if (this.directory.Exists(fontContentDirPath) is false)
            {
                this.directory.CreateDirectory(fontContentDirPath);
            }

            foreach (var fontName in fontNames)
            {
                var filePath = $"{fontContentDirPath}{fontName}";

                // If the regular font does not exist in the font content directory, extract it from the embedded resources
                if (this.file.Exists(filePath) is not false)
                {
                    continue;
                }

                using var fontFileStream = this.embeddedFontResourceService.LoadResource(fontName);
                using var copyToStream = this.fileStream.Create(filePath, FileMode.Create, FileAccess.Write);

                fontFileStream?.CopyTo(copyToStream);
            }
        }
    }
}

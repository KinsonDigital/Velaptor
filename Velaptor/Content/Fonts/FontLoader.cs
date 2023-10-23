// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Caching;
using Exceptions;
using Factories;
using Graphics;
using Guards;
using Velaptor.Factories;
using Velaptor.Services;

/// <summary>
/// Loads font content for rendering text.
/// </summary>
public sealed class FontLoader : ILoader<IFont>
{
    private const string ExpectedMetaDataSyntax = "size:<font-size>";
    private const string FontFileExtension = ".ttf";
    private const string DefaultRegularFontName = $"TimesNewRoman-Regular{FontFileExtension}";
    private const string DefaultBoldFontName = $"TimesNewRoman-Bold{FontFileExtension}";
    private const string DefaultItalicFontName = $"TimesNewRoman-Italic{FontFileExtension}";
    private const string DefaultBoldItalicFontName = $"TimesNewRoman-BoldItalic{FontFileExtension}";
    private const uint DefaultFontSize = 12;
    private readonly IFontAtlasService fontAtlasService;
    private readonly IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService;
    private readonly IContentPathResolver contentPathResolver;
    private readonly IContentPathResolver fontPathResolver;
    private readonly IItemCache<string, ITexture> textureCache;
    private readonly IFontFactory fontFactory;
    private readonly IFontMetaDataParser fontMetaDataParser;
    private readonly IDirectory directory;
    private readonly IFileStreamFactory fileStream;
    private readonly IFile file;
    private readonly IPath path;
    private readonly string[] defaultFontNames =
    {
        DefaultRegularFontName, DefaultBoldFontName,
        DefaultItalicFontName, DefaultBoldItalicFontName,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public FontLoader()
    {
        this.fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
        this.embeddedFontResourceService = IoC.Container.GetInstance<IEmbeddedResourceLoaderService<Stream?>>();
        this.contentPathResolver = PathResolverFactory.CreateContentFontPathResolver();
        this.fontPathResolver = PathResolverFactory.CreateFontPathResolver();
        this.textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        this.fontFactory = IoC.Container.GetInstance<IFontFactory>();
        this.fontMetaDataParser = IoC.Container.GetInstance<IFontMetaDataParser>();
        this.directory = IoC.Container.GetInstance<IDirectory>();
        this.file = IoC.Container.GetInstance<IFile>();
        this.fileStream = IoC.Container.GetInstance<IFileStreamFactory>();
        this.path = IoC.Container.GetInstance<IPath>();

        SetupDefaultFonts();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoader"/> class.
    /// </summary>
    /// <param name="fontAtlasService">Creates font atlas textures and glyph metric data.</param>
    /// <param name="embeddedFontResourceService">Gives access to embedded font file resources.</param>
    /// <param name="contentPathResolver">Resolves paths to the application's content directory.</param>
    /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
    /// <param name="textureCache">Caches textures for later use to improve performance.</param>
    /// <param name="fontFactory">Generates new <see cref="IFont"/> instances.</param>
    /// <param name="fontMetaDataParser">Parses metadata from strings.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="fileStream">Provides a stream to a file for file operations.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    internal FontLoader(
        IFontAtlasService fontAtlasService,
        IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService,
        IContentPathResolver contentPathResolver,
        IContentPathResolver fontPathResolver,
        IItemCache<string, ITexture> textureCache,
        IFontFactory fontFactory,
        IFontMetaDataParser fontMetaDataParser,
        IDirectory directory,
        IFile file,
        IFileStreamFactory fileStream,
        IPath path)
    {
        EnsureThat.ParamIsNotNull(fontAtlasService);
        EnsureThat.ParamIsNotNull(embeddedFontResourceService);
        EnsureThat.ParamIsNotNull(contentPathResolver);
        EnsureThat.ParamIsNotNull(fontPathResolver);
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.ParamIsNotNull(fontFactory);
        EnsureThat.ParamIsNotNull(fontMetaDataParser);
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(fileStream);
        EnsureThat.ParamIsNotNull(path);

        this.fontAtlasService = fontAtlasService;
        this.embeddedFontResourceService = embeddedFontResourceService;
        this.contentPathResolver = contentPathResolver;
        this.fontPathResolver = fontPathResolver;
        this.textureCache = textureCache;
        this.fontFactory = fontFactory;
        this.fontMetaDataParser = fontMetaDataParser;
        this.directory = directory;
        this.file = file;
        this.fileStream = fileStream;
        this.path = path;

        SetupDefaultFonts();
    }

    /// <summary>
    /// Loads font content from the application's content directory or directly using a full file path.
    /// </summary>
    /// <param name="contentPathOrName">The name or full file path to the font with metadata.</param>
    /// <returns>The loaded font.</returns>
    /// <exception cref="ArgumentNullException">
    ///     Occurs when the <paramref name="contentPathOrName"/> argument is null or empty.
    /// </exception>
    /// <exception cref="CachingMetaDataException">
    ///     Occurs if the metadata is invalid.
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Occurs if the font file does not exist.
    /// </exception>
    /// <remarks>
    ///     If a path is used, it must be a fully qualified file path.
    ///     <para>Directory paths are not valid.</para>
    ///     <para>If no metadata is provided, then a default font size of 12 will be used.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///         // Valid Example 1
    ///         ContentLoader.Load("my-font|size:12");
    ///         <br/>
    ///         // Valid Example 2
    ///         ContentLoader.Load("my-font");
    ///         <br/>
    ///         // Valid Example 3
    ///         ContentLoader.Load("my-font.ttf");
    ///         <br/>
    ///         // Valid Example 4
    ///         ContentLoader.Load(@"C:\fonts\my-font.ttf|size:12");
    ///         <br/>
    ///         // Invalid Example 1
    ///         ContentLoader.Load("my-font|size:12");
    ///         <br/>
    ///         // Invalid Example 2
    ///         ContentLoader.Load("my-font|size:12");
    ///         <br/>
    ///         // Invalid Example 3
    ///         ContentLoader.Load("my-font|size12");
    ///     </code>
    /// </example>
    public IFont Load(string contentPathOrName)
    {
        if (string.IsNullOrEmpty(contentPathOrName))
        {
            throw new ArgumentNullException(nameof(contentPathOrName), "The parameter must not be null.");
        }

        var parseResult = this.fontMetaDataParser.Parse(contentPathOrName);
        string fullFontFilePath;

        if (parseResult.ContainsMetaData)
        {
            if (parseResult.IsValid)
            {
                fullFontFilePath = parseResult.MetaDataPrefix;

                // If the file path is a full file path, leave it be.
                // If it is not, then it is a content name and could be a file name with an extension.
                // If this is the case, remove the extension
                if (!this.path.IsPathRooted(parseResult.MetaDataPrefix))
                {
                    var newMetaDataPrefix = this.path.GetFileNameWithoutExtension(parseResult.MetaDataPrefix);

                    parseResult = parseResult with { MetaDataPrefix = newMetaDataPrefix };
                }
            }
            else
            {
                var exceptionMsg = $"The metadata '{parseResult.MetaData}' is invalid when loading '{contentPathOrName}'.";
                exceptionMsg += $"{Environment.NewLine}\tExpected MetaData Syntax: {ExpectedMetaDataSyntax}";
                exceptionMsg += $"{Environment.NewLine}\tExample: size:12";
                throw new CachingMetaDataException(exceptionMsg);
            }
        }
        else
        {
            var defaultMetaDataPrefix = this.path.GetFileNameWithoutExtension(contentPathOrName);

            parseResult = new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = defaultMetaDataPrefix,
                MetaData = $"size:{DefaultFontSize}",
                FontSize = DefaultFontSize,
            };

            fullFontFilePath = defaultMetaDataPrefix;
        }

        fullFontFilePath = this.path.IsPathRooted(fullFontFilePath)
            ? parseResult.MetaDataPrefix
            : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

        // If the full font file path is empty, then the font does not exist. Throw an exception
        if (!this.file.Exists(fullFontFilePath))
        {
            var exceptionMsg = $"The font content item '{fullFontFilePath}' does not exist.";

            throw new FileNotFoundException(exceptionMsg, fullFontFilePath);
        }

        var contentName = this.path.GetFileNameWithoutExtension(fullFontFilePath);

        (_, GlyphMetrics[] glyphMetrics) = this.fontAtlasService.CreateAtlas(fullFontFilePath, parseResult.FontSize);

        var cacheKey = $"{fullFontFilePath}|{parseResult.MetaData}";
        var fileName = this.path.GetFileName(fullFontFilePath);
        var fontAtlasTexture = this.textureCache.GetItem(cacheKey);

        var isDefaultFont = this.defaultFontNames.Contains(fileName);

        return this.fontFactory.Create(
            fontAtlasTexture,
            contentName,
            fullFontFilePath,
            parseResult.FontSize,
            isDefaultFont,
            glyphMetrics);
    }

    /// <inheritdoc/>
    /// <exception cref="CachingMetaDataException">
    ///     Thrown when the metadata is invalid if metadata exists.
    /// </exception>
    /// <remarks>
    ///     If a path is used, it must be a fully qualified file path.
    ///     <para>Directory paths are not valid.</para>
    ///     <para>If no metadata is provided, then a default font size of 12 will be used.</para>
    /// </remarks>
    /// <example>
    ///     <code>
    ///         // Valid Example 1
    ///         ContentLoader.Unload("my-font|size:12");
    ///         <br/>
    ///         // Valid Example 2
    ///         ContentLoader.Unload("my-font");
    ///         <br/>
    ///         // Valid Example 3
    ///         ContentLoader.Unload("my-font.ttf");
    ///         <br/>
    ///         // Valid Example 4
    ///         ContentLoader.Unload(@"C:\fonts\my-font.ttf|size:12");
    ///         <br/>
    ///         // Invalid Example 1
    ///         ContentLoader.Unload("my-font|size:12");
    ///         <br/>
    ///         // Invalid Example 2
    ///         ContentLoader.Unload("my-font|size:12");
    ///         <br/>
    ///         // Invalid Example 3
    ///         ContentLoader.Unload("my-font|size12");
    ///     </code>
    /// </example>
    public void Unload(string contentPathOrName)
    {
        var parseResult = this.fontMetaDataParser.Parse(contentPathOrName);

        if (parseResult.ContainsMetaData)
        {
            if (parseResult.IsValid)
            {
                var fullFilePath = this.path.IsPathRooted(parseResult.MetaDataPrefix)
                    ? parseResult.MetaDataPrefix
                    : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

                var cacheKey = $"{fullFilePath}|{parseResult.MetaData}";

                this.textureCache.Unload(cacheKey);
            }
            else
            {
                var exceptionMsg = $"The metadata '{parseResult.MetaData}' is invalid when unloading '{contentPathOrName}'.";
                exceptionMsg += $"{Environment.NewLine}\tExpected MetaData Syntax: {ExpectedMetaDataSyntax}";
                exceptionMsg += $"{Environment.NewLine}\tExample: size:12";
                throw new CachingMetaDataException(exceptionMsg);
            }
        }
        else
        {
            var metaDataPrefix = this.path.GetFileNameWithoutExtension(contentPathOrName);

            parseResult = new FontMetaDataParseResult
            {
                ContainsMetaData = true,
                IsValid = true,
                MetaDataPrefix = metaDataPrefix,
                MetaData = $"size:{DefaultFontSize}",
                FontSize = DefaultFontSize,
            };

            var fullFilePath = this.path.IsPathRooted(metaDataPrefix)
                ? parseResult.MetaDataPrefix
                : this.fontPathResolver.ResolveFilePath(parseResult.MetaDataPrefix);

            var fileName = this.path.GetFileName(fullFilePath);
            var isDefaultFont = this.defaultFontNames.Contains(fileName);

            var cacheKey = isDefaultFont
                ? $"[DEFAULT]{fullFilePath}|{parseResult.MetaData}"
                : $"{fullFilePath}|{parseResult.MetaData}";

            this.textureCache.Unload(cacheKey);
        }
    }

    /// <summary>
    /// Checks for and sets up the default fonts.
    /// </summary>
    private void SetupDefaultFonts()
    {
        var separator = this.path.AltDirectorySeparatorChar;
        var contentDirPath = this.contentPathResolver.RootDirectoryPath;
        var contentDirName = this.fontPathResolver.ContentDirectoryName;
        var fontContentDirPath = $"{contentDirPath}{separator}{contentDirName}";

        // Create the font content directory if it does not exist
        if (!this.directory.Exists(fontContentDirPath))
        {
            this.directory.CreateDirectory(fontContentDirPath);
        }

        foreach (var fontName in this.defaultFontNames)
        {
            var filePath = $"{fontContentDirPath}{separator}{fontName}";

            // If the regular font does not exist in the font content directory, extract it from the embedded resources
            if (this.file.Exists(filePath))
            {
                continue;
            }

            using var fontFileStream = this.embeddedFontResourceService.LoadResource(fontName);
            using var copyToStream = this.fileStream.New(filePath, FileMode.Create, FileAccess.Write);

            fontFileStream?.CopyTo(copyToStream);
        }
    }
}

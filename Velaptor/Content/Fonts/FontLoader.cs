// <copyright file="FontLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using Carbonate;
using Carbonate.OneWay;
using Exceptions;
using ExtensionMethods;
using Factories;
using Graphics;
using Velaptor.Factories;
using Velaptor.Services;
using ReactableData;

/// <summary>
/// Loads font content for rendering text.
/// </summary>
internal sealed class FontLoader : IFontLoader
{
    private const string FontFileExtension = ".ttf";
    private const string DefaultRegularFontName = $"TimesNewRoman-Regular{FontFileExtension}";
    private const string DefaultBoldFontName = $"TimesNewRoman-Bold{FontFileExtension}";
    private const string DefaultItalicFontName = $"TimesNewRoman-Italic{FontFileExtension}";
    private const string DefaultBoldItalicFontName = $"TimesNewRoman-BoldItalic{FontFileExtension}";
    private const string DefaultFontPrefix = "[DEFAULT]";
    private readonly ConcurrentDictionary<string, FontCacheEntry> fontCache = new ();
    private readonly IPushReactable<DisposeTextureData> disposeReactable;
    private readonly IFontAtlasService fontAtlasService;
    private readonly IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService;
    private readonly IContentPathResolver fontPathResolver;
    private readonly ITextureFactory textureFactory;
    private readonly IFontFactory fontFactory;
    private readonly IDirectory directory;
    private readonly IFileStreamFactory fileStreamFactory;
    private readonly IFile file;
    private readonly IPath path;
    private readonly IDisposable unsubscriber;
    private readonly string[] defaultFontNames =
    [
        DefaultRegularFontName, DefaultBoldFontName,
        DefaultItalicFontName, DefaultBoldItalicFontName
    ];
    private int isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontLoader"/> class.
    /// </summary>
    /// <param name="fontPathResolver">Resolves paths to JSON font data files.</param>
    /// <param name="textureFactory">Creates textures.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="fileStreamFactory">Provides a stream to a file for file operations.</param>
    /// <param name="fontFactory">Creates font objects.</param>
    /// <param name="embeddedFontResourceService">Gives access to embedded font file resources.</param>
    /// <param name="fontAtlasService">Creates font atlas textures and glyph metric data.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public FontLoader(
        IContentPathResolver fontPathResolver,
        ITextureFactory textureFactory,
        IReactableFactory reactableFactory,
        IFileStreamFactory fileStreamFactory,
        IFontFactory fontFactory,
        IEmbeddedResourceLoaderService<Stream?> embeddedFontResourceService,
        IFontAtlasService fontAtlasService,
        IDirectory directory,
        IFile file,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(fontPathResolver);
        ArgumentNullException.ThrowIfNull(textureFactory);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(fileStreamFactory);
        ArgumentNullException.ThrowIfNull(fontFactory);
        ArgumentNullException.ThrowIfNull(embeddedFontResourceService);
        ArgumentNullException.ThrowIfNull(fontAtlasService);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);

        this.fontPathResolver = fontPathResolver;
        this.textureFactory = textureFactory;
        this.fileStreamFactory = fileStreamFactory;
        this.fontFactory = fontFactory;
        this.embeddedFontResourceService = embeddedFontResourceService;
        this.fontAtlasService = fontAtlasService;
        this.directory = directory;
        this.file = file;
        this.path = path;

        this.disposeReactable = reactableFactory.CreateDisposeTextureReactable();
        var shutDownReactable = reactableFactory.CreateNoDataPushReactable();

        this.unsubscriber = shutDownReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.unsubscriber?.Dispose());

        SetupDefaultFonts();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="FontLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~FontLoader()
    {
#if DEBUG
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }
#endif

        ShutDown();
    }

    /// <inheritdoc cref="IFontLoader.TotalCachedItems"/>
    public int TotalCachedItems => this.fontCache.Count;

    /// <inheritdoc cref="IFontLoader.Load"/>
    /// <exception cref="ArgumentException">
    ///     Occurs when the <paramref name="pathOrName"/> argument is null or empty.
    /// </exception>
    /// <exception cref="FontException">
    ///     Occurs if something has gone wrong creating the font object.
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
    ///         // Valid Example 1
    ///         ContentLoader.Load("my-font", 12);
    ///         <br/>
    ///         // Valid Example 2
    ///         ContentLoader.Load(@"C:\fonts\my-font.ttf", 14);
    ///     </code>
    /// </example>
    public IFont Load(string pathOrName, uint size)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathOrName);
        pathOrName = pathOrName.NormalizePath();

        var fullFontFilePath = this.path.IsPathRooted(pathOrName)
            ? pathOrName
            : this.fontPathResolver.ResolveFilePath(pathOrName);

        // If the full font file path is empty, then the font does not exist. Throw an exception
        if (!this.file.Exists(fullFontFilePath))
        {
            var exceptionMsg = $"The font content item '{fullFontFilePath}' does not exist.";

            throw new FileNotFoundException(exceptionMsg, fullFontFilePath);
        }

        var contentName = this.path.GetFileNameWithoutExtension(fullFontFilePath);

        var fileName = this.path.GetFileName(fullFontFilePath);
        var isDefaultFont = this.defaultFontNames.Contains(fileName);
        var cacheKeyPrefix = isDefaultFont ? DefaultFontPrefix : string.Empty;
        var cacheKey = $"{cacheKeyPrefix}{fullFontFilePath}|{size}";

        var entry = this.fontCache.GetOrAdd(cacheKey, _ =>
        {
            (ImageData imageData, GlyphMetrics[] glyphMetrics) = this.fontAtlasService.CreateAtlas(fullFontFilePath, size);
            imageData.FlipVertically();
            var loadedTexture = this.textureFactory.Create(contentName, fullFontFilePath, imageData);

            return new FontCacheEntry { FontTextureAtlas = loadedTexture, Metrics = glyphMetrics };
        });

        Interlocked.Increment(ref entry.RefCount);

        return this.fontFactory.Create(entry.FontTextureAtlas, contentName, fullFontFilePath, size, isDefaultFont, entry.Metrics);
    }

    /// <inheritdoc cref="IUnloader{T}.Unload"/>
    public void Unload(IFont font)
    {
        var fileName = this.path.GetFileName(font.FilePath);
        var isDefaultFont = this.defaultFontNames.Contains(fileName);
        var cacheKey = BuildCacheKey(font.FilePath, font.Size, isDefaultFont);

        if (!this.fontCache.TryGetValue(cacheKey, out var entry))
        {
            return;
        }

        if (Interlocked.Decrement(ref entry.RefCount) > 0)
        {
            return;
        }

        this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = font.Atlas.Id });

        this.fontCache.TryRemove(cacheKey, out _);
    }

    /// <summary>
    /// Builds the cache key used to store/retrieve font data in/from the cache.
    /// </summary>
    /// <param name="filePath">The full path to the font content file.</param>
    /// <param name="size">The size of the font.</param>
    /// <param name="isDefaultFont">True if the font is a default font.</param>
    /// <returns>The cache key.</returns>
    private static string BuildCacheKey(string filePath, uint size, bool isDefaultFont)
    {
        var cacheKeyPrefix = isDefaultFont ? DefaultFontPrefix : string.Empty;

        return $"{cacheKeyPrefix}{filePath}|{size}";
    }

    /// <summary>
    /// Checks for and sets up the default fonts.
    /// </summary>
    private void SetupDefaultFonts()
    {
        var separator = this.path.AltDirectorySeparatorChar;
        var contentDirPath = this.fontPathResolver.RootDirectoryPath;
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
            using var copyToStream = this.fileStreamFactory.New(filePath, FileMode.Create, FileAccess.Write);

            fontFileStream?.CopyTo(copyToStream);
        }
    }

    /// <summary>
    /// Disposes of all resources.
    /// </summary>
    private void ShutDown()
    {
        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        foreach (var entry in this.fontCache.Values)
        {
            this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = entry.FontTextureAtlas.Id });
        }

        this.fontCache.Clear();
    }
}

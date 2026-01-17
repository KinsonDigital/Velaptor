// <copyright file="AtlasLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Carbonate;
using Carbonate.OneWay;
using Exceptions;
using Factories;
using Graphics;
using Services;
using Velaptor.Factories;
using ReactableData;

/// <inheritdoc cref="IAtlasLoader"/>
internal sealed class AtlasLoader : IAtlasLoader
{
    private const string TextureExtension = ".png";
    private const string AtlasDataExtension = ".json";
    private readonly ConcurrentDictionary<string, (ITexture atlasTexture, AtlasSubTextureData[] subTextureData)> atlasCache = new ();
    private readonly IPushReactable<DisposeTextureData> disposeReactable;
    private readonly IDisposable unsubscriber;
    private readonly ITextureFactory textureFactory;
    private readonly IAtlasDataFactory atlasDataFactory;
    private readonly IContentPathResolver atlasDataPathResolver;
    private readonly IImageService imageService;
    private readonly IJsonService jsonService;
    private readonly IDirectory directory;
    private readonly IFile file;
    private readonly IPath path;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasLoader"/> class.
    /// </summary>
    /// <param name="textureFactory">Creates textures.</param>
    /// <param name="atlasDataFactory">Generates <see cref="IAtlasData"/> instances.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="atlasDataPathResolver">Resolves paths to JSON atlas data files.</param>
    /// <param name="imageService">Provides image related services.</param>
    /// <param name="jsonService">Provides JSON related services.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public AtlasLoader(
        ITextureFactory textureFactory,
        IAtlasDataFactory atlasDataFactory,
        IReactableFactory reactableFactory,
        IContentPathResolver atlasDataPathResolver,
        IImageService imageService,
        IJsonService jsonService,
        IDirectory directory,
        IFile file,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(textureFactory);
        ArgumentNullException.ThrowIfNull(atlasDataFactory);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(atlasDataPathResolver);
        ArgumentNullException.ThrowIfNull(imageService);
        ArgumentNullException.ThrowIfNull(jsonService);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);

        this.textureFactory = textureFactory;
        this.atlasDataFactory = atlasDataFactory;
        this.atlasDataPathResolver = atlasDataPathResolver;
        this.imageService = imageService;
        this.jsonService = jsonService;
        this.directory = directory;
        this.file = file;
        this.path = path;

        this.disposeReactable = reactableFactory.CreateDisposeTextureReactable();
        var shutDownReactable = reactableFactory.CreateNoDataPushReactable();

        this.unsubscriber = shutDownReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.unsubscriber?.Dispose());
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AtlasLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~AtlasLoader()
    {
#if DEBUG
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }
#endif

        ShutDown();
    }

    /// <inheritdoc cref="IAtlasLoader.TotalCachedItems"/>
    public int TotalCachedItems => this.atlasCache.Count;

    /// <inheritdoc cref="IAtlasLoader.Load"/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="atlasPathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadAtlasException">Thrown if the .</exception>
    /// <exception cref="LoadContentException">Thrown if an issue occurs with loading the atlas JSON data.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the atlas data and/or image files are not found.</exception>
    /// <remarks>
    /// Valid Values:
    /// <list type="bullet">
    ///     <item>MyAtlas</item>
    ///     <item>C:/Atlas/MyAtlas.png</item>
    ///     <item>C:/Atlas/MyAtlas.json</item>
    /// </list>
    ///
    /// Invalid Values:
    /// <list type="bullet">
    ///     <item>C:/Atlas/MyAtlas</item>
    ///     <item>C:/Atlas/MyAtlas.txt</item>
    /// </list>
    /// </remarks>
    public IAtlasData Load(string atlasPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(atlasPathOrName);

        var isPathRooted = this.path.IsPathRooted(atlasPathOrName);
        var contentDirPath = isPathRooted
            ? this.path.GetDirectoryName(atlasPathOrName) ?? string.Empty
            : this.atlasDataPathResolver.ResolveDirPath();

        if (!isPathRooted && !this.directory.Exists(contentDirPath))
        {
            this.directory.CreateDirectory(contentDirPath);
        }

        var name = this.path.GetFileNameWithoutExtension(atlasPathOrName);

        if (isPathRooted)
        {
            var validExtensions = new[] { TextureExtension, AtlasDataExtension };
            var extension = this.path.GetExtension(atlasPathOrName);

            if (validExtensions.All(e => e != extension))
            {
                var exceptionMsg = "When loading atlas data with fully qualified paths,";
                exceptionMsg += $" the files must be a '{TextureExtension}' or '{AtlasDataExtension}' extension.";

                throw new LoadAtlasException(exceptionMsg);
            }
        }

        var atlasDataFilePath = this.path.Combine(contentDirPath, name + AtlasDataExtension);

        if (!this.file.Exists(atlasDataFilePath))
        {
            var exceptionMsg = $"The atlas data directory '{contentDirPath}' does not contain the";
            exceptionMsg += $" required '{atlasDataFilePath}' atlas data file.";

            throw new FileNotFoundException(exceptionMsg, atlasDataFilePath);
        }

        var atlasImageFilePath = this.path.Combine(contentDirPath, name + TextureExtension);

        if (!this.file.Exists(atlasImageFilePath))
        {
            var exceptionMsg = $"The atlas data directory '{contentDirPath}' does not contain the";
            exceptionMsg += $" required '{atlasImageFilePath}' atlas image file.";

            throw new FileNotFoundException(exceptionMsg, atlasImageFilePath);
        }

        var atlasName = isPathRooted
            ? name
            : atlasPathOrName;

        (ITexture atlasTexture, AtlasSubTextureData[] subTextureData) = this.atlasCache.GetOrAdd(atlasImageFilePath, (_) =>
        {
            var rawData = this.file.ReadAllText(atlasDataFilePath);
            var subTextureData = this.jsonService.Deserialize<AtlasSubTextureData[]>(rawData)
                ?? throw new LoadContentException($"There was an issue deserializing the JSON atlas data file at '{atlasDataFilePath}'.");
            var atlasImageData = this.imageService.Load(atlasImageFilePath);
            var atlasTexture = this.textureFactory.Create(atlasName, atlasImageFilePath, atlasImageData);

            return (atlasTexture, subTextureData);
        });

        return this.atlasDataFactory.Create(atlasTexture, subTextureData, contentDirPath, atlasName);
    }

    /// <inheritdoc/>
    public void Unload(IAtlasData atlasData)
    {
        this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = atlasData.Texture.Id });
        var cacheKey = atlasData.FilePath;
        this.atlasCache.TryRemove(cacheKey, out _);
    }

    /// <summary>
    /// Disposes of all resources.
    /// </summary>
    private void ShutDown()
    {
        if (this.isDisposed)
        {
            return;
        }

        foreach (var atlasDataItem in this.atlasCache)
        {
            (ITexture texture, _) = atlasDataItem.Value;

            this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = texture.Id });
        }

        this.atlasCache.Clear();
        this.isDisposed = true;
    }
}

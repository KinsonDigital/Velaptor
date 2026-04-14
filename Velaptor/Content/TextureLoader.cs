// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Threading;
using Carbonate;
using Carbonate.OneWay;
using Factories;
using Velaptor.Factories;
using ReactableData;
using Services;

/// <summary>
/// Loads textures.
/// </summary>
internal sealed class TextureLoader : ITextureLoader
{
    private readonly IImageService imageService;
    private readonly ConcurrentDictionary<string, ITexture> textureCache = new ();
    private readonly IPushReactable<DisposeTextureData> disposeReactable;
    private readonly IDisposable unsubscriber;
    private readonly ITextureFactory textureFactory;
    private readonly IContentPathResolver texturePathResolver;
    private readonly IPath path;
    private readonly IDirectory directory;
    private int isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoader"/> class.
    /// </summary>
    /// <param name="textureFactory">Creates textures.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="imageService">Provides image-related services.</param>
    /// <param name="texturePathResolver">Resolves paths to texture content.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public TextureLoader(
        ITextureFactory textureFactory,
        IReactableFactory reactableFactory,
        IImageService imageService,
        IContentPathResolver texturePathResolver,
        IDirectory directory,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(textureFactory);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(imageService);
        ArgumentNullException.ThrowIfNull(texturePathResolver);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(path);

        this.imageService = imageService;
        this.textureFactory = textureFactory;
        this.texturePathResolver = texturePathResolver;
        this.path = path;
        this.directory = directory;

        this.disposeReactable = reactableFactory.CreateDisposeTextureReactable();
        var shutDownReactable = reactableFactory.CreateNoDataPushReactable();

        this.unsubscriber = shutDownReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.unsubscriber?.Dispose());
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TextureLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~TextureLoader()
    {
#if DEBUG
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }
#endif

        ShutDown();
    }

    /// <inheritdoc cref="ITextureLoader.TotalCachedItems"/>
    public int TotalCachedItems => this.textureCache.Count;

    /// <inheritdoc cref="ITextureLoader.Load"/>
    /// <exception cref="ArgumentException">Thrown if the <paramref name="pathOrName"/> is null or empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    public ITexture Load(string pathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathOrName);

        var contentDirPath = this.texturePathResolver.ResolveDirPath();

        if (!this.directory.Exists(contentDirPath))
        {
            this.directory.CreateDirectory(contentDirPath);
        }

        var textureFilePath = this.texturePathResolver.ResolveFilePath(pathOrName);

        return this.textureCache.GetOrAdd(textureFilePath, (filePath) =>
        {
            var imageData = this.imageService.Load(filePath);
            var name = this.path.GetFileNameWithoutExtension(textureFilePath);

            return this.textureFactory.Create(name, filePath, imageData);
        });
    }

    /// <inheritdoc cref="IUnloader{T}.Unload"/>
    public void Unload(ITexture texture)
    {
        this.textureCache.TryRemove(texture.FilePath, out _);
        this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = texture.Id });
    }

    /// <summary>
    /// Disposes of resources.
    /// </summary>
    private void ShutDown()
    {
        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        foreach (var textureDataItem in this.textureCache)
        {
            this.disposeReactable.Push(PushNotifications.TextureDisposedId, new DisposeTextureData { TextureId = textureDataItem.Value.Id });
        }

        this.textureCache.Clear();
    }
}

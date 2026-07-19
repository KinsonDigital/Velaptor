// <copyright file="AudioLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using System.Threading;
using Carbonate;
using Carbonate.OneWay;
using Exceptions;
using ExtensionMethods;
using Factories;
using ReactableData;
using Velaptor.Factories;

/// <summary>
/// Loads audio content.
/// </summary>
internal sealed class AudioLoader : IAudioLoader
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private readonly Dictionary<string, IAudio> audioCache = new ();
    private readonly IPushReactable<DisposeAudioData> disposeReactable;
    private readonly IDisposable unsubscriber;
    private readonly IAudioFactory audioFactory;
    private readonly IContentPathResolver audioPathResolver;
    private readonly IDirectory directory;
    private readonly IFile file;
    private readonly IPath path;
    private int isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLoader"/> class.
    /// </summary>
    /// <param name="audioFactory">Creates audio objects.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <param name="audioPathResolver">Resolves the path to the audio content.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public AudioLoader(
        IAudioFactory audioFactory,
        IReactableFactory reactableFactory,
        IContentPathResolver audioPathResolver,
        IDirectory directory,
        IFile file,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(audioFactory);
        ArgumentNullException.ThrowIfNull(reactableFactory);
        ArgumentNullException.ThrowIfNull(audioPathResolver);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);

        this.audioFactory = audioFactory;
        this.audioPathResolver = audioPathResolver;
        this.directory = directory;
        this.file = file;
        this.path = path;

        this.disposeReactable = reactableFactory.CreateDisposeAudioReactable();
        var shutDownReactable = reactableFactory.CreateNoDataPushReactable();

        this.unsubscriber = shutDownReactable.CreateNonReceiveOrRespond(
            PushNotifications.SystemShuttingDownId,
            ShutDown,
            () => this.unsubscriber?.Dispose());
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="AudioLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage]
    ~AudioLoader()
    {
#if DEBUG
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }
#endif

        ShutDown();
    }

    /// <inheritdoc cref="IAudioLoader.TotalCachedItems"/>
    public int TotalCachedItems => this.audioCache.Count;

    /// <inheritdoc cref="IAudioLoader.Load"/>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="pathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadAudioException">Thrown if the resulting audio content file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="NotSupportedException">The path contains a colon character <c>:</c> that is not part of a drive label.</exception>
    public IAudio Load(string pathOrName, AudioBuffer bufferType)
    {
        ArgumentException.ThrowIfNullOrEmpty(pathOrName);
        pathOrName = pathOrName.NormalizePath();

        var isPathRooted = this.path.IsPathRooted(pathOrName);

        if (!isPathRooted)
        {
            var contentDirPath = this.audioPathResolver.ResolveDirPath();

            if (!this.directory.Exists(contentDirPath))
            {
                this.directory.CreateDirectory(contentDirPath);
            }
        }

        var filePath = isPathRooted
            ? pathOrName
            : this.audioPathResolver.ResolveFilePath(pathOrName);

        if (!this.file.Exists(filePath))
        {
            throw new FileNotFoundException("The audio file does not exist.", filePath);
        }

        var fileExtension = this.path.GetExtension(filePath);
        var validExtensions = new[] { OggFileExtension, Mp3FileExtension };
        var isInvalidExtension = Array.TrueForAll(validExtensions, e => e != fileExtension);

        if (isInvalidExtension)
        {
            var exceptionMsg = $"The file '{filePath}' must be an audio file with";
            exceptionMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

            throw new LoadAudioException(exceptionMsg);
        }

        var cacheKey = BuildCacheKey(filePath, bufferType);

        ref var cacheItem = ref CollectionsMarshal.GetValueRefOrAddDefault(this.audioCache, cacheKey, out var exists);
        if (!exists || cacheItem is null)
        {
            cacheItem = this.audioFactory.Create(filePath, bufferType);
        }

        return cacheItem!;
    }

    /// <inheritdoc cref="IUnloader{T}.Unload"/>
    public void Unload(IAudio audio)
    {
        var cacheKey = BuildCacheKey(audio.FilePath, audio.BufferType);
        this.disposeReactable.Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = audio.Id });
        this.audioCache.Remove(cacheKey);
    }

    /// <summary>
    /// Builds the cache key used to store/retrieve font data in/from the cache.
    /// </summary>
    /// <param name="filePath">The full path to the font content file.</param>
    /// <param name="bufferType">The buffer mode.</param>
    /// <returns>The cache key.</returns>
    private static string BuildCacheKey(string filePath, AudioBuffer bufferType) => $"{filePath}|{bufferType}";

    /// <summary>
    /// Disposes of all resources.
    /// </summary>
    private void ShutDown()
    {
        if (Interlocked.Exchange(ref this.isDisposed, 1) != 0)
        {
            return;
        }

        foreach (var audioCacheItem in this.audioCache)
        {
            this.disposeReactable.Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = audioCacheItem.Value.Id });
        }

        this.audioCache.Clear();
    }
}

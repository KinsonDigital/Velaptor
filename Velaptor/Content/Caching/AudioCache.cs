// <copyright file="AudioCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Carbonate.OneWay;
using Exceptions;
using Factories;
using ReactableData;
using Velaptor.Factories;

/// <summary>
/// Caches <see cref="IAudio"/> objects for retrieval at a later time.
/// </summary>
internal sealed class AudioCache : IItemCache<string, IAudio>
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private readonly ConcurrentDictionary<string, IAudio> allAudio = new ();
    private readonly IAudioFactory audioFactory;
    private readonly IFile file;
    private readonly IPath path;
    private readonly IPushReactable<DisposeAudioData> disposeReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioCache"/> class.
    /// </summary>
    /// <param name="audioFactory">Creates <see cref="IAudio"/> objects.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    public AudioCache(
        IAudioFactory audioFactory,
        IFile file,
        IPath path,
        IReactableFactory reactableFactory)
    {
        ArgumentNullException.ThrowIfNull(audioFactory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(reactableFactory);

        this.audioFactory = audioFactory;
        this.file = file;
        this.path = path;

        this.disposeReactable = reactableFactory.CreateDisposeAudioReactable();
    }

    /// <inheritdoc/>
    public int TotalCachedItems => this.allAudio.Count;

    /// <inheritdoc/>
    public IReadOnlyCollection<string> CacheKeys => this.allAudio.Keys.ToArray().AsReadOnly();

    /// <summary>
    /// Gets the audio using the given <paramref name="audioFilePath"/>.
    /// </summary>
    /// <param name="audioFilePath">
    ///     The full file path to a <c>Sound</c> file.
    /// </param>
    /// <returns>Audio loaded from either an <c>'.ogg'</c> or <c>'.mp3'</c> file.</returns>
    /// <remarks>
    /// <para>
    ///     If the item has not been previously created, the <see cref="TextureCache"/> class
    ///     will retrieve it, and then cache it for fast retrieval.
    /// </para>
    /// <para>
    ///     The only audio files supported are <c>'.ogg'</c> and <c>'.mp3'</c> files.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="audioFilePath"/> is null or empty.
    /// </exception>
    /// <exception cref="LoadAudioException">
    ///     Thrown if the type of audio file being requested is not any of the supported types below:
    /// <list type="bullet">
    ///     <item><c>'.ogg'</c></item>
    ///     <item><c>'.mp3'</c></item>
    /// </list>
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the file at the given <paramref name="audioFilePath"/> is not found.
    /// </exception>
    public IAudio GetItem(string audioFilePath)
    {
        const char metaDataSignifier = '|';

        ArgumentException.ThrowIfNullOrEmpty(audioFilePath);

        if (audioFilePath.DoesNotContain(metaDataSignifier))
        {
            throw new LoadAudioException("The audio file path must contain metadata.");
        }

        var wholeSections = audioFilePath.Split(metaDataSignifier);
        var contentFilePath = wholeSections[0];
        var bufferTypeStr = wholeSections[1];

        var parseSuccess = Enum.TryParse(bufferTypeStr, out AudioBuffer bufferType);

        if (!parseSuccess)
        {
            throw new LoadAudioException("The audio buffer type could not be determined");
        }

        var extension = this.path.GetExtension(contentFilePath);

        if (extension != OggFileExtension && extension != Mp3FileExtension)
        {
            var exceptionMsg = $"Audio file type '{extension}' is not supported.";
            exceptionMsg += $"{Environment.NewLine}Supported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

            throw new LoadAudioException(exceptionMsg);
        }

        if (this.file.Exists(contentFilePath))
        {
            return this.allAudio.GetOrAdd(contentFilePath, filePath => this.audioFactory.Create(filePath, bufferType));
        }

        throw new FileNotFoundException($"The '{extension}' audio file does not exist.", contentFilePath);
    }

    /// <inheritdoc/>
    public void Unload(string cacheKey)
    {
        this.allAudio.TryRemove(cacheKey, out var audio);

        if (audio is null)
        {
            return;
        }

        this.disposeReactable.Push(PushNotifications.AudioDisposedId, new DisposeAudioData { AudioId = audio.Id });
    }
}

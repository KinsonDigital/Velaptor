// <copyright file="SoundCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching;

using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Carbonate;
using Exceptions;
using Factories;
using Guards;
using Reactables.ReactableData;

/// <summary>
/// Caches <see cref="ISound"/> objects for retrieval at a later time.
/// </summary>
internal sealed class SoundCache : IItemCache<string, ISound>
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private readonly ConcurrentDictionary<string, ISound> sounds = new ();
    private readonly ISoundFactory soundFactory;
    private readonly IFile file;
    private readonly IPath path;
    private readonly IReactable reactable;
    private readonly IDisposable shutDownUnsubscriber;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundCache"/> class.
    /// </summary>
    /// <param name="soundFactory">Creates <see cref="ISound"/> objects.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Provides path related services.</param>
    /// <param name="reactable">Sends and receives push notifications.</param>
    public SoundCache(
        ISoundFactory soundFactory,
        IFile file,
        IPath path,
        IReactable reactable)
    {
        EnsureThat.ParamIsNotNull(soundFactory);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(path);
        EnsureThat.ParamIsNotNull(reactable);

        this.soundFactory = soundFactory;
        this.file = file;
        this.path = path;
        this.reactable = reactable;

        this.shutDownUnsubscriber = this.reactable.Subscribe(new Reactor(
            eventId: NotificationIds.ShutDownId,
            onNext: ShutDown));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="SoundCache"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = "De-constructors cannot be unit tested.")]
    ~SoundCache()
    {
        if (UnitTestDetector.IsRunningFromUnitTest)
        {
            return;
        }

        ShutDown();
    }

    /// <inheritdoc/>
    public int TotalCachedItems => this.sounds.Count;

    /// <inheritdoc/>
    public ReadOnlyCollection<string> CacheKeys => new (this.sounds.Keys.ToArray());

    /// <summary>
    /// Gets a sound using the given <paramref name="soundFilePath"/>.
    /// </summary>
    /// <param name="soundFilePath">
    ///     The full file path to a <c>Sound</c> file.
    /// </param>
    /// <returns>A sound loaded from either an <c>'.ogg'</c> or <c>'.mp3'</c> file.</returns>
    /// <remarks>
    /// <para>
    ///     If the item has not been previously created, the <see cref="TextureCache"/> class
    ///     will retrieve it, and then cache it for fast retrieval.
    /// </para>
    /// <para>
    ///     The only sound files supported are <c>'.ogg'</c> and <c>'.mp3'</c> files.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if the <paramref name="soundFilePath"/> is null or empty.
    /// </exception>
    /// <exception cref="LoadSoundException">
    ///     Thrown if the type of sound file being requested is not any of the supported types below:
    /// <list type="bullet">
    ///     <item><c>'.ogg'</c></item>
    ///     <item><c>'.mp3'</c></item>
    /// </list>
    /// </exception>
    /// <exception cref="FileNotFoundException">
    ///     Thrown if the file at the given <paramref name="soundFilePath"/> is not found.
    /// </exception>
    public ISound GetItem(string soundFilePath)
    {
        if (string.IsNullOrEmpty(soundFilePath))
        {
            throw new ArgumentNullException(nameof(soundFilePath), "The string parameter must not be null or empty.");
        }

        var extension = this.path.GetExtension(soundFilePath);

        if (extension != OggFileExtension && extension != Mp3FileExtension)
        {
            var exceptionMsg = $"Sound file type '{extension}' is not supported.";
            exceptionMsg += $"{Environment.NewLine}Supported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

            throw new LoadSoundException(exceptionMsg);
        }

        var cacheKey = soundFilePath;

        if (this.file.Exists(soundFilePath))
        {
            return this.sounds.GetOrAdd(cacheKey, filePath => this.soundFactory.Create(filePath));
        }

        throw new FileNotFoundException($"The '{extension}' sound file does not exist.", soundFilePath);
    }

    /// <inheritdoc/>
    public void Unload(string cacheKey)
    {
        this.sounds.TryRemove(cacheKey, out var sound);

        if (sound is not null)
        {
            this.reactable.PushData(new DisposeSoundData { SoundId = sound.Id }, NotificationIds.DisposeSoundId);
        }
    }

    /// <summary>
    /// Disposes of all sounds.
    /// </summary>
    private void ShutDown()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.shutDownUnsubscriber.Dispose();
        this.reactable.Unsubscribe(NotificationIds.DisposeSoundId);

        this.sounds.Clear();
        this.isDisposed = true;
    }
}

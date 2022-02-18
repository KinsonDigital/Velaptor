// <copyright file="SoundCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Guards;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Caches <see cref="ISound"/> objects for retrieval at a later time.
    /// </summary>
    internal class SoundCache : IItemCache<string, ISound>
    {
        private const string OggFileExtension = ".ogg";
        private const string Mp3FileExtension = ".mp3";
        private const string NullParamExceptionMsg = "The parameter must not be null.";
        private readonly ConcurrentDictionary<string, ISound> sounds = new ();
        private readonly ISoundFactory soundFactory;
        private readonly IFile file;
        private readonly IPath path;
        private readonly IDisposable shutDownUnsubscriber;
        private readonly IReactable<DisposeSoundData> disposeSoundsReactable;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundCache"/> class.
        /// </summary>
        /// <param name="soundFactory">Creates <see cref="ISound"/> objects.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="path">Provides path related services.</param>
        /// <param name="disposeSoundsReactable">Sends push notifications to dispose of sounds.</param>
        /// <param name="shutDownReactable">Sends a push notifications that the application is shutting down.</param>
        public SoundCache(
            ISoundFactory soundFactory,
            IFile file,
            IPath path,
            IReactable<DisposeSoundData> disposeSoundsReactable,
            IReactable<ShutDownData> shutDownReactable)
        {
            EnsureThat.ParamIsNotNull(soundFactory);
            EnsureThat.ParamIsNotNull(file);
            EnsureThat.ParamIsNotNull(path);
            EnsureThat.ParamIsNotNull(disposeSoundsReactable);
            EnsureThat.ParamIsNotNull(shutDownReactable);

            this.soundFactory = soundFactory;
            this.file = file;
            this.path = path;

            this.shutDownUnsubscriber = shutDownReactable.Subscribe(new Reactor<ShutDownData>(_ => ShutDown()));

            this.disposeSoundsReactable = disposeSoundsReactable ??
                                          throw new ArgumentNullException(nameof(disposeSoundsReactable), NullParamExceptionMsg);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="SoundCache"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
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
                throw new ArgumentNullException(nameof(soundFilePath), "The parameter must not be null or empty.");
            }

            var extension = this.path.GetExtension(soundFilePath);

            if (extension != OggFileExtension && extension != Mp3FileExtension)
            {
                var exceptionMsg = $"Sound file type '{extension}' is not supported.";
                exceptionMsg += $"\nSupported file types are '{OggFileExtension}' and '{Mp3FileExtension}'.";

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
                this.disposeSoundsReactable.PushNotification(new DisposeSoundData(sound.Id));
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

            var cacheKeys = this.sounds.Keys.ToArray();

            // Dispose of all default and non default textures
            foreach (var cacheKey in cacheKeys)
            {
                this.sounds.TryRemove(cacheKey, out var sound);

                if (sound is not null)
                {
                    this.disposeSoundsReactable.PushNotification(new DisposeSoundData(sound.Id));
                }
            }

            this.shutDownUnsubscriber.Dispose();

            this.isDisposed = true;
        }
    }
}

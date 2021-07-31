// <copyright file="SoundLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;
    using System.Collections.Concurrent;
    using Velaptor.Factories;

    /// <summary>
    /// Loads sound content.
    /// </summary>
    public class SoundLoader : ILoader<ISound>
    {
        private readonly ConcurrentDictionary<string, ISound> sounds = new ();
        private readonly IPathResolver soundPathResolver;
        private readonly ISoundFactory soundFactory;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="soundPathResolver">Resolves the path to the sound content.</param>
        public SoundLoader(IPathResolver soundPathResolver, ISoundFactory soundFactory)
        {
            this.soundPathResolver = soundPathResolver;
            this.soundFactory = soundFactory;
        }

        /// <summary>
        /// Loads a sound with the given name.
        /// </summary>
        /// <param name="name">The name of the sound content to load.</param>
        /// <returns>The name of the sound content item to load.</returns>
        /// <remarks>
        ///     The content <see cref="name"/> can have or not have the extension of the content file to load.
        ///     Using the file extension has no effect and the content item is matched using the name without the extension.
        ///     The only sounds supported are .ogg and .mp3 files.
        ///     Also, duplicate names are not aloud in the same content source and this is matched against the name without the extension.
        ///
        ///     Example: sound.ogg and sound.mp3 will result in an exception.
        /// </remarks>
        public ISound Load(string name)
        {
            var filePath = this.soundPathResolver.ResolveFilePath(name);

            return this.sounds.GetOrAdd(filePath, (key) =>
            {
                return this.soundFactory.CreateSound(filePath);
            });
        }

        /// <inheritdoc/>
        public void Unload(string name)
        {
            var filePath = this.soundPathResolver.ResolveFilePath(name);

            if (this.sounds.TryRemove(filePath, out var sound))
            {
                sound.Dispose();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var sound in this.sounds.Values)
                {
                    sound.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}

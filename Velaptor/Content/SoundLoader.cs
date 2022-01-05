// <copyright file="SoundLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using Velaptor.Factories;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads sound content.
    /// </summary>
    public sealed class SoundLoader : ILoader<ISound>
    {
        private readonly ConcurrentDictionary<string, ISound> sounds = new ();
        private readonly IPathResolver soundPathResolver;
        private readonly ISoundFactory soundFactory;
        private readonly IPath path;
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="soundPathResolver">Resolves the path to the sound content.</param>
        /// <param name="soundFactory">Creates sound instances.</param>
        [ExcludeFromCodeCoverage]
        public SoundLoader(IPathResolver soundPathResolver, ISoundFactory soundFactory)
        {
            this.soundPathResolver = soundPathResolver;
            this.soundFactory = soundFactory;
            this.path = IoC.Container.GetInstance<IPath>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="soundPathResolver">Resolves the path to the sound content.</param>
        /// <param name="soundFactory">Creates sound instances.</param>
        /// <param name="path">Processes directory and fle paths.</param>
        internal SoundLoader(IPathResolver soundPathResolver, ISoundFactory soundFactory, IPath path)
        {
            this.soundPathResolver = soundPathResolver;
            this.soundFactory = soundFactory;
            this.path = path;
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
        ///     Also, duplicate names are not allowed in the same content source and this is matched against the name without the extension.
        ///
        ///     Example: sound.ogg and sound.mp3 will result in an exception.
        /// </remarks>
        public ISound Load(string name)
        {
            name = this.path.HasExtension(name)
                ? this.path.GetFileNameWithoutExtension(name)
                : name;

            var filePath = this.soundPathResolver.ResolveFilePath(name);

            // If the requested font is already loaded into the pool
            // and has been disposed, remove it.
            foreach (var sound in this.sounds)
            {
                if (sound.Key != filePath || !sound.Value.IsDisposed)
                {
                    continue;
                }

                this.sounds.TryRemove(sound);
                break;
            }

            return this.sounds.GetOrAdd(filePath, (key) =>
            {
                var sound = this.soundFactory.CreateSound(key);
                sound.IsPooled = true;

                return sound;
            });
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string name)
        {
            var filePath = this.soundPathResolver.ResolveFilePath(name);

            if (this.sounds.TryRemove(filePath, out var sound))
            {
                sound.IsPooled = false;
                sound.Dispose();
            }
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">Disposes managed resources when <see langword="true"/>.</param>
        private void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var sound in this.sounds.Values)
                {
                    sound.IsPooled = false;
                    sound.Dispose();
                }
            }

            this.isDisposed = true;
        }
    }
}

// <copyright file="SoundLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Abstractions;
    using System.Linq;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Factories;
    using Velaptor.Factories;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Loads sound content.
    /// </summary>
    public sealed class SoundLoader : ILoader<ISound>
    {
        private const string OggFileExtension = ".ogg";
        private const string Mp3FileExtension = ".mp3";
        private readonly ConcurrentDictionary<string, ISound> sounds = new ();
        private readonly IPathResolver soundPathResolver;
        private readonly ISoundFactory soundFactory;
        private readonly IFile file;
        private readonly IPath path;
        private readonly IReactable<DisposeSoundData> disposeSoundReactable;

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
        public SoundLoader()
        {
            this.soundPathResolver = PathResolverFactory.CreateSoundPathResolver();
            this.soundFactory = IoC.Container.GetInstance<ISoundFactory>();
            this.file = IoC.Container.GetInstance<IFile>();
            this.path = IoC.Container.GetInstance<IPath>();
            this.disposeSoundReactable = IoC.Container.GetInstance<IReactable<DisposeSoundData>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SoundLoader"/> class.
        /// </summary>
        /// <param name="soundPathResolver">Resolves the path to the sound content.</param>
        /// <param name="soundFactory">Creates sound instances.</param>
        /// <param name="file">Performs file related operations.</param>
        /// <param name="path">Processes directory and file paths.</param>
        /// <param name="disposeSoundReactable">Sends a push notifications to dispose of sounds.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        internal SoundLoader(
            IPathResolver soundPathResolver,
            ISoundFactory soundFactory,
            IFile file,
            IPath path,
            IReactable<DisposeSoundData> disposeSoundReactable)
        {
            this.soundPathResolver = soundPathResolver ?? throw new ArgumentNullException(nameof(soundPathResolver), "The parameter must not be null.");
            this.soundFactory = soundFactory ?? throw new ArgumentNullException(nameof(soundFactory), "The parameter must not be null.");
            this.file = file ?? throw new ArgumentNullException(nameof(file), "The parameter must not be null.");
            this.path = path ?? throw new ArgumentNullException(nameof(path), "The parameter must not be null.");
            this.disposeSoundReactable = disposeSoundReactable ?? throw new ArgumentNullException(nameof(disposeSoundReactable), "The parameter must not be null.");
        }

        /// <summary>
        /// Loads a sound with the given name.
        /// </summary>
        /// <param name="contentPathOrName">The full file path or name of the sound to load.</param>
        /// <returns>The loaded sound.</returns>
        public ISound Load(string contentPathOrName)
        {
            var isFullFilePath = contentPathOrName.HasValidFullFilePathSyntax();
            string filePath;

            if (isFullFilePath)
            {
                filePath = contentPathOrName;
            }
            else
            {
                contentPathOrName = this.path.GetFileNameWithoutExtension(contentPathOrName);
                filePath = this.soundPathResolver.ResolveFilePath(contentPathOrName);
            }

            if (this.file.Exists(filePath))
            {
                var fileExtension = this.path.GetExtension(filePath);
                var validExtensions = new[] { OggFileExtension, Mp3FileExtension };
                var isInvalidExtension = validExtensions.All(e => e != fileExtension);

                if (isInvalidExtension)
                {
                    var exceptionMsg = $"The file '{filePath}' must be a sound file with";
                    exceptionMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

                    throw new LoadSoundException(exceptionMsg);
                }
            }
            else
            {
                throw new FileNotFoundException($"The sound file does not exist.", filePath);
            }

            return this.sounds.GetOrAdd(filePath, (filePathCacheKey) =>
            {
                var sound = this.soundFactory.Create(filePathCacheKey);

                return sound;
            });
        }

        /// <inheritdoc/>
        [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
        public void Unload(string contentNameOrPath)
        {
            var isInvalidFullFilePath = contentNameOrPath.HasInvalidFullFilePathSyntax();

            string filePath;

            if (isInvalidFullFilePath)
            {
                filePath = this.soundPathResolver.ResolveFilePath(contentNameOrPath);
            }
            else
            {
                filePath = contentNameOrPath;
            }

            if (this.sounds.TryRemove(filePath, out var sound))
            {
                this.disposeSoundReactable.PushNotification(new DisposeSoundData(sound.Id));
            }
        }
    }
}

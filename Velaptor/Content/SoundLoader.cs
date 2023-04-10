// <copyright file="SoundLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using Caching;
using Exceptions;
using Guards;
using Velaptor.Factories;

/// <summary>
/// Loads sound content.
/// </summary>
public sealed class SoundLoader : ILoader<ISound>
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private readonly IItemCache<string, ISound> soundCache;
    private readonly IContentPathResolver soundPathResolver;
    private readonly IFile file;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public SoundLoader()
    {
        this.soundCache = IoC.Container.GetInstance<IItemCache<string, ISound>>();
        this.soundPathResolver = PathResolverFactory.CreateSoundPathResolver();
        this.file = IoC.Container.GetInstance<IFile>();
        this.path = IoC.Container.GetInstance<IPath>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoundLoader"/> class.
    /// </summary>
    /// <param name="soundCache">Caches textures for later use.</param>
    /// <param name="soundPathResolver">Resolves the path to the sound content.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    internal SoundLoader(
        IItemCache<string, ISound> soundCache,
        IContentPathResolver soundPathResolver,
        IFile file,
        IPath path)
    {
        EnsureThat.ParamIsNotNull(soundCache);
        EnsureThat.ParamIsNotNull(soundPathResolver);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(path);

        this.soundCache = soundCache;
        this.soundPathResolver = soundPathResolver;
        this.file = file;
        this.path = path;
    }

    /// <summary>
    /// Loads a sound with the given name.
    /// </summary>
    /// <param name="contentPathOrName">The full file path or name of the sound to load.</param>
    /// <returns>The loaded sound.</returns>
    public ISound Load(string contentPathOrName)
    {
        string filePath;
        string cacheKey;

        if (this.path.IsPathRooted(contentPathOrName))
        {
            filePath = contentPathOrName;
            cacheKey = filePath;
        }
        else
        {
            contentPathOrName = this.path.GetFileNameWithoutExtension(contentPathOrName);
            filePath = this.soundPathResolver.ResolveFilePath(contentPathOrName);
            cacheKey = filePath;
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
            throw new FileNotFoundException("The sound file does not exist.", filePath);
        }

        return this.soundCache.GetItem(cacheKey);
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
    public void Unload(string contentPathOrName)
    {
        var isInvalidFullFilePath = !this.path.IsPathRooted(contentPathOrName);

        string filePath;

        if (isInvalidFullFilePath)
        {
            filePath = this.soundPathResolver.ResolveFilePath(contentPathOrName);
        }
        else
        {
            filePath = contentPathOrName;
        }

        this.soundCache.Unload(filePath);
    }
}

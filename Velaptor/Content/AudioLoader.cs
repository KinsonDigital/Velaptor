// <copyright file="AudioLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Caching;
using Exceptions;

/// <summary>
/// Loads audio content.
/// </summary>
internal sealed class AudioLoader : ILoader<IAudio>
{
    private const string OggFileExtension = ".ogg";
    private const string Mp3FileExtension = ".mp3";
    private const char MetaDataSignifier = '|';
    private const char WinDirSepChar = '\\';
    private readonly IItemCache<string, IAudio> audioCache;
    private readonly IContentPathResolver audioPathResolver;
    private readonly IDirectory directory;
    private readonly IFile file;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioLoader"/> class.
    /// </summary>
    /// <param name="audioCache">Caches textures for later use.</param>
    /// <param name="audioPathResolver">Resolves the path to the audio content.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public AudioLoader(
        IItemCache<string, IAudio> audioCache,
        IContentPathResolver audioPathResolver,
        IDirectory directory,
        IFile file,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(audioCache);
        ArgumentNullException.ThrowIfNull(audioPathResolver);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);

        this.audioCache = audioCache;
        this.audioPathResolver = audioPathResolver;
        this.directory = directory;
        this.file = file;
        this.path = path;
    }

    /// <summary>
    /// Loads the audio with the given name.
    /// </summary>
    /// <param name="contentPathOrName">The full file path or name of the audio to load.</param>
    /// <returns>The loaded audio.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="contentPathOrName"/> is null or empty.</exception>
    /// <exception cref="LoadTextureException">Thrown if the resulting texture content file path is invalid.</exception>
    /// <exception cref="FileNotFoundException">Thrown if the texture file does not exist.</exception>
    /// <exception cref="IOException">The directory specified a file or the network name is not known.</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permissions.</exception>
    /// <exception cref="PathTooLongException">
    ///     The specified path, file name, or both exceed the system-defined maximum length.
    /// </exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="NotSupportedException">The path contains a colon character <c>:</c> that is not part of a drive label.</exception>
    public IAudio Load(string contentPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentPathOrName);

        if (contentPathOrName.DoesNotContain(MetaDataSignifier))
        {
            throw new LoadAudioException("The audio file path must contain metadata.");
        }

        var wholeSections = contentPathOrName.Split(MetaDataSignifier);
        var contentFilePath = wholeSections[0];
        var bufferTypeStr = wholeSections[1];

        var parseSuccess = Enum.TryParse(bufferTypeStr, out AudioBuffer bufferType);

        if (!parseSuccess)
        {
            throw new LoadAudioException("The audio buffer type could not be determined");
        }

        var isPathRooted = this.path.IsPathRooted(contentFilePath);

        if (!isPathRooted)
        {
            var contentDirPath = this.audioPathResolver.ResolveDirPath();

            if (!this.directory.Exists(contentDirPath))
            {
                this.directory.CreateDirectory(contentDirPath);
            }
        }

        var filePath = isPathRooted
            ? contentFilePath
            : this.audioPathResolver.ResolveFilePath(contentFilePath);

        if (!this.file.Exists(filePath))
        {
            throw new FileNotFoundException("The audio file does not exist.", filePath);
        }

        var fileExtension = this.path.GetExtension(filePath);
        var validExtensions = new[] { OggFileExtension, Mp3FileExtension };
        var isInvalidExtension = Array.TrueForAll(validExtensions, e => e != fileExtension);

        if (!isInvalidExtension)
        {
            return this.audioCache.GetItem($"{filePath}|{bufferType}");
        }

        var exceptionMsg = $"The file '{filePath}' must be a audio file with";
        exceptionMsg += $" the extension '{OggFileExtension}' or '{Mp3FileExtension}'.";

        throw new LoadAudioException(exceptionMsg);
    }

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "InvertIf", Justification = "Readability")]
    public void Unload(string contentPathOrName)
    {
        var filePath = this.path.IsPathRooted(contentPathOrName)
            ? contentPathOrName
            : this.audioPathResolver.ResolveFilePath(contentPathOrName);

        this.audioCache.Unload(filePath);
    }
}

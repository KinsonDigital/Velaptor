// <copyright file="AudioPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Services;

/// <summary>
/// Resolves paths to audio content.
/// </summary>
internal sealed class AudioPathResolver : ContentPathResolver
{
    private const string OggExtension = ".ogg";
    private const string Mp3Extension = ".mp3";
    private readonly IPath path;
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="AudioPathResolver"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the following parameters are null:
    /// <list type="bullet">
    ///     <item><paramref name="appService"/></item>
    ///     <item><paramref name="file"/></item>
    ///     <item><paramref name="path"/></item>
    ///     <item><paramref name="platform"/></item>
    /// </list>
    /// </exception>
    public AudioPathResolver(IAppService appService, IFile file, IPath path, IPlatform platform)
        : base(appService, file, path, platform)
    {
        this.path = path;
        this.platform = platform;
        ContentDirectoryName = "Audio";
    }

    /// <summary>
    /// Returns the path to the audio content.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content.</param>
    /// <returns>The path to the content item.</returns>
    /// <remarks>
    ///     The two types of audio formats supported are '.ogg' and '.mp3'.
    /// <para>
    ///     Precedence is taken with '.ogg' files over '.mp3'.  What this means is that if
    ///     there are two files <br/> with the same name but with different extensions in the
    ///     same <see cref="ContentPathResolver.ContentDirectoryName"/>, <br/> the '.ogg'
    ///     file will be loaded, not the '.mp3' file.
    /// </para>
    /// <para>
    ///     If no <c>.ogg</c> file exists but an <c>.mp3</c> file does, then the <c>.mp3</c> file will be loaded.
    /// </para>
    /// </remarks>
    public override string ResolveFilePath(string contentPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentPathOrName);

        contentPathOrName = this.path.HasExtension(contentPathOrName) ? contentPathOrName : $"{contentPathOrName}{OggExtension}";

        var comparisonType = this.platform.CurrentPlatform == OSPlatform.Windows
            ? StringComparison.OrdinalIgnoreCase
            : StringComparison.Ordinal;

        var extension = this.path.GetExtension(contentPathOrName);

        // Check if the file extension is supported
        if (string.Compare(extension, OggExtension, comparisonType) != 0 && string.Compare(extension, Mp3Extension, comparisonType) != 0)
        {
            var msg = $"The file extension '{extension}' is not supported.  Supported audio formats are '{OggExtension}' and '{Mp3Extension}'.";
            msg += this.platform.CurrentPlatform == OSPlatform.Windows
                ? string.Empty
                : "\nNote: Linux and MacOS are case-sensitive.";

            throw new ArgumentException(msg);
        }

        return base.ResolveFilePath(contentPathOrName);
    }
}

// <copyright file="WindowsFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.InteropServices;
using Guards;

/// <summary>
/// Resolves paths to Windows system fonts.
/// </summary>
internal sealed class WindowsFontPathResolver : IContentPathResolver
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string FileExtension = ".ttf";
    private readonly IDirectory directory;

    /// <summary>
    /// Initializes a new instance of the <see cref="WindowsFontPathResolver"/> class.
    /// </summary>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public WindowsFontPathResolver(IDirectory directory, IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(platform);

        // This should only be running if on windows.  If not windows, throw an exception.
        if (platform.CurrentPlatform != OSPlatform.Windows)
        {
            throw new PlatformNotSupportedException(
                $"The '{nameof(WindowsFontPathResolver)}' can only be used on the 'Windows' platform.");
        }

        this.directory = directory;
    }

    /// <inheritdoc/>
    public string RootDirectoryPath => $"C:{CrossPlatDirSeparatorChar}Windows";

    /// <inheritdoc/>
    public string ContentDirectoryName => "Fonts";

    /// <inheritdoc/>
    public string ResolveFilePath(string contentName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentName);

        if (contentName.EndsWith(CrossPlatDirSeparatorChar))
        {
            throw new ArgumentException($"The '{contentName}' cannot end with a folder.  It must end with a file name with or without the extension.", nameof(contentName));
        }

        var contentDirPath = $@"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
        var fullContentPath = $"{contentDirPath}{CrossPlatDirSeparatorChar}{contentName}{FileExtension}";

        var possibleFiles = this.directory.GetFiles(contentDirPath, $"*{FileExtension}")
            .NormalizePaths();

        var files = (from f in possibleFiles
            where string.Compare(
                f,
                fullContentPath,
                StringComparison.OrdinalIgnoreCase) == 0
            select f).ToArray();

        return files.Length <= 0 ? string.Empty : files[0];
    }

    /// <inheritdoc/>
    public string ResolveDirPath() => $@"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
}

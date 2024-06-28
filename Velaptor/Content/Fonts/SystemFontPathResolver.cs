// <copyright file="WindowsFontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

/// <summary>
/// Resolves paths to system fonts.
/// </summary>
internal sealed class SystemFontPathResolver : IContentPathResolver
{
    private const char CrossPlatDirSeparatorChar = '/';
    private const string FileExtension = ".ttf";
    private readonly IDirectory directory;
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemFontPathResolver"/> class.
    /// </summary>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public SystemFontPathResolver(IDirectory directory, IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(platform);

        // This should only be running if on windows/linux/osx.  If not, throw an exception.
        if (platform.CurrentPlatform == OSPlatform.Windows ||
            platform.CurrentPlatform == OSPlatform.Linux ||
            platform.CurrentPlatform == OSPlatform.OSX)
        {
            this.directory = directory;
            this.platform = platform;
        }
        else
        {
            throw new PlatformNotSupportedException(
                $"The '{nameof(SystemFontPathResolver)}' can only be used on the 'Windows,Linux,or OSX' platforms.");
        }
    }

    /// <summary>
    /// Gets the root directory of the content.
    /// </summary>
    /// <remarks>
    ///     Will return the application's font content directory if it exists.  If it does not exist, it returns
    ///     the current system's font content directory.
    /// </remarks>
    /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows/Linux/OSX.</exception>
    public string RootDirectoryPath
    {
        get
        {
            if (this.platform.CurrentPlatform == OSPlatform.Windows)
            {
                return $"C:{CrossPlatDirSeparatorChar}Windows";
            }
            else if (this.platform.CurrentPlatform == OSPlatform.Linux)
            {
                return
                    $"{CrossPlatDirSeparatorChar}usr{CrossPlatDirSeparatorChar}share";
            }
            else if (this.platform.CurrentPlatform == OSPlatform.OSX)
            {
                return $"{CrossPlatDirSeparatorChar}System{CrossPlatDirSeparatorChar}Library";
            }
            else
            {
                throw new NotSupportedException("Only Windows/Linux/OSX are supported.");
            }
        }
    }

    /// <inheritdoc/>
    public string ContentDirectoryName
    {
        get
        {
            if (this.platform.CurrentPlatform == OSPlatform.Windows)
            {
                return "Font";
            }
            else if (this.platform.CurrentPlatform == OSPlatform.Linux)
            {
                return $"fonts{CrossPlatDirSeparatorChar}TTF";
            }
            else if (this.platform.CurrentPlatform == OSPlatform.OSX)
            {
                return "Font";
            }
            else
            {
                throw new NotSupportedException("Only Windows/Linux/OSX are supported.");
            }
        }
    }

    /// <inheritdoc/>
    public string ResolveFilePath(string contentName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentName);

        if (contentName.EndsWith(CrossPlatDirSeparatorChar))
        {
            throw new ArgumentException($"The '{contentName}' cannot end with a folder.  It must end with a file name with or without the extension.", nameof(contentName));
        }

        var contentDirPath = $"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
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
    public string ResolveDirPath() => $"{RootDirectoryPath}{CrossPlatDirSeparatorChar}{ContentDirectoryName}";
}

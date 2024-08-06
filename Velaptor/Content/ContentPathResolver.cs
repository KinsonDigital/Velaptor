// <copyright file="ContentPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using ExtensionMethods;
using Services;

/// <summary>
/// Manages the content source.
/// </summary>
internal abstract class ContentPathResolver : IContentPathResolver
{
    private readonly IFile file;
    private readonly IPath path;
    private readonly IPlatform platform;
    private string rootDirPath;
    private string contentDirName = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ContentPathResolver"/> class.
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
    protected ContentPathResolver(IAppService appService, IFile file, IPath path, IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(appService);
        ArgumentNullException.ThrowIfNull(file);
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(platform);

        this.file = file;
        this.path = path;
        this.platform = platform;
        this.rootDirPath = $"{appService.AppDirectory}{path.AltDirectorySeparatorChar}Content";
    }

    /// <inheritdoc/>
    public string RootDirectoryPath
    {
        get => this.rootDirPath;
        set
        {
            var isNullOrEmpty = string.IsNullOrEmpty(value);
            value = isNullOrEmpty ? string.Empty : value;

            if (isNullOrEmpty)
            {
                return;
            }

            value = value.Replace(this.path.DirectorySeparatorChar, this.path.AltDirectorySeparatorChar);

            this.rootDirPath = value;
        }
    }

    /// <inheritdoc/>
    public string ContentDirectoryName
    {
        get => this.contentDirName;
        set => this.contentDirName = value.GetLastDirName();
    }

    /// <inheritdoc/>
    /// <returns>The resolved content file path.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="contentPathOrName"/> is null.</exception>
    /// <exception cref="ArgumentException">
    /// Thrown for the following reasons:
    /// <list type="bullet">
    ///     <item>If the <paramref name="contentPathOrName"/> is empty.</item>
    ///     <item>If the <paramref name="contentPathOrName"/> ends with a directory separator character.</item>
    ///     <item>If the <paramref name="contentPathOrName"/> does not end with an extension.</item>
    /// </list>
    /// </exception>
    /// <exception cref="FileNotFoundException">Thrown if the content file does not exist.</exception>
    public virtual string ResolveFilePath(string contentPathOrName)
    {
        if (string.IsNullOrEmpty(contentPathOrName))
        {
            throw new ArgumentNullException(nameof(contentPathOrName), "The string parameter must not be null or empty.");
        }

        if (contentPathOrName.EndsWith(this.path.DirectorySeparatorChar) || contentPathOrName.EndsWith(this.path.AltDirectorySeparatorChar))
        {
            var exMsg = $"The '{contentPathOrName}' cannot end with a folder. It must end with or without an extension.";
            throw new ArgumentException(exMsg, nameof(contentPathOrName));
        }

        if (this.platform.CurrentPlatform == OSPlatform.Windows)
        {
            contentPathOrName = contentPathOrName.Replace(this.path.DirectorySeparatorChar, this.path.AltDirectorySeparatorChar);
        }

        contentPathOrName = contentPathOrName.StartsWith($".{this.path.AltDirectorySeparatorChar}")
            ? contentPathOrName[2..]
            : contentPathOrName;

        if (!this.path.HasExtension(contentPathOrName))
        {
            throw new ArgumentException($"The '{contentPathOrName}' must end with an extension.", nameof(contentPathOrName));
        }

        var contentDirPath = GetContentDirPath();

        var fullContentFilePath = this.path.IsPathRooted(contentPathOrName)
            ? contentPathOrName
            : $"{contentDirPath}{this.path.AltDirectorySeparatorChar}{contentPathOrName}";

        if (!this.file.Exists(fullContentFilePath))
        {
            throw new FileNotFoundException("The content file could not be found.", fullContentFilePath);
        }

        return fullContentFilePath;
    }

    /// <inheritdoc/>
    public string ResolveDirPath()
    {
        var rootPath = this.rootDirPath.EndsWith(this.path.DirectorySeparatorChar) ||
                       this.rootDirPath.EndsWith(this.path.AltDirectorySeparatorChar)
            ? this.rootDirPath[..^1]
            : this.rootDirPath;

        return $"{rootPath}{this.path.AltDirectorySeparatorChar}{this.contentDirName}";
    }

    /// <summary>
    /// Gets the directory path of the content.
    /// </summary>
    /// <returns>The full directory path to the content directory.</returns>
    private string GetContentDirPath()
    {
        var rootPath = this.rootDirPath.EndsWith(this.path.DirectorySeparatorChar) ||
                       this.rootDirPath.EndsWith(this.path.AltDirectorySeparatorChar)
            ? this.rootDirPath[..^1]
            : this.rootDirPath;

        return $"{rootPath}{this.path.AltDirectorySeparatorChar}{this.contentDirName}";
    }
}

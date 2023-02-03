// <copyright file="FontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Guards;

/// <summary>
/// Resolves paths to font content to be used for rendering text.
/// </summary>
/// <remarks>
///     The location of fonts will first be checked in the default content location
///     '&lt;app-dir&gt;/Content/Fonts' directory.  If the font exists in this directory, it will be loaded.
///     If the font does not exist in that location, then the path will be resolved to the current
///     operating system font location.
/// <para/>
/// <para>
///     NOTE: Only Windows system fonts are currently supported.
///     Other systems will be supported in a future releases.
/// </para>
/// </remarks>
internal sealed class FontPathResolver : IPathResolver
{
    private const string OnlyWindowsSupportMessage = "Currently loading system fonts is only supported on Windows.";
    private readonly IPlatform platform;
    private readonly IPathResolver windowsFontPathResolver;
    private readonly IPathResolver contentFontPathResolver;
    private readonly IDirectory directory;
    private readonly IFile file;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPathResolver"/> class.
    /// </summary>
    /// <param name="contentFontPathResolver">
    /// Resolves the path to the font path in the currently set content directory.
    /// </param>
    /// <param name="windowsFontPathResolver">
    /// Resolves the path to the Windows system fonts directory.
    /// </param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public FontPathResolver(
        IPathResolver contentFontPathResolver,
        IPathResolver windowsFontPathResolver,
        IFile file,
        IDirectory directory,
        IPlatform platform)
    {
        EnsureThat.ParamIsNotNull(contentFontPathResolver);
        EnsureThat.ParamIsNotNull(windowsFontPathResolver);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(platform);

        this.contentFontPathResolver = contentFontPathResolver;
        this.windowsFontPathResolver = windowsFontPathResolver;
        this.file = file;
        this.directory = directory;
        this.platform = platform;
    }

    /// <summary>
    /// Gets the root directory of the content.
    /// </summary>
    /// <remarks>
    ///     Will return the application's font content directory if it exists.  If it does not exist, it returns
    ///     the current system's font content directory.
    /// </remarks>
    /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
    public string RootDirectoryPath
    {
        get
        {
            if (this.directory.Exists(this.contentFontPathResolver.RootDirectoryPath))
            {
                return this.contentFontPathResolver.RootDirectoryPath;
            }

            if (this.platform.CurrentPlatform != OSPlatform.Windows)
            {
                throw new NotSupportedException(OnlyWindowsSupportMessage);
            }

            return this.windowsFontPathResolver.RootDirectoryPath;
        }
    }

    /// <summary>
    /// Gets the name of the directory that contains the content that is located in the <see cref="RootDirectoryPath"/>.
    /// </summary>
    /// <remarks>
    ///     Will return the application's font content directory name if it exists.  If it does not exist, it
    ///     returns the current system's font content directory name.
    /// </remarks>
    /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
    public string ContentDirectoryName
    {
        get
        {
            if (this.directory.Exists(this.contentFontPathResolver.RootDirectoryPath))
            {
                return this.contentFontPathResolver.ContentDirectoryName;
            }

            if (this.platform.CurrentPlatform != OSPlatform.Windows)
            {
                throw new NotSupportedException(OnlyWindowsSupportMessage);
            }

            return this.windowsFontPathResolver.ContentDirectoryName;
        }
    }

    /// <summary>
    /// Resolves the full file path to a content item that matches the given <paramref name="contentName"/>.
    /// </summary>
    /// <param name="contentName">The name of the content item with or without the file extension.</param>
    /// <returns>
    ///     The <see cref="RootDirectoryPath"/>, content file name, and the <see cref="ContentDirectoryName"/> combined.
    /// </returns>
    /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
    public string ResolveFilePath(string contentName)
    {
        if (this.platform.CurrentPlatform != OSPlatform.Windows)
        {
            throw new NotSupportedException(OnlyWindowsSupportMessage);
        }

        var contentFilePath = this.contentFontPathResolver.ResolveFilePath(contentName);

        return this.file.Exists(contentFilePath)
            ? contentFilePath
            : this.windowsFontPathResolver.ResolveFilePath(contentName);
    }

    /// <summary>
    /// Resolves the full directory path to font content.
    /// </summary>
    /// <returns>The directory only path to font content.</returns>
    /// <exception cref="NotImplementedException">Thrown if the current platform is not Windows.</exception>
    public string ResolveDirPath()
    {
        if (this.platform.CurrentPlatform != OSPlatform.Windows)
        {
            throw new NotSupportedException(OnlyWindowsSupportMessage);
        }

        var contentDirPath = this.contentFontPathResolver.ResolveDirPath();

        return this.directory.Exists(contentDirPath)
            ? contentDirPath
            : this.windowsFontPathResolver.ResolveDirPath();
    }
}

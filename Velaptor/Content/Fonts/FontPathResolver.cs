// <copyright file="FontPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Fonts;

using System;
using System.IO.Abstractions;
using Velaptor.Services;

/// <summary>
/// Resolves paths to font content to be used for rendering text.
/// </summary>
/// <remarks>
///     The location of fonts will first be checked in the default content location
///     <c>./Content/Fonts</c> directory.  If the font exists in this directory, it will be loaded.
///     If the font does not exist in that location, then the path will be resolved to the current
///     operating system font location.
/// <para/>
/// <para>
///     NOTE: Only Windows system fonts are currently supported.
///     Other systems will be supported in a future releases.
/// </para>
/// </remarks>
internal sealed class FontPathResolver : ContentPathResolver
{
    private const string FileExtension = ".ttf";
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontPathResolver"/> class.
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
    public FontPathResolver(IAppService appService, IFile file, IPath path, IPlatform platform)
        : base(appService, file, path, platform)
    {
        this.path = path;
        ContentDirectoryName = "Fonts";
    }

    /// <summary>
    /// Resolves the full file path to a content item that matches the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content item with or without the file extension.</param>
    /// <returns>The fully qualified file path.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the parameter is empty.</exception>
    public override string ResolveFilePath(string contentPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentPathOrName);

        contentPathOrName = this.path.HasExtension(contentPathOrName) ? contentPathOrName : $"{contentPathOrName}{FileExtension}";

        return base.ResolveFilePath(contentPathOrName);
    }
}

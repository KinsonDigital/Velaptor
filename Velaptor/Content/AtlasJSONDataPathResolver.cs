// <copyright file="AtlasJSONDataPathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO.Abstractions;
using Services;

/// <summary>
/// Resolves paths to atlas data content.
/// </summary>
internal sealed class AtlasJSONDataPathResolver : ContentPathResolver
{
    private const string FileExtension = ".json";
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasJSONDataPathResolver"/> class.
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
    public AtlasJSONDataPathResolver(IAppService appService, IFile file, IPath path, IPlatform platform)
        : base(appService, file, path, platform)
    {
        this.path = path;
        ContentDirectoryName = "Atlas";
    }

    /// <summary>
    /// Returns the path to the texture atlas data content.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content item with or without the file extension.</param>
    /// <returns>The path to the content.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown if the parameter is empty.</exception>
    public override string ResolveFilePath(string contentPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentPathOrName);

        contentPathOrName = this.path.HasExtension(contentPathOrName) ? contentPathOrName : $"{contentPathOrName}{FileExtension}";

        return base.ResolveFilePath(contentPathOrName);
    }
}

// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.IO;
using System.IO.Abstractions;
using Caching;
using Exceptions;

/// <summary>
/// Loads textures.
/// </summary>
internal sealed class TextureLoader : ILoader<ITexture>
{
    private readonly IItemCache<string, ITexture> textureCache;
    private readonly IContentPathResolver texturePathResolver;
    private readonly IDirectory directory;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoader"/> class.
    /// </summary>
    /// <param name="textureCache">Caches textures for later use to improve performance.</param>
    /// <param name="texturePathResolver">Resolves paths to texture content.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public TextureLoader(
        IItemCache<string, ITexture> textureCache,
        IContentPathResolver texturePathResolver,
        IDirectory directory,
        IPath path)
    {
        ArgumentNullException.ThrowIfNull(textureCache);
        ArgumentNullException.ThrowIfNull(texturePathResolver);
        ArgumentNullException.ThrowIfNull(directory);
        ArgumentNullException.ThrowIfNull(path);

        this.textureCache = textureCache;
        this.texturePathResolver = texturePathResolver;
        this.directory = directory;
        this.path = path;
    }

    /// <summary>
    /// Loads a texture with the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The full file path or name of the texture to load.</param>
    /// <returns>The loaded texture.</returns>
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
    public ITexture Load(string contentPathOrName)
    {
        ArgumentException.ThrowIfNullOrEmpty(contentPathOrName);

        var contentDirPath = $"{this.texturePathResolver.RootDirectoryPath}{this.path.AltDirectorySeparatorChar}{this.texturePathResolver.ContentDirectoryName}";
        if (!this.directory.Exists(contentDirPath))
        {
            this.directory.CreateDirectory(contentDirPath);
        }

        var filePath = this.texturePathResolver.ResolveFilePath(contentPathOrName);

        return this.textureCache.GetItem(filePath);
    }

    /// <inheritdoc/>
    public void Unload(string contentPathOrName) => this.textureCache.Unload(contentPathOrName);
}

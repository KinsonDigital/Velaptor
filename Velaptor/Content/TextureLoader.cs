// <copyright file="TextureLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Caching;
using Exceptions;
using Guards;
using Velaptor.Factories;

/// <summary>
/// Loads textures.
/// </summary>
public sealed class TextureLoader : ILoader<ITexture>
{
    private const string TextureFileExtension = ".png";
    private readonly IItemCache<string, ITexture> textureCache;
    private readonly IContentPathResolver texturePathResolver;
    private readonly IDirectory directory;
    private readonly IFile file;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Used by library users.")]
    public TextureLoader()
    {
        this.textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        this.texturePathResolver = PathResolverFactory.CreateTexturePathResolver();
        this.file = IoC.Container.GetInstance<IFile>();
        this.path = IoC.Container.GetInstance<IPath>();
        this.directory = IoC.Container.GetInstance<IDirectory>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureLoader"/> class.
    /// </summary>
    /// <param name="textureCache">Caches textures for later use to improve performance.</param>
    /// <param name="texturePathResolver">Resolves paths to texture content.</param>
    /// <param name="directory">Performs operations with directories.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and fle paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    internal TextureLoader(
        IItemCache<string, ITexture> textureCache,
        IContentPathResolver texturePathResolver,
        IDirectory directory,
        IFile file,
        IPath path)
    {
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.ParamIsNotNull(texturePathResolver);
        EnsureThat.ParamIsNotNull(directory);
        EnsureThat.ParamIsNotNull(file);
        EnsureThat.ParamIsNotNull(path);

        this.textureCache = textureCache;
        this.texturePathResolver = texturePathResolver;
        this.file = file;
        this.path = path;
        this.directory = directory;
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
        EnsureThat.StringParamIsNotNullOrEmpty(contentPathOrName);

        var isPathRooted = this.path.IsPathRooted(contentPathOrName);

        if (isPathRooted)
        {
            var contentDirPath = this.texturePathResolver.ResolveDirPath();

            if (!this.directory.Exists(contentDirPath))
            {
                this.directory.CreateDirectory(contentDirPath);
            }
        }

        var filePath = isPathRooted
            ? contentPathOrName
            : this.texturePathResolver.ResolveFilePath(contentPathOrName);

        if (this.file.Exists(filePath))
        {
            if (this.path.GetExtension(filePath) is not TextureFileExtension)
            {
                throw new LoadTextureException(
                    $"The file '{filePath}' must be a texture file with the extension '{TextureFileExtension}'.");
            }
        }
        else
        {
            throw new FileNotFoundException($"The texture file '{filePath}' does not exist.", filePath);
        }

        return this.textureCache.GetItem(filePath);
    }

    /// <inheritdoc/>
    public void Unload(string contentPathOrName) => this.textureCache.Unload(contentPathOrName);
}

// <copyright file="ImageLoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Content;
using Factories;
using Guards;
using Services;

/// <inheritdoc/>
public class ImageLoader : IImageLoader
{
    private readonly IPath path;
    private readonly IImageService imageService;
    private readonly IContentPathResolver texturePathResolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLoader"/> class.
    /// </summary>
    [ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
    public ImageLoader()
    {
        this.path = IoC.Container.GetInstance<IPath>();
        this.imageService = IoC.Container.GetInstance<IImageService>();
        this.texturePathResolver = PathResolverFactory.CreateTexturePathResolver();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLoader"/> class.
    /// </summary>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="imageService">Provides image related services.</param>
    /// <param name="texturePathResolver">Resolves paths to texture content.</param>
    internal ImageLoader(IPath path, IImageService imageService, IContentPathResolver texturePathResolver)
    {
        EnsureThat.ParamIsNotNull(path);
        EnsureThat.ParamIsNotNull(imageService);
        EnsureThat.ParamIsNotNull(texturePathResolver);

        this.path = path;
        this.imageService = imageService;
        this.texturePathResolver = texturePathResolver;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="filePath">The relative or absolute file path to the image.</param>
    /// <remarks>
    ///     If the <paramref name="filePath"/> is a relative path, it will be resolved
    ///     to the content directory of the application.
    ///     <br/>
    ///     The default content directory is <b>Content/Graphics/</b>.
    /// </remarks>
    /// <returns>The image data.</returns>
    public ImageData LoadImage(string filePath)
    {
        // 1. check if the file is a relative or absolute path
        if (this.path.IsPathRooted(filePath))
        {
            return this.imageService.Load(filePath);
        }

        filePath = this.texturePathResolver.ResolveFilePath(filePath);

        return this.imageService.Load(filePath);
    }
}

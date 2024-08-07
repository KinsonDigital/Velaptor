// <copyright file="AtlasTexturePathResolver.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

using System.IO.Abstractions;
using Services;

/// <summary>
/// Resolves paths to atlas texture content.
/// </summary>
internal sealed class AtlasTexturePathResolver : TexturePathResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AtlasTexturePathResolver"/> class.
    /// </summary>
    /// <param name="appService">Provides application services.</param>
    /// <param name="file">Performs operations with files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="platform">Provides information about the current platform.</param>
    public AtlasTexturePathResolver(IAppService appService, IFile file, IPath path, IPlatform platform)
        : base(appService, file, path, platform) => ContentDirectoryName = "Atlas";
}

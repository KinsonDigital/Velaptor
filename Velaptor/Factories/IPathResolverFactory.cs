// <copyright file="IPathResolverFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using Content;

/// <summary>
/// Creates path resolver instances.
/// </summary>
public interface IPathResolverFactory
{
    /// <summary>
    /// Creates a path resolver that resolves paths to texture content.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    IContentPathResolver CreateTexturePathResolver();

    /// <summary>
    /// Creates a path resolver that resolves paths to texture atlas textures.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    IContentPathResolver CreateAtlasPathResolver();

    /// <summary>
    /// Creates a path resolver that resolves paths to font content.
    /// </summary>
    /// <returns>The resolver to atlas content.</returns>
    IContentPathResolver CreateFontPathResolver();

    /// <summary>
    /// Creates a path resolver that resolves paths to audio content.
    /// </summary>
    /// <returns>The resolver to audio content.</returns>
    IContentPathResolver CreateAudioPathResolver();
}

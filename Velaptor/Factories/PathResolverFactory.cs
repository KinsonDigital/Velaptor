// <copyright file="PathResolverFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Content;
using Content.Fonts;
using Services;

/// <summary>
/// Creates path resolver instances.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due direct interaction with IoC container.")]
public static class PathResolverFactory
{
    private static IContentPathResolver? texturePathResolver;
    private static IContentPathResolver? atlasPathResolver;
    private static IContentPathResolver? audioPathResolver;
    private static IContentPathResolver? fontPathResolver;

    /// <summary>
    /// Creates a path resolver that resolves paths to texture content.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateTexturePathResolver() =>
        texturePathResolver ??= new TexturePathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>(),
            IoC.Container.GetInstance<IPlatform>());

    /// <summary>
    /// Creates a path resolver that resolves paths to texture atlas textures.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateAtlasPathResolver() =>
        atlasPathResolver ??= new AtlasTexturePathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>(),
            IoC.Container.GetInstance<IPlatform>());

    /// <summary>
    /// Creates a path resolver that resolves paths to font content.
    /// </summary>
    /// <returns>The resolver to atlas content.</returns>
    public static IContentPathResolver CreateFontPathResolver() =>
        fontPathResolver ??= new FontPathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>(),
            IoC.Container.GetInstance<IPlatform>());

    /// <summary>
    /// Creates a path resolver that resolves paths to audio content.
    /// </summary>
    /// <returns>The resolver to audio content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateAudioPathResolver() =>
        audioPathResolver ??= new AudioPathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>(),
            IoC.Container.GetInstance<IPlatform>());
}

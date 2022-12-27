// <copyright file="PathResolverFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.Runtime.InteropServices;
using Content;
using Content.Fonts;

/// <summary>
/// Creates path resolver instances.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due direct interaction with IoC container.")]
public static class PathResolverFactory
{
    private static readonly IPlatform Platform;
    private static IPathResolver? texturePathResolver;
    private static IPathResolver? atlasPathResolver;
    private static IPathResolver? soundPathResolver;
    private static IPathResolver? fontPathResolver;
    private static IPathResolver? contentFontPathResolver;
    private static IPathResolver? windowsFontPathResolver;

    /// <summary>
    /// Initializes static members of the <see cref="PathResolverFactory"/> class.
    /// </summary>
    static PathResolverFactory() => Platform = IoC.Container.GetInstance<IPlatform>();

    /// <summary>
    /// Creates a path resolver that resolves paths to texture content.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IPathResolver CreateTexturePathResolver() =>
        texturePathResolver ??= new TexturePathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to texture atlas textures.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IPathResolver CreateAtlasPathResolver() =>
        atlasPathResolver ??= new AtlasTexturePathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the application's content directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IPathResolver CreateContentFontPathResolver() =>
        contentFontPathResolver ??= new ContentFontPathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the system's font directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    public static IPathResolver CreateSystemFontPathResolver()
    {
        if (Platform.CurrentPlatform == OSPlatform.Windows)
        {
            return CreateWindowsFontPathResolver();
        }

        throw new NotImplementedException("Currently loading system fonts is only supported on Windows.");
    }

    /// <summary>
    /// Creates a path resolver that resolves paths to font content.
    /// </summary>
    /// <returns>The resolver to atlas content.</returns>
    public static IPathResolver CreateFontPathResolver()
    {
        var contentPathResolver = new ContentFontPathResolver(IoC.Container.GetInstance<IDirectory>());

        return fontPathResolver ??= new FontPathResolver(
            contentPathResolver,
            CreateSystemFontPathResolver(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IDirectory>(),
            IoC.Container.GetInstance<IPlatform>());
    }

    /// <summary>
    /// Creates a path resolver that resolves paths to sound content.
    /// </summary>
    /// <returns>The resolver to sound content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IPathResolver CreateSoundPathResolver() =>
        soundPathResolver ??= new SoundPathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the window's font directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left internal for future access")]
    internal static IPathResolver CreateWindowsFontPathResolver() =>
        windowsFontPathResolver ??= new WindowsFontPathResolver(
            IoC.Container.GetInstance<IDirectory>(),
            IoC.Container.GetInstance<IPlatform>());
}

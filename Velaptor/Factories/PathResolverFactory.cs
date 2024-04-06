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
    private static IContentPathResolver? texturePathResolver;
    private static IContentPathResolver? atlasPathResolver;
    private static IContentPathResolver? soundPathResolver;
    private static IContentPathResolver? fontPathResolver;
    private static IContentPathResolver? contentFontPathResolver;
    private static IContentPathResolver? windowsFontPathResolver;

    /// <summary>
    /// Initializes static members of the <see cref="PathResolverFactory"/> class.
    /// </summary>
    static PathResolverFactory() => Platform = IoC.Container.GetInstance<IPlatform>();

    /// <summary>
    /// Creates a path resolver that resolves paths to texture content.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateTexturePathResolver() =>
        texturePathResolver ??= new TexturePathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to texture atlas textures.
    /// </summary>
    /// <returns>The resolver to texture content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateAtlasPathResolver() =>
        atlasPathResolver ??= new AtlasTexturePathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the application's content directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateContentFontPathResolver() =>
        contentFontPathResolver ??= new ContentFontPathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the system's font directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    public static IContentPathResolver CreateSystemFontPathResolver()
    {
        if (Platform.CurrentPlatform == OSPlatform.Windows)
        {
            return CreateWindowsFontPathResolver();
        }

        throw new NotSupportedException("Currently loading system fonts is only supported on Windows.");
    }

    /// <summary>
    /// Creates a path resolver that resolves paths to font content.
    /// </summary>
    /// <returns>The resolver to atlas content.</returns>
    public static IContentPathResolver CreateFontPathResolver()
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
    /// Creates a path resolver that resolves paths to audio content.
    /// </summary>
    /// <returns>The resolver to audio content.</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public static IContentPathResolver CreateAudioPathResolver() =>
        soundPathResolver ??= new AudioPathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the window's font directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left internal for future access")]
    internal static IContentPathResolver CreateWindowsFontPathResolver() =>
        windowsFontPathResolver ??= new WindowsFontPathResolver(
            IoC.Container.GetInstance<IDirectory>(),
            IoC.Container.GetInstance<IPlatform>());
}

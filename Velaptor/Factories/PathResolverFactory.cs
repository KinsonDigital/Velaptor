﻿// <copyright file="PathResolverFactory.cs" company="KinsonDigital">
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
    private static IContentPathResolver? texturePathResolver;
    private static IContentPathResolver? atlasPathResolver;
    private static IContentPathResolver? audioPathResolver;
    private static IContentPathResolver? fontPathResolver;
    private static IContentPathResolver? contentFontPathResolver;
    private static IContentPathResolver? systemFontPathResolver;

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
        var platform = IoC.Container.GetInstance<IPlatform>();

        if (platform.CurrentPlatform == OSPlatform.Windows ||
            platform.CurrentPlatform == OSPlatform.Linux ||
            platform.CurrentPlatform == OSPlatform.OSX)
        {
            return CreateSysFontPathResolver();
        }
        else
        {
            throw new NotSupportedException("Currently loading system fonts is only supported on Windows.");
        }
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
        audioPathResolver ??= new AudioPathResolver(IoC.Container.GetInstance<IDirectory>());

    /// <summary>
    /// Creates a path resolver that resolves paths to fonts in the window's font directory.
    /// </summary>
    /// <returns>The resolver instance.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left internal for future access")]
    internal static IContentPathResolver CreateSysFontPathResolver() =>
        systemFontPathResolver ??= new SystemFontPathResolver(
            IoC.Container.GetInstance<IDirectory>(),
            IoC.Container.GetInstance<IPlatform>());
}

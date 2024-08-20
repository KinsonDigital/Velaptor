// <copyright file="ContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Content;
using Content.Caching;
using Content.Factories;
using Content.Fonts;
using Services;

/// <summary>
/// Creates instances of a content loader.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
public static class ContentLoaderFactory
{
    private static ILoader<ITexture>? textureLoader;
    private static ILoader<IAtlasData>? atlasLoader;
    private static ILoader<IAudio>? audioLoader;
    private static ILoader<IFont>? fontLoader;

    /// <summary>
    /// Creates a loader that loads textures from disk.
    /// </summary>
    /// <returns>A loader for loading textures.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for users.")]
    public static ILoader<ITexture> CreateTextureLoader()
    {
        if (textureLoader is not null)
        {
            return textureLoader;
        }

        var cache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var appService = IoC.Container.GetInstance<IAppService>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();
        var platform = IoC.Container.GetInstance<IPlatform>();
        var pathResolver = new TexturePathResolver(appService, file, path, platform);

        var directory = IoC.Container.GetInstance<IDirectory>();
        textureLoader = new TextureLoader(
            cache,
            pathResolver,
            directory,
            path);

        return textureLoader;
    }

    /// <summary>
    /// Creates a loader for loading atlas data from disk.
    /// </summary>
    /// <returns>A loader for loading texture atlas data.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left public for library users.")]
    public static ILoader<IAtlasData> CreateAtlasLoader()
    {
        if (atlasLoader is not null)
        {
            return atlasLoader;
        }

        var cache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var atlasDataFactory = IoC.Container.GetInstance<IAtlasDataFactory>();
        var pathResolver = PathResolverFactory.CreateAtlasPathResolver();
        var jsonService = IoC.Container.GetInstance<IJsonService>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        atlasLoader = new AtlasLoader(
            cache,
            atlasDataFactory,
            pathResolver,
            jsonService,
            directory,
            file,
            path);

        return atlasLoader;
    }

    /// <summary>
    /// Creates a loader that loads audio from disk.
    /// </summary>
    /// <returns>A loader for loading audio data.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for users.")]
    public static ILoader<IAudio> CreateAudioLoader()
    {
        if (audioLoader is not null)
        {
            return audioLoader;
        }

        var cache = IoC.Container.GetInstance<IItemCache<string, IAudio>>();
        var appService = IoC.Container.GetInstance<IAppService>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();
        var platform = IoC.Container.GetInstance<IPlatform>();
        var pathResolver = new AudioPathResolver(appService, file, path, platform);

        var directory = IoC.Container.GetInstance<IDirectory>();
        audioLoader = new AudioLoader(
            cache,
            pathResolver,
            directory,
            file,
            path);

        return audioLoader;
    }

    /// <summary>
    /// Creates a loader that loads fonts from disk for rendering test.
    /// </summary>
    /// <returns>A loader for loading audio data.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Public API for users.")]
    public static ILoader<IFont> CreateFontLoader()
    {
        if (fontLoader is not null)
        {
            return fontLoader;
        }

        var fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
        var embeddedResourceService = IoC.Container.GetInstance<IEmbeddedResourceLoaderService<Stream?>>();
        var pathResolver = PathResolverFactory.CreateFontPathResolver();
        var cache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var fontFactory = IoC.Container.GetInstance<IFontFactory>();
        var fontMetaDataParser = IoC.Container.GetInstance<IFontMetaDataParser>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var fileStream = IoC.Container.GetInstance<IFileStreamFactory>();
        var path = IoC.Container.GetInstance<IPath>();

        fontLoader = new FontLoader(
            fontAtlasService,
            embeddedResourceService,
            pathResolver,
            pathResolver,
            cache,
            fontFactory,
            fontMetaDataParser,
            directory,
            file,
            fileStream,
            path);

        return fontLoader;
    }
}

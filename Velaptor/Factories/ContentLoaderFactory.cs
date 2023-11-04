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
using IVelaptorSound = Content.ISound;

/// <summary>
/// Creates instances of a content loader.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with '{nameof(IoC)}' container.")]
public static class ContentLoaderFactory
{
    private static ILoader<ITexture>? textureLoader;
    private static ILoader<IAtlasData>? atlasLoader;
    private static ILoader<IVelaptorSound>? soundLoader;
    private static ILoader<IFont>? fontLoader;

    /// <summary>
    /// Creates a loader that loads textures from disk.
    /// </summary>
    /// <returns>A loader for loading textures.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    public static ILoader<ITexture> CreateTextureLoader()
    {
        if (textureLoader is not null)
        {
            return textureLoader;
        }

        var textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var texturePathResolver = new TexturePathResolver(IoC.Container.GetInstance<IDirectory>());
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        textureLoader = new TextureLoader(
            textureCache,
            texturePathResolver,
            directory,
            file,
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

        var textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var atlasDataFactory = IoC.Container.GetInstance<IAtlasDataFactory>();
        var atlasDataPathResolver = PathResolverFactory.CreateAtlasPathResolver();
        var jsonService = IoC.Container.GetInstance<IJSONService>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        atlasLoader = new AtlasLoader(
            textureCache,
            atlasDataFactory,
            atlasDataPathResolver,
            jsonService,
            directory,
            file,
            path);

        return atlasLoader;
    }

    /// <summary>
    /// Creates a loader that loads sounds from disk.
    /// </summary>
    /// <returns>A loader for loading sound data.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    public static ILoader<IVelaptorSound> CreateSoundLoader()
    {
        if (soundLoader is not null)
        {
            return soundLoader;
        }

        var soundCache = IoC.Container.GetInstance<IItemCache<string, ISound>>();
        var soundPathResolver = new SoundPathResolver(IoC.Container.GetInstance<IDirectory>());
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        soundLoader = new SoundLoader(
            soundCache,
            soundPathResolver,
            directory,
            file,
            path);

        return soundLoader;
    }

    /// <summary>
    /// Creates a loader that loads fonts from disk for rendering test.
    /// </summary>
    /// <returns>A loader for loading sound data.</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Used by library users.")]
    public static ILoader<IFont> CreateFontLoader()
    {
        if (fontLoader is not null)
        {
            return fontLoader;
        }

        var fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
        var embeddedFontResourceService = IoC.Container.GetInstance<IEmbeddedResourceLoaderService<Stream?>>();
        var contentPathResolver = PathResolverFactory.CreateContentFontPathResolver();
        var fontPathResolver = PathResolverFactory.CreateFontPathResolver();
        var textureCache = IoC.Container.GetInstance<IItemCache<string, ITexture>>();
        var fontFactory = IoC.Container.GetInstance<IFontFactory>();
        var fontMetaDataParser = IoC.Container.GetInstance<IFontMetaDataParser>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var fileStream = IoC.Container.GetInstance<IFileStreamFactory>();
        var path = IoC.Container.GetInstance<IPath>();

        fontLoader = new FontLoader(
            fontAtlasService,
            embeddedFontResourceService,
            contentPathResolver,
            fontPathResolver,
            textureCache,
            fontFactory,
            fontMetaDataParser,
            directory,
            file,
            fileStream,
            path);

        return fontLoader;
    }
}

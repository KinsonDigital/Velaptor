// <copyright file="ContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using Content;
using Fonts;
using Services;
using Velaptor.Factories;

/// <inheritdoc />
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
internal sealed class ContentLoaderFactory : IContentLoaderFactory
{
    private static ITextureLoader? textureLoader;
    private static IAtlasLoader? atlasLoader;
    private static IAudioLoader? audioLoader;
    private static IFontLoader? fontLoader;

    /// <inheritdoc />
    public ITextureLoader CreateTextureLoader()
    {
        if (textureLoader is not null)
        {
            return textureLoader;
        }

        var appService = IoC.Container.GetInstance<IAppService>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();
        var pathResolver = new TexturePathResolver(appService, file, path);

        return CreateTextureLoader(pathResolver);
    }

    /// <inheritdoc />
    public ITextureLoader CreateTextureLoader(IContentPathResolver pathResolver)
    {
        if (textureLoader is not null)
        {
            return textureLoader;
        }

        var imageService = IoC.Container.GetInstance<IImageService>();
        var textureFactory = IoC.Container.GetInstance<ITextureFactory>();
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var path = IoC.Container.GetInstance<IPath>();

        textureLoader = new TextureLoader(
            textureFactory,
            reactableFactory,
            imageService,
            pathResolver,
            directory,
            path);

        return textureLoader;
    }

    /// <inheritdoc />
    public IAtlasLoader CreateAtlasLoader()
    {
        if (atlasLoader is not null)
        {
            return atlasLoader;
        }

        var pathResolverFactory = IoC.Container.GetInstance<IPathResolverFactory>();
        var pathResolver = pathResolverFactory.CreateAtlasPathResolver();

        return CreateAtlasLoader(pathResolver);
    }

    /// <inheritdoc />
    public IAtlasLoader CreateAtlasLoader(IContentPathResolver pathResolver)
    {
        if (atlasLoader is not null)
        {
            return atlasLoader;
        }

        var textureFactory = IoC.Container.GetInstance<ITextureFactory>();
        var atlasDataFactory = IoC.Container.GetInstance<IAtlasDataFactory>();
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        var imageService = IoC.Container.GetInstance<IImageService>();
        var jsonService = IoC.Container.GetInstance<IJsonService>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        atlasLoader = new AtlasLoader(
            textureFactory,
            atlasDataFactory,
            reactableFactory,
            pathResolver,
            imageService,
            jsonService,
            directory,
            file,
            path);

        return atlasLoader;
    }

    /// <inheritdoc />
    public IAudioLoader CreateAudioLoader(IContentPathResolver pathResolver)
    {
        if (audioLoader is not null)
        {
            return audioLoader;
        }

        var audioFactory = IoC.Container.GetInstance<IAudioFactory>();
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();

        audioLoader = new AudioLoader(
            audioFactory,
            reactableFactory,
            pathResolver,
            directory,
            file,
            path);

        return audioLoader;
    }

    /// <inheritdoc />
    public IAudioLoader CreateAudioLoader()
    {
        if (audioLoader is not null)
        {
            return audioLoader;
        }

        var appService = IoC.Container.GetInstance<IAppService>();
        var file = IoC.Container.GetInstance<IFile>();
        var path = IoC.Container.GetInstance<IPath>();
        var platform = IoC.Container.GetInstance<IPlatform>();
        var pathResolver = new AudioPathResolver(appService, file, path, platform);

        return CreateAudioLoader(pathResolver);
    }

    /// <inheritdoc />
    public IFontLoader CreateFontLoader()
    {
        if (fontLoader is not null)
        {
            return fontLoader;
        }

        var pathResolverFactory = IoC.Container.GetInstance<IPathResolverFactory>();
        var pathResolver = pathResolverFactory.CreateFontPathResolver();

        return CreateFontLoader(pathResolver);
    }

    /// <inheritdoc />
    public IFontLoader CreateFontLoader(IContentPathResolver pathResolver)
    {
        if (fontLoader is not null)
        {
            return fontLoader;
        }

        var fontAtlasService = IoC.Container.GetInstance<IFontAtlasService>();
        var embeddedResourceService = IoC.Container.GetInstance<IEmbeddedResourceLoaderService<Stream?>>();
        var textureFactory = IoC.Container.GetInstance<ITextureFactory>();
        var reactableFactory = IoC.Container.GetInstance<IReactableFactory>();
        var fontFactory = IoC.Container.GetInstance<IFontFactory>();
        var directory = IoC.Container.GetInstance<IDirectory>();
        var file = IoC.Container.GetInstance<IFile>();
        var fileStreamFactory = IoC.Container.GetInstance<IFileStreamFactory>();
        var path = IoC.Container.GetInstance<IPath>();

        fontLoader = new FontLoader(
            pathResolver,
            textureFactory,
            reactableFactory,
            fileStreamFactory,
            fontFactory,
            embeddedResourceService,
            fontAtlasService,
            directory,
            file,
            path);

        return fontLoader;
    }
}

// <copyright file="PathResolverFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using Content;
using Content.Fonts;
using Services;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Cannot unit test due to the direct interaction with IoC container.")]
public class PathResolverFactory : IPathResolverFactory
{
    private static IContentPathResolver? texturePathResolver;
    private static IContentPathResolver? atlasPathResolver;
    private static IContentPathResolver? audioPathResolver;
    private static IContentPathResolver? fontPathResolver;

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public IContentPathResolver CreateTexturePathResolver() =>
        texturePathResolver ??= new TexturePathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>());

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public IContentPathResolver CreateAtlasPathResolver() =>
        atlasPathResolver ??= new AtlasTexturePathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>());

    /// <inheritdoc/>
    public IContentPathResolver CreateFontPathResolver() =>
        fontPathResolver ??= new FontPathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>());

    /// <inheritdoc/>
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Left here for future development.")]
    public IContentPathResolver CreateAudioPathResolver() =>
        audioPathResolver ??= new AudioPathResolver(
            IoC.Container.GetInstance<IAppService>(),
            IoC.Container.GetInstance<IFile>(),
            IoC.Container.GetInstance<IPath>(),
            IoC.Container.GetInstance<IPlatform>());
}

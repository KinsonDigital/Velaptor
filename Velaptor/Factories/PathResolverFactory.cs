// <copyright file="PathResolverFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO.Abstractions;
    using System.Runtime.InteropServices;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Creates path resolver instances.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class PathResolverFactory
    {
        private static readonly IPlatform Platform;
        private static IPathResolver? texturePathResolver;
        private static IPathResolver? atlasJSONDataPathResolver;
        private static IPathResolver? soundPathResolver;
        private static IPathResolver? fontPathResolver;

        /// <summary>
        /// Initializes static members of the <see cref="PathResolverFactory"/> class.
        /// </summary>
        static PathResolverFactory() => Platform = IoC.Container.GetInstance<IPlatform>();

        /// <summary>
        /// Creates a path resolver that resolves paths to texture content.
        /// </summary>
        /// <returns>The resolver to texture content.</returns>
        public static IPathResolver CreateTexturePathResolver() =>
            texturePathResolver ??= new TexturePathResolver(IoC.Container.GetInstance<IDirectory>());

        /// <summary>
        /// Creates a path resolver that resolves paths to atlas content.
        /// </summary>
        /// <returns>The resolver to atlas content.</returns>
        public static IPathResolver CreateAtlasJSONDataPathResolver() =>
            atlasJSONDataPathResolver ??= new AtlasJSONDataPathResolver(IoC.Container.GetInstance<IDirectory>());

        /// <summary>
        /// Creates a path resolver that resolves paths to font content.
        /// </summary>
        /// <returns>The resolver to atlas content.</returns>
        public static IPathResolver CreateFontPathResolver()
        {
            var contentPathResolver = new ContentFontPathResolver(IoC.Container.GetInstance<IDirectory>());
            IPathResolver systemFontPathResolver;

            if (Platform.CurrentPlatform == OSPlatform.Windows)
            {
                systemFontPathResolver = new WindowsFontPathResolver(IoC.Container.GetInstance<IDirectory>());
            }
            else
            {
                throw new NotImplementedException("Currently loading system fonts is only supported on Windows.");
            }

            return fontPathResolver ??= new FontPathResolver(
                contentPathResolver,
                systemFontPathResolver,
                IoC.Container.GetInstance<IFile>(),
                IoC.Container.GetInstance<IDirectory>(),
                IoC.Container.GetInstance<IPlatform>());
        }

        /// <summary>
        /// Creates a path resolver that resolves paths to sound content.
        /// </summary>
        /// <returns>The resolver to sound content.</returns>
        public static IPathResolver CreateSoundPathResolver() =>
            soundPathResolver ??= new SoundPathResolver(IoC.Container.GetInstance<IDirectory>());
    }
}

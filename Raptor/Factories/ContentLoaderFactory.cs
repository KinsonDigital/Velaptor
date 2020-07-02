// <copyright file="ContentLoaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Factories
{
    using System.Diagnostics.CodeAnalysis;
    using Raptor.Content;
    using Raptor.Graphics;

    /// <summary>
    /// Creates instances of a raptor content loader.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ContentLoaderFactory
    {
        /// <summary>
        /// Creates a single instance of a raptor content aloder.
        /// </summary>
        /// <returns>A raptor framework content loader implementation.</returns>
        public static IContentLoader CreateContentLoader()
        {
            var textureLoader = IoC.Container.GetInstance<ILoader<ITexture>>();
            var atlasLoader = IoC.Container.GetInstance<ILoader<AtlasRegionRectangle[]>>();

            return new ContentLoader(textureLoader, atlasLoader);
        }
    }
}

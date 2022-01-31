// <copyright file="FontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Fonts;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Graphics;
    using Velaptor.Services;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates <see cref="IFont"/> instances.
    /// </summary>
    [ExcludeFromCodeCoverage]
    internal class FontFactory : IFontFactory
    {
        private const string NullCtorParamMessage = "The parameter must not be null.";
        private readonly IFontService fontService;
        private readonly IFontStatsService fontStatsService;
        private readonly IFontAtlasService fontAtlasService;
        private readonly IItemCache<string, ITexture> textureCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFactory"/> class.
        /// </summary>
        /// <param name="fontService">Helper methods for <c>FreeType</c> like operations.</param>
        /// <param name="fontStatsService">Provides font stat services.</param>
        /// <param name="fontAtlasService">Provides services for building font atlas textures.</param>
        /// <param name="textureCache">Creates and caches textures for later retrieval.</param>
        public FontFactory(
            IFontService fontService,
            IFontStatsService fontStatsService,
            IFontAtlasService fontAtlasService,
            IItemCache<string, ITexture> textureCache)
        {
            this.fontAtlasService = fontAtlasService ?? throw new ArgumentNullException(nameof(fontAtlasService), NullCtorParamMessage);
            this.textureCache = textureCache ?? throw new ArgumentNullException(nameof(textureCache), NullCtorParamMessage);
            this.fontService = fontService ?? throw new ArgumentNullException(nameof(fontService), NullCtorParamMessage);
            this.fontStatsService = fontStatsService ?? throw new ArgumentNullException(nameof(fontStatsService), NullCtorParamMessage);
        }

        /// <inheritdoc/>
        public IFont Create(
            ITexture textureAtlas,
            string name,
            string fontFilePath,
            uint size,
            bool isDefaultFont,
            GlyphMetrics[] glyphMetrics) =>
            new Font(
                textureAtlas,
                this.fontService,
                this.fontStatsService,
                this.fontAtlasService,
                this.textureCache,
                name,
                fontFilePath,
                size,
                isDefaultFont,
                glyphMetrics);
    }
}

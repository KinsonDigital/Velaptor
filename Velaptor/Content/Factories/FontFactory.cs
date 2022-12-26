// <copyright file="FontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using Caching;
using Fonts;
using Velaptor.Content.Fonts.Services;
using Graphics;
using Guards;
using Services;

/// <summary>
/// Generates <see cref="IFont"/> instances.
/// </summary>
internal sealed class FontFactory : IFontFactory
{
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
        EnsureThat.ParamIsNotNull(fontAtlasService);
        EnsureThat.ParamIsNotNull(textureCache);
        EnsureThat.ParamIsNotNull(fontService);
        EnsureThat.ParamIsNotNull(fontStatsService);

        this.fontAtlasService = fontAtlasService;
        this.textureCache = textureCache;
        this.fontService = fontService;
        this.fontStatsService = fontStatsService;
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

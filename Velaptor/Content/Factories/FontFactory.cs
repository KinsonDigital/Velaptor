﻿// <copyright file="FontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System;
using Caching;
using Fonts;
using Fonts.Services;
using Graphics;
using NativeInterop.Services;
using Services;

/// <summary>
/// Generates <see cref="IFont"/> instances.
/// </summary>
internal sealed class FontFactory : IFontFactory
{
    private readonly IFreeTypeService freeTypeService;
    private readonly IFontStatsService fontStatsService;
    private readonly IFontAtlasService fontAtlasService;
    private readonly IItemCache<string, ITexture> textureCache;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontFactory"/> class.
    /// </summary>
    /// <param name="freeTypeService">Provides extensions/helpers to <c>FreeType</c> library functionality.</param>
    /// <param name="fontStatsService">Used to gather stats about content or system fonts.</param>
    /// <param name="fontAtlasService">Creates font atlas textures and glyph metric data.</param>
    /// <param name="textureCache">Creates and caches textures for later retrieval.</param>
    public FontFactory(
        IFreeTypeService freeTypeService,
        IFontStatsService fontStatsService,
        IFontAtlasService fontAtlasService,
        IItemCache<string, ITexture> textureCache)
    {
        ArgumentNullException.ThrowIfNull(fontAtlasService);
        ArgumentNullException.ThrowIfNull(textureCache);
        ArgumentNullException.ThrowIfNull(freeTypeService);
        ArgumentNullException.ThrowIfNull(fontStatsService);

        this.fontAtlasService = fontAtlasService;
        this.textureCache = textureCache;
        this.freeTypeService = freeTypeService;
        this.fontStatsService = fontStatsService;
    }

    /// <inheritdoc/>
    public IFont Create(
        ITexture atlasTexture,
        string name,
        string fontFilePath,
        uint size,
        bool isDefaultFont,
        GlyphMetrics[] glyphMetrics) =>
        new Font(
            atlasTexture,
            this.freeTypeService,
            this.fontStatsService,
            this.fontAtlasService,
            this.textureCache,
            name,
            fontFilePath,
            size,
            isDefaultFont,
            glyphMetrics);
}

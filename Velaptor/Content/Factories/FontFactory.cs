// <copyright file="FontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories;

using System.Diagnostics.CodeAnalysis;
using Fonts;
using Fonts.Services;
using Graphics;
using NativeInterop.Services;

/// <summary>
/// Generates <see cref="IFont"/> instances.
/// </summary>
[ExcludeFromCodeCoverage(Justification = $"Cannot test due to interaction with 'IoC' container.")]
internal sealed class FontFactory : IFontFactory
{
    private readonly IFontStatsService fontStatsService;
    private readonly IFreeTypeService freeTypeService;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontFactory"/> class.
    /// </summary>
    public FontFactory()
    {
        this.fontStatsService = IoC.Container.GetInstance<IFontStatsService>();
        this.freeTypeService = IoC.Container.GetInstance<IFreeTypeService>();
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
            name,
            fontFilePath,
            size,
            isDefaultFont,
            glyphMetrics);
}

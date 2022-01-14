// <copyright file="FontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Content.Fonts;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.FreeType;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="FontFactory"/> class.
        /// </summary>
        /// <param name="fontService">Helper methods for FreeType like operations.</param>
        /// <param name="fontStatsService">Provides font stat services.</param>
        public FontFactory(
            IFontService fontService,
            IFontStatsService fontStatsService)
        {
            this.fontService = fontService ?? throw new ArgumentNullException(nameof(fontService), NullCtorParamMessage);
            this.fontStatsService = fontStatsService ?? throw new ArgumentNullException(nameof(fontStatsService), NullCtorParamMessage);
        }

        /// <inheritdoc/>
        public IFont Create(ITexture textureAtlas, string name, string fontFilePath, int size, GlyphMetrics[] glyphMetrics) =>
            new Font(
                textureAtlas,
                this.fontService,
                this.fontStatsService,
                name,
                fontFilePath,
                size,
                glyphMetrics);
    }
}

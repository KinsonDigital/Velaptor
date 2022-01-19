// <copyright file="IFontFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Factories
{
    // ReSharper disable RedundantNameQualifier
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Generates <see cref="IFont"/> instances.
    /// </summary>
    internal interface IFontFactory
    {
        /// <summary>
        /// Creates an <see cref="IFont"/> object for use for rendering text to the screen.
        /// </summary>
        /// <param name="textureAtlas">The texture atlas of the font face.</param>
        /// <param name="name">The name of the font content item.</param>
        /// <param name="fontFilePath">The file path to the font file.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="glyphMetrics">The metrics of all the font glyphs.</param>
        /// <returns>The <see cref="IFont"/> instance.</returns>
        IFont Create(ITexture textureAtlas, string name, string fontFilePath, uint size, GlyphMetrics[] glyphMetrics);
    }
}

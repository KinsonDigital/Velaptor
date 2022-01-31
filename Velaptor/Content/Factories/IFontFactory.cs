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
        /// Creates an <see cref="IFont"/> object to use when rendering text to the screen.
        /// </summary>
        /// <param name="textureAtlas">The texture atlas of the font face.</param>
        /// <param name="name">The name of the font content item.</param>
        /// <param name="fontFilePath">The file path to the font file.</param>
        /// <param name="size">The size of the font.</param>
        /// <param name="isDefaultFont">True if the font loaded is the default font.</param>
        /// <param name="glyphMetrics">The metrics of all the font glyphs.</param>
        /// <returns>The <see cref="IFont"/> instance.</returns>
        /// <remarks>
        ///     The default font is Times New Roman with all included styles.
        /// </remarks>
        IFont Create(ITexture textureAtlas, string name, string fontFilePath, uint size, bool isDefaultFont, GlyphMetrics[] glyphMetrics);
    }
}

// <copyright file="GraphicsExtensionMethods.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics
{
    using System.Collections.Generic;
    using System.Linq;
    using Velaptor;

    /// <summary>
    /// Provides various helper extension methods.
    /// </summary>
    public static class GraphicsExtensionMethods
    {
        /// <summary>
        /// Gets the max height from the list of glyph metrics at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="value">The list of glyph metric items.</param>
        /// <param name="index">The index of the list of glyph metric lists to get the offset from.</param>
        /// <returns>The maximum height.</returns>
        public static float MaxHeight(this List<GlyphMetrics[]> value, int index)
            => value[index].Max(i => i.GlyphHeight);

        /// <summary>
        /// Returns the maximum vertical offset out of all the given glyphs at the given <paramref name="index"/>.
        /// </summary>
        /// <param name="value">The list of glyph metric items.</param>
        /// <param name="index">The index of the list of glyph metric lists to get the offset from.</param>
        /// <returns>The maximum vertical offset.</returns>
        public static float MaxVerticalOffset(this List<GlyphMetrics[]> value, int index)
            => value.ToArray()[index]
                .Max(i => i.GlyphHeight - i.HoriBearingY > 0
                    ? i.GlyphHeight - i.HoriBearingY
                    : 0);
    }
}

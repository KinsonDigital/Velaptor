// <copyright file="FontCacheEntry.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable IDE0036 // Order modifiers
#pragma warning disable SA1401 // Fields should be private

namespace Velaptor.Content.Fonts;

using Velaptor.Content;
using Velaptor.Graphics;

/// <summary>
/// Holds a cached font atlas entry with a reference count to track how many
/// <see cref="IFont"/> instances share the same atlas texture.
/// </summary>
internal record FontCacheEntry
{
    /// <summary>
    /// Gets the number of <see cref="IFont"/> instances referencing this cache entry.
    /// When this reaches zero, the atlas texture is disposed and the entry is removed.
    /// </summary>
    public int RefCount;

    /// <summary>
    /// Gets the font atlas texture containing bitmap data for all glyphs.
    /// </summary>
    required public ITexture FontTextureAtlas { get; init; }

    /// <summary>
    /// Gets the glyph metrics for all characters in the atlas.
    /// </summary>
    required public GlyphMetrics[] Metrics { get; init; }
}

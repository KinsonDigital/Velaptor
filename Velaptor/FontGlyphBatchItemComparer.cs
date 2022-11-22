// <copyright file="FontGlyphBatchItemComparer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using Velaptor.OpenGL;

namespace Velaptor;

/// <summary>
/// Used to compare 2 <see cref="FontGlyphBatchItem"/>'s for the purpose of sorting.
/// </summary>
internal sealed class FontGlyphBatchItemComparer : IComparer<FontGlyphBatchItem>
{
    /// <summary>
    /// Compares two <see cref="FontGlyphBatchItem"/>'s.
    /// Returns a value less than zero if the item's layer <paramref name="x"/> is less
    /// than the layer for item <paramref name="y"/>, zero if the layers for both <paramref name="x"/>
    /// and <paramref name="y"/> are equal, or a value greater than zero if the layer for item <paramref name="x"/>
    /// greater than the layer for item <paramref name="y"/>.
    /// </summary>
    /// <param name="x">The item to compare to <paramref name="y"/>.</param>
    /// <param name="y">The item to compare to <paramref name="x"/>.</param>
    /// <returns>A value representing the comparison result.</returns>
    public int Compare(FontGlyphBatchItem x, FontGlyphBatchItem y) => x.Layer.CompareTo(y.Layer);
}

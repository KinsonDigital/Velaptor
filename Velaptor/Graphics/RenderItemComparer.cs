// <copyright file="RenderItemComparer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Graphics;

using System.Collections.Generic;
using OpenGL.Batching;

/// <summary>
/// Uses a binary search type of comparison between 2 <see cref="RenderItem{T}"/> objects
/// for the purpose of sorting them in ascending order by <see cref="RenderItem{T}"/>.<see cref="RenderItem{T}.Layer"/>.
/// </summary>
/// <typeparam name="T">The type of batch item.</typeparam>
internal sealed class RenderItemComparer<T> : IComparer<RenderItem<T>>
{
    /// <summary>
    /// Compares two <see cref="RenderItem{T}"/>s. Returns a
    /// value less than zero if the <paramref name="x"/> layer is less than
    /// the <paramref name="y"/> layer, zero if <paramref name="x"/> layer
    /// is equal to the <paramref name="y"/> layer, or a value greater than zero if
    /// <paramref name="x"/> layer is greater than the <paramref name="y"/> layer.
    /// </summary>
    /// <param name="x">The left side of the comparison.</param>
    /// <param name="y">The right side of the comparison.</param>
    /// <returns>The numerical value representing how to sort the items <paramref name="x"/> and <paramref name="y"/>.</returns>
    public int Compare(RenderItem<T> x, RenderItem<T> y)
    {
        if (x.Layer != y.Layer)
        {
            return x.Layer < y.Layer ? -1 : 1;
        }

        if (x.RenderStamp == y.RenderStamp)
        {
            return 0;
        }

        return x.RenderStamp < y.RenderStamp ? -1 : 1;
    }
}

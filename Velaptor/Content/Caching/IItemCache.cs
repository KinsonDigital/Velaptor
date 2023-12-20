// <copyright file="IItemCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching;

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Caches items for retrieval at a later time.
/// </summary>
/// <typeparam name="TCacheKey">The unique key assigned for a particular cached item.</typeparam>
/// <typeparam name="TCacheType">The type of item being cached.</typeparam>
public interface IItemCache<in TCacheKey, out TCacheType>
{
    /// <summary>
    /// Gets the total number of cached items.
    /// </summary>
    [SuppressMessage("ReSharper", "UnusedMemberInSuper.Global", Justification = "Used by library users.")]
    int TotalCachedItems { get; }

    /// <summary>
    /// Gets the list of all cache keys.
    /// </summary>
    IReadOnlyCollection<string> CacheKeys { get; }

    /// <summary>
    /// Gets a cached item that matches the given <paramref name="cacheKey"/>.
    /// </summary>
    /// <param name="cacheKey">The unique key to identify a cached item.</param>
    /// <returns>The cached item.</returns>
    /// <remarks>
    /// <para>If the item does not already exist in the cache, it gets created, then cached.</para>
    /// <para>If the item does already exist in the cache, then that cached item is returned.</para>
    /// </remarks>
    TCacheType GetItem(TCacheKey cacheKey);

    /// <summary>
    /// Unloads a cached item that matches the given <paramref name="cacheKey"/>.
    /// </summary>
    /// <param name="cacheKey">The unique key to identify a cached item.</param>
    void Unload(TCacheKey cacheKey);
}

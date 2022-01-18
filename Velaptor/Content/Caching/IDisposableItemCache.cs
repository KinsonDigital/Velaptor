// <copyright file="IDisposableItemCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content.Caching
{
    using System;

    /// <summary>
    /// Caches disposable items for retrieval at a later time.
    /// </summary>
    /// <typeparam name="TCacheKey">The unique assigned for a particular cached item.</typeparam>
    /// <typeparam name="TCacheType">The type of item being cached.</typeparam>
    public interface IDisposableItemCache<TCacheKey, out TCacheType> : IItemCache<TCacheKey, TCacheType>, IDisposable
        where TCacheType : IDisposable
    {
    }
}

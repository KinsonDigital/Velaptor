// <copyright file="IDisposableItemCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;

    public interface IDisposableItemCache<in TCacheKey, out TCacheType> : IItemCache<TCacheKey, TCacheType>, IDisposable
        where TCacheType : IDisposable
    {
    }
}

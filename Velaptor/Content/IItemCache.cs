// <copyright file="IItemCache.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    public interface IItemCache<in TCacheKey, out TCacheType>
    {
        TCacheType GetItem(TCacheKey cacheKey);

        void Unload(TCacheKey key);
    }
}

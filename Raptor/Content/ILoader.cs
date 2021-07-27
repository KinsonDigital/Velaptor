// <copyright file="ILoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content
{
    using System;

    /// <summary>
    /// Loads data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public interface ILoader<T> : IDisposable
        where T : IContent
    {
        /// <summary>
        /// Loads data with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content of the data to load.</param>
        /// <returns>The data loaded from disk.</returns>
        T Load(string name);

        /// <summary>
        /// Unloads the data with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the content item to unload.</param>
        void Unload(string name);
    }
}

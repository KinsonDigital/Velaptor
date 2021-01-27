// <copyright file="ILoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.Content
{
    /// <summary>
    /// Loads data of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of data to load.</typeparam>
    public interface ILoader<T>
        where T : IContent
    {
        /// <summary>
        /// Loads data at the given <paramref name="contentNameOrPath"/>.
        /// </summary>
        /// <param name="contentNameOrPath">The name or path to the content of the data to load.</param>
        /// <returns>The data loaded from disk.</returns>
        T Load(string contentNameOrPath);
    }
}

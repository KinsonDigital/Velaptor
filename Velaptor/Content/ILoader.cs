// <copyright file="ILoader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Loads data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data to load.</typeparam>
public interface ILoader<out T>
    where T : IContent
{
    /// <summary>
    /// Loads data with the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content of the data to load.</param>
    /// <returns>The data loaded from disk.</returns>
    internal T Load(string contentPathOrName);

    /// <summary>
    /// Unloads the data with the given <paramref name="contentPathOrName"/>.
    /// </summary>
    /// <param name="contentPathOrName">The name of the content item to unload.</param>
    void Unload(string contentPathOrName);
}

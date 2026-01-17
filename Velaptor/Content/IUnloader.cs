// <copyright file="IUnloader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Content;

/// <summary>
/// Unloads data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data to load.</typeparam>
internal interface IUnloader<in T>
    where T : IContent
{
    /// <summary>
    /// Unloads the data with the given <paramref name="item"/>.
    /// </summary>
    /// <param name="item">The content item to unload.</param>
    void Unload(T item);
}

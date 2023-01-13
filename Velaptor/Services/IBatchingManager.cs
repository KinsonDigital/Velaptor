// <copyright file="IBatchingManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System.Collections.ObjectModel;

/// <summary>
/// Manages the process of batching items.
/// </summary>
/// <typeparam name="T">The type of items stored in the batch.</typeparam>
internal interface IBatchingManager<T>
{
    /// <summary>
    /// Gets or sets the list of batch items.
    /// </summary>
    ReadOnlyCollection<T> BatchItems { get; set; }

    /// <summary>
    /// Adds the given <paramref name="item"/> to the batch.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    void Add(in T item);

    /// <summary>
    /// Empties the entire batch.
    /// </summary>
    void EmptyBatch();
}

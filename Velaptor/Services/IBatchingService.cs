// <copyright file="IBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services;

using System;
using System.Collections.ObjectModel;

/// <summary>
/// Manages the process of batching items.
/// </summary>
/// <typeparam name="T">The type of items stored in the batch.</typeparam>
internal interface IBatchingService<T>
{
    /// <summary>
    /// Occurs when the batch is ready for rendering.
    /// </summary>
    /// <remarks>
    ///     The batch is ready when all of the items are ready to be rendered.
    /// </remarks>
    event EventHandler<EventArgs>? ReadyForRendering;

    /// <summary>
    /// Gets or sets the list of batch items.
    /// </summary>
    ReadOnlyCollection<(bool shouldRender, T item)> BatchItems { get; set; }

    /// <summary>
    /// Adds the given <paramref name="item"/> to the batch.
    /// </summary>
    /// <param name="item">The item to be added.</param>
    void Add(T item);

    /// <summary>
    /// Empties the entire batch.
    /// </summary>
    void EmptyBatch();
}

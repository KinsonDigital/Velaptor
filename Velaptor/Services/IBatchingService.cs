// <copyright file="IBatchingService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Manages the process of batching items.
    /// </summary>
    /// <typeparam name="T">The type of items stored in the batch.</typeparam>
    internal interface IBatchingService<T>
    {
        /// <summary>
        /// Occurs when a batch is full.
        /// </summary>
        /// <remarks>
        /// Scenarios When The Batch Is Ready:
        /// <list type="number">
        ///     <item>The batch is ready when draw calls switch to another circle.</item>
        ///     <item>The batch is ready when the total amount of render calls have reached the <see cref="BatchSize"/>.</item>
        /// </list>
        /// </remarks>
        event EventHandler<EventArgs>? BatchFilled;

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        uint BatchSize { get; set; }

        /// <summary>
        /// Gets or sets the list of batch items.
        /// </summary>
        ReadOnlyDictionary<uint, (bool shouldRender, T item)> BatchItems { get; set; }

        /// <summary>
        /// Adds the given <paramref name="item"/> to the batch.
        /// </summary>
        /// <param name="item">The item to be added.</param>
        void Add(T item);

        /// <summary>
        /// Adds the given list of <paramref name="items"/> to batch.
        /// </summary>
        /// <param name="items">The items to be added.</param>
        void AddRange(IEnumerable<T> items);

        /// <summary>
        /// Empties the entire batch.
        /// </summary>
        void EmptyBatch();
    }
}

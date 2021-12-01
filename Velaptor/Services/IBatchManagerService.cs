// <copyright file="IBatchManagerService_NEW.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Services
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Manages the process of batching textures together when rendering them.
    /// </summary>
    internal interface IBatchManagerService<T>
    {
        /// <summary>
        /// Occurs when a batch is ready to be rendered.
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
        /// Gets the list of batch items.
        /// </summary>
        /// <remarks>
        ///     Represents a list of items that are ready or not ready to be rendered.
        /// </remarks>
        ReadOnlyDictionary<uint, (bool shouldRender, T item)> AllBatchItems { get; }

        /// <summary>
        /// Gets the total number of batch items that are ready for rendering.
        /// </summary>
        uint TotalItemsToRender { get; }

        /// <summary>
        /// Gets a value indicating whether the entire batch is ready for rendering.
        /// </summary>
        /// <returns>True if every batch item is ready.</returns>
        bool BatchEmpty { get; }

        ReadOnlyCollection<(uint batchIndex, T item)> RenderableItems { get;  }

        void Add(T rect);

        /// <summary>
        /// Empties the entire batch.
        /// </summary>
        void EmptyBatch();
    }
}

// <copyright file="BatchSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Reactables.ReactableData;

/// <summary>
/// Holds data for the <see cref="BatchSizeReactable"/> reactable.
/// </summary>
internal readonly struct BatchSizeData
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BatchSizeData"/> struct.
    /// </summary>
    /// <param name="batchSize">The size of a batch.</param>
    public BatchSizeData(in uint batchSize) => BatchSize = batchSize;

    /// <summary>
    /// Gets the data.
    /// </summary>
    public uint BatchSize { get; }
}

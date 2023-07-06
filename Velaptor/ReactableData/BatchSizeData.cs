// <copyright file="BatchSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds data for the size of the batch.
/// </summary>
internal readonly record struct BatchSizeData
{
    /// <summary>
    /// Gets the data.
    /// </summary>
    public uint BatchSize { get; init; }

    /// <summary>
    /// Gets the type of batch.
    /// </summary>
    public BatchType TypeOfBatch { get; init; }
}

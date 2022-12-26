// <copyright file="BatchSizeData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

using Carbonate;

/// <summary>
/// Holds data for the <see cref="Reactable"/> reactable.
/// </summary>
internal record BatchSizeData
{
    /// <summary>
    /// Gets the data.
    /// </summary>
    public uint BatchSize { get; init; }
}

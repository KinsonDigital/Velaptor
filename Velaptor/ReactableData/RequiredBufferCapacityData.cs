// <copyright file="RequiredBufferCapacityData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.ReactableData;

/// <summary>
/// Holds the data for the buffer capacities for all buffer types.
/// </summary>
internal readonly record struct RequiredBufferCapacityData
{
    /// <summary>
    /// Gets the total number of texture items to render during a single frame.
    /// </summary>
    public uint TotalTextureItems { get; init; }

    /// <summary>
    /// Gets the total number of font items to render during a single frame.
    /// </summary>
    public uint TotalFontItems { get; init; }

    /// <summary>
    /// Gets the total number of shape items to render during a single frame.
    /// </summary>
    public uint TotalShapeItems { get; init; }

    /// <summary>
    /// Gets the total number of line items to render during a single frame.
    /// </summary>
    public uint TotalLineItems { get; init; }
}

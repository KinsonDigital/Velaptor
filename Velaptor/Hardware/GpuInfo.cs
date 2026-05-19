// <copyright file="GpuInfo.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents information about a GPU.
/// </summary>
/// <param name="gpuId">The ID of the GPU.</param>
/// <param name="model">The name of the GPU.</param>
/// <param name="ram">The RAM of the GPU in GB.</param>
/// <param name="ramType">The architecture type of the RAM.</param>
[ExcludeFromCodeCoverage(Justification = "Very minimal properties with no logic.")]
internal readonly struct GpuInfo(
    uint gpuId,
    string model,
    float ram,
    RamType ramType)
{
    /// <summary>
    /// Gets the ID of the GPU.
    /// </summary>
    public uint GpuId { get; init; } = gpuId;

    /// <summary>
    /// Gets the name of the GPU.
    /// </summary>
    public string Model { get; init; } = model;

    /// <summary>
    /// Gets the RAM of the GPU in GB.
    /// </summary>
    public float Ram { get; init; } = ram;

    /// <summary>
    /// Gets the architecture type of the RAM.
    /// </summary>
    public RamType RamType { get; init; } = ramType;
}

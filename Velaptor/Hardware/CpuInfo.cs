// <copyright file="CpuInfo.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents information about a CPU.
/// </summary>
/// <param name="model">The model of the CPU.</param>
/// <param name="physicalCores">The total number of physical cores of the CPU.</param>
/// <param name="logicalCores">The total number of logical cores of the CPU.</param>
/// <param name="maxClockSpeed">The max clock speed of the CPU in GHz.</param>
[ExcludeFromCodeCoverage(Justification = "Very minimal properties with no logic.")]
internal readonly struct CpuInfo(string model, uint physicalCores, uint logicalCores, float maxClockSpeed)
{
    /// <summary>
    /// Gets the CPU model name.
    /// </summary>
    public string Model { get; init; } = model;

    /// <summary>
    /// Gets the total number of physical cores.
    /// </summary>
    public uint PhysicalCores { get; init; } = physicalCores;

    /// <summary>
    /// Gets the total number of logical cores.
    /// </summary>
    public uint LogicalCores { get; init; } = logicalCores;

    /// <summary>
    /// Gets the maximum clock speed of the CPU in GHz.
    /// </summary>
    public float MaxClockSpeed { get; init; } = maxClockSpeed;
}

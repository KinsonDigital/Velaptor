// <copyright file="HardwareData.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Telemetry;

using System.Diagnostics.CodeAnalysis;
using Hardware;

/// <summary>
/// Represents a user's hardware data.
/// </summary>
/// <param name="cpu">The CPU information.</param>
/// <param name="gpus">The GPU information.</param>
[ExcludeFromCodeCoverage(Justification = "Very minimal properties with no logic.")]
internal readonly struct HardwareData(CpuInfo cpu, GpuInfo[] gpus)
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// Gets the CPU information.
    /// </summary>
    public CpuInfo Cpu { get; init; } = cpu;

    /// <summary>
    /// Gets the GPU information.
    /// </summary>
    public GpuInfo[] Gpus { get; init; } = gpus;

    // ReSharper disable UnusedAutoPropertyAccessor.Global
}

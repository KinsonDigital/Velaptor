// <copyright file="IGpuService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware.Services;

using System.Collections.Generic;

/// <summary>
/// Provides services related to get information about the GPU.
/// </summary>
internal interface IGpuService
{
    /// <summary>
    /// Gets information about the GPU based on the operating system.
    /// </summary>
    /// <returns>Information about the GPU.</returns>
    IReadOnlyCollection<GpuInfo> GetGpuInfo();
}

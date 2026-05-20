// <copyright file="ICpuService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware.Services;

/// <summary>
/// Provides services related to get information about the CPU.
/// </summary>
internal interface ICpuService
{
    /// <summary>
    /// Gets information about the CPU based on the operating system.
    /// </summary>
    /// <returns>Information about the CPU.</returns>
    CpuInfo GetCpuInfo();
}

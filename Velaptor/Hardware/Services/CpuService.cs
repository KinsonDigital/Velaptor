// <copyright file="CpuService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware.Services;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using NativeInterop.MacOS;
using HardwareInfo = global::Hardware.Info.HardwareInfo;

/// <inheritdoc/>
[ExcludeFromCodeCoverage(Justification = "Not worth testing due to library calls.")]
internal class CpuService : ICpuService
{
    private readonly HardwareInfo hw = new ();
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="CpuService"/> class.
    /// </summary>
    /// <param name="platform">Provides information about the current platform.</param>
    /// <exception cref="ArgumentNullException">Thrown if the parameter <paramref name="platform"/> is null.</exception>
    public CpuService(IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);

        this.platform = platform;
    }

    /// <inheritdoc/>
    public CpuInfo GetCpuInfo()
    {
        if (this.platform.CurrentPlatform == OSPlatform.OSX)
        {
            return MacCpu.IsArmMac() ? MacCpu.GetArmCpuInfo() : MacCpu.GetIntelCpuInfo();
        }

        return GetWinLinuxCpuInfo();
    }

    /// <summary>
    /// Gets CPU information for Windows and Linux.
    /// </summary>
    /// <returns>The CPU information.</returns>
    private CpuInfo GetWinLinuxCpuInfo()
    {
        this.hw.RefreshCPUList(includePercentProcessorTime: false);
        var cpuName = string.Empty;
        var totalPhysicalCores = 0u;
        var totalLogicalProcessors = 0u;
        var cpuClockSpeed = 0f;

        foreach (var cpu in this.hw.CpuList)
        {
            totalPhysicalCores += cpu.NumberOfCores;
            totalLogicalProcessors += cpu.NumberOfLogicalProcessors;
            cpuName = cpu.Name;
            cpuClockSpeed = cpu.MaxClockSpeed / 1000f;
        }

        return new CpuInfo(cpuName, totalPhysicalCores, totalLogicalProcessors, cpuClockSpeed);
    }
}

// <copyright file="GpuService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware.Services;

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NativeInterop.MacOS;
using HardwareInfo = global::Hardware.Info.HardwareInfo;

/// <inheritdoc/>
internal sealed class GpuService : IGpuService
{
    private readonly HardwareInfo hw = new ();
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="GpuService"/> class.
    /// </summary>
    /// <param name="platform">Provides information about the current platform.</param>///
    public GpuService(IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        this.platform = platform;
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<GpuInfo> GetGpuInfo() => this.platform.CurrentPlatform == OSPlatform.OSX ? GetMacGpu() : GetWinLinuxGpuInfo();

    /// <summary>
    /// Gets information about the mac-based GPU.
    /// </summary>
    /// <returns>Information about the GPU.</returns>
    private IReadOnlyCollection<GpuInfo> GetMacGpu()
    {
        var gpu = MacGpu.GetGpu();

        if (gpu is null)
        {
            return [];
        }

        return [(GpuInfo)gpu];
    }

    /// <summary>
    /// Gets information about the Windows/Linux-based GPU.
    /// </summary>
    /// <returns>Information about the GPU.</returns>
    private IReadOnlyCollection<GpuInfo> GetWinLinuxGpuInfo()
    {
        this.hw.RefreshVideoControllerList();

        var displays = new List<GpuInfo>(this.hw.VideoControllerList.Count);
        for (var i = 0; i < this.hw.VideoControllerList.Count; i++)
        {
            var gpu = this.hw.VideoControllerList[i];
            var ramGb = gpu.AdapterRAM / (1024f * 1024f * 1024f);

            displays.Add(new GpuInfo((uint)(i + 1), gpu.Name, ramGb, RamType.Discrete));
        }

        return displays.AsReadOnly();
    }
}

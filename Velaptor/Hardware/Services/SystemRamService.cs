// <copyright file="SystemRamService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Hardware.Services;

using System;
using System.Runtime.InteropServices;
using NativeInterop.MacOS;
using HardwareInfo = global::Hardware.Info.HardwareInfo;

/// <summary>
/// Provides services to get information about the system memory.
/// </summary>
internal class SystemRamService
{
    private readonly HardwareInfo hw = new ();
    private readonly IPlatform platform;

    /// <summary>
    /// Initializes a new instance of the <see cref="SystemRamService"/> class.
    /// </summary>
    /// <param name="platform">Provides information about the current platform.</param>///
    public SystemRamService(IPlatform platform)
    {
        ArgumentNullException.ThrowIfNull(platform);
        this.platform = platform;
    }

    /// <summary>
    /// Gets the total amount of memory in GB.
    /// </summary>
    /// <returns>The total amount of memory in GB.</returns>
    public float GetTotalMemory() => this.platform.CurrentPlatform == OSPlatform.OSX ? GetMacOsMemory() : GetWinLinuxMemory();

    /// <summary>
    /// Gets the total amount of system memory in GB for macOS.
    /// </summary>
    /// <returns>The total amount of memory.</returns>
    /// <exception cref="ExternalException">Thrown if there is a problem with getting the memory.</exception>
    private static float GetMacOsMemory()
    {
        ulong totalMemory = 0;
        nint size = sizeof(ulong);
        var result = LibC.sysctlbyname("hw.memsize", ref totalMemory, ref size, nint.Zero, 0);

        if (result != 0)
        {
            throw new ExternalException("Issue getting memory for macOS machine using 'hw.memsize'.", Marshal.GetLastPInvokeError());
        }

        return totalMemory / (1024f * 1024f * 1024f);
    }

    /// <summary>
    /// Gets the total amount of system memory in GB for Windows/Linux.
    /// </summary>
    /// <returns>The total amount of memory.</returns>
    private float GetWinLinuxMemory()
    {
        this.hw.RefreshMemoryList();

        ulong totalMemoryBytes = 0;

        foreach (var mem in this.hw.MemoryList)
        {
            totalMemoryBytes += mem.Capacity;
        }

        return totalMemoryBytes / (1024f * 1024f * 1024f);
    }
}

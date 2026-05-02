// <copyright file="MacCpu.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.MacOS;

using System;
using System.Runtime.InteropServices;
using Hardware;

/// <summary>
/// Provides macOS-specific CPU detection and information retrieval via <c>sysctl</c> system calls.
/// </summary>
internal static class MacCpu
{
    /// <summary>
    /// Determines whether the current Mac is running on Apple Silicon (ARM64).
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the CPU is Apple Silicon (M-series); otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Queries the <c>hw.optional.arm64</c> sysctl key. A value of <c>1</c> indicates Apple Silicon.
    /// </remarks>
    /// <exception cref="ExternalException">
    /// Thrown when the <c>hw.optional.arm64</c> sysctl call fails. The error code is retrieved
    /// via <see cref="Marshal.GetLastPInvokeError"/>.
    /// </exception>
    internal static bool IsArmMac()
    {
        // hw.optional.arm64 == 1 means Apple Silicon (M series)
        var isArm = 0;
        nint size = sizeof(int);
        var result = LibC.sysctlbyname("hw.optional.arm64", ref isArm, ref size, nint.Zero, 0);

        if (result == 0)
        {
            return isArm == 1;
        }

        const string exMsg = "There was an issue getting CPU information for macOS using 'hw.optional.arm64'.";

        throw new ExternalException(exMsg, Marshal.GetLastPInvokeError());
    }

    /// <summary>
    /// Retrieves CPU information for an Apple Silicon (ARM64) Mac.
    /// </summary>
    /// <returns>
    /// A <see cref="CpuInfo"/> instance populated with the CPU brand name and physical core count.
    /// ARM-based Apple Silicon does not expose a discrete max clock speed via sysctl,
    /// so the clock speed is reported as <c>0</c>.
    /// </returns>
    /// <remarks>
    /// Queries <c>machdep.cpu.brand_string</c> for the CPU name and <c>hw.physicalcpu</c> for
    /// core counts. Because Apple Silicon uses an efficiency/performance core model without a
    /// single advertised frequency, logical core count is set equal to physical core count.
    /// </remarks>
    internal static CpuInfo GetArmCpuInfo()
    {
        var cpuName = GetCpuName();
        var physicalCores = GetSysctlInt("hw.physicalcpu");

        return new CpuInfo(cpuName, (uint)physicalCores, (uint)physicalCores, 0u);
    }

    /// <summary>
    /// Retrieves CPU information for an Intel-based Mac.
    /// </summary>
    /// <returns>
    /// A <see cref="CpuInfo"/> instance populated with the CPU brand name, physical and logical
    /// core counts, and maximum clock speed in GHz.
    /// </returns>
    /// <remarks>
    /// Queries the following sysctl keys:
    /// <list type="bullet">
    ///   <item><description><c>hw.cpufrequency_max</c> — maximum CPU frequency in Hz, converted to GHz.</description></item>
    ///   <item><description><c>machdep.cpu.brand_string</c> — CPU brand name.</description></item>
    ///   <item><description><c>hw.physicalcpu</c> — physical core count.</description></item>
    ///   <item><description><c>hw.logicalcpu</c> — logical core count (includes Hyper-Threading).</description></item>
    /// </list>
    /// </remarks>
    /// <exception cref="ExternalException">
    /// Thrown when the <c>hw.cpufrequency_max</c> sysctl call fails. The error code is retrieved
    /// via <see cref="Marshal.GetLastPInvokeError"/>.
    /// </exception>
    internal static CpuInfo GetIntelCpuInfo()
    {
        ulong freq = 0;
        nint size = sizeof(ulong);
        var result = LibC.sysctlbyname("hw.cpufrequency_max", ref freq, ref size, nint.Zero, 0);

        if (result != 0)
        {
            const string exMsg = "There was an issue getting CPU information for macOS using 'hw.cpufrequency_max'.";

            throw new ExternalException(exMsg, Marshal.GetLastPInvokeError());
        }

        var cpuName = GetCpuName();
        var physicalCores = GetSysctlInt("hw.physicalcpu");
        var logicalCores = GetSysctlInt("hw.logicalcpu");
        var maxClockSpeedGhz = freq / 1_000_000_000.0f;

        return new CpuInfo(cpuName, (uint)physicalCores, (uint)logicalCores, maxClockSpeedGhz);
    }

    /// <summary>
    /// Retrieves the CPU brand string from the <c>machdep.cpu.brand_string</c> sysctl key.
    /// </summary>
    /// <returns>
    /// The CPU brand name as reported by the OS (e.g. <c>"Intel(R) Core(TM) i9-9980HK CPU @ 2.40GHz"</c>),
    /// or <c>"Unknown"</c> if the sysctl call fails or returns an empty result.
    /// </returns>
    /// <remarks>
    /// Performs two sysctl calls: the first to determine the required buffer size, and the second
    /// to populate the buffer. Unmanaged memory is allocated via <see cref="Marshal.AllocHGlobal(int)"/>
    /// and is always freed in a <see langword="finally"/> block regardless of the outcome.
    /// </remarks>
    private static string GetCpuName()
    {
        // Try to get CPU name from sysctl
        nint size = 0;

        // First, get the size
        var result = LibC.sysctlbyname("machdep.cpu.brand_string", IntPtr.Zero, ref size, nint.Zero, 0);

        if (result != 0 || size == 0)
        {
            return "Unknown";
        }

        var buffer = Marshal.AllocHGlobal((int)size);

        try
        {
            result = LibC.sysctlbyname("machdep.cpu.brand_string", buffer, ref size, nint.Zero, 0);

            if (result != 0)
            {
                return "Unknown";
            }

            return Marshal.PtrToStringAnsi(buffer) ?? "Unknown";
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <summary>
    /// Reads an integer value from the specified sysctl key.
    /// </summary>
    /// <param name="key">The sysctl key to query (e.g. <c>"hw.physicalcpu"</c>, <c>"hw.logicalcpu"</c>).</param>
    /// <returns>
    /// The integer value associated with <paramref name="key"/>, or <c>0</c> if the sysctl call fails.
    /// </returns>
    private static int GetSysctlInt(string key)
    {
        var value = 0;
        nint size = sizeof(int);
        LibC.sysctlbyname(key, ref value, ref size, nint.Zero, 0);

        return value;
    }
}

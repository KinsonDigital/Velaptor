// <copyright file="MacGpu.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.MacOS;

using static IOKit;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Hardware;
using Hardware.Services;

/// <summary>
/// Represents a GPU on Apple Silicon Macs.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with native macOS interop.")]
internal static class MacGpu
{
    /// <summary>
    /// Gets GPU information for Apple Silicon Macs.
    /// </summary>
    /// <returns>The list of GPU information.</returns>
    public static GpuInfo? GetGpu()
    {
        if (RuntimeInformation.ProcessArchitecture != Architecture.Arm64)
        {
            return null;
        }

        var platform = IoC.Container.GetInstance<IPlatform>();
        var memory = new SystemRamService(platform);
        var ram = memory.GetTotalMemory();
        var name = GetGpuName() ?? "Apple Silicon GPU";

        return new GpuInfo(1u, name, ram, RamType.Unified);
    }

    /// <summary>
    /// Reads the GPU model name from the AGXAccelerator IOKit service.
    /// The "model" property contains a human-readable string such as
    /// "Apple M1", "Apple M2 Pro", or "Apple M3 Max".
    /// Returns null if the service or property cannot be found.
    /// </summary>
    private static string? GetGpuName()
    {
        // Build a query dictionary that tells IOKit we want services of class "AGXAccelerator".
        // AGXAccelerator is Apple's GPU driver service — it exists on every Apple Silicon Mac.
        // Note: IOServiceMatching transfers ownership of this dict to IOServiceGetMatchingServices,
        // so we must NOT call CFRelease on it ourselves.
        var matching = IOServiceMatching("AGXAccelerator");

        if (matching == IntPtr.Zero)
        {
            return null;
        }

        // Run the query. On success, iterator is an IOKit handle we walk to visit each result.
        // Pass 0 for masterPort — that is kIOMainPortDefault (the default IOKit connection).
        if (IOServiceGetMatchingServices(0, matching, out var iterator) != 0)
        {
            return null;
        }

        try
        {
            uint service;

            // IOIteratorNext returns the next matching service handle, or 0 when exhausted.
            // Each non-zero handle must be released via IOObjectRelease when we're done with it.
            while ((service = IOIteratorNext(iterator)) != 0)
            {
                try
                {
                    // We need to pass the property key as a CFString (Apple's string type).
                    // "model" is the IOKit property name that holds the chip name, e.g. "Apple M3 Pro".
                    var keyStr = CoreFoundation.CFStringCreateWithCString(IntPtr.Zero, "model", CoreFoundation.CFStringEncodingUtf8);

                    if (keyStr == IntPtr.Zero)
                    {
                        continue;
                    }

                    // Read the "model" property from this service entry.
                    // The result is a CF object we own and must release — IntPtr.Zero means not found.
                    IntPtr cfValue;
                    try
                    {
                        cfValue = IORegistryEntryCreateCFProperty(service, keyStr, IntPtr.Zero, 0);
                    }
                    finally
                    {
                        // The key CFString is no longer needed once the property lookup is done.
                        CoreFoundation.CFRelease(keyStr);
                    }

                    if (cfValue == IntPtr.Zero)
                    {
                        continue; // property absent on this service — shouldn't happen for AGXAccelerator but be safe
                    }

                    try
                    {
                        // Convert the CFString value to a .NET string and return it if non-empty.
                        // There is normally only one AGXAccelerator service on Apple Silicon,
                        // so the first valid result is the GPU name.
                        var name = CoreFoundation.CFStringToString(cfValue);

                        if (!string.IsNullOrEmpty(name))
                        {
                            return name;
                        }
                    }
                    finally
                    {
                        // Always release the CF object returned by IORegistryEntryCreateCFProperty.
                        CoreFoundation.CFRelease(cfValue);
                    }
                }
                finally
                {
                    // Release the service handle returned by IOIteratorNext.
                    IOObjectRelease(service);
                }
            }
        }
        finally
        {
            // Release the iterator itself once we're done walking results.
            IOObjectRelease(iterator);
        }

        return null;
    }
}

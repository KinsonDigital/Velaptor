// <copyright file="IGraphicsDevice.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Owns the four-object chain that underpins all WebGPU work: Instance → Adapter → Device → Queue.
/// </summary>
internal interface IGraphicsDevice : IDisposable
{
    /// <summary>
    /// Gets the WebGPU invoker that wraps all native WebGPU API calls.
    /// </summary>
    IWgpuInvoker Wgpu { get; }

    /// <summary>
    /// Gets the WebGPU instance — the runtime entry point from which adapters are enumerated
    /// and surfaces are created.
    /// </summary>
    SafeInstanceHandle Instance { get; }

    /// <summary>
    /// Gets the GPU adapter.
    /// </summary>
    SafeAdapterHandle? Adapter { get; }

    /// <summary>
    /// Gets the logical device.
    /// </summary>
    SafeDeviceHandle? Handle { get; }

    /// <summary>
    /// Gets the GPU command queue.
    /// </summary>
    SafeQueueHandle? Queue { get; }

    /// <summary>
    /// Gets the maximum texture width supported by the adapter.
    /// </summary>
    uint MaxWidth { get; }

    /// <summary>
    /// Gets the maximum texture height supported by the adapter.
    /// </summary>
    uint MaxHeight { get; }

    /// <summary>
    /// Asks the WebGPU instance for a physical GPU adapter that can render to
    /// <paramref name="surface"/>.
    /// </summary>
    /// <param name="surface">The surface the adapter must support.</param>
    void InitializeAdapter(SafeSurfaceHandle surface);

    /// <summary>
    /// Creates a logical device from the adapter, registers the error callback, and
    /// retrieves the default command queue. Must be called after <see cref="InitializeAdapter"/>.
    /// </summary>
    void InitializeDevice();

    /// <summary>
    /// Compiles WGSL source into a GPU-side shader module.
    /// </summary>
    /// <param name="wgsl">The WGSL shader source to compile.</param>
    /// <returns>The compiled shader module handle.</returns>
    SafeShaderModuleHandle CreateShaderModule(string wgsl);
}

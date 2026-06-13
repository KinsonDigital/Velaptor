// <copyright file="IGraphicsDevice.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU;

using System;
using NativeInterop.WebGPU;
using NativeInterop.WebGPU.Handles;

/// <summary>
/// Owns the four-object chain that underpins all WebGPU work: Instance → Adapter → Device → Queue.
/// </summary>
internal interface IGraphicsDevice : IDisposable
{
    /// <summary>
    /// Gets the WebGPU invoker that wraps all native WebGPU API calls.
    /// </summary>
    IWGPUInvoker Wgpu { get; }

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
}

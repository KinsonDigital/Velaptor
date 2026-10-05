// <copyright file="GraphicsDevice.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using System.Diagnostics.CodeAnalysis;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Owns the four-object chain that underpins all WebGPU work on this machine:
/// <b>Instance</b> → <b>Adapter</b> → <b>Device</b> → <b>Queue</b>.
/// </summary>
/// <remarks>
/// <para>
/// The <b>Instance</b> is the runtime entry point — the root from which adapters are discovered.
/// </para>
/// <para>
/// An <b>Adapter</b> identifies a specific physical GPU and its driver. It describes what
/// the hardware supports (optional features, limits) but does not itself execute work.
/// An adapter can become unavailable at runtime — for example if the GPU is unplugged or
/// the OS powers it down — so it is intentionally kept separate from the device.
/// </para>
/// <para>
/// A <b>Device</b> is a logical, isolated connection to the adapter. WebGPU guarantees that
/// code owning a device acts as if it is the sole user of the GPU — resources created on
/// one device are invisible to another. The device is the factory for every other GPU
/// resource: textures, buffers, pipelines, shader modules. All of those are released
/// automatically when the device itself is destroyed.
/// </para>
/// <para>
/// The <b>Queue</b> is the submission point for GPU work. Commands are not sent to the GPU
/// one at a time; they are first recorded into a command buffer by a command encoder,
/// then the whole buffer is submitted to the queue. This batching lets the driver schedule
/// and optimize a chunk of work at once.
/// </para>
/// </remarks>
[ExcludeFromCodeCoverage(Justification = "Cannot test due to direct interaction with native WebGPU library.")]
internal sealed class GraphicsDevice : IGraphicsDevice
{
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsDevice"/> class.
    /// </summary>
    /// <param name="wgpu">The WebGPU invoker.</param>
    public GraphicsDevice(IWgpuInvoker wgpu)
    {
        Wgpu = wgpu;

        var desc = default(InstanceDescriptor);
        Instance = Wgpu.CreateInstance(in desc);

        if (Instance is null || Instance.IsInvalid)
        {
            throw new Exception("Failed to create WebGPU instance.");
        }
    }

    /// <summary>
    /// Gets the WebGPU invoker that wraps all native WebGPU API calls.
    /// </summary>
    public IWgpuInvoker Wgpu { get; }

    /// <summary>
    /// Gets the WebGPU instance — the runtime entry point from which adapters are enumerated
    /// and surfaces are created.
    /// </summary>
    public SafeInstanceHandle Instance { get; }

    /// <summary>
    /// Gets the GPU adapter — a handle that identifies one specific physical GPU (or software
    /// fallback) and its driver. Must be initialized via <see cref="InitializeAdapter"/>.
    /// </summary>
    public SafeAdapterHandle? Adapter { get; private set; }

    /// <summary>
    /// Gets the logical device — a compartmentalized, isolated connection to the adapter.
    /// All GPU resources are created through the device. Must be initialized via
    /// <see cref="InitializeDevice"/>.
    /// </summary>
    public SafeDeviceHandle? Handle { get; private set; }

    /// <summary>
    /// Gets the GPU command queue. Encoded command buffers are submitted here for the GPU
    /// to execute. Retrieved inside <see cref="InitializeDevice"/>.
    /// </summary>
    public SafeQueueHandle? Queue { get; private set; }

    /// <summary>
    /// Gets the maximum texture width supported by the adapter.
    /// </summary>
    public uint MaxWidth { get; private set; }

    /// <summary>
    /// Gets the maximum texture height supported by the adapter.
    /// </summary>
    public uint MaxHeight { get; private set; }

    /// <summary>
    /// Asks the WebGPU instance for a physical GPU adapter that can render to
    /// <paramref name="surface"/>.
    /// </summary>
    /// <param name="surface">The surface the adapter must support.</param>
    public void InitializeAdapter(SafeSurfaceHandle surface)
    {
        // wgpu-native fires this callback synchronously, so Adapter is set
        // by the time InstanceRequestAdapter returns.
        Wgpu.InstanceRequestAdapter(Instance, surface, OnAdapterReceived);

        if (Adapter is null)
        {
            throw new Exception("Failed to get a WebGPU adapter (no compatible GPU?).");
        }

        SupportedLimits supportedLimits = default;
        var success = Wgpu.AdapterGetLimits(Adapter, ref supportedLimits);

        if (success)
        {
            var limits = supportedLimits.Limits;
            var maxWidthOrHeight = limits.MaxTextureDimension2D;
            MaxWidth = maxWidthOrHeight;
            MaxHeight = maxWidthOrHeight;
        }
    }

    /// <summary>
    /// Creates a logical device from the adapter, registers the error callback, and
    /// retrieves the default command queue. Must be called after <see cref="InitializeAdapter"/>.
    /// </summary>
    public void InitializeDevice()
    {
        if (Adapter is null)
        {
            throw new InvalidOperationException("Adapter must be initialized before creating the device.");
        }

        var desc = default(DeviceDescriptor);

        Wgpu.AdapterRequestDevice(Adapter, in desc, OnDeviceReceived);

        if (Handle is null)
        {
            throw new Exception("Failed to get a WebGPU device.");
        }

        // Register an error callback so GPU-side errors surface in the console.
        Wgpu.DeviceSetUncapturedErrorCallback(Handle, OnDeviceError);

        Queue = new SafeQueueHandle(Wgpu, Handle);
    }

    /// <inheritdoc/>
    public SafeShaderModuleHandle CreateShaderModule(string wgsl)
    {
        if (Handle is null)
        {
            throw new InvalidOperationException("Device must be initialized before creating shader modules.");
        }

        return Wgpu.DeviceCreateShaderModule(Handle, wgsl);
    }

    /// <inheritdoc/>
    public void Dispose() => Dispose(true);

    /// <summary>
    /// Fired when a GPU error escapes all active error scopes.
    /// </summary>
    private static void OnDeviceError(ErrorType type, nint message, nint userdata)
    {
        var msg = SilkMarshal.PtrToString(message);
        Console.WriteLine($"[WebGPU Error] {type}: {msg}");
    }

    /// <inheritdoc cref="IDisposable.Dispose"/>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            Queue?.Dispose();
            Handle?.Dispose();
            Adapter?.Dispose();
            Instance.Dispose();
        }

        this.isDisposed = true;
    }

    /// <summary>
    /// Fired synchronously by wgpu-native when the adapter request completes.
    /// </summary>
    private void OnAdapterReceived(RequestAdapterStatus status, nint adapter, nint message, nint userdata)
    {
        if (status == RequestAdapterStatus.Success)
        {
            Adapter = new SafeAdapterHandle(Wgpu, adapter);
        }
        else
        {
            Console.WriteLine($"Adapter request failed: {SilkMarshal.PtrToString(message)}");
        }
    }

    /// <summary>
    /// Fired synchronously by wgpu-native when the device request completes.
    /// </summary>
    private void OnDeviceReceived(RequestDeviceStatus status, nint device, nint message, nint userdata)
    {
        if (status == RequestDeviceStatus.Success)
        {
            if (device == 0)
            {
                throw new Exception("Failed to get a WebGPU device.");
            }

            Handle = new SafeDeviceHandle(Wgpu, device);
        }
        else
        {
            Console.WriteLine($"Device request failed: {SilkMarshal.PtrToString(message)}");
        }
    }
}

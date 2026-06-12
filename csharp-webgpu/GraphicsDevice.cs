// <copyright file="GraphicsDevice.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;

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
/// one at a time; they are first recorded into a <see cref="CommandBuffer"/>
/// by a <see cref="CommandEncoder"/>, then the whole buffer is submitted
/// to the queue. This batching lets the driver schedule and optimize a chunk of work at once.
/// </para>
/// </remarks>
internal sealed class GraphicsDevice : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsDevice"/> class.
    /// </summary>
    /// <remarks>
    /// Loads wgpu-native and creates the WebGPU instance — the runtime entry point.
    /// The instance is all that exists at this stage; the adapter and device are not
    /// ready until you call <see cref="InitializeAdapter"/> (passing the surface handle)
    /// and then <see cref="InitializeDevice"/>. This two-step split exists because the
    /// adapter must be selected based on which surface it will render to.
    /// </remarks>
    public GraphicsDevice()
    {
        Wgpu = WebGPU.GetApi();

        Instance = new SafeInstanceHandle(Wgpu);
        if (Instance == null)
        {
            throw new Exception("Failed to create WebGPU instance.");
        }
    }

    /// <summary>
    /// Gets the Silk.NET wrapper around the wgpu-native C library. Every WebGPU API call
    /// in this project is made through this object.
    /// </summary>
    public WebGPU Wgpu { get; }

    /// <summary>
    /// Gets the WebGPU instance — the runtime entry point from which adapters are enumerated
    /// and surfaces are created. All other WebGPU objects ultimately trace back to this.
    /// </summary>
    public SafeInstanceHandle Instance { get; private set; }

    /// <summary>
    /// Gets the GPU adapter — a handle that identifies one specific physical GPU (or software
    /// fallback) and its driver. The adapter describes what the hardware can do but does not
    /// execute work itself. Must be initialized via <see cref="InitializeAdapter"/>.
    /// </summary>
    public SafeAdapterHandle Adapter { get; private set; }

    /// <summary>
    /// Gets the logical device — a compartmentalized, isolated connection to the adapter.
    /// All GPU resources (textures, buffers, pipelines, shader modules) are created through
    /// the device and are owned by it. Destroying the device reclaims all of them.
    /// Must be initialized via <see cref="InitializeDevice"/>.
    /// </summary>
    public SafeDeviceHandle Handle { get; private set; }

    /// <summary>
    /// Gets the GPU command queue. Encoded command buffers are submitted here for the GPU
    /// to execute. Each device has exactly one default queue. Retrieved from the device
    /// inside <see cref="InitializeDevice"/>.
    /// </summary>
    public SafeQueueHandle Queue { get; private set; }

    /// <summary>
    /// Gets the maximum texture width supported by the adapter. This is a property of the
    /// physical GPU and is relevant when creating render targets, including the swap chain.
    /// </summary>
    public uint MaxWidth { get; private set; }

    /// <summary>
    /// Gets the maximum texture height supported by the adapter. This is a property of the
    /// physical GPU and is relevant when creating render targets, including the swap chain.
    /// </summary>
    public uint MaxHeight { get; private set; }

    /// <summary>
    /// Asks the WebGPU instance for a physical GPU adapter that can render to
    /// <paramref name="surface"/>.
    /// </summary>
    /// <param name="surface">
    /// The surface the adapter must support. An adapter that is incompatible with the
    /// surface cannot configure a swap chain against it, so the surface must exist before
    /// the adapter is selected. Pass <see cref="GraphicsSurface.Handle"/> here.
    /// </param>
    /// <remarks>
    /// wgpu-native resolves adapter requests synchronously — the callback fires before
    /// <c>InstanceRequestAdapter</c> returns — so <see cref="Adapter"/> is populated
    /// immediately after this call.
    /// </remarks>
    public void InitializeAdapter(SafeSurfaceHandle surface)
    {
        unsafe
        {
            var opts = new RequestAdapterOptions { CompatibleSurface = (Surface*)surface.DangerousGetHandle() };

            // wgpu-native fires this callback synchronously, so Adapter is set
            // by the time InstanceRequestAdapter returns.
            Wgpu.InstanceRequestAdapter((Instance*)Instance.DangerousGetHandle(),
                in opts,
                new PfnRequestAdapterCallback(OnAdapterReceived),
                null);

            if (Adapter == null)
            {
                throw new Exception("Failed to get a WebGPU adapter (no compatible GPU?).");
            }

            SupportedLimits supportedLimits = default;
            var success = Wgpu.AdapterGetLimits((Adapter*)Adapter.DangerousGetHandle(), &supportedLimits);
            if (!success)
            {
                return;
            }

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
    /// <remarks>
    /// The device is created with default capabilities, which is sufficient for this demo.
    /// A production application would supply a <see cref="DeviceDescriptor"/>
    /// requesting the specific optional features and limits its rendering needs. An
    /// uncaptured-error callback is registered so that GPU-side validation errors —
    /// which WebGPU processes asynchronously — surface immediately in the console.
    /// </remarks>
    public void InitializeDevice()
    {
        var desc = default(DeviceDescriptor);

        unsafe
        {
            Wgpu.AdapterRequestDevice((Adapter*)Adapter.DangerousGetHandle(), in desc, new PfnRequestDeviceCallback(OnDeviceReceived), null);

            if (Handle == null)
            {
                throw new Exception("Failed to get a WebGPU device.");
            }

            // Register an error callback so GPU-side errors surface in the console.
            var deviceHandle = (Device*)Handle.DangerousGetHandle();

            Wgpu.DeviceSetUncapturedErrorCallback(deviceHandle, new PfnErrorCallback(OnDeviceError), null);

            Queue = new SafeQueueHandle(Wgpu, Handle);
        }
    }

    /// <summary>
    /// Compiles WGSL source into a GPU-side shader module.
    /// </summary>
    /// <param name="wgsl">The WGSL shader source to compile.</param>
    /// <returns>
    /// An opaque pointer to the compiled shader module. The caller is responsible for
    /// releasing it with <c>Wgpu.ShaderModuleRelease</c> when no longer needed — typically
    /// right after building the pipeline that uses it.
    /// </returns>
    /// <remarks>
    /// A shader module is a compiled container for one or more shader entry points. It can
    /// hold both the vertex and fragment functions simultaneously, as in this demo. Once a
    /// pipeline is built from a module, the pipeline retains its own internal copy of the
    /// compiled code and the module handle can be freed.
    /// </remarks>
    public SafeShaderModuleHandle CreateShaderModule(string wgsl)
    {
        var wgslPtr = SilkMarshal.StringToPtr(wgsl);

        unsafe
        {
            var wgslDesc = new ShaderModuleWGSLDescriptor
            {
                Chain = new ChainedStruct { SType = SType.ShaderModuleWgslDescriptor }, Code = (byte*)wgslPtr,
            };

            var moduleDesc = new ShaderModuleDescriptor { NextInChain = (ChainedStruct*)&wgslDesc, };

            var module = new SafeShaderModuleHandle(Wgpu, Handle, moduleDesc);

            // DeviceCreateShaderModule copies the source internally, so the
            // temporary pointer is no longer needed after the module is created.
            SilkMarshal.Free(wgslPtr);

            return module;
        }
    }

    /// <summary>
    /// Releases the queue, device, adapter, and instance in reverse creation order.
    /// Because the device owns all GPU resources created from it, releasing the device
    /// implicitly invalidates any GPU objects still alive (pipelines, textures, etc.).
    /// Call <see cref="GraphicsPipeline.Dispose"/> and <see cref="GraphicsSurface.Dispose"/>
    /// before this.
    /// </summary>
    public void Dispose()
    {
        Queue.Dispose();
        Handle.Dispose();
        Adapter.Dispose();
        Instance.Dispose();
        Wgpu.Dispose();
    }

    /// <summary>
    /// Fired when a GPU error escapes all active error scopes. WebGPU validates API calls
    /// asynchronously on the GPU process, so errors from invalid descriptors or misuse may
    /// not surface until after the offending call has returned. This callback ensures those
    /// errors are printed to the console during development rather than silently disappearing.
    /// </summary>
    /// <param name="type">The error category (Validation, OutOfMemory, Internal, etc.).</param>
    /// <param name="message">A null-terminated UTF-8 string describing the error.</param>
    /// <param name="_">User data pointer — unused here.</param>
    private static unsafe void OnDeviceError(ErrorType type, byte* message, void* _)
        => Console.WriteLine($"[WebGPU Error] {type}: {SilkMarshal.PtrToString((nint)message)}");

    /// <summary>
    /// Fired synchronously by wgpu-native when the adapter request completes.
    /// On success, stores the adapter. On failure, logs the reason — typically meaning
    /// no GPU on the system is compatible with the requested surface, or no GPU exists.
    /// </summary>
    /// <param name="status">Whether the request succeeded.</param>
    /// <param name="a">The resolved adapter handle, or null on failure.</param>
    /// <param name="message">A null-terminated UTF-8 failure description, or null on success.</param>
    /// <param name="_">User data pointer — unused here.</param>
    private unsafe void OnAdapterReceived(RequestAdapterStatus status, Adapter* a, byte* message, void* _)
    {
        if (status == RequestAdapterStatus.Success)
        {
            Adapter = new SafeAdapterHandle(Wgpu, (nint)a);
        }
        else
        {
            Console.WriteLine($"Adapter request failed: {SilkMarshal.PtrToString((nint)message)}");
        }
    }

    /// <summary>
    /// Fired synchronously by wgpu-native when the device request completes.
    /// On success, stores the device. On failure, logs the reason — this typically means
    /// the requested features or limits exceed what the adapter supports.
    /// </summary>
    /// <param name="status">Whether the request succeeded.</param>
    /// <param name="d">The resolved device handle, or null on failure.</param>
    /// <param name="message">A null-terminated UTF-8 failure description, or null on success.</param>
    /// <param name="_">User data pointer — unused here.</param>
    private unsafe void OnDeviceReceived(RequestDeviceStatus status, Device* d, byte* message, void* _)
    {
        if (status == RequestDeviceStatus.Success)
        {
            Handle = new SafeDeviceHandle(Wgpu, (nint)d);
        }
        else
        {
            Console.WriteLine($"Device request failed: {SilkMarshal.PtrToString((nint)message)}");
        }
    }
}

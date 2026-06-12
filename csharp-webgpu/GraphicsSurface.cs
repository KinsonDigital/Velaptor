// <copyright file="GraphicsSurface.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

/// <summary>
/// Connects an OS window to the WebGPU rendering system and manages the swap chain
/// that controls how rendered frames reach the display.
/// </summary>
/// <remarks>
/// <para>
/// WebGPU has no implicit or default drawing buffer. To display rendered output,
/// you must create a <b>surface</b> — a platform-specific binding between the window's
/// native handle (HWND on Windows, a CAMetalLayer on macOS, etc.) and the WebGPU
/// instance. The surface must exist <em>before</em> the adapter is selected, because the
/// chosen adapter must be compatible with the surface it will render to.
/// </para>
/// <para>
/// Under the hood the surface manages a <b>swap chain</b>: a small ring of textures
/// where the GPU renders into one texture while the display presents another.
/// Configuring the surface sets the pixel format, dimensions, and present mode
/// for those textures. The configuration must be refreshed whenever the window
/// is resized, because the textures must exactly match the framebuffer dimensions.
/// </para>
/// </remarks>
internal sealed class GraphicsSurface : IDisposable
{
    private readonly GraphicsDevice gd;
    private readonly IWindow window;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsSurface"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="window">The window to create the surface for.</param>
    /// <remarks>
    /// The surface is created immediately from the window's native handle. The adapter
    /// and device do not exist yet at construction time — follow up with
    /// <see cref="GraphicsDevice.InitializeAdapter"/> (passing <see cref="Handle"/>) and
    /// <see cref="GraphicsDevice.InitializeDevice"/>, then call <see cref="Configure"/>
    /// to set up the swap chain once the device is ready.
    /// </remarks>
    /// <exception cref="Exception">Thrown if the WebGPU surface could not be created.</exception>
    public GraphicsSurface(GraphicsDevice gd, IWindow window)
    {
        this.gd = gd;
        this.window = window;

        Handle = new SafeSurfaceHandle(this.gd.Wgpu, window, this.gd.Instance);

        if (Handle == null)
        {
            throw new Exception("Failed to create WebGPU surface.");
        }
    }

    /// <summary>
    /// Gets the raw WebGPU surface handle — the platform-specific binding between the
    /// OS window and the WebGPU instance. Pass this to
    /// <see cref="GraphicsDevice.InitializeAdapter"/> so that the selected adapter is
    /// guaranteed to support rendering to this window.
    /// </summary>
    public SafeSurfaceHandle Handle { get; }

    /// <summary>
    /// Gets the pixel format the swap chain textures are allocated in.
    /// Populated by <see cref="Configure"/> by querying the adapter for its preference.
    /// Most desktop hardware prefers <c>Bgra8Unorm</c> (8-bit Blue, Green, Red, Alpha);
    /// some mobile hardware prefers <c>Rgba8Unorm</c>. Using the preferred format avoids
    /// a per-frame color conversion on every presented frame.
    /// </summary>
    public TextureFormat Format { get; private set; }

    /// <summary>
    /// Queries the adapter's preferred pixel format and (re)configures the swap chain
    /// to match the current framebuffer size. Must be called once after the device is
    /// ready, and again after every window resize.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The preferred format is a property of the adapter↔surface pair. On most desktop
    /// hardware this resolves to <c>Bgra8Unorm</c>: bytes are stored in Blue-Green-Red-Alpha
    /// order, with each byte mapped linearly from 0 (0.0) to 255 (1.0). Using this format
    /// instead of forcing a different one avoids an implicit conversion on every frame.
    /// </para>
    /// <para>
    /// <c>PresentMode.Fifo</c> (FIFO = first-in, first-out) is WebGPU's equivalent of
    /// VSync — frames are queued and presented on the display's vertical blank boundary,
    /// which prevents screen tearing. It is the only present mode guaranteed to be
    /// available on all implementations.
    /// </para>
    /// </remarks>
    public void Configure()
    {
        unsafe
        {
            var size = this.window.FramebufferSize;

            var surfaceHandle = (Surface*)Handle.DangerousGetHandle();

            // Using the adapter's preferred format avoids an implicit color conversion
            // on every presented frame. The format is a property of the adapter↔surface
            // pair and must match the format used by the pipeline's color target.
            Format = this.gd.Wgpu.SurfaceGetPreferredFormat(surfaceHandle, (Adapter*)this.gd.Adapter.DangerousGetHandle());

            var config = new SurfaceConfiguration
            {
                Device = (Device*)this.gd.Handle.DangerousGetHandle(),
                Format = Format,
                Usage = TextureUsage.RenderAttachment, // We render directly into the swap chain textures
                Width = (uint)size.X,
                Height = (uint)size.Y,
                PresentMode = PresentMode.Fifo, // VSync — queues frames and swaps on vertical blank
            };

            this.gd.Wgpu.SurfaceConfigure(surfaceHandle, in config);
        }
    }

    public SafeSurfaceTextureHandle GetSurfaceTexture()
    {
        unsafe
        {
            // Ask the surface for the texture to draw into this frame.
            // A local variable is used for the pointer operation because the address of an
            // instance field (&this.surfaceTexture) would require a `fixed` block in unsafe C#,
            // whereas a local is stack-allocated and automatically pinned.
            SurfaceTexture st;
            this.gd.Wgpu.SurfaceGetCurrentTexture((Surface*)Handle.DangerousGetHandle(), &st);

            if (this.surfaceTextureHandle is null)
            {
                this.surfaceTextureHandle = new SafeSurfaceTextureHandle(this.gd.Wgpu, (nint)st.Texture, st.Status);
            }
            else
            {
                this.surfaceTextureHandle.UpdateHandleAndStatus((nint)st.Texture, st.Status);
            }
        }

        return this.surfaceTextureHandle;
    }

    /// <summary>
    /// Unconfigure the swap chain and releases the surface handle. After disposal
    /// the window will no longer receive rendered output. Must be called before
    /// <see cref="GraphicsDevice.Dispose"/>.
    /// </summary>
    public void Dispose() => Handle.Dispose();
}

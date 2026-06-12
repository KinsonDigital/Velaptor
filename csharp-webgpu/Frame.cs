// <copyright file="Frame.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;
using Silk.NET.WebGPU;
using NETColor = System.Drawing.Color;
using SilkColor = Silk.NET.WebGPU.Color;

/// <summary>
/// Encapsulates all per-frame GPU work: acquiring a render target, recording draw
/// commands into a command buffer, submitting them to the GPU, and presenting the result.
/// </summary>
/// <remarks>
/// <para>
/// In WebGPU, GPU commands are not issued one at a time. A
/// <see cref="CommandEncoder"/> records a sequence of commands into a
/// <see cref="CommandBuffer"/>; the entire buffer is then submitted to the device
/// <see cref="Queue"/> in one call. This batching lets the GPU driver see the full
/// workload at once and schedule it efficiently.
/// </para>
/// <para>
/// A <b>render pass</b> is a recording session within the encoder where the GPU writes
/// pixels to one or more attached textures. Each pass opens with a load operation
/// (typically <c>Clear</c>) and closes with a store operation before the encoder can
/// be finalized.
/// </para>
/// <para>
/// <see cref="Frame"/> is transient — create it at the top of the render loop,
/// call <see cref="Begin"/>, issue draw calls, call <see cref="Submit"/>, then let
/// a <c>using</c> block call <see cref="Dispose"/> to release the per-frame handles.
/// </para>
/// </remarks>
internal sealed class Frame : IDisposable
{
    private readonly GraphicsDevice gd;
    private readonly GraphicsSurface surface;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;
    private SafeTextureViewHandle? textureViewHandle;
    private SafeRenderPassEncoderHandle? renderPassHandle;
    private SafeCommandEncoderHandle? encoder;

    /// <summary>
    /// Initializes a new instance of the <see cref="Frame"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="surface">The graphics surface.</param>
    public Frame(GraphicsDevice gd, GraphicsSurface surface)
    {
        this.gd = gd;
        this.surface = surface;
    }

    /// <summary>
    /// Gets a value indicating whether the frame is valid and ready for draw calls.
    /// </summary>
    /// <remarks>
    /// Set to <c>true</c> by a successful <see cref="Begin"/> call. A frame is invalid
    /// when the surface cannot provide a texture — for example when the window is minimized
    /// or the swap chain is in an error state. Rendering on an invalid frame is a no-op;
    /// skip the draw calls and call <see cref="Dispose"/> directly.
    /// </remarks>
    public bool IsValid { get; private set; }

    /// <summary>
    /// Acquires this frame's render target from the swap chain, creates a command encoder,
    /// and opens a render pass that clears the target to <paramref name="clearColor"/>.
    /// </summary>
    /// <param name="clearColor">The RGBA color to fill the render target with before drawing.</param>
    /// <returns>
    /// <c>true</c> if the frame is ready for draw calls; <c>false</c> if the swap chain
    /// could not provide a texture (window minimized, surface lost, etc.).
    /// </returns>
    /// <remarks>
    /// Each call acquires a fresh texture from the surface's swap chain for this frame.
    /// A <see cref="TextureView"/> is created to tell the GPU how to interpret that texture
    /// (format, dimensionality, which mip level). The render pass color attachment
    /// points at this view and declares: clear with <paramref name="clearColor"/> at the
    /// start of the pass, and store the result at the end so it can be presented.
    /// </remarks>
    public bool Begin(NETColor clearColor)
    {
        this.surfaceTextureHandle = this.surface.GetSurfaceTexture();

        if (this.surfaceTextureHandle.SurfaceTextureStatus != SurfaceGetCurrentTextureStatus.Success)
        {
            IsValid = false;
            return false;
        }

        // Create a texture view into the acquired surface texture.
        // A texture view is a typed "lens" onto a texture: it specifies the format,
        // dimensionality, and which mip level and array layer the GPU should access.
        // The render pass color attachment references the view, not the texture directly.
        var viewDesc = new TextureViewDescriptor
        {
            Format = this.surface.Format,
            Dimension = TextureViewDimension.Dimension2D,
            MipLevelCount = 1,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All,
        };

        if (this.textureViewHandle is null)
        {
            this.textureViewHandle = new SafeTextureViewHandle(this.gd.Wgpu, this.surfaceTextureHandle.DangerousGetHandle(), in viewDesc);
        }
        else
        {
            this.textureViewHandle.UpdateHandle(this.surfaceTextureHandle.DangerousGetHandle(), in viewDesc);
        }

        if (this.textureViewHandle.IsInvalid)
        {
            IsValid = false;

            return false;
        }

        // Create a command encoder. It records a sequence of GPU commands into a
        // command buffer that can be submitted to the queue in one batch.
        var encoderDesc = default(CommandEncoderDescriptor);

        if (this.encoder is null)
        {
            this.encoder = new SafeCommandEncoderHandle(this.gd, in encoderDesc);
        }
        else
        {
            this.encoder.UpdateHandle(in encoderDesc);
        }

        unsafe
        {
            // Open the render pass. This is the recording session where draw commands
            // will write pixels to the attached texture view.
            //   LoadOp.Clear  — fill the texture with clearColor before any drawing.
            //   StoreOp.Store — keep the pixels written during the pass so they can be presented.
            // Without Store, the GPU is free to discard the results (valid on tile-based GPUs).
            var colorAttachment = new RenderPassColorAttachment
            {
                View = (TextureView*)this.textureViewHandle.DangerousGetHandle(),
                LoadOp = LoadOp.Clear, // Fill with clearColor before any draw call
                StoreOp = StoreOp.Store, // Preserve the result for presentation
                ClearValue = ToLinearClearColor(clearColor, this.surface.Format),
            };

            var passDesc = new RenderPassDescriptor
            {
                ColorAttachmentCount = 1,
                ColorAttachments = &colorAttachment,
            };

            this.renderPassHandle = new SafeRenderPassEncoderHandle(this.gd.Wgpu, this.encoder, in passDesc);
            IsValid = true;

            return true;
        }
    }

    /// <summary>
    /// Binds <paramref name="pipeline"/> and <paramref name="textureBindGroup"/> to the
    /// active render pass and draws <paramref name="quadCount"/> textured quads from
    /// <paramref name="textureBuffer"/>.
    /// </summary>
    /// <param name="pipeline">The render pipeline to activate.</param>
    /// <param name="textureBuffer">
    /// The buffer containing pre-uploaded vertex and index data for the quads to draw.
    /// Call <see cref="GraphicsTextureBuffer.Upload"/> before calling this method.
    /// </param>
    /// <param name="textureBindGroup">
    /// The bind group for <c>@group(0)</c>: texture view (binding 0) and sampler (binding 1).
    /// Typically <see cref="GraphicsTexture.BindGroup"/>.
    /// </param>
    /// <param name="quadCount">Number of quads to draw from the buffer.</param>
    /// <param name="firstQuad">Index of the first quad to draw.</param>
    public void Draw(
        GraphicsPipeline pipeline,
        GraphicsTextureBuffer textureBuffer,
        SafeBindGroupHandle textureBindGroup,
        uint quadCount = 1,
        uint firstQuad = 0)
    {
        if (this.renderPassHandle is null)
        {
            throw new Exception("The render pass handle cannot be null.");
        }

        pipeline.Bind(this.renderPassHandle);

        unsafe
        {
            var renderPassEncoder = (RenderPassEncoder*)this.renderPassHandle.DangerousGetHandle();

            this.gd.Wgpu.RenderPassEncoderSetBindGroup(renderPassEncoder, 0, (BindGroup*)textureBindGroup.DangerousGetHandle(), 0, null);
        }

        textureBuffer.Draw(this.renderPassHandle, quadCount, firstQuad);
    }

    /// <summary>
    /// Binds the rectangle pipeline and draws rectangles from the given buffer.
    /// </summary>
    /// <param name="rectPipeline">The rectangle render pipeline.</param>
    /// <param name="rectBuffer">The buffer containing vertex and index data for rectangles.</param>
    /// <param name="rectCount">The number of rectangles to draw.</param>
    /// <param name="firstRect">The index of the first rectangle to draw.</param>
    /// <remarks>
    /// Call this after the textured pipeline draw to render rectangles on top.
    /// No bind groups are set because the rectangle pipeline uses no bind groups.
    /// </remarks>
    public void DrawRectangles(
        GraphicsRectPipeline rectPipeline,
        GraphicsRectBuffer rectBuffer,
        uint rectCount = 1,
        uint firstRect = 0)
    {
        if (this.renderPassHandle is null)
        {
            throw new Exception("The render pass handle cannot be null.");
        }

        rectPipeline.Bind(this.renderPassHandle);
        rectBuffer.Draw(this.renderPassHandle, rectCount, firstRect);
    }

    /// <summary>
    /// Closes the render pass, seals the command buffer, submits it to the GPU queue,
    /// and presents the completed frame to the display.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Ending the render pass signals that all drawing for this pass is complete. The
    /// GPU may now execute the recorded commands and write pixel output to the texture.
    /// </para>
    /// <para>
    /// <c>CommandEncoderFinish</c> seals the buffer; from that point the encoder is
    /// no longer usable. <c>QueueSubmit</c> hands the buffer to the GPU for asynchronous
    /// execution. <c>SurfacePresent</c> tells the swap chain to display the texture
    /// once the GPU has finished writing into it.
    /// </para>
    /// </remarks>
    public void Submit()
    {
        if (this.encoder is null)
        {
            throw new Exception("The command encoder cannot be null.");
        }

        if (this.renderPassHandle is null)
        {
            throw new Exception("The render pass handle cannot be null.");
        }

        // End the render pass — all draw commands for this pass are now finalized.
        this.renderPassHandle.Dispose();

        unsafe
        {
            // Seal the command buffer and submit it to the GPU queue.
            // The encoder is released after Finish; only the sealed buffer is submitted.
            var cmdBufDesc = default(CommandBufferDescriptor);
            var cmdBuf = this.gd.Wgpu.CommandEncoderFinish((CommandEncoder*)this.encoder.DangerousGetHandle(), in cmdBufDesc);

            this.encoder.Dispose();

            this.gd.Wgpu.QueueSubmit((Queue*)this.gd.Queue.DangerousGetHandle(), 1, &cmdBuf);
            this.gd.Wgpu.CommandBufferRelease(cmdBuf);

            // Swap the completed texture onto the display (waits for vertical blank in Fifo mode).
            this.gd.Wgpu.SurfacePresent((Surface*)this.surface.Handle.DangerousGetHandle());
        }
    }

    /// <summary>
    /// Releases the per-frame texture view and surface texture back to the runtime.
    /// The command encoder and render pass encoder are already released inside
    /// <see cref="Submit"/>, so only the texture handles need cleanup here.
    /// </summary>
    public void Dispose()
    {
        this.textureViewHandle?.Dispose();
        this.surfaceTextureHandle?.Dispose();
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="format"/> uses sRGB encoding,
    /// meaning WebGPU will linearise clear values before writing them to the framebuffer.
    /// </summary>
    private static bool IsSrgbFormat(TextureFormat format)
        => format is TextureFormat.Bgra8UnormSrgb or TextureFormat.Rgba8UnormSrgb;

    /// <summary>
    /// Converts a single sRGB component (0–1) to its linear-light equivalent using the
    /// IEC 61966-2-1 piecewise formula.
    /// </summary>
    private static float SrgbToLinear(float c)
        => c <= 0.04045f
            ? c / 12.92f
            : MathF.Pow((c + 0.055f) / 1.055f, 2.4f);

    /// <summary>
    /// Builds the <see cref="SilkColor"/> value used as the render-pass clear colour.
    /// </summary>
    /// <remarks>
    /// When the swap-chain surface is in sRGB format (e.g. <c>Bgra8UnormSrgb</c>),
    /// WebGPU interprets the clear value as <em>linear</em> and applies sRGB encoding
    /// before writing it to the framebuffer.  The user-supplied colour is already in
    /// sRGB space (the normal way humans and tools express colours), so the components
    /// must be converted to linear first; the GPU then round-trips them back to the
    /// expected sRGB value on screen.
    /// Alpha is always linear and is never converted.
    /// </remarks>
    private static SilkColor ToLinearClearColor(NETColor color, TextureFormat format)
    {
        var r = color.R / 255.0f;
        var g = color.G / 255.0f;
        var b = color.B / 255.0f;
        var a = color.A / 255.0f;

        if (IsSrgbFormat(format))
        {
            r = SrgbToLinear(r);
            g = SrgbToLinear(g);
            b = SrgbToLinear(b);
        }

        return new SilkColor(r, g, b, a);
    }
}

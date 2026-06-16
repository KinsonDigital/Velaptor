// <copyright file="Frame.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU;

using System;
using Silk.NET.WebGPU;
using NativeInterop.WebGPU.Handles;
using NETColor = System.Drawing.Color;
using SilkColor = Silk.NET.WebGPU.Color;

/// <summary>
/// Encapsulates all per-frame GPU work: acquiring a render target, recording draw
/// commands into a command buffer, submitting them to the GPU, and presenting the result.
/// </summary>
/// <remarks>
/// <para>
/// In WebGPU, GPU commands are not issued one at a time. A <see cref="CommandEncoder"/>
/// records a sequence of commands into a <see cref="CommandBuffer"/>; the entire buffer is
/// then submitted to the device <see cref="Queue"/> in one call.
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
    private bool surfaceConfigured;

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
    public bool IsValid { get; private set; }

    /// <summary>
    /// Gets the active render pass encoder, or <c>null</c> if <see cref="Begin"/> has not
    /// been called yet or the frame is invalid.
    /// </summary>
    public SafeRenderPassEncoderHandle? RenderPass => this.renderPassHandle;

    /// <summary>
    /// Marks the swap chain as needing reconfiguration on the next <see cref="Begin"/> call.
    /// Call this from the window resize handler when the framebuffer dimensions change.
    /// </summary>
    public void Reconfigure() => this.surfaceConfigured = false;

    /// <summary>
    /// Performs the full WebGPU initialization sequence: creates the platform surface,
    /// requests a GPU adapter, initializes the logical device, and queries the surface
    /// format. Swap chain configuration is deferred to the first <see cref="Begin"/>
    /// call when the window has reached its final size.
    /// Must be called after the window has been created and shown.
    /// </summary>
    public void Initialize()
    {
        this.surface.Initialize();
        this.gd.InitializeAdapter(this.surface.Handle);
        this.gd.InitializeDevice();
        this.surface.InitializeFormat();
    }

    /// <summary>
    /// Acquires this frame's render target from the swap chain, creates a command encoder,
    /// and opens a render pass that clears the target to <paramref name="clearColor"/>.
    /// </summary>
    /// <param name="clearColor">The RGBA color to fill the render target with before drawing.</param>
    /// <returns>
    /// <c>true</c> if the frame is ready for draw calls; <c>false</c> if the swap chain
    /// could not provide a texture.
    /// </returns>
    public bool Begin(NETColor clearColor)
    {
        // If a render pass is already active (nested Begin call via the
        // batcher's frame-depth tracking), reuse the existing pass rather
        // than acquiring a second swap-chain texture.  This keeps all
        // rendering for a logical frame inside a single GPU render pass.
        if (this.renderPassHandle is not null)
        {
            IsValid = true;
            return true;
        }

        // Release all references to the previous frame's swap-chain texture
        // BEFORE reconfiguring the surface.  WebGPU requires that no views or
        // handles to the old swap chain remain alive when Configure() is called.
        this.textureViewHandle?.Dispose();
        this.textureViewHandle = null;
        // surfaceTextureHandle is disposed inside GetSurfaceTexture(), but
        // ensure we don't leak if Begin() is called again without going through
        // the normal Submit() → Begin() cycle.
        this.surfaceTextureHandle?.Dispose();
        this.surfaceTextureHandle = null;

        this.renderPassHandle?.Dispose();
        this.renderPassHandle = null;
        this.encoder?.Dispose();
        this.encoder = null;

        // Configure the swap chain on first frame, or after a resize.
        // This is deferred from Initialize() because the native window
        // may not have reached its final framebuffer size yet at that point.
        if (!this.surfaceConfigured)
        {
            this.surface.Configure();
            this.surfaceConfigured = true;
        }

        this.surfaceTextureHandle = this.surface.GetSurfaceTexture();

        if (this.surfaceTextureHandle.SurfaceTextureStatus != SurfaceGetCurrentTextureStatus.Success)
        {
            IsValid = false;
            return false;
        }


        var viewDesc = new TextureViewDescriptor
        {
            Format = this.surface.Format,
            Dimension = TextureViewDimension.Dimension2D,
            MipLevelCount = 1,
            ArrayLayerCount = 1,
            Aspect = TextureAspect.All,
        };

        this.textureViewHandle = new SafeTextureViewHandle(
            this.gd.Wgpu,
            this.surfaceTextureHandle.DangerousGetHandle(),
            in viewDesc);

        if (this.textureViewHandle.IsInvalid)
        {
            IsValid = false;
            return false;
        }


        var encoderDesc = default(CommandEncoderDescriptor);

        if (this.encoder is null)
        {
            var encoderHandle = this.gd.Wgpu.DeviceCreateCommandEncoder(this.gd.Handle!, in encoderDesc);
            this.encoder = new SafeCommandEncoderHandle(this.gd.Wgpu, encoderHandle);
        }
        else
        {
            this.encoder.UpdateHandle(in encoderDesc);
        }


        unsafe
        {
            var colorAttachment = new RenderPassColorAttachment
            {
                View = (TextureView*)this.textureViewHandle.DangerousGetHandle(),
                LoadOp = LoadOp.Clear,
                StoreOp = StoreOp.Store,
                ClearValue = ToLinearClearColor(clearColor, this.surface.Format),
            };

            var passDesc = new RenderPassDescriptor
            {
                ColorAttachmentCount = 1,
                ColorAttachments = &colorAttachment,
            };

            var passHandle = this.gd.Wgpu.CommandEncoderBeginRenderPass(this.encoder, in passDesc);

            this.renderPassHandle?.Dispose();
            this.renderPassHandle = new SafeRenderPassEncoderHandle(this.gd.Wgpu, passHandle);
            IsValid = true;

            return true;
        }
    }

    /// <summary>
    /// Closes the render pass, seals the command buffer, submits it to the GPU queue,
    /// and presents the completed frame to the display.
    /// </summary>
    public void Submit()
    {
        this.renderPassHandle.End();
        this.renderPassHandle.Dispose();
        this.renderPassHandle = null;

        unsafe
        {
            var cmdBufDesc = default(CommandBufferDescriptor);
            var cmdBuf = this.gd.Wgpu.CommandEncoderFinish(this.encoder, in cmdBufDesc);

            this.encoder.Dispose();
            this.encoder = null;

            try
            {
                this.gd.Wgpu.QueueSubmit(this.gd.Queue!, 1, cmdBuf);
            }
            finally
            {
                this.gd.Wgpu.CommandBufferRelease(cmdBuf);
            }

            this.gd.Wgpu.SurfacePresent(this.surface.Handle);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.renderPassHandle?.Dispose();
        this.encoder?.Dispose();
        this.textureViewHandle?.Dispose();
        this.surfaceTextureHandle?.Dispose();
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="format"/> uses sRGB encoding.
    /// </summary>
    private static bool IsSrgbFormat(TextureFormat format)
        => format is TextureFormat.Bgra8UnormSrgb or TextureFormat.Rgba8UnormSrgb;

    /// <summary>
    /// Converts a single sRGB component (0–1) to its linear-light equivalent.
    /// </summary>
    private static float SrgbToLinear(float c)
        => c <= 0.04045f
            ? c / 12.92f
            : MathF.Pow((c + 0.055f) / 1.055f, 2.4f);

    /// <summary>
    /// Builds the clear colour value for the render pass.
    /// When the swap-chain surface is in sRGB format, WebGPU interprets the clear value as
    /// <em>linear</em> and applies sRGB encoding before writing — so we pre-convert from sRGB to linear.
    /// </summary>
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

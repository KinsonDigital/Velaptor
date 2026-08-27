// <copyright file="Frame.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using Silk.NET.WebGPU;
using NativeInterop.WebGpu.Handles;
using NETColor = System.Drawing.Color;
using SilkColor = Silk.NET.WebGPU.Color;

/// <inheritdoc/>
internal sealed class Frame : IFrame
{
    private readonly IGraphicsDevice grfxDevice;
    private readonly IGraphicsSurface surface;
    private SafeSurfaceTextureHandle? surfaceTextureHandle;
    private SafeTextureViewHandle? textureViewHandle;
    private SafeRenderPassEncoderHandle? renderPassHandle;
    private SafeCommandEncoderHandle? cmdEncoderHandle;
    private bool surfaceConfigured;
    private bool initialized;
    private bool hasBegun;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="Frame"/> class.
    /// </summary>
    /// <param name="grfxDevice">The graphics device.</param>
    /// <param name="surface">The graphics surface.</param>
    public Frame(IGraphicsDevice grfxDevice, IGraphicsSurface surface)
    {
        ArgumentNullException.ThrowIfNull(grfxDevice);
        ArgumentNullException.ThrowIfNull(surface);

        this.grfxDevice = grfxDevice;
        this.surface = surface;
    }

    /// <inheritdoc/>
    public bool IsValid { get; private set; }

    /// <inheritdoc/>
    public SafeRenderPassEncoderHandle? RenderPass => this.renderPassHandle;

    /// <inheritdoc/>
    public void Reconfigure() => this.surfaceConfigured = false;

    /// <inheritdoc/>
    public void Initialize()
    {
        if (this.initialized)
        {
            return;
        }

        this.surface.Initialize();
        this.grfxDevice.InitializeAdapter(this.surface.Handle);
        this.grfxDevice.InitializeDevice();
        this.surface.InitializeFormat();

        this.initialized = true;
    }

    /// <inheritdoc/>
    public bool Begin(NETColor clearColor)
    {
        if (this.hasBegun)
        {
            return IsValid;
        }

        if (!this.initialized)
        {
            throw new InvalidOperationException("Cannot begin frame. WebGPU has not been initialized.");
        }

        // Configure the swap chain on first-frame, or after a resize.
        // This is deferred from Initialize() because the native window
        // may not have reached its final framebuffer size yet at that point.
        if (!this.surfaceConfigured)
        {
            this.surfaceConfigured = this.surface.Configure();
        }

        if (this.surfaceTextureHandle is null)
        {
            this.surfaceTextureHandle = this.surface.GetSurfaceTexture();
        }
        else
        {
            this.surfaceTextureHandle.ResetHandle(this.surface.Handle);
        }

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

        if (this.textureViewHandle is null)
        {
            this.textureViewHandle = new SafeTextureViewHandle(
                this.grfxDevice.Wgpu,
                this.surfaceTextureHandle,
                in viewDesc);
        }
        else
        {
            this.textureViewHandle.ResetHandle(this.surfaceTextureHandle, in viewDesc);
        }

        if (this.textureViewHandle.IsInvalid)
        {
            IsValid = false;

            return false;
        }

        var encoderDesc = default(CommandEncoderDescriptor);

        if (this.cmdEncoderHandle is null)
        {
            this.cmdEncoderHandle = this.grfxDevice.Wgpu.DeviceCreateCommandEncoder(this.grfxDevice.Handle!, in encoderDesc);
        }
        else
        {
            this.cmdEncoderHandle.ResetHandle(encoderDesc, this.grfxDevice.Handle);
        }

        var clearValue = ToLinearClearColor(clearColor, this.surface.Format);

        if (this.renderPassHandle is null)
        {
            var passHandle = this.grfxDevice.Wgpu.CommandEncoderBeginRenderPass(
                this.cmdEncoderHandle,
                this.textureViewHandle,
                LoadOp.Clear,
                StoreOp.Store,
                clearValue.R,
                clearValue.G,
                clearValue.B,
                clearValue.A);

            this.renderPassHandle = passHandle;
        }
        else
        {
            this.renderPassHandle.ResetHandle(this.cmdEncoderHandle, this.textureViewHandle, clearValue);
        }

        IsValid = true;
        this.hasBegun = true;

        return true;
    }

    /// <inheritdoc/>
    public void Submit()
    {
        if (!this.hasBegun)
        {
            throw new InvalidOperationException($"The '{nameof(Frame)}.{nameof(Submit)}()' method was invoked without invoking '{nameof(Frame)}.{nameof(Begin)}()'.");
        }

        if (this.renderPassHandle is null)
        {
            throw new InvalidOperationException($"The render pass handle cannot be null. You must invoke the '{nameof(Frame)}.{nameof(Begin)}()' method first before invoking the '{nameof(Frame)}.{nameof(Submit)}()'.");
        }

        if (this.cmdEncoderHandle is null)
        {
            throw new InvalidOperationException($"The encoder handle cannot be null. You must invoke the '{nameof(Frame)}.{nameof(Begin)}()' method first before invoking the '{nameof(Frame)}.{nameof(Submit)}()'.");
        }

        this.renderPassHandle.End();

        var cmdBufDesc = default(CommandBufferDescriptor);
        var cmdBuf = this.grfxDevice.Wgpu.CommandEncoderFinish(this.cmdEncoderHandle, in cmdBufDesc);

        try
        {
            this.grfxDevice.Wgpu.QueueSubmit(this.grfxDevice.Queue!, 1, cmdBuf);
        }
        finally
        {
            this.grfxDevice.Wgpu.CommandBufferRelease(cmdBuf);
        }

        this.grfxDevice.Wgpu.SurfacePresent(this.surface.Handle);
        this.hasBegun = false;
    }

    /// <inheritdoc/>
    public void Dispose() => Dispose(true);

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
    /// Builds the clear color value for the render pass.
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

    /// <inheritdoc cref="IDisposable.Dispose"/>
    /// <param name="disposing">True to dispose of managed resources.</param>
    private void Dispose(bool disposing)
    {
        if (this.isDisposed)
        {
            return;
        }

        if (disposing)
        {
            this.renderPassHandle?.Dispose();
            this.cmdEncoderHandle?.Dispose();
            this.textureViewHandle?.Dispose();
            this.surfaceTextureHandle?.Dispose();
            this.initialized = false;
        }

        this.isDisposed = true;
    }
}

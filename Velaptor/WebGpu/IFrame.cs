// <copyright file="IFrame.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;
using Silk.NET.WebGPU;
using NETColor = System.Drawing.Color;

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
/// a <c>using</c> block call <see cref="IDisposable.Dispose"/> to release the per-frame handles.
/// </para>
/// </remarks>
internal interface IFrame : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the frame is valid and ready for draw calls.
    /// </summary>
    // ReSharper disable once UnusedMemberInSuper.Global
    bool IsValid { get; }

    /// <summary>
    /// Gets the active render pass encoder, or <c>null</c> if <see cref="Begin"/> has not
    /// been called yet or the frame is invalid.
    /// </summary>
    SafeRenderPassEncoderHandle? RenderPass { get; }

    /// <summary>
    /// Marks the swap chain as needing reconfiguration on the next <see cref="Begin"/> call.
    /// Call this from the window resize handler when the framebuffer dimensions change.
    /// </summary>
    void Reconfigure();

    /// <summary>
    /// Performs the full WebGPU initialization sequence: creates the platform surface,
    /// requests a GPU adapter, initializes the logical device, and queries the surface
    /// format. Swap chain configuration is deferred to the first <see cref="Begin"/>
    /// call when the window has reached its final size.
    /// Must be called after the window has been created and shown.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Acquires this frame's render target from the swap chain, creates a command encoder,
    /// and opens a render pass that clears the target to <paramref name="clearColor"/>.
    /// </summary>
    /// <param name="clearColor">The RGBA color to fill the render target with before drawing.</param>
    /// <returns>
    /// <c>true</c> if the frame is ready for draw calls; <c>false</c> if the swap chain
    /// could not provide a texture.
    /// </returns>
    bool Begin(NETColor clearColor);

    /// <summary>
    /// Closes the render pass, seals the command buffer, submits it to the GPU queue,
    /// and presents the completed frame to the display.
    /// </summary>
    void Submit();
}

// <copyright file="IGraphicsTexturePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for 2-D textured quads — the GPU state
/// object that controls how vertices are transformed and how pixels are colored
/// when drawing textures and font atlases.
/// </summary>
/// <remarks>
/// <para>
/// This pipeline hard-codes two programmable stages (vertex and fragment shaders)
/// together with all fixed-function states: primitive topology, blend mode, the pixel
/// format of the color target, and multisampling settings.
/// </para>
/// <para>
/// The bind group layout for <c>@group(0)</c> exposes binding 0 (2-D float texture)
/// and binding 1 (filtering sampler). Pass <see cref="BindGroupLayout"/> to
/// the texture buffer so it can create a compatible bind group.
/// </para>
/// <para>
/// Pipelines are immutable by design. Creating a pipeline is expensive — the driver
/// compiles and validates the combined state against the device. Binding one during
/// rendering is inexpensive. To change shaders or blend behavior, create a new pipeline up
/// front, then switch between pre‑compiled pipelines at draw time.
/// </para>
/// </remarks>
internal interface IGraphicsTexturePipeline : IDisposable
{
    /// <summary>
    /// Gets the bind group layout for <c>@group(0)</c>: binding 0 = a 2-D float texture,
    /// binding 1 = a filtering sampler. Pass this to the texture buffer so it can create a
    /// compatible bind group.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    SafeBindGroupLayoutHandle BindGroupLayout { get; }

    /// <summary>
    /// Compiles the shader, creates the bind group layout, and builds the GPU render
    /// pipeline. Must be called after the WebGPU device has been initialized and the
    /// surface has been configured.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's shaders and fixed-function state until a different
    /// pipeline is bound or the pass ends.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    void Bind(SafeRenderPassEncoderHandle pass);
}

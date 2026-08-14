// <copyright file="IGraphicsShapePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for drawing rounded rectangles. Unlike the
/// texture pipeline, this pipeline uses a per-vertex shape attribute layout
/// (position, bounding box, color, corner radii, etc.) with no bind groups.
/// </summary>
/// <remarks>
/// <para>
/// Pipeline layout is <b>empty</b> — no bind groups (no textures, no camera uniform).
/// The fragment shader uses <c>@builtin(position)</c> directly for pixel-space
/// coordinates.
/// </para>
/// <para>
/// Vertex stride is 64 bytes with 9 attributes.
/// </para>
/// </remarks>
internal interface IGraphicsShapePipeline : IDisposable
{
    /// <summary>
    /// Compiles the shader and builds the GPU render pipeline. Must be called after
    /// the WebGPU device has been initialized and the surface has been configured.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's shape shaders and fixed-function state.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    void Bind(SafeRenderPassEncoderHandle pass);
}

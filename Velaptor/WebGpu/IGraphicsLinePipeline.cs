// <copyright file="IGraphicsLinePipeline.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// A compiled, immutable render pipeline for drawing 2-D lines. The vertex shader
/// passes through NDC positions and per-vertex color; the fragment shader applies
/// sRGB→linear conversion and outputs the color directly.
/// </summary>
/// <remarks>
/// <para>
/// Pipeline layout is <b>empty</b> — no bind groups. Vertex stride is 24 bytes:
/// vec2 position + vec4 color.
/// </para>
/// </remarks>
internal interface IGraphicsLinePipeline : IDisposable
{
    /// <summary>
    /// Compiles the shader and builds the GPU render pipeline. Must be called after
    /// the WebGPU device has been initialized and the surface has been configured.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Activates this pipeline on <paramref name="pass"/>. All subsequent draw calls on
    /// the pass will use this pipeline's line shaders and fixed-function state.
    /// </summary>
    /// <param name="pass">The active render pass encoder to bind to.</param>
    void Bind(SafeRenderPassEncoderHandle pass);
}

// <copyright file="SafeRenderPipelineDescriptor.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Structures;

using Handles;
using Silk.NET.WebGPU;

/// <summary>
/// A safe version of <see cref="RenderPipelineDescriptor"/>. All pointer
/// fields are replaced with safe handles and managed data so callers never
/// need <c>unsafe</c> context. The marshaling to native pointers happens
/// inside <see cref="IWgpuInvoker"/>.
/// </summary>
internal struct SafeRenderPipelineDescriptor
{
    /// <summary>
    /// The pipeline layout (bind-group configuration) shared by all shader stages.
    /// </summary>
    public SafePipelineLayoutHandle Layout;

    /// <summary>
    /// The vertex shader stage configuration.
    /// </summary>
    public SafeVertexState Vertex;

    /// <summary>
    /// The fragment shader stage configuration.
    /// </summary>
    public SafeFragmentState Fragment;

    /// <summary>
    /// Primitive topology, front-face winding, and cull mode.
    /// </summary>
    public PrimitiveState Primitive;

    /// <summary>
    /// Multisample count and mask.
    /// </summary>
    public MultisampleState Multisample;
}

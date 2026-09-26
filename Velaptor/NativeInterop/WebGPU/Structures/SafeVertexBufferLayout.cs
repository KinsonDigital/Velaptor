// <copyright file="SafeVertexBufferLayout.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Structures;

using Silk.NET.WebGPU;

/// <summary>
/// A safe version of <see cref="VertexBufferLayout"/> that holds vertex
/// attributes as a managed array instead of a raw pointer.
/// </summary>
internal struct SafeVertexBufferLayout
{
    /// <summary>
    /// The stride, in bytes, between consecutive vertex records.
    /// </summary>
    public ulong ArrayStride;

    /// <summary>
    /// Whether each slot advances per-vertex or per-instance.
    /// </summary>
    public VertexStepMode StepMode;

    /// <summary>
    /// The per-attribute format, offset, and shader-location descriptors.
    /// The length of this array sets <c>AttributeCount</c> in the native struct.
    /// </summary>
    public VertexAttribute[] Attributes;
}

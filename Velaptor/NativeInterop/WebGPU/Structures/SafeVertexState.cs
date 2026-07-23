// <copyright file="SafeVertexState.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.WebGpu.Structures;

using Handles;

/// <summary>
/// A safe version of the vertex stage descriptor. Holds a safe shader module
/// handle, a managed entry-point name, and a managed array of buffer layouts.
/// </summary>
internal struct SafeVertexState
{
    /// <summary>
    /// The compiled vertex shader module.
    /// </summary>
    public SafeShaderModuleHandle Module;

    /// <summary>
    /// The name of the vertex entry-point function (e.g. <c>"vs_main"</c>).
    /// </summary>
    public string EntryPoint;

    /// <summary>
    /// One layout per vertex buffer slot.
    /// </summary>
    public SafeVertexBufferLayout[] Buffers;
}

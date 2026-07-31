// <copyright file="IGraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Loads separate vertex and fragment WGSL shader sources from embedded resources
/// and compiles them into GPU shader module handles via <see cref="GraphicsDevice"/>.
/// </summary>
/// <remarks>
/// <para>
/// Shader modules are not created until <see cref="Initialize"/> is called, which
/// must happen after the WebGPU device is available. Once a pipeline is built using
/// <see cref="VertexHandle"/> and <see cref="FragmentHandle"/>, the pipeline retains
/// its own internal reference to the compiled code. It is therefore safe — and
/// recommended — to dispose this object immediately after the pipeline is created.
/// </para>
/// </remarks>
internal interface IGraphicsShader : IDisposable
{
    /// <summary>
    /// Gets the compiled vertex shader module handle.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    SafeShaderModuleHandle VertexHandle { get; }

    /// <summary>
    /// Gets the compiled fragment shader module handle.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    SafeShaderModuleHandle FragmentHandle { get; }

    /// <summary>
    /// Compiles the vertex and fragment WGSL shader sources into GPU shader modules.
    /// Must be called after the WebGPU device has been initialized.
    /// </summary>
    /// <param name="gd">The graphics device used to compile the shaders.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="gd"/> is null.</exception>
    void Initialize(IGraphicsDevice gd);
}

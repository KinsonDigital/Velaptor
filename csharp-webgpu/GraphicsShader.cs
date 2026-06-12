// <copyright file="GraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using Handles;

/// <summary>
/// Loads separate vertex and fragment WGSL shader modules from disk and
/// compiles them into GPU shader module handles.
/// </summary>
/// <remarks>
/// <para>
/// Once a <see cref="GraphicsPipeline"/> or <see cref="GraphicsRectPipeline"/> is
/// built using <see cref="VertexHandle"/> and <see cref="FragmentHandle"/>, the pipeline
/// retains its own internal reference to the compiled code. It is therefore safe — and
/// recommended — to dispose this object immediately after the pipeline is created.
/// </para>
/// </remarks>
internal sealed class GraphicsShader : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShader"/> class.
    /// Loads the vertex WGSL from <paramref name="vertFilePath"/> and the fragment
    /// WGSL from <paramref name="fragFilePath"/>, then compiles each into a separate
    /// shader module.
    /// </summary>
    /// <param name="gd">The graphics device used to compile the shaders.</param>
    /// <param name="vertFilePath">Path to the vertex <c>.wgsl</c> file.</param>
    /// <param name="fragFilePath">Path to the fragment <c>.wgsl</c> file.</param>
    /// <exception cref="Exception">Thrown if either shader module could not be compiled.</exception>
    public GraphicsShader(GraphicsDevice gd, string vertFilePath, string fragFilePath)
    {
        var vertWgsl = File.ReadAllText(vertFilePath);
        var fragWgsl = File.ReadAllText(fragFilePath);

        VertexHandle = gd.CreateShaderModule(vertWgsl);
        FragmentHandle = gd.CreateShaderModule(fragWgsl);
    }

    /// <summary>
    /// Gets the compiled vertex shader module handle. Pass this to
    /// <see cref="GraphicsPipeline"/> or <see cref="GraphicsRectPipeline"/> during construction.
    /// </summary>
    public SafeShaderModuleHandle VertexHandle { get; }

    /// <summary>
    /// Gets the compiled fragment shader module handle. Pass this to
    /// <see cref="GraphicsPipeline"/> or <see cref="GraphicsRectPipeline"/> during construction.
    /// </summary>
    public SafeShaderModuleHandle FragmentHandle { get; }

    /// <summary>
    /// Releases both shader module handles. Any pipeline that was built from these
    /// handles retains its own internal reference to the compiled code and remains
    /// valid after disposal.
    /// </summary>
    public void Dispose()
    {
        VertexHandle.Dispose();
        FragmentHandle.Dispose();
    }
}

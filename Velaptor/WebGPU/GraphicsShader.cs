// <copyright file="GraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU;

using System;
using System.IO.Abstractions;
using Services;
using Velaptor.NativeInterop.WebGPU.Handles;

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
internal sealed class GraphicsShader : IDisposable
{
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
    private readonly IPath path;
    private readonly string shaderName;
    private SafeShaderModuleHandle? vertexHandle;
    private SafeShaderModuleHandle? fragmentHandle;
    private bool isDisposed;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShader"/> class.
    /// </summary>
    /// <param name="resourceLoaderService">Loads embedded WGSL resource files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <param name="shaderName">
    ///     The base name of the shader (e.g., <c>"texture"</c>).
    ///     The vertex shader is loaded from <c>shaderName.vert.wgsl</c> and the
    ///     fragment shader from <c>shaderName.frag.wgsl</c>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="resourceLoaderService"/> or <paramref name="path"/> is null.
    /// </exception>
    public GraphicsShader(
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IPath path,
        string shaderName)
    {
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(path);

        this.resourceLoaderService = resourceLoaderService;
        this.path = path;
        this.shaderName = shaderName;
    }

    /// <summary>
    /// Gets the compiled vertex shader module handle.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    public SafeShaderModuleHandle VertexHandle
    {
        get
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException(
                    "Shader has not been initialized. Call Initialize() first.");
            }

            return this.vertexHandle!;
        }
    }

    /// <summary>
    /// Gets the compiled fragment shader module handle.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/> is called.</exception>
    public SafeShaderModuleHandle FragmentHandle
    {
        get
        {
            if (!this.isInitialized)
            {
                throw new InvalidOperationException(
                    "Shader has not been initialized. Call Initialize() first.");
            }

            return this.fragmentHandle!;
        }
    }

    /// <summary>
    /// Compiles the vertex and fragment WGSL shader sources into GPU shader modules.
    /// Must be called after the WebGPU device has been initialized.
    /// </summary>
    /// <param name="gd">The graphics device used to compile the shaders.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="gd"/> is null.</exception>
    public void Initialize(GraphicsDevice gd)
    {
        if (this.isInitialized)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gd);

        var vertSource = LoadWgslSource($"{this.shaderName}.vert");
        var fragSource = LoadWgslSource($"{this.shaderName}.frag");

        this.vertexHandle = gd.CreateShaderModule(vertSource);
        this.fragmentHandle = gd.CreateShaderModule(fragSource);

        this.isInitialized = true;
    }

    /// <summary>
    /// Releases both shader module handles. Any pipeline that was built from these
    /// handles retains its own internal reference to the compiled code and remains
    /// valid after disposal.
    /// </summary>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.vertexHandle?.Dispose();
        this.fragmentHandle?.Dispose();
    }

    /// <summary>
    /// Loads the WGSL source for a shader stage from embedded resources.
    /// </summary>
    /// <param name="shaderName">
    ///     The embedded resource name suffix (e.g., <c>"texture.vert"</c>).
    /// </param>
    /// <returns>The WGSL source code string.</returns>
    private string LoadWgslSource(string shaderName)
    {
        var wgslFileName = $"{shaderName}.wgsl";
        return this.resourceLoaderService.LoadResource(wgslFileName);
    }
}

// <copyright file="GraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using System.IO.Abstractions;
using Services;
using NativeInterop.WebGpu.Handles;

/// <inheritdoc/>
internal sealed class GraphicsShader : IGraphicsShader
{
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public void Initialize(IGraphicsDevice gd)
    {
        if (this.isInitialized)
        {
            return;
        }

        ArgumentNullException.ThrowIfNull(gd);

        var vertSource = this.resourceLoaderService.LoadResource($"{this.shaderName}.vert.wgsl");
        var fragSource = this.resourceLoaderService.LoadResource($"{this.shaderName}.frag.wgsl");

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
}

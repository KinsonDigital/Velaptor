// <copyright file="GraphicsShader.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

using System;
using System.IO.Abstractions;
using System.Linq;
using Services;
using NativeInterop.WebGpu.Handles;

/// <inheritdoc/>
internal sealed class GraphicsShader : IGraphicsShader
{
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
    private SafeShaderModuleHandle? vertexHandle;
    private SafeShaderModuleHandle? fragmentHandle;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShader"/> class.
    /// </summary>
    /// <param name="resourceLoaderService">Loads embedded WGSL resource files.</param>
    /// <param name="path">Processes directory and file paths.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="resourceLoaderService"/> or <paramref name="path"/> is null.
    /// </exception>
    public GraphicsShader(IEmbeddedResourceLoaderService<string> resourceLoaderService, IPath path)
    {
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(path);

        this.resourceLoaderService = resourceLoaderService;
    }

    /// <inheritdoc/>
    public void Initialize(IGraphicsDevice grfxDevice, TypeOfShader shaderType, Action<SafeShaderModuleHandle, SafeShaderModuleHandle> onInitialized)
    {
        ArgumentNullException.ThrowIfNull(grfxDevice);

        if (!Enum.GetValues<TypeOfShader>().Contains(shaderType))
        {
            throw new ArgumentException($"The '{nameof(TypeOfShader)}.{shaderType}' is invalid. Could not create vertex and fragment shaders.");
        }

        var vertSource = this.resourceLoaderService.LoadResource($"{shaderType}.vert.wgsl");
        var fragSource = this.resourceLoaderService.LoadResource($"{shaderType}.frag.wgsl");

        this.vertexHandle = grfxDevice.CreateShaderModule(vertSource);
        this.fragmentHandle = grfxDevice.CreateShaderModule(fragSource);

        onInitialized.Invoke(this.vertexHandle, this.fragmentHandle);

        // NOTE: Ensure to only release/dispose of the handles AFTER the initialization is complete.
        this.vertexHandle?.Dispose();
        this.vertexHandle = null;

        this.fragmentHandle?.Dispose();
        this.fragmentHandle = null;
    }
}

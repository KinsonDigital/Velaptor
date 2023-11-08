// <copyright file="TextureShaderResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Services;

using System;
using System.IO.Abstractions;
using Velaptor.Services;

/// <summary>
/// Loads the source code for the vertex and fragment shaders for rendering 2D textures.
/// </summary>
/// <remarks>These are loaded from the embedded library resources.</remarks>
internal sealed class TextureShaderResourceLoaderService : IShaderLoaderService
{
    private const string VertShaderFileExtension = ".vert";
    private const string FragShaderFileExtension = ".frag";
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShaderResourceLoaderService"/> class.
    /// </summary>
    /// <param name="resourceLoaderService">Loads the shader from the embedded resources.</param>
    /// <param name="path">Processes directory and file paths.</param>
    public TextureShaderResourceLoaderService(IEmbeddedResourceLoaderService<string> resourceLoaderService, IPath path)
    {
        ArgumentNullException.ThrowIfNull(resourceLoaderService);
        ArgumentNullException.ThrowIfNull(path);

        this.resourceLoaderService = resourceLoaderService;
        this.path = path;
    }

    /// <inheritdoc cref="IShaderLoaderService.LoadVertSource"/>
    /// <remarks>
    ///     The props in this context only need to be a single tuple item which is the batch size.
    /// </remarks>
    public string LoadVertSource(string shaderName)
    {
        var vertShaderName = this.path.HasExtension(shaderName)
            ? $"{this.path.GetFileNameWithoutExtension(shaderName)}{VertShaderFileExtension}"
            : $"{shaderName}{VertShaderFileExtension}";

        // Load the vertex and shader source code
        var vertShaderSrc = this.resourceLoaderService.LoadResource(vertShaderName);

        return vertShaderSrc;
    }

    /// <inheritdoc cref="IShaderLoaderService.LoadFragSource"/>
    /// <remarks>
    ///     The props in this context will be ignored.
    /// </remarks>
    public string LoadFragSource(string shaderName)
    {
        var fragShaderName = this.path.HasExtension(shaderName)
            ? $"{this.path.GetFileNameWithoutExtension(shaderName)}{FragShaderFileExtension}"
            : $"{shaderName}{FragShaderFileExtension}";

        // Load the fragment and vertices shader source code
        return this.resourceLoaderService.LoadResource(fragShaderName);
    }
}

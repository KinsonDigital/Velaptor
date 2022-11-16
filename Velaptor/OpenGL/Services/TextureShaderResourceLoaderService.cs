// <copyright file="TextureShaderResourceLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Velaptor.Guards;
using Velaptor.Services;

namespace Velaptor.OpenGL.Services;

/// <summary>
/// Loads the source code for the vertex and fragment shaders for rendering 2D textures.
/// </summary>
/// <remarks>These are loaded from the embedded library resources.</remarks>
internal sealed class TextureShaderResourceLoaderService : IShaderLoaderService<uint>
{
    private const string BatchSizeVarName = "BATCH_SIZE";
    private const string VertShaderFileExtension = ".vert";
    private const string FragShaderFileExtension = ".frag";
    private readonly ITemplateProcessorService shaderSrcTemplateService;
    private readonly IEmbeddedResourceLoaderService<string> resourceLoaderService;
    private readonly IPath path;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShaderResourceLoaderService"/> class.
    /// </summary>
    /// <param name="shaderSrcTemplateService">Processes template variables in shader source code.</param>
    /// <param name="resourceLoaderService">Loads the shader from the embedded resources.</param>
    /// <param name="path">Processes directory and file paths.</param>
    public TextureShaderResourceLoaderService(
        ITemplateProcessorService shaderSrcTemplateService,
        IEmbeddedResourceLoaderService<string> resourceLoaderService,
        IPath path)
    {
        EnsureThat.ParamIsNotNull(shaderSrcTemplateService);
        EnsureThat.ParamIsNotNull(resourceLoaderService);
        EnsureThat.ParamIsNotNull(path);

        this.shaderSrcTemplateService = shaderSrcTemplateService;
        this.resourceLoaderService = resourceLoaderService;
        this.path = path;
    }

    /// <inheritdoc cref="IShaderLoaderService{TValue}.LoadVertSource"/>
    /// <remarks>
    ///     The props in this context only need to be a single tuple item which is the batch size.
    /// </remarks>
    public string LoadVertSource(string shaderName, IEnumerable<(string name, uint value)>? props = null)
    {
        const string missingTemplateVariableMessage =
            $"Missing the property '{BatchSizeVarName}' template variable for shader processing.";

        if (props is null)
        {
            throw new Exception(missingTemplateVariableMessage);
        }

        var propsList = props as (string name, uint value)[] ?? props.ToArray();
        var varNameExists = (from p in propsList
            where p.name == BatchSizeVarName
            select p.name).FirstOrDefault();

        if (varNameExists is null)
        {
            throw new Exception(missingTemplateVariableMessage);
        }

        var vertShaderName = this.path.HasExtension(shaderName)
            ? $"{this.path.GetFileNameWithoutExtension(shaderName)}{VertShaderFileExtension}"
            : $"{shaderName}{VertShaderFileExtension}";

        var batchSize = (from p in propsList
            where p.name == BatchSizeVarName
            select p.value).FirstOrDefault();

        // Creates the required vert shader variables to process
        IEnumerable<(string, string)> vertVars = new[]
        {
            (BatchSizeVarName, batchSize.ToString()),
        };

        // Load the vertex and shader source code
        var vertShaderSrc = this.resourceLoaderService.LoadResource(vertShaderName);

        // Send the source code through the template variable service to process any template variables
        vertShaderSrc = this.shaderSrcTemplateService.ProcessTemplateVariables(vertShaderSrc, vertVars);

        return vertShaderSrc;
    }

    /// <inheritdoc cref="IShaderLoaderService{TValue}.LoadFragSource"/>
    /// <remarks>
    ///     The props in this context will be ignored.
    /// </remarks>
    public string LoadFragSource(string shaderName, IEnumerable<(string name, uint value)>? props = null)
    {
        var fragShaderName = this.path.HasExtension(shaderName)
            ? $"{this.path.GetFileNameWithoutExtension(shaderName)}{FragShaderFileExtension}"
            : $"{shaderName}{FragShaderFileExtension}";

        // Load the fragment and vertices shader source code
        return this.resourceLoaderService.LoadResource(fragShaderName);
    }
}

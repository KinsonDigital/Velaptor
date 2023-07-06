// <copyright file="IShaderLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Services;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Loads the source code for the vertex and fragment shaders from a resource.
/// </summary>
internal interface IShaderLoaderService
{
    /// <summary>
    /// Loads the texture vertex shader source code.
    /// </summary>
    /// <param name="shaderName">The name of the vertex shader.</param>
    /// <returns>
    ///     The texture shader source code.
    /// </returns>
    string LoadVertSource(string shaderName);

    /// <summary>
    /// Loads the texture fragment shader source code.
    /// </summary>
    /// <param name="shaderName">The name of the fragment shader.</param>
    /// <returns>
    ///     The texture shader source code.
    /// </returns>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Need to keep API as is.")]
    string LoadFragSource(string shaderName);
}

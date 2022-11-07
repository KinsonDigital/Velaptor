// <copyright file="IShaderLoaderService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Velaptor.OpenGL.Services;

/// <summary>
/// Loads the source code for the vertex and fragment shaders from a resource.
/// </summary>
/// <typeparam name="TValue">The type of data to use for the loading process.</typeparam>
internal interface IShaderLoaderService<TValue>
{
    /// <summary>
    /// Loads the texture vertex shader source code.
    /// </summary>
    /// <param name="shaderName">The name of the vertex shader.</param>
    /// <param name="props">Contains the size of the batch for the shader to operate in.</param>
    /// <returns>
    ///     The texture shader source code.
    /// </returns>
    string LoadVertSource(string shaderName, IEnumerable<(string name, TValue value)>? props = null);

    /// <summary>
    /// Loads the texture fragment shader source code.
    /// </summary>
    /// <param name="shaderName">The name of the fragment shader.</param>
    /// <param name="props">Contains the size of the batch for the shader to operate in.</param>
    /// <returns>
    ///     The texture shader source code.
    /// </returns>
    [SuppressMessage("ReSharper", "UnusedParameter.Global", Justification = "Need to keep API as is.")]
    string LoadFragSource(string shaderName, IEnumerable<(string name, TValue value)>? props = null);
}

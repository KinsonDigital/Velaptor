// <copyright file="IShaderManager.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

/// <summary>
/// Manages shaders.
/// </summary>
internal interface IShaderManager
{
    /// <summary>
    /// Gets the ID of a shader that matches the given <paramref name="shaderType"/>.
    /// </summary>
    /// <param name="shaderType">The type of shader.</param>
    /// <returns>The ID of the shader.</returns>
    uint GetShaderId(ShaderType shaderType);

    /// <summary>
    /// Gets the name of a shader that matches the given <paramref name="shaderType"/>.
    /// </summary>
    /// <param name="shaderType">The type of shader.</param>
    /// <returns>The name of the shader.</returns>
    string GetShaderName(ShaderType shaderType);

    /// <summary>
    /// Uses a shader that matches the given <paramref name="shaderType"/>.
    /// </summary>
    /// <param name="shaderType">The name type of shader.</param>
    void Use(ShaderType shaderType);
}

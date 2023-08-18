// <copyright file="IShaderFactory.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.Factories;

using OpenGL.Shaders;

/// <summary>
/// Creates different shaders.
/// </summary>
internal interface IShaderFactory
{
    /// <summary>
    /// Creates a shader for rendering textures.
    /// </summary>
    /// <returns>The shader program.</returns>
    IShaderProgram CreateTextureShader();

    /// <summary>
    /// Creates a shader for rendering text using a font.
    /// </summary>
    /// <returns>The shader program.</returns>
    IShaderProgram CreateFontShader();

    /// <summary>
    /// Creates a shader for rendering rectangles.
    /// </summary>
    /// <returns>The shader program.</returns>
    IShaderProgram CreateShapeShader();

    /// <summary>
    /// Creates a shader for rendering lines.
    /// </summary>
    /// <returns>The shader program.</returns>
    IShaderProgram CreateLineShader();
}

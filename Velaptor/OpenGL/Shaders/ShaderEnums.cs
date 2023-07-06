// <copyright file="ShaderEnums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Shaders;

/// <summary>
/// Represents a type of shader.
/// </summary>
internal enum ShaderType
{
    /// <summary>
    /// A texture shader.
    /// </summary>
    Texture = 1,

    /// <summary>
    /// A font shader.
    /// </summary>
    Font = 2,

    /// <summary>
    /// A rectangle shape shader.
    /// </summary>
    Rectangle = 3,

    /// <summary>
    /// A line shader.
    /// </summary>
    Line = 4,
}

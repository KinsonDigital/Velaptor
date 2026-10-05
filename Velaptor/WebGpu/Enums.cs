// <copyright file="Enums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu;

/// <summary>
/// Represents the types of shaders.
/// </summary>
internal enum TypeOfShader
{
    /// <summary>
    /// Texture shader.
    /// </summary>
    Texture,

    /// <summary>
    /// Shape shader.
    /// </summary>
    Shape,

    /// <summary>
    /// Line shader.
    /// </summary>
    Line,
}

/// <summary>
/// Represents the position of a vertex.
/// </summary>
internal enum VertexPosition
{
    /// <summary>
    /// The top left vertex.
    /// </summary>
    TopLeft = 1,

    /// <summary>
    /// The top right vertex.
    /// </summary>
    TopRight = 2,

    /// <summary>
    /// The bottom right vertex.
    /// </summary>
    BottomRight = 3,

    /// <summary>
    /// The bottom left vertex.
    /// </summary>
    BottomLeft = 4,
}

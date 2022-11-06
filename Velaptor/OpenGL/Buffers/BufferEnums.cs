// <copyright file="BufferEnums.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

/// <summary>
/// Represents a type of GPU buffer.
/// </summary>
internal enum VelaptorBufferType
{
    /// <summary>
    /// A texture GPU buffer.
    /// </summary>
    Texture = 1,

    /// <summary>
    /// A font glpyh GPU buffer.
    /// </summary>
    Font = 2,

    /// <summary>
    /// A rectangle GPU buffer.
    /// </summary>
    Rectangle = 3,
}

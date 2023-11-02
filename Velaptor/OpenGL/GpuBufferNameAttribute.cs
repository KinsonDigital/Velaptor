// <copyright file="GpuBufferNameAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL;

using System;
using Guards;

/// <summary>
/// Represents the name of a buffer.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class GpuBufferNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GpuBufferNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The name to give a buffer.</param>
    public GpuBufferNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        Name = name;
    }

    /// <summary>
    /// Gets the name of a buffer.
    /// </summary>
    public string Name { get; }
}

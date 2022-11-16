// <copyright file="ShaderNameAttribute.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Velaptor.Guards;

namespace Velaptor.OpenGL;

/// <summary>
/// Represents the name of a shader.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal sealed class ShaderNameAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShaderNameAttribute"/> class.
    /// </summary>
    /// <param name="name">The name to give a shader.</param>
    public ShaderNameAttribute(string name)
    {
        EnsureThat.StringParamIsNotNullOrEmpty(name);
        Name = name;
    }

    /// <summary>
    /// Gets the name of a shader.
    /// </summary>
    public string Name { get; }
}

// <copyright file="IShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    /// <summary>
    /// A shader program consisting of a vertex and fragment shader.
    /// </summary>
    internal interface IShaderProgram
    {
        /// <summary>
        /// Gets the shader program ID on the GPU.
        /// </summary>
        uint ShaderId { get; }

        /// <summary>
        /// Gets the name of the shader.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sets the active shader program to use on the GPU.
        /// </summary>
        void Use();
    }
}

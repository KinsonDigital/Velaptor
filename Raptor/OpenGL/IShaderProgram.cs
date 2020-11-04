// <copyright file="IShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;

    /// <summary>
    /// A shader program consisting of a vertex and fragment shader.
    /// </summary>
    internal interface IShaderProgram : IDisposable
    {
        /// <summary>
        /// Gets the shader program ID on the GPU.
        /// </summary>
        uint ProgramId { get; }

        /// <summary>
        /// Gets the ID of the fragment shader on the GPU.
        /// </summary>
        uint FragmentShaderId { get; }

        /// <summary>
        /// Gets the ID of the vertex shader.
        /// </summary>
        uint VertexShaderId { get; }

        /// <summary>
        /// Gets or sets the size of the batch.
        /// </summary>
        uint BatchSize { get; set; }

        /// <summary>
        /// Sets the active shader program to use on the GPU.
        /// </summary>
        void UseProgram();
    }
}

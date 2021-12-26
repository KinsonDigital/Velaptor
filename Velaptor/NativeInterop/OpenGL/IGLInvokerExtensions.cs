// <copyright file="IGLInvokerExtensions.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    using System.Drawing;
    using System.Numerics;
    using Velaptor.OpenGL;

    // TODO: Convert these into literal extension methods instead
    // This will prevent having to instantiate the implementation and injecting
    // Also look in IGLInvoker interface and pull out anything that should be an extensions and make it an extension

    /// <summary>
    /// Provides OpenGL extensions/helper methods to improve OpenGL related functionality.
    /// </summary>
    internal interface IGLInvokerExtensions
    {
        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        /// <returns>The size of the viewport.</returns>
        Size GetViewPortSize();

        /// <summary>
        /// Sets the size of the view port.
        /// </summary>
        /// <param name="size">The vector representing the width and height of the viewport.</param>
        void SetViewPortSize(Size size);

        /// <summary>
        /// Gets the position of the viewport.
        /// </summary>
        /// <returns>The position of the viewport.</returns>
        Vector2 GetViewPortPosition();

        /// <summary>
        /// Returns a value indicating if the program linking process was successful.
        /// </summary>
        /// <param name="program">The ID of the program to check.</param>
        /// <returns><see langword="true"/> if the linking was successful.</returns>
        bool LinkProgramSuccess(uint program);

        /// <summary>
        /// Returns a value indicating if the shader was compiled successfully.
        /// </summary>
        /// <param name="shaderId">The ID of the shader to check.</param>
        /// <returns><see langword="true"/> if the shader compiled successfully.</returns>
        bool ShaderCompileSuccess(uint shaderId);

        /// <summary>
        /// Creates a debug group into the command stream.
        /// </summary>
        /// <param name="label">The label for the debug group.</param>
        void BeginGroup(string label);

        /// <summary>
        /// Ends the most recent debug group.
        /// </summary>
        void EndGroup();

        /// <summary>
        /// Labels a shader with the given <paramref name="shaderId"/> with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="shaderId">The ID of the shader.</param>
        /// <param name="label">The label to give the shader.</param>
        void LabelShader(uint shaderId, string label);

        /// <summary>
        /// Labels a shader program with the given <paramref name="shaderId"/> with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="shaderId">The ID of the shader program.</param>
        /// <param name="label">The label to give the shader program.</param>
        void LabelShaderProgram(uint shaderId, string label);

        /// <summary>
        /// Labels a vertex array object with the given <paramref name="vertexArrayId"/> with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="vertexArrayId">The ID of the shader.</param>
        /// <param name="label">The label to give the vertex array object.</param>
        void LabelVertexArray(uint vertexArrayId, string label);

        /// <summary>
        /// Labels a buffer object with the given <paramref name="bufferId"/> with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="bufferId">The ID of the buffer object.</param>
        /// <param name="label">The label to give the buffer object.</param>
        /// <param name="bufferType">The type of buffer.</param>
        void LabelBuffer(uint bufferId, string label, BufferType bufferType);

        /// <summary>
        /// Labels a texture with the given <paramref name="textureId"/> with the given <paramref name="label"/>.
        /// </summary>
        /// <param name="textureId">The ID of the texture.</param>
        /// <param name="label">The label to give the texture.</param>
        void LabelTexture(uint textureId, string label);
    }
}

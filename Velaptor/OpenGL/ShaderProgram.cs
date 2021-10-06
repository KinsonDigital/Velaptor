// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Services;

    /// <inheritdoc/>
    internal sealed class ShaderProgram : IShaderProgram
    {
        private const string TextureShaderName = "texture-shader";
        private const string BatchSizeVarName = "BATCH_SIZE";
        private readonly IGLInvoker gl;
        private readonly IGLInvokerExtensions glExtensions;
        private readonly IShaderLoaderService<uint> shaderLoaderService;
        private bool isDisposed;
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes OpenGL extension methods.</param>
        /// <param name="shaderLoaderService">Loads the vertex and fragment shaders..</param>
        public ShaderProgram(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService)
        {
            this.gl = gl;
            this.glExtensions = glExtensions;
            this.shaderLoaderService = shaderLoaderService;
        }

        /// <inheritdoc/>
        public uint VertexShaderId { get; private set; }

        /// <inheritdoc/>
        public uint FragmentShaderId { get; private set; }

        /// <inheritdoc/>
        public uint ProgramId { get; private set; }

        /// <inheritdoc/>
        public uint BatchSize { get; set; } = 10;

        /// <inheritdoc/>
        public void Init()
        {
            if (this.isInitialized)
            {
                return;
            }

            IEnumerable<(string, uint)> templateVars = new[]
            {
                (BatchSizeVarName, BatchSize),
            };

            // Load the vertex shader source code and create a shader and upload to the GPU
            var vertexShaderSrc = this.shaderLoaderService.LoadVertSource(TextureShaderName, templateVars);
            VertexShaderId = CreateShader(GLShaderType.VertexShader, vertexShaderSrc);

            // Load the fragment shader source code and create a shader and upload to the GPU
            var fragShaderSrc = this.shaderLoaderService.LoadFragSource(TextureShaderName);
            FragmentShaderId = CreateShader(GLShaderType.FragmentShader, fragShaderSrc);

            // Merge both shaders into a shader program, which can then be used by OpenGL
            ProgramId = CreateShaderProgram(VertexShaderId, FragmentShaderId);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it.
            // The compiled code is copied into the shader program.
            // Detach and then delete them.
            DestroyShader(ProgramId, VertexShaderId);
            DestroyShader(ProgramId, FragmentShaderId);
            this.isInitialized = true;
        }

        /// <inheritdoc/>
        public void UseProgram() => this.gl.UseProgram(ProgramId);

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            // Delete unmanaged resources
            this.gl.DeleteProgram(ProgramId);

            this.isDisposed = true;
            this.isInitialized = false;
        }

        /// <summary>
        /// Creates a shader program using the given vertex and fragment shader ID numbers.
        /// </summary>
        /// <param name="vertexShaderId">The ID of the vertex shader.</param>
        /// <param name="fragmentShaderId">The ID of the fragment shader.</param>
        /// <returns>The shader program ID.</returns>
        private uint CreateShaderProgram(uint vertexShaderId, uint fragmentShaderId)
        {
            var programHandle = this.gl.CreateProgram();

            // Attach both shaders...
            this.gl.AttachShader(programHandle, vertexShaderId);

            this.gl.AttachShader(programHandle, fragmentShaderId);

            // Link them together
            LinkProgram(programHandle);

            return programHandle;
        }

        /// <summary>
        /// Links the program using the given <paramref name="shaderProgramId"/>.
        /// </summary>
        /// <param name="shaderProgramId">The ID of the shader program.</param>
        private void LinkProgram(uint shaderProgramId)
        {
            // We link the program
            this.gl.LinkProgram(shaderProgramId);

            // Check for linking errors
            if (!this.glExtensions.LinkProgramSuccess(shaderProgramId))
            {
                // We can use `this.gl.GetProgramInfoLog(program)` to get information about the error.
                var programInfoLog = this.gl.GetProgramInfoLog(shaderProgramId);

                throw new Exception($"Error occurred while linking program with ID '{shaderProgramId}'\n{programInfoLog}");
            }
        }

        /// <summary>
        /// Creates a shader of the given <paramref name="shaderType"/> using the
        /// given <paramref name="shaderSrc"/> code.
        /// </summary>
        /// <param name="shaderType">The type of shader to create.</param>
        /// <param name="shaderSrc">The shader source code to use for the shader program.</param>
        /// <returns>The OpenGL shader ID.</returns>
        private uint CreateShader(GLShaderType shaderType, string shaderSrc)
        {
            var shaderId = this.gl.CreateShader(shaderType);

            this.gl.ShaderSource(shaderId, shaderSrc);

            CompileShader(shaderId);

            return shaderId;
        }

        /// <summary>
        /// Destroys the shaders.  Any shaders setup and sent to the GPU will still reside there.
        /// </summary>
        /// <param name="shaderProgramId">The program ID of the shader.</param>
        /// <param name="shaderId">The shader ID of the shader.</param>
        private void DestroyShader(uint shaderProgramId, uint shaderId)
        {
            this.gl.DetachShader(shaderProgramId, shaderId);
            this.gl.DeleteShader(shaderId);
        }

        /// <summary>
        /// Compiles the currently set shader source code on the GPU.
        /// </summary>
        /// <param name="shaderId">The shader ID.</param>
        private void CompileShader(uint shaderId)
        {
            // Try to compile the shader
            this.gl.CompileShader(shaderId);

            // Check for compilation errors
            if (!this.glExtensions.ShaderCompileSuccess(shaderId))
            {
                var errorInfo = this.gl.GetShaderInfoLog(shaderId);

                throw new Exception($"Error occurred while compiling shader with ID '{shaderId}'\n{errorInfo}");
            }
        }
    }
}

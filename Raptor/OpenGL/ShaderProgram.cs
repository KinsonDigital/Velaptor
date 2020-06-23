// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;
    using FileIO.Core;
    using FileIO.File;
    using OpenToolkit.Graphics.OpenGL4;

    /// <summary>
    /// A shader program consisting of a vertex and fragment shader.
    /// </summary>
    public class ShaderProgram : IDisposable
    {
        private readonly IGLInvoker gl;
        private readonly ITextFile textFile;
        private int batchSize;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="batchSize">The batch size that the shader will support.</param>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        [ExcludeFromCodeCoverage]
        public ShaderProgram(int batchSize, string vertexShaderPath, string fragmentShaderPath)
        {
            this.gl = new GLInvoker();
            this.textFile = new TextFile();
            Init(batchSize, vertexShaderPath, fragmentShaderPath);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="textFile">Loads text file data.</param>
        /// <param name="batchSize">The batch size that the shader will support.</param>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        internal ShaderProgram(IGLInvoker gl, ITextFile textFile, int batchSize, string vertexShaderPath, string fragmentShaderPath)
        {
            this.gl = gl;
            this.textFile = textFile;
            Init(batchSize, vertexShaderPath, fragmentShaderPath);
        }

        /// <summary>
        /// Gets the ID of the vertex shader.
        /// </summary>
        public int VertexShaderId { get; private set; }

        /// <summary>
        /// Gets the ID of the fragment shader on the GPU.
        /// </summary>
        public int FragmentShaderId { get; private set; }

        /// <summary>
        /// Gets the shader program ID on the GPU.
        /// </summary>
        public int ProgramId { get; private set; }

        /// <summary>
        /// Sets the active shader program to use on the GPU.
        /// </summary>
        public void UseProgram() => this.gl.UseProgram(ProgramId);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed of.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue)
                return;

            // Delete unmanaged resources
            this.gl.DeleteProgram(ProgramId);

            this.disposedValue = true;
        }

        /// <summary>
        /// Initializes the shader program.
        /// </summary>
        /// <param name="batchSize">The batch size that the shader will support.</param>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        private void Init(int batchSize, string vertexShaderPath, string fragmentShaderPath)
        {
            this.batchSize = batchSize;

            var shaderSource = LoadShaderSourceCode(vertexShaderPath);
            VertexShaderId = CreateShader(ShaderType.VertexShader, shaderSource);

            // We do the same for the fragment shader
            shaderSource = LoadShaderSourceCode(fragmentShaderPath);
            FragmentShaderId = CreateShader(ShaderType.FragmentShader, shaderSource);

            // Merge both shaders into a shader program, which can then be used by Openthis.gl.
            ProgramId = CreateShaderProgram(VertexShaderId, FragmentShaderId);

            // When the shader program is linked, it no longer needs the individual shaders attacked to it.
            // The compiled code is copied into the shader program.
            // Detach and then delete them.
            DestroyShader(ProgramId, VertexShaderId);
            DestroyShader(ProgramId, FragmentShaderId);
        }

        /// <summary>
        /// Creates a shader program using the given vertex and fragment shader ID numbers.
        /// </summary>
        /// <param name="vertexShaderId">The ID of the vertex shader.</param>
        /// <param name="fragmentShaderId">The ID of the fragment shader.</param>
        /// <returns>The shader program ID.</returns>
        private int CreateShaderProgram(int vertexShaderId, int fragmentShaderId)
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
        private void LinkProgram(int shaderProgramId)
        {
            // We link the program
            this.gl.LinkProgram(shaderProgramId);

            // Check for linking errors
            this.gl.GetProgram(shaderProgramId, GetProgramParameterName.LinkStatus, out var statusCode);

            if (statusCode != (int)All.True)
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
        private int CreateShader(ShaderType shaderType, string shaderSrc)
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
        private void DestroyShader(int shaderProgramId, int shaderId)
        {
            this.gl.DetachShader(shaderProgramId, shaderId);
            this.gl.DeleteShader(shaderId);
        }

        /// <summary>
        /// Compiles the currently set shader source code on the GPU.
        /// </summary>
        /// <param name="shaderId">The shader ID.</param>
        private void CompileShader(int shaderId)
        {
            // Try to compile the shader
            this.gl.CompileShader(shaderId);

            // Check for compilation errors
            this.gl.GetShader(shaderId, ShaderParameter.CompileStatus, out var statusCode);

            if (statusCode != (int)All.True)
            {
                var errorInfo = this.gl.GetShaderInfoLog(shaderId);

                throw new Exception($"Error occurred while compiling shader with ID '{shaderId}'\n{errorInfo}");
            }
        }

        /// <summary>
        /// Loads the shader source code at the given <paramref name="shaderFilePath"/>.
        /// </summary>
        /// <param name="shaderFilePath">The file path of the shader source code file.</param>
        /// <returns>The source code.</returns>
        private string LoadShaderSourceCode(string shaderFilePath)
        {
            // Load the source code from the shader files
            var result = new StringBuilder();

            var sourceCodeLines = this.textFile.LoadAsLines(shaderFilePath);

            foreach (var line in sourceCodeLines)
            {
                var processedLine = ProcessLine(line is null ? string.Empty : line);

                result.AppendLine(processedLine);
            }

            return result.ToString();
        }

        /// <summary>
        /// Processes a line of source code to check for replacement tags.
        /// This will modify the source code depending on the tag.
        /// </summary>
        /// <param name="line">The line of source code to process.</param>
        /// <returns>The processed line of source code.</returns>
        private string ProcessLine(string line)
        {
            var result = string.Empty;

            // If the line of code has a replacement tag on it
            if (line.Contains("//$REPLACE_INDEX", StringComparison.Ordinal))
            {
                var sections = line.Split(' ');

                // Find the section with the array brackets
                for (var i = 0; i < sections.Length; i++)
                {
                    if (sections[i].Contains('[', StringComparison.Ordinal) && sections[i].Contains(']', StringComparison.Ordinal))
                    {
                        result += $"{sections[i].Split('[')[0]}[{this.batchSize}];//MODIFIED_DURING_COMPILE_TIME";
                    }
                    else
                    {
                        result += sections[i] + ' ';
                    }
                }
            }
            else
            {
                result = line;
            }

            return result;
        }
    }
}

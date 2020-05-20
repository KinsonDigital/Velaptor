using System;
using System.Collections.Generic;
using FileIO.Core;
using OpenToolkit.Graphics.OpenGL4;

namespace Raptor.GLHelperClasses
{
    /// <summary>
    /// A shader program consisting of a vertex and fragment shader.
    /// </summary>
    internal class ShaderProgram : IDisposable
    {
        #region Private Fields
        private readonly Dictionary<string, int> _uniformLocations = new Dictionary<string, int>();
        private readonly ITextFile _file;
        private bool _disposedValue;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ShaderProgram"/>.
        /// </summary>
        /// <param name="gl">Provides access to OpenGL funtionality.</param>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        public ShaderProgram(string vertexShaderPath, string fragmentShaderPath, ITextFile file)
        {
            _file = file;
            var shaderSource = _file.Load(vertexShaderPath);
            VertexShaderId = CreateShader(ShaderType.VertexShader, shaderSource);

            //We do the same for the fragment shader
            shaderSource = _file.Load(fragmentShaderPath);
            FragmentShaderId = CreateShader(ShaderType.FragmentShader, shaderSource);

            //Merge both shaders into a shader program, which can then be used by OpenGL.
            ProgramId = SetupShaderProgram(VertexShaderId, FragmentShaderId);

            //When the shader program is linked, it no longer needs the individual shaders attacked to it.
            //The compiled code is copied into the shader program.
            //Detach and then delete them.
            DestroyShader(ProgramId, VertexShaderId);
            DestroyShader(ProgramId, FragmentShaderId);

            //This is for the purpose of caching the locations of the uniforms.
            //The reason is because GetUniformLocation() is a slow call.
            //Get the number of active uniforms in the shader.
            GL.GetProgram(ProgramId, GetProgramParameterName.ActiveUniforms, out int totalActiveUniforms);

            //Loop over all the uniforms
            for (var i = 0; i < totalActiveUniforms; i++)
            {
                //get the name of this uniform,
                var key = GL.GetActiveUniform(ProgramId, i, out _, out _);

                //Get the location of the uniform on the GPU
                var location = GL.GetUniformLocation(ProgramId, key);

                if (location == -1)
                    throw new Exception($"The uniform with the name '{key}' does not exist.");

                //Cache it
                _uniformLocations.Add(key, location);
            }
        }
        #endregion


        #region Props
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
        #endregion


        #region Public Methods
        /// <summary>
        /// Sets the active shader program to use on the GPU.
        /// </summary>
        public void UseProgram() => GL.UseProgram(ProgramId);


        /// <summary>
        /// Disposes of the <see cref="ShaderProgram"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion


        #region Protected Methods
        /// <summary>
        /// Disposes of the internal resources if the given <paramref name="disposing"/> value is true.
        /// </summary>
        /// <param name="disposing">True to dispose of internal resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue)
                return;

            //Dipose of managed resources
            if (disposing)
                _uniformLocations.Clear();

            //Delete unmanaged resources
            GL.DeleteProgram(ProgramId);

            _disposedValue = true;
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Creates a shader program using the given vertex and fragment shader ID numbers.
        /// </summary>
        /// <param name="vertexShaderId">The ID of the vertex shader.</param>
        /// <param name="fragmentShaderId">The ID of the fragment shader.</param>
        /// <returns></returns>
        private int SetupShaderProgram(int vertexShaderId, int fragmentShaderId)
        {
            var programHandle = GL.CreateProgram();

            //Attach both shaders...
            GL.AttachShader(programHandle, vertexShaderId);

            GL.AttachShader(programHandle, fragmentShaderId);

            //Link them together
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
            GL.LinkProgram(shaderProgramId);

            // Check for linking errors
            GL.GetProgram(shaderProgramId, GetProgramParameterName.LinkStatus, out int statusCode);

            if (statusCode != (int)All.True)
            {
                // We can use `GL.GetProgramInfoLog(program)` to get information about the error.
                throw new Exception($"Error occurred while linking Program({shaderProgramId})");
            }
        }


        /// <summary>
        /// Creates a shader of the given <paramref name="shaderType"/> using the
        /// given <paramref name="shaderSrc"/> code.
        /// </summary>
        /// <param name="shaderType">The type of shader to create.</param>
        /// <param name="shaderSrc">The shader source code to use for the shader program.</param>
        /// <returns></returns>
        private int CreateShader(ShaderType shaderType, string shaderSrc)
        {
            var shaderId = GL.CreateShader(shaderType);

            GL.ShaderSource(shaderId, shaderSrc);

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
            GL.DetachShader(shaderProgramId, shaderId);
            GL.DeleteShader(shaderId);
        }


        /// <summary>
        /// Compiles the currently set shader source code on the GPU.
        /// </summary>
        /// <param name="shaderId">The shader ID.</param>
        private void CompileShader(int shaderId)
        {
            // Try to compile the shader
            GL.CompileShader(shaderId);

            // Check for compilation errors
            GL.GetShader(shaderId, ShaderParameter.CompileStatus, out int statusCode);

            if (statusCode != (int)All.True)
            {
                var errorInfo = GL.GetShaderInfoLog(shaderId);

                throw new Exception($"Error occurred while compiling Shader({shaderId})\n{errorInfo}");
            }
        }
        #endregion
    }
}

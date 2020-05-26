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
        private bool _disposedValue;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates a new instance of <see cref="ShaderProgram"/>.
        /// </summary>
        /// <param name="vertexShaderPath">The path to the vertex shader code.</param>
        /// <param name="fragmentShaderPath">The path to the fragment shader code.</param>
        /// <param name="file">Loads the shader source code from the given shader paths.</param>
        public ShaderProgram(string vertexShaderPath, string fragmentShaderPath, ITextFile file)
        {
            //Load the vertex and fragment shader source code
            var vertexShaderSrc = file.Load(vertexShaderPath);
            var fragmentShaderSrc = file.Load(fragmentShaderPath);

            SetupProgram(vertexShaderSrc, fragmentShaderSrc);
        }


        /// <summary>
        /// Creates a new instance of <see cref="ShaderProgram"/>.
        /// </summary>
        /// <param name="vertexShaderSrc">The vertex shader source code.</param>
        /// <param name="fragmentShaderSrc">The fragment shader source code.</param>
        public ShaderProgram(string vertexShaderSrc, string fragmentShaderSrc)
        {
            SetupProgram(vertexShaderSrc, fragmentShaderSrc);
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
        public int ID { get; private set; }
        #endregion


        #region Public Methods
        /// <summary>
        /// Sets the active shader program to use on the GPU.
        /// </summary>
        public void UseProgram() => GL.UseProgram(ID);


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
            GL.DeleteProgram(ID);

            _disposedValue = true;
        }
        #endregion


        #region Private Methods
        private void SetupProgram(string vertexShaderSrc, string fragmentShaderSrc)
        {
            VertexShaderId = CreateShader(ShaderType.VertexShader, vertexShaderSrc);
            FragmentShaderId = CreateShader(ShaderType.FragmentShader, fragmentShaderSrc);

            //Create a program ID
            ID = GL.CreateProgram();

            //Attach both shaders
            GL.AttachShader(ID, VertexShaderId);
            GL.AttachShader(ID, FragmentShaderId);

            //Link them together
            LinkProgram(ID);

            //When the shader program is linked, it no longer needs the individual shaders attacked to it.
            //The compiled code is copied into the shader program.
            //Detach and then delete them.
            DestroyShader(ID, VertexShaderId);
            DestroyShader(ID, FragmentShaderId);

            //This is for the purpose of caching the locations of the uniforms.
            //The reason is because GetUniformLocation() is a slow call.
            //Get the number of active uniforms in the shader.
            GL.GetProgram(ID, GetProgramParameterName.ActiveUniforms, out int totalActiveUniforms);

            //Loop over all the uniforms
            for (var i = 0; i < totalActiveUniforms; i++)
            {
                //get the name of this uniform,
                var key = GL.GetActiveUniform(ID, i, out _, out _);

                //Get the location of the uniform on the GPU
                var location = GL.GetUniformLocation(ID, key);

                if (location == -1)
                    throw new Exception($"The uniform with the name '{key}' does not exist.");

                //Cache it
                _uniformLocations.Add(key, location);
            }
        }


        /// <summary>
        /// Links the program using the given <paramref name="shaderProgramId"/>.
        /// </summary>
        /// <param name="shaderProgramId">The ID of the shader program.</param>
        private static void LinkProgram(int shaderProgramId)
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
        private static int CreateShader(ShaderType shaderType, string shaderSrc)
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
        private static void DestroyShader(int shaderProgramId, int shaderId)
        {
            GL.DetachShader(shaderProgramId, shaderId);
            GL.DeleteShader(shaderId);
        }


        /// <summary>
        /// Compiles the currently set shader source code on the GPU.
        /// </summary>
        /// <param name="shaderId">The shader ID.</param>
        private static void CompileShader(int shaderId)
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

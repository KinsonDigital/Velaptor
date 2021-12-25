// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL.Exceptions;
    using Velaptor.OpenGL.Services;

    // TODO: Setup debug groups for shader

    /// <inheritdoc/>
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    public abstract class ShaderProgram : IShaderProgram
    {
#pragma warning disable SA1401
        /// <summary>
        /// Invokes OpenGL functions.
        /// </summary>
        private protected readonly IGLInvoker GL;
#pragma warning restore SA1401
        private readonly IShaderLoaderService<uint> shaderLoaderService;
        private readonly IDisposable glObservableUnsubscriber;
        private bool isDisposed;
        private bool isInitialized;
        private uint batchSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="shaderLoaderService">Loads shader source code for compilation and linking.</param>
        /// <param name="glInitObservable">Initializes the shader once it receives a notification.</param>
        internal ShaderProgram(
            IGLInvoker gl,
            IShaderLoaderService<uint> shaderLoaderService,
            IObservable<bool> glInitObservable)
        {
            this.GL = gl;
            this.shaderLoaderService = shaderLoaderService;
            ProcessCustomAttributes();

            this.glObservableUnsubscriber = glInitObservable.Subscribe(new Observer<bool>(_ => Init()));
        }

        /// <inheritdoc/>
        public uint ShaderId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// <inheritdoc cref="IShaderProgram.Use"/>
        /// </summary>
        /// <exception cref="ShaderNotInitializedException">
        ///     Thrown when invoked without the shader being initialized.
        /// </exception>
        public virtual void Use()
        {
            if (this.isInitialized is false)
            {
                throw new ShaderNotInitializedException("The shader has not been initialized.");
            }

            this.GL.UseProgram(ShaderId);
        }

        /// <inheritdoc cref="IDisposable.Dispose"/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// <inheritdoc cref="IDisposable.Dispose"/>
        /// </summary>
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (this.isDisposed)
            {
                return;
            }

            if (disposing)
            {
                this.glObservableUnsubscriber.Dispose();
            }

            this.GL.DeleteProgram(ShaderId);

            this.isDisposed = true;
        }

        private void Init()
        {
            if (this.isInitialized)
            {
                return;
            }

            this.GL.BeginGroup($"Load {Name} Vertex Shader");

            var vertShaderSrc = this.shaderLoaderService.LoadVertSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var vertShaderId = this.GL.CreateShader(GLShaderType.VertexShader);

            this.GL.LabelShader(vertShaderId, $"{Name} Vertex Shader");

            this.GL.ShaderSource(vertShaderId, vertShaderSrc);
            this.GL.CompileShader(vertShaderId);

            // Checking the shader for compilation errors.
            var infoLog = this.GL.GetShaderInfoLog(vertShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                // TODO: Create custom compile shader exception
                throw new Exception($"Error compiling vertex shader '{Name}' with shader ID '{vertShaderId}'.\n{infoLog}");
            }

            this.GL.EndGroup();

            this.GL.BeginGroup($"Load {Name} Fragment Shader");

            var fragShaderSrc = this.shaderLoaderService.LoadFragSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var fragShaderId = this.GL.CreateShader(GLShaderType.FragmentShader);

            this.GL.LabelShader(fragShaderId, $"{Name} Fragment Shader");

            this.GL.ShaderSource(fragShaderId, fragShaderSrc);
            this.GL.CompileShader(fragShaderId);

            // Checking the shader for compilation errors.
            infoLog = this.GL.GetShaderInfoLog(fragShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                // TODO: Create custom compile shader exception
                throw new Exception($"Error compiling fragment shader '{Name}' with shader ID '{fragShaderId}'.\n{infoLog}");
            }

            this.GL.EndGroup();

            CreateProgram(Name, vertShaderId, fragShaderId);
            CleanShadersIfReady(Name, vertShaderId, fragShaderId);

            this.isInitialized = true;
        }

        private void CreateProgram(string shaderName, uint vertShaderId, uint fragShaderId)
        {
            this.GL.BeginGroup($"Create {shaderName} Shader Program");

            // Combining the shaders under one shader program.
            ShaderId = this.GL.CreateProgram();

            this.GL.LabelShaderProgram(ShaderId, $"{shaderName} Shader Program");

            this.GL.AttachShader(ShaderId, vertShaderId);
            this.GL.AttachShader(ShaderId, fragShaderId);

            // Link and check for for errors.
            this.GL.LinkProgram(ShaderId);
            this.GL.GetProgram(ShaderId, GLProgramParameterName.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Error linking shader with ID '{ShaderId}'\n{this.GL.GetProgramInfoLog(ShaderId)}");
            }

            this.GL.EndGroup();
        }

        private void CleanShadersIfReady(string name, uint vertShaderId, uint fragShaderId)
        {
            this.GL.BeginGroup($"Clean Up {name} Vertex Shader");

            this.GL.DetachShader(ShaderId, vertShaderId);
            this.GL.DeleteShader(vertShaderId);

            this.GL.EndGroup();

            this.GL.BeginGroup($"Clean Up {name} Fragment Shader");

            // Delete the no longer useful individual shaders
            this.GL.DetachShader(ShaderId, fragShaderId);
            this.GL.DeleteShader(fragShaderId);

            this.GL.EndGroup();
        }

        private void ProcessCustomAttributes()
        {
            Attribute[]? attributes = null;
            var currentType = GetType();

            if (currentType == typeof(TextureShader))
            {
                attributes = Attribute.GetCustomAttributes(typeof(TextureShader));
            }
            else if (currentType == typeof(FontShader))
            {
                attributes = Attribute.GetCustomAttributes(typeof(FontShader));
            }
            else
            {
                Name = "UNKNOWN";
            }

            if (attributes is null || attributes.Length <= 0)
            {
                return;
            }

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case ShaderNameAttribute nameAttribute:
                        Name = nameAttribute.Name;
                        break;
                    case SpriteBatchSizeAttribute sizeAttribute:
                        this.batchSize = sizeAttribute.BatchSize;
                        break;
                }
            }
        }
    }
}

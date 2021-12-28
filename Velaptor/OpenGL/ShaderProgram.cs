// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using Velaptor.OpenGL.Exceptions;
    using Velaptor.OpenGL.Services;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    public abstract class ShaderProgram : IShaderProgram
    {
        private readonly IShaderLoaderService<uint> shaderLoaderService;
        private readonly IDisposable glObservableUnsubscriber;
        private bool isDisposed;
        private bool isInitialized;
        private uint batchSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="shaderLoaderService">Loads shader source code for compilation and linking.</param>
        /// <param name="glInitObservable">Initializes the shader once it receives a notification.</param>
        internal ShaderProgram(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            IObservable<bool> glInitObservable)
        {
            GL = gl;
            GLExtensions = glExtensions;

            this.shaderLoaderService = shaderLoaderService;
            ProcessCustomAttributes();

            this.glObservableUnsubscriber = glInitObservable.Subscribe(new Observer<bool>(_ => Init()));
        }

        /// <inheritdoc/>
        public uint ShaderId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets invokes OpenGL functions.
        /// </summary>
        private protected IGLInvoker GL { get; }

        /// <summary>
        /// Gets the invoker that contains helper methods for simplified OpenGL function calls.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Intended to be available in classes inheriting this class.")]
        private protected IGLInvokerExtensions GLExtensions { get; }

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

            GL.UseProgram(ShaderId);
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

            GL.DeleteProgram(ShaderId);

            this.isDisposed = true;
        }

        private void Init()
        {
            if (this.isInitialized)
            {
                return;
            }

            GLExtensions.BeginGroup($"Load {Name} Vertex Shader");

            var vertShaderSrc = this.shaderLoaderService.LoadVertSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var vertShaderId = GL.CreateShader(GLShaderType.VertexShader);

            GLExtensions.LabelShader(vertShaderId, $"{Name} Vertex Shader");

            GL.ShaderSource(vertShaderId, vertShaderSrc);
            GL.CompileShader(vertShaderId);

            // Checking the shader for compilation errors.
            var infoLog = GL.GetShaderInfoLog(vertShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                // TODO: Create custom compile shader exception
                throw new Exception($"Error compiling vertex shader '{Name}' with shader ID '{vertShaderId}'.\n{infoLog}");
            }

            GLExtensions.EndGroup();

            GLExtensions.BeginGroup($"Load {Name} Fragment Shader");

            var fragShaderSrc = this.shaderLoaderService.LoadFragSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var fragShaderId = GL.CreateShader(GLShaderType.FragmentShader);

            GLExtensions.LabelShader(fragShaderId, $"{Name} Fragment Shader");

            GL.ShaderSource(fragShaderId, fragShaderSrc);
            GL.CompileShader(fragShaderId);

            // Checking the shader for compilation errors.
            infoLog = GL.GetShaderInfoLog(fragShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                // TODO: Create custom compile shader exception
                throw new Exception($"Error compiling fragment shader '{Name}' with shader ID '{fragShaderId}'.\n{infoLog}");
            }

            GLExtensions.EndGroup();

            CreateProgram(Name, vertShaderId, fragShaderId);
            CleanShadersIfReady(Name, vertShaderId, fragShaderId);

            this.isInitialized = true;
        }

        private void CreateProgram(string shaderName, uint vertShaderId, uint fragShaderId)
        {
            GLExtensions.BeginGroup($"Create {shaderName} Shader Program");

            // Combining the shaders under one shader program.
            ShaderId = GL.CreateProgram();

            GLExtensions.LabelShaderProgram(ShaderId, $"{shaderName} Shader Program");

            GL.AttachShader(ShaderId, vertShaderId);
            GL.AttachShader(ShaderId, fragShaderId);

            // Link and check for for errors.
            GL.LinkProgram(ShaderId);
            GL.GetProgram(ShaderId, GLProgramParameterName.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Error linking shader with ID '{ShaderId}'\n{GL.GetProgramInfoLog(ShaderId)}");
            }

            GLExtensions.EndGroup();
        }

        private void CleanShadersIfReady(string name, uint vertShaderId, uint fragShaderId)
        {
            GLExtensions.BeginGroup($"Clean Up {name} Vertex Shader");

            GL.DetachShader(ShaderId, vertShaderId);
            GL.DeleteShader(vertShaderId);

            GLExtensions.EndGroup();

            GLExtensions.BeginGroup($"Clean Up {name} Fragment Shader");

            // Delete the no longer useful individual shaders
            GL.DetachShader(ShaderId, fragShaderId);
            GL.DeleteShader(fragShaderId);

            GLExtensions.EndGroup();
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

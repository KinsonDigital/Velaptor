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
    using Velaptor.OpenGL.Exceptions;
    using Velaptor.OpenGL.Services;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;

    // ReSharper restore RedundantNameQualifier

    /// <inheritdoc/>
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    internal abstract class ShaderProgram : IShaderProgram
    {
        private readonly IShaderLoaderService<uint> shaderLoaderService;
        private readonly IDisposable glInitReactorUnsubscriber;
        private readonly IDisposable shutDownReactorUnsubscriber;
        private bool isInitialized;
        private uint batchSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="shaderLoaderService">Loads shader source code for compilation and linking.</param>
        /// <param name="glInitReactable">Initializes the shader once it receives a notification.</param>
        /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        internal ShaderProgram(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            IShaderLoaderService<uint> shaderLoaderService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
        {
            GL = gl ?? throw new ArgumentNullException(nameof(gl), "The parameter must not be null.");
            GLExtensions = glExtensions ?? throw new ArgumentNullException(nameof(glExtensions), "The parameter must not be null.");
            this.shaderLoaderService = shaderLoaderService ?? throw new ArgumentNullException(nameof(shaderLoaderService), "The parameter must not be null.");

            if (glInitReactable is null)
            {
                throw new ArgumentNullException(nameof(glInitReactable), "The parameter must not be null.");
            }

            this.glInitReactorUnsubscriber = glInitReactable.Subscribe(new Reactor<GLInitData>(_ => Init()));

            if (shutDownReactable is null)
            {
                throw new ArgumentNullException(nameof(shutDownReactable), "The parameter must not be null.");
            }

            this.shutDownReactorUnsubscriber =
                shutDownReactable.Subscribe(new Reactor<ShutDownData>(_ => Dispose()));

            ProcessCustomAttributes();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ShaderProgram"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
        ~ShaderProgram()
        {
            if (UnitTestDetector.IsRunningFromUnitTest)
            {
                return;
            }

            Dispose();
        }

        /// <inheritdoc/>
        public uint ShaderId { get; private set; }

        /// <inheritdoc/>
        public string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether or not a value if the <see cref="ShaderProgram"/> is disposed.
        /// </summary>
        [SuppressMessage(
            "ReSharper",
            "MemberCanBePrivate.Global",
            Justification = "Left of inheriting members to use.")]
        protected bool IsDisposed { get; set; }

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

        /// <summary>
        /// Shuts down the application by disposing of any resources.
        /// </summary>
        [SuppressMessage(
            "ReSharper",
            "VirtualMemberNeverOverridden.Global",
            Justification = "Will be used in the future.")]
        protected virtual void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            GL.DeleteProgram(ShaderId);

            this.glInitReactorUnsubscriber.Dispose();
            this.shutDownReactorUnsubscriber.Dispose();

            IsDisposed = true;
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

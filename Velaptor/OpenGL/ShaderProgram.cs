// <copyright file="ShaderProgram.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using Velaptor.Graphics;
using Velaptor.Observables;
using Velaptor.Observables.Core;
using Velaptor.OpenGL.Services;

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using Velaptor.NativeInterop.OpenGL;

    // TODO: Setup debug groups for shader

    /// <inheritdoc/>
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    public abstract class ShaderProgram : IShaderProgram
    {
        private static readonly Dictionary<uint, bool> _shadersInUse = new();
        private protected readonly IGLInvoker gl;
        private readonly IShaderLoaderService<uint> _shaderLoaderService;
        private readonly IDisposable glObservableUnsubscriber;
        private bool isDisposed;
        private bool isInitialized;
        private uint batchSize;

        internal ShaderProgram(
            IGLInvoker gl,
            IShaderLoaderService<uint> shaderLoaderService,
            OpenGLInitObservable glObservable)
        {
            this.gl = gl;
            this._shaderLoaderService = shaderLoaderService;
            ProcessCustomAttributes();

            this.glObservableUnsubscriber = glObservable.Subscribe(new Observer<bool>(_ => Init()));
        }

        public uint ShaderId { get; private set; }

        public string Name { get; private set; } = "UNKNOWN SHADER";

        public virtual void Use()
        {
            // TODO: Add a check here to see if the shader has been loaded and throw exception
            // if it has not been

            if (_shadersInUse[ShaderId])
            {
                return;
            }

            this.gl.UseProgram(ShaderId);

            var progIds = _shadersInUse.Keys;
            foreach (var id in progIds)
            {
                _shadersInUse[id] = false;
            }

            _shadersInUse[ShaderId] = true;
        }

        public void Dispose()
        {
            this.glObservableUnsubscriber.Dispose();
            this.gl.DeleteProgram(ShaderId);
            _shadersInUse.Remove(ShaderId);
        }

        private void Init()
        {
            this.gl.BeginGroup($"Load {Name} Vertex Shader");

            var vertShaderSrc = this._shaderLoaderService.LoadVertSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var vertShaderId = this.gl.CreateShader(GLShaderType.VertexShader);

            this.gl.LabelShader(vertShaderId, $"{Name} Vertex Shader");

            this.gl.ShaderSource(vertShaderId, vertShaderSrc);
            this.gl.CompileShader(vertShaderId);

            // Checking the shader for compilation errors.
            var infoLog = this.gl.GetShaderInfoLog(vertShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling vertex shader '{Name}'\n{infoLog}");
            }

            this.gl.EndGroup();

            this.gl.BeginGroup($"Load {Name} Fragment Shader");

            var fragShaderSrc = this._shaderLoaderService.LoadFragSource(Name, new (string name, uint value)[] { ("BATCH_SIZE", this.batchSize) });
            var fragShaderId = this.gl.CreateShader(GLShaderType.FragmentShader);

            this.gl.LabelShader(fragShaderId, $"{Name} Fragment Shader");

            this.gl.ShaderSource(fragShaderId, fragShaderSrc);
            this.gl.CompileShader(fragShaderId);

            // Checking the shader for compilation errors.
            infoLog = this.gl.GetShaderInfoLog(fragShaderId);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                throw new Exception($"Error compiling fragment shader '{Name}'\n{infoLog}");
            }

            this.gl.EndGroup();

            CreateProgram(Name, vertShaderId, fragShaderId);
            CleanShadersIfReady(Name, vertShaderId, fragShaderId);
        }

        private void CreateProgram(string shaderName, uint vertShaderId, uint fragShaderId)
        {
            this.gl.BeginGroup($"Create {shaderName} Shader Program");

            // Combining the shaders under one shader program.
            ShaderId = this.gl.CreateProgram();

            _shadersInUse.Add(ShaderId, false);

            this.gl.LabelShaderProgram(ShaderId, $"{shaderName} Shader Program");

            this.gl.AttachShader(ShaderId, vertShaderId);
            this.gl.AttachShader(ShaderId, fragShaderId);

            // Link and check for for errors.
            this.gl.LinkProgram(ShaderId);
            this.gl.GetProgram(ShaderId, GLProgramParameterName.LinkStatus, out var status);
            if (status == 0)
            {
                throw new Exception($"Error linking shader {this.gl.GetProgramInfoLog(ShaderId)}");
            }

            this.gl.EndGroup();
        }

        private void CleanShadersIfReady(string name, uint vertShaderId, uint fragShaderId)
        {
            this.gl.BeginGroup($"Clean Up {name} Vertex Shader");

            this.gl.DetachShader(ShaderId, vertShaderId);
            this.gl.DeleteShader(vertShaderId);

            this.gl.EndGroup();

            this.gl.BeginGroup($"Clean Up {name} Fragment Shader");

            // Delete the no longer useful individual shaders
            this.gl.DetachShader(ShaderId, fragShaderId);
            this.gl.DeleteShader(fragShaderId);

            this.gl.EndGroup();
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

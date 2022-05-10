// <copyright file="OpenGLService.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.NativeInterop.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Drawing;
    using System.Numerics;
    using Velaptor.Guards;
    using Velaptor.OpenGL;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Provides OpenGL helper methods to improve OpenGL related operations.
    /// </summary>
    internal class OpenGLService : IOpenGLService
    {
        private readonly IGLInvoker glInvoker;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLService"/> class.
        /// </summary>
        /// <param name="glInvoker">Invokes OpenGL functions.</param>
        public OpenGLService(IGLInvoker glInvoker)
        {
            EnsureThat.ParamIsNotNull(glInvoker);
            this.glInvoker = glInvoker;
        }

        /// <inheritdoc/>
        public bool IsVBOBound { get; private set; }

        /// <inheritdoc/>
        public bool IsEBOBound { get; private set; }

        /// <inheritdoc/>
        public bool IsVAOBound { get; private set; }

        /// <inheritdoc/>
        public Size GetViewPortSize()
        {
            /*
             * [0] = X
             * [1] = Y
             * [3] = Width
             * [4] = Height
             */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            return new Size(data[2], data[3]);
        }

        /// <inheritdoc/>
        public void SetViewPortSize(Size size)
        {
            /*
             * [0] = X
             * [1] = Y
             * [3] = Width
             * [4] = Height
             */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            this.glInvoker.Viewport(data[0], data[1], (uint)size.Width, (uint)size.Height);
        }

        /// <inheritdoc/>
        public Vector2 GetViewPortPosition()
        {
            /*
           * [0] = X
           * [1] = Y
           * [3] = Width
           * [4] = Height
           */
            var data = new int[4];

            this.glInvoker.GetInteger(GLGetPName.Viewport, data);

            return new Vector2(data[0], data[1]);
        }

        /// <inheritdoc/>
        public void BindVBO(uint vbo)
        {
            this.glInvoker.BindBuffer(GLBufferTarget.ArrayBuffer, vbo);
            IsVBOBound = true;
        }

        /// <inheritdoc/>
        public void UnbindVBO()
        {
            this.glInvoker.BindBuffer(GLBufferTarget.ArrayBuffer, 0u);
            IsVBOBound = false;
        }

        /// <inheritdoc/>
        public void BindEBO(uint ebo)
        {
            this.glInvoker.BindBuffer(GLBufferTarget.ElementArrayBuffer, ebo);
            IsEBOBound = true;
        }

        /// <inheritdoc/>
        public void UnbindEBO()
        {
            if (IsVAOBound)
            {
                throw new InvalidOperationException("The VAO object must be unbound before unbinding an EBO object.");
            }

            this.glInvoker.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);
            IsEBOBound = false;
        }

        /// <inheritdoc/>
        public void BindVAO(uint vao)
        {
            this.glInvoker.BindVertexArray(vao);
            IsVAOBound = true;
        }

        /// <inheritdoc/>
        public void UnbindVAO()
        {
            this.glInvoker.BindVertexArray(0);
            IsVAOBound = false;
        }

        /// <inheritdoc/>
        public void BindTexture2D(uint textureId) => this.glInvoker.BindTexture(GLTextureTarget.Texture2D, textureId);

        /// <inheritdoc/>
        public void UnbindTexture2D() => this.glInvoker.BindTexture(GLTextureTarget.Texture2D, 0u);

        /// <inheritdoc/>
        public bool ProgramLinkedSuccessfully(uint programId)
        {
            var programParams = this.glInvoker.GetProgram(programId, GLProgramParameterName.LinkStatus);

            return programParams >= 1;
        }

        /// <inheritdoc/>
        public bool ShaderCompiledSuccessfully(uint shaderId)
        {
            var shaderParams = this.glInvoker.GetShader(shaderId, GLShaderParameter.CompileStatus);

            return shaderParams >= 1;
        }

        /// <inheritdoc/>
        public void BeginGroup(string label)
                    =>
                        this.glInvoker.PushDebugGroup(
                            GLDebugSource.DebugSourceApplication,
                            100,
                            (uint)label.Length,
                            label);

        /// <inheritdoc/>
        public void EndGroup() => this.glInvoker.PopDebugGroup();

        /// <inheritdoc/>
        public void LabelShader(uint shaderId, string label)
            => this.glInvoker.ObjectLabel(GLObjectIdentifier.Shader, shaderId, (uint)label.Length, label);

        /// <inheritdoc/>
        public void LabelShaderProgram(uint shaderId, string label)
            => this.glInvoker.ObjectLabel(GLObjectIdentifier.Program, shaderId, (uint)label.Length, label);

        /// <inheritdoc/>
        public void LabelVertexArray(uint vertexArrayId, string label)
        {
            label = string.IsNullOrEmpty(label)
                ? "NOT SET"
                : label;

            var newLabel = $"{label} VAO";

            this.glInvoker.ObjectLabel(GLObjectIdentifier.VertexArray, vertexArrayId, (uint)newLabel.Length, newLabel);
        }

        /// <inheritdoc/>
        public void LabelBuffer(uint bufferId, string label, OpenGLBufferType bufferType)
        {
            label = string.IsNullOrEmpty(label)
                ? "NOT SET"
                : label;

            var bufferTypeAcronym = bufferType switch
            {
                OpenGLBufferType.VertexBufferObject => "VBO",
                OpenGLBufferType.IndexArrayObject => "EBO",
                _ => throw new ArgumentOutOfRangeException(nameof(bufferType), bufferType, null)
            };

            var newLabel = $"{label} {bufferTypeAcronym}";

            this.glInvoker.ObjectLabel(GLObjectIdentifier.Buffer, bufferId, (uint)newLabel.Length, newLabel);
        }

        /// <inheritdoc/>
        public void LabelTexture(uint textureId, string label)
        {
            label = string.IsNullOrEmpty(label)
                ? "NOT SET"
                : label;

            this.glInvoker.ObjectLabel(GLObjectIdentifier.Texture, textureId, (uint)label.Length, label);
        }
    }
}

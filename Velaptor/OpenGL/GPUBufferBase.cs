// <copyright file="GPUBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using NETSizeF = System.Drawing.SizeF;

    /// <summary>
    /// Base class for a GPU buffer.
    /// </summary>
    /// <typeparam name="TData">The type of data in the GPU buffer.</typeparam>
    internal abstract class GPUBufferBase<TData> : IGPUBuffer<TData, NETSizeF>
        where TData : struct
    {
        private static readonly Dictionary<uint, bool> BoundVAOList = new ();
        private static readonly Dictionary<uint, bool> BoundVBOList = new ();
        private static readonly Dictionary<uint, bool> BoundEBOList = new ();
        private string bufferName = string.Empty;
        private uint vao; // Vertex Array Object
        private uint vbo; // Vertex Buffer Object
        private uint ebo; // Element Buffer Object
        private uint[] indices = Array.Empty<uint>();
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferBase{TData}"/> class.
        /// </summary>
        /// <param name="gl">The OpenGL function invoker.</param>
        public GPUBufferBase(IGLInvoker gl) => GL = gl;

        /// <summary>
        /// Finalizes an instance of the <see cref="GPUBufferBase{TData}"/> class.
        /// </summary>
        ~GPUBufferBase() => Dispose();

        /// <summary>
        /// Gets the size of the viewport.
        /// </summary>
        protected NETSizeF ViewPortSize { get; private set; }

        /// <summary>
        /// Gets the size of the sprite batch.
        /// </summary>
        protected uint BatchSize { get; private set; } = 100;

        /// <summary>
        /// Gets the invoker used to invoke OpenGL functions.
        /// </summary>
        protected IGLInvoker GL { get; }

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the buffer has been initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public void Init()
        {
            // TODO: Setup an OpenGLInitObservable in the ctor in the GPUBufferBase class
            ProcessCustomAttributes();

            // Generate the VAO and VBO with only 1 object each
            this.vao = GL.GenVertexArray();
            BoundVAOList.Add(this.vao, false);
            GL.LabelVertexArray(this.vao, this.bufferName, BindVAO);

            this.vbo = GL.GenBuffer();
            BoundVBOList.Add(this.vbo, false);
            GL.LabelBuffer(this.vbo, this.bufferName, BufferType.VertexBufferObject, BindVBO);

            this.ebo = GL.GenBuffer();
            BoundEBOList.Add(this.ebo, false);
            GL.LabelBuffer(this.ebo, this.bufferName, BufferType.IndexArrayObject, BindEBO);

            GL.BeginGroup($"Setup {this.bufferName} Data");
            GL.BeginGroup($"Upload {this.bufferName} Vertex Data");

            IsInitialized = true;

            var vertBufferData = GenerateData();
            var vertData = new ReadOnlySpan<float>(vertBufferData);

            GL.BufferData(GLBufferTarget.ArrayBuffer, vertBufferData, GLBufferUsageHint.DynamicDraw);

            GL.EndGroup();

            GL.BeginGroup($"Upload {this.bufferName} Indices Data");

            this.indices = GenerateIndices();

            // Configure the Vertex Attribute so that OpenGL knows how to read the VBO
            GL.BufferData(GLBufferTarget.ElementArrayBuffer, this.indices, GLBufferUsageHint.StaticDraw);
            GL.EndGroup();

            SetupVAO();

            UnbindVBO();
            UnbindVAO();
            UnbindEBO();
            GL.EndGroup();
        }

        public void UpdateData(TData data, uint batchIndex)
        {
            PrepareForUse();
            UpdateVertexData(data, batchIndex);
        }

        public void SetState(NETSizeF viewPortSize) => ViewPortSize = viewPortSize;

        public NETSizeF GetState() => ViewPortSize;

        protected abstract void UpdateVertexData(TData data, uint batchIndex);

        protected abstract void PrepareForUse();

        protected abstract float[] GenerateData();

        protected abstract void SetupVAO();

        protected abstract uint[] GenerateIndices();

        protected void BindVBO()
        {
            if (BoundVBOList[this.vbo])
            {
                return;
            }

            GL.BindBuffer(GLBufferTarget.ArrayBuffer, this.vbo);
            BoundVBOList[this.vbo] = true;
        }

        protected void UnbindVBO()
        {
            GL.BindBuffer(GLBufferTarget.ArrayBuffer, 0); // Unbind the VBO
            BoundVBOList[this.vbo] = false;
        }

        protected void BindEBO()
        {
            if (BoundEBOList[this.ebo])
            {
                return;
            }

            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, this.ebo);
            BoundEBOList[this.ebo] = true;
        }

        /// <summary>
        /// NOTE: Make sure to unbind AFTER you unbind the VAO.  This is because the EBO is stored
        /// inside of the VAO.  Unbinding the EBO before unbinding, (or without unbinding the VAO),
        /// you are telling OpenGL that you don't want your VAO to use the EBO.
        /// </summary>
        protected void UnbindEBO()
        {
            if (BoundVAOList[this.vao])
            {
                throw new Exception("Cannot unbind the EBO before unbinding the VAO.");
            }

            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);
            BoundEBOList[this.ebo] = false;
        }

        protected void BindVAO()
        {
            if (BoundVAOList[this.vao])
            {
                return;
            }

            GL.BindVertexArray(this.vao);
            BoundVAOList[this.vao] = true;
        }

        protected void UnbindVAO()
        {
            GL.BindVertexArray(0); // Unbind the VAO
            BoundVAOList[this.vao] = false;
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            GL.DeleteVertexArray(this.vao);
            BoundVAOList.Remove(this.vao);

            GL.DeleteBuffer(this.vbo);
            BoundVBOList.Remove(this.vbo);

            GL.DeleteBuffer(this.ebo);
            BoundEBOList.Remove(this.ebo);

            this.isDisposed = true;

            GC.SuppressFinalize(this);
        }

        private void ProcessCustomAttributes()
        {
            Attribute[] attributes;
            var currentType = GetType();

            if (currentType == typeof(TextureGPUBuffer))
            {
                attributes = Attribute.GetCustomAttributes(typeof(TextureGPUBuffer));
            }
            else
            {
                throw new Exception("The GPU Buffer is unrecognized");
            }

            if (attributes.Length <= 0)
            {
                this.bufferName = "UNKNOWN BUFFER";
                return;
            }

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case SpriteBatchSizeAttribute sizeAttribute:
                        BatchSize = sizeAttribute.BatchSize;
                        break;
                    case GPUBufferNameAttribute nameAttribute:
                        this.bufferName = nameAttribute.Name;
                        break;
                }
            }
        }
    }
}

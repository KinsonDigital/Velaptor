// <copyright file="GPUBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using Velaptor.NativeInterop.OpenGL;
    using Observables;
    using Observables.Core;
    using NETSizeF = System.Drawing.SizeF;

    /// <summary>
    /// Base functionality for managing buffer data in the GPU.
    /// </summary>
    /// <typeparam name="TData">The type of data in the GPU buffer.</typeparam>
    public abstract class GPUBufferBase<TData> : IGPUBuffer<TData>
        where TData : struct
    {
        private readonly IDisposable glObservableUnsubscriber;
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
        internal GPUBufferBase(IGLInvoker gl, OpenGLInitObservable glObservable)
        {
            GL = gl;
            ProcessCustomAttributes();

            this.glObservableUnsubscriber = glObservable.Subscribe(new Observer<bool>(_ => Init()));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GPUBufferBase{TData}"/> class.
        /// </summary>
        ~GPUBufferBase() => Dispose();

        /// <summary>
        /// Gets the size of the sprite batch.
        /// </summary>
        protected uint BatchSize { get; private set; } = 100;

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the buffer has been initialized.
        /// </summary>
        protected bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the invoker that makes OpenGL calls.
        /// </summary>
        private protected IGLInvoker GL { get; }

        /// <inheritdoc/>
        public void Init()
        {
            // Generate the VAO and VBO with only 1 object each
            this.vao = GL.GenVertexArray();
            GL.LabelVertexArray(this.vao, this.bufferName, BindVAO);

            this.vbo = GL.GenBuffer();
            GL.LabelBuffer(this.vbo, this.bufferName, BufferType.VertexBufferObject, BindVBO);

            this.ebo = GL.GenBuffer();
            GL.LabelBuffer(this.ebo, this.bufferName, BufferType.IndexArrayObject, BindEBO);

            GL.BeginGroup($"Setup {this.bufferName} Data");
            GL.BeginGroup($"Upload {this.bufferName} Vertex Data");

            IsInitialized = true;

            var vertBufferData = GenerateData();

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

        /// <summary>
        /// Updates GPU buffer with the given <paramref name="data"/> at the given <paramref name="batchIndex"/>.
        /// </summary>
        /// <param name="data">The data to send to the GPU.</param>
        /// <param name="batchIndex">The index of the batch of data to update.</param>
        public void UpdateData(TData data, uint batchIndex)
        {
            PrepareForUse();
            UpdateVertexData(data, batchIndex);
        }

        protected abstract void UpdateVertexData(TData data, uint batchIndex);

        protected abstract void PrepareForUse();

        protected abstract float[] GenerateData();

        protected abstract void SetupVAO();

        protected abstract uint[] GenerateIndices();

        protected void BindVBO()
        {
            GL.BindBuffer(GLBufferTarget.ArrayBuffer, this.vbo);
        }

        protected void UnbindVBO()
        {
            GL.BindBuffer(GLBufferTarget.ArrayBuffer, 0); // Unbind the VBO
        }

        protected void BindEBO()
        {
            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, this.ebo);
        }

        /// <summary>
        /// NOTE: Make sure to unbind AFTER you unbind the VAO.  This is because the EBO is stored
        /// inside of the VAO.  Unbinding the EBO before unbinding, (or without unbinding the VAO),
        /// you are telling OpenGL that you don't want your VAO to use the EBO.
        /// </summary>
        protected void UnbindEBO()
        {
            // After implementing cached ID states in GLInvoker, set this back up to get it working again
            // if (BoundVAOList[this.vao])
            // {
            //     throw new Exception("Cannot unbind the EBO before unbinding the VAO.");
            // }

            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);
        }

        protected void BindVAO()
        {
            GL.BindVertexArray(this.vao);
        }

        protected void UnbindVAO()
        {
            GL.BindVertexArray(0); // Unbind the VAO
        }

        public void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.glObservableUnsubscriber.Dispose();

            GL.DeleteVertexArray(this.vao);
            GL.DeleteBuffer(this.vbo);
            GL.DeleteBuffer(this.ebo);

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
            else if (currentType == typeof(FontGPUBuffer))
            {
                attributes = Attribute.GetCustomAttributes(typeof(FontGPUBuffer));
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

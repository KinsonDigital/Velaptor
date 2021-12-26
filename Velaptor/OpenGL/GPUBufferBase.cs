// <copyright file="GPUBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables.Core;
    using NETSizeF = System.Drawing.SizeF;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Base functionality for managing buffer data in the GPU.
    /// </summary>
    /// <typeparam name="TData">The type of data in the GPU buffer.</typeparam>
    public abstract class GPUBufferBase<TData> : IGPUBuffer<TData>
        where TData : struct
    {
        private readonly IDisposable glObservableUnsubscriber;
        private uint vao; // Vertex Array Object
        private uint vbo; // Vertex Buffer Object
        private uint ebo; // Element Buffer Object
        private uint[] indices = Array.Empty<uint>();
        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBufferBase{TData}"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="glInitObservable">Receives a notification when OpenGL has been initialized.</param>
        internal GPUBufferBase(IGLInvoker gl, IGLInvokerExtensions glExtensions, IObservable<bool> glInitObservable)
        {
            GL = gl;
            GLExtensions = glExtensions;
            ProcessCustomAttributes();

            this.glObservableUnsubscriber = glInitObservable.Subscribe(new Observer<bool>(_ => Init()));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="GPUBufferBase{TData}"/> class.
        /// </summary>
        ~GPUBufferBase() => Dispose();

        /// <summary>
        /// Gets the size of the sprite batch.
        /// </summary>
        protected internal uint BatchSize { get; private set; } = 100;

        /// <summary>
        /// Gets a value indicating whether gets a value indicating if the buffer has been initialized.
        /// </summary>
        protected internal bool IsInitialized { get; private set; }

        /// <summary>
        /// Gets the name of the buffer.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
        protected internal string Name { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the invoker that makes OpenGL calls.
        /// </summary>
        private protected IGLInvoker GL { get; }

        /// <summary>
        /// Gets the invoker that contains helper methods for simplified OpenGL function calls.
        /// </summary>
        private protected IGLInvokerExtensions GLExtensions { get; }

        /// <summary>
        /// Updates GPU buffer with the given <paramref name="data"/> at the given <paramref name="batchIndex"/>.
        /// </summary>
        /// <param name="data">The data to send to the GPU.</param>
        /// <param name="batchIndex">The index of the batch of data to update.</param>
        public void UploadData(TData data, uint batchIndex)
        {
            PrepareForUpload();
            UploadVertexData(data, batchIndex);
        }

        /// <inheritdoc/>
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

        // TODO: Change the generic TData data param to an IN parameter for perf boost
        /* Make sure that the TData coming in though is readonly or perf will be worse.
         This means you might have to constrain TData to an in generic type to enforce this as well
            https://www.youtube.com/watch?v=VCGXubxKL9I
         */

        /// <summary>
        /// Updates the vertex data in the GPU.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        /// <param name="batchIndex">The index location of the data in a batch of data to upload.</param>
        protected internal abstract void UploadVertexData(TData data, uint batchIndex);

        /// <summary>
        /// Prepares anything that might need to be done before sending data to the GPU.
        /// </summary>
        /// <summary>
        ///     This could be binding buffers, setting state, etc.
        /// </summary>
        protected internal abstract void PrepareForUpload();

        /// <summary>
        /// Generates the data to be sent to the GPU.
        /// </summary>
        /// <returns>The data to be sent to the GPU.</returns>
        protected internal abstract float[] GenerateData();

        /// <summary>
        /// Sets up the vertex array object to describe to the GPU how
        /// the vertex array data is laid out and how it should be used.
        /// </summary>
        protected internal abstract void SetupVAO();

        /// <summary>
        /// Generates the data for the indices that are used in the vertex array object.
        /// </summary>
        /// <returns>The indices data.</returns>
        protected internal abstract uint[] GenerateIndices();

        /// <summary>
        /// Binds a vertex buffer object for updating buffer data in OpenGL.
        /// </summary>
        protected void BindVBO() => GL.BindBuffer(GLBufferTarget.ArrayBuffer, this.vbo);

        /// <summary>
        /// Unbinds the current vertex buffer object if one is currently bound.
        /// </summary>
        protected void UnbindVBO() => GL.BindBuffer(GLBufferTarget.ArrayBuffer, 0);

        /// <summary>
        /// Binds an element buffer object for updating element data in OpenGL.
        /// </summary>
        /// <summary>
        ///     This is also called an IBO (Index Buffer Object).
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
        protected void BindEBO() => GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, this.ebo);

        /// <summary>
        /// Unbinds the element array buffer object if one is currently bound.
        /// </summary>
        /// <remarks>
        /// NOTE: Make sure to unbind AFTER you unbind the VAO.  This is because the EBO is stored
        /// inside of the VAO.  Unbinding the EBO before unbinding, (or without unbinding the VAO),
        /// you are telling OpenGL that you don't want your VAO to use the EBO.
        /// </remarks>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
        protected void UnbindEBO() =>
            // After implementing cached ID states in GLInvoker, set this back up to get it working again
            // This might need to be setup in GLInvoker
            // if (BoundVAOList[this.vao])
            // {
            //     throw new Exception("Cannot unbind the EBO before unbinding the VAO.");
            // }
            GL.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);

        /// <summary>
        /// Binds the element array buffer object for updating vertex buffer data in OpenGL.
        /// </summary>
        protected void BindVAO() => GL.BindVertexArray(this.vao);

        /// <summary>
        /// Unbinds the current element array buffer that is currently bound if one is currently bound.
        /// </summary>
        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Left here for future development.")]
        protected void UnbindVAO() => GL.BindVertexArray(0); // Unbind the VAO

        /// <summary>
        /// Initializes the GPU buffer.
        /// </summary>
        private void Init()
        {
            // Generate the VAO and VBO with only 1 object each
            this.vao = GL.GenVertexArray();
            BindVAO();
            GLExtensions.LabelVertexArray(this.vao, Name);

            this.vbo = GL.GenBuffer();
            BindVBO();
            GLExtensions.LabelBuffer(this.vbo, Name, BufferType.VertexBufferObject);

            this.ebo = GL.GenBuffer();
            BindEBO();
            GLExtensions.LabelBuffer(this.ebo, Name, BufferType.IndexArrayObject);

            GLExtensions.BeginGroup($"Setup {Name} Data");
            GLExtensions.BeginGroup($"Upload {Name} Vertex Data");

            IsInitialized = true;

            var vertBufferData = GenerateData();

            GL.BufferData(GLBufferTarget.ArrayBuffer, vertBufferData, GLBufferUsageHint.DynamicDraw);

            GLExtensions.EndGroup();

            GLExtensions.BeginGroup($"Upload {Name} Indices Data");

            this.indices = GenerateIndices();

            // Configure the Vertex Attribute so that OpenGL knows how to read the VBO
            GL.BufferData(GLBufferTarget.ElementArrayBuffer, this.indices, GLBufferUsageHint.StaticDraw);
            GLExtensions.EndGroup();

            SetupVAO();

            UnbindVBO();
            UnbindVAO();
            UnbindEBO();
            GLExtensions.EndGroup();
        }

        /// <summary>
        /// Looks for an pulls settings out of various attributes to help set the state of the buffer.
        /// </summary>
        private void ProcessCustomAttributes()
        {
            Attribute[]? attributes = null;
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
                Name = "UNKNOWN BUFFER";
            }

            if (attributes is null || attributes.Length <= 0)
            {
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
                        Name = nameAttribute.Name;
                        break;
                }
            }
        }
    }
}

// <copyright file="GPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Raptor.NativeInterop;

    /// <inheritdoc/>
    internal class GPUBuffer<T> : IGPUBuffer
        where T : struct
    {
        private readonly IGLInvoker gl;
        private QuadData quadData = CreateQuad();
        private uint vertexArrayID;
        private uint vertexBufferID;
        private uint indexBufferID;
        private uint totalQuads = 10u;
        private uint totalVertexBytes;
        private uint totalQuadSizeInBytes;
        private bool isDisposed;
        private bool isInitialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBuffer{T}"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="totalQuads">The total number or quads to render per batch.</param>
        public GPUBuffer(IGLInvoker gl) => this.gl = gl;

        /// <inheritdoc/>
        public uint TotalQuads
        {
            get => this.totalQuads;
            set
            {
                Dispose(true);
                this.totalQuads = value;
                Init();
            }
        }

        /// <inheritdoc/>
        public void Init()
        {
            if (this.isInitialized)
            {
                return;
            }

            CreateVertexBuffer();
            CreateIndexBuffer();

            this.vertexArrayID = this.gl.GenVertexArray();

            // Bind the buffers to setup the attrib pointers
            BindVertexArray();
            BindVertexBuffer();
            BindIndexBuffer();

            SetupAttribPointers(this.vertexArrayID);
            this.isDisposed = false;
            this.isInitialized = true;
        }

        /// <inheritdoc/>
        public void UpdateQuad(uint quadID, Rectangle srcRect, int textureWidth, int textureHeight, Color tintColor)
        {
            var glColor = new Vector4()
            {
                X = tintColor.R,
                Y = tintColor.G,
                Z = tintColor.B,
                W = tintColor.A,
            };

            // Convert the tint color to a normalized 4 component vector
            glColor.X = tintColor.R.MapValue(0f, 255f, 0f, 1f);
            glColor.Y = tintColor.G.MapValue(0f, 255f, 0f, 1f);
            glColor.Z = tintColor.B.MapValue(0f, 255f, 0f, 1f);
            glColor.W = tintColor.A.MapValue(0f, 255f, 0f, 1f);

            // Update the texture coordinates to NDC (Normalized Device Coordinates)
            this.quadData.Vertex1.TextureCoord = new Vector2(srcRect.Left.MapValue(0, textureWidth, 0, 1), srcRect.Top.MapValue(0, textureHeight, 1, 0));
            this.quadData.Vertex2.TextureCoord = new Vector2(srcRect.Right.MapValue(0, textureWidth, 0, 1), srcRect.Top.MapValue(0, textureHeight, 1, 0));
            this.quadData.Vertex3.TextureCoord = new Vector2(srcRect.Right.MapValue(0, textureWidth, 0, 1), srcRect.Bottom.MapValue(0, textureHeight, 1, 0));
            this.quadData.Vertex4.TextureCoord = new Vector2(srcRect.Left.MapValue(0, textureWidth, 0, 1), srcRect.Bottom.MapValue(0, textureHeight, 1, 0));

            // Update the color
            this.quadData.Vertex1.TintColor = glColor;
            this.quadData.Vertex2.TintColor = glColor;
            this.quadData.Vertex3.TintColor = glColor;
            this.quadData.Vertex4.TintColor = glColor;

            // Update the quad ID
            this.quadData.Vertex1.TransformIndex = quadID;
            this.quadData.Vertex2.TransformIndex = quadID;
            this.quadData.Vertex3.TransformIndex = quadID;
            this.quadData.Vertex4.TransformIndex = quadID;

            var offset = this.totalQuadSizeInBytes * quadID;

            this.gl.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, this.totalQuadSizeInBytes, ref this.quadData);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing"><see langword="true"/> to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                this.gl.DeleteVertexArray(this.vertexArrayID);
                this.gl.DeleteBuffer(this.vertexBufferID);
                this.gl.DeleteBuffer(this.indexBufferID);

                this.isDisposed = true;
                this.isInitialized = false;
            }
        }

        /// <summary>
        /// Creates a single quad of data to be sent to the GPU.
        /// </summary>
        /// <returns>The quad data.</returns>
        private static QuadData CreateQuad() => new ()
        {
            Vertex1 = new VertexData()
            {
                Vertex = new Vector3(-1, 1, 0), // Top Left
            },

            Vertex2 = new VertexData()
            {
                Vertex = new Vector3(1, 1, 0), // Top Right
            },

            Vertex3 = new VertexData()
            {
                Vertex = new Vector3(1, -1, 0), // Bottom Right
            },

            Vertex4 = new VertexData()
            {
                Vertex = new Vector3(-1, -1, 0), // Bottom Left
            },
        };

        /// <summary>
        /// Unbinds the index buffer that is attached to the vertex array.
        /// </summary>
        private void UnbindIndexBuffer() => this.gl.BindBuffer(GLBufferTarget.ElementArrayBuffer, 0);

        /// <summary>
        /// Unbinds the vertex buffer.
        /// </summary>
        private void UnbindVertexBuffer() => this.gl.BindBuffer(GLBufferTarget.ArrayBuffer, 0);

        /// <summary>
        /// Sets up the attribute pointers in the vertex buffer to hold vertex buffer data for each pixel.
        /// </summary>
        /// <param name="vertexArrayID">The ID of the vertex array.</param>
        private void SetupAttribPointers(uint vertexArrayID)
        {
            var fields = typeof(T).GetFields();

            /*TODO:
             * 3. Possibly use an custom attribute to set the shader location of a field
             * 4. Need to check if type is a struct and throw exception if it is not
             */

            var offset = 0u;
            var previousSize = 0u; // The element size of the previous field

            for (uint i = 0; i < fields.Length; i++)
            {
                var stride = this.totalVertexBytes;

                // The number of float elements in the field. Ex: Vector3 has a size of 3
                var totalElements = VertexDataAnalyzer.TotalDataElementsForType(fields[i].FieldType);

                // The type of OpenGL VertexAttribPointerType based on the field type
                var attribType = VertexDataAnalyzer.GetVertexPointerType(fields[i].FieldType);

                this.gl.EnableVertexArrayAttrib(vertexArrayID, i);

                offset = i == 0 ? 0 : offset + (previousSize * VertexDataAnalyzer.GetPrimitiveByteSize(typeof(float)));
                this.gl.VertexAttribPointer(i, (int)totalElements, attribType, false, stride, offset);

                previousSize = totalElements;
            }
        }

        /// <summary>
        /// Creates a vertex buffer large enough to hold the total number of quads
        /// in the <paramref name="totalQuads"/> param.
        /// </summary>
        /// <param name="totalQuads">The total number of quads of data that can be held in the GPU's vertex buffer.</param>
        private void CreateVertexBuffer()
        {
            this.totalVertexBytes = VertexDataAnalyzer.GetTotalBytesForStruct(typeof(T));
            this.totalQuadSizeInBytes = this.totalVertexBytes * 4u;

            this.vertexBufferID = this.gl.GenBuffer();

            AllocateVertexBuffer();
        }

        /// <summary>
        /// Creates an index buffer that is large enough to hold enough indices for the
        /// total number quads in the <paramref name="totalQuads"/> param.
        /// </summary>
        /// <param name="totalQuads">The total number of quads of data that can be held in the GPU's vertex buffer.</param>
        private void CreateIndexBuffer()
        {
            this.indexBufferID = this.gl.GenBuffer();

            var indexData = new List<uint>();

            for (uint i = 0; i < this.totalQuads; i++)
            {
                var maxIndex = indexData.Count <= 0 ? 0 : indexData.Max() + 1;

                indexData.AddRange(new uint[]
                {
                    maxIndex,
                    maxIndex + 1,
                    maxIndex + 3,
                    maxIndex + 1,
                    maxIndex + 2,
                    maxIndex + 3,
                });
            }

            UploadIndexBufferData(indexData.ToArray());
        }

        /// <summary>
        /// Allocates enough memory for the vertex buffer to hold the given quad <paramref name="totalQuads"/> items.
        /// </summary>
        /// <param name="totalQuads">The total number of quads that the vertex buffer can hold.</param>
        private void AllocateVertexBuffer()
        {
            var sizeInBytes = this.totalQuadSizeInBytes * this.totalQuads;

            BindVertexBuffer();
            this.gl.BufferData(GLBufferTarget.ArrayBuffer, sizeInBytes, IntPtr.Zero, GLBufferUsageHint.DynamicDraw);
            UnbindVertexBuffer();
        }

        /// <summary>
        /// Binds the vertex buffer.
        /// </summary>
        private void BindVertexBuffer() => this.gl.BindBuffer(GLBufferTarget.ArrayBuffer, this.vertexBufferID);

        /// <summary>
        /// Uploads the given <paramref name="data"/> to the GPU.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        private void UploadIndexBufferData(uint[] data)
        {
            BindIndexBuffer();

            unsafe
            {
                fixed (uint* dataPtr = data)
                {
                    this.gl.BufferData(
                        GLBufferTarget.ElementArrayBuffer,
                        (uint)(data.Length * sizeof(uint)),
                        (IntPtr)dataPtr,
                        GLBufferUsageHint.DynamicDraw);
                }
            }

            UnbindIndexBuffer();
        }

        /// <summary>
        /// Binds the index buffer.
        /// </summary>
        private void BindIndexBuffer() => this.gl.BindBuffer(GLBufferTarget.ElementArrayBuffer, this.indexBufferID);

        /// <summary>
        /// Binds the vertex array.
        /// </summary>
        private void BindVertexArray() => this.gl.BindVertexArray(this.vertexArrayID);
    }
}

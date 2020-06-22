// <copyright file="GPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Raptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Linq;
    using OpenToolkit.Graphics.OpenGL4;
    using OpenToolkit.Mathematics;

    /// <summary>
    /// Represents GPU vertex buffer memory.
    /// </summary>
    /// <typeparam name="T">The type of data to send to the GPU.</typeparam>
    internal class GPUBuffer<T> : IDisposable
        where T : struct
    {
        private readonly IGLInvoker gl;
        private int vertexArrayID = -1;
        private int vertexBufferID = -1;
        private int indexBufferID = -1;
        private int totalVertexBytes;
        private int totalQuadSizeInBytes;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBuffer{T}"/> class.
        /// </summary>
        /// <param name="totalQuads">The total number or quads to render per batch.</param>
        [ExcludeFromCodeCoverage]
        public GPUBuffer(int totalQuads)
        {
            this.gl = new GLInvoker();
            Init(totalQuads);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPUBuffer{T}"/> class.
        /// NOTE: Used for unit testing to inject a mocked <see cref="IGLInvoker"/>.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="totalQuads">The total number or quads to render per batch.</param>
        internal GPUBuffer(IGLInvoker gl, int totalQuads)
        {
            this.gl = gl;
            Init(totalQuads);
        }

        /// <summary>
        /// Updates the given quad using the given information for a particular quad item in the GPU.
        /// </summary>
        /// <param name="quadID">The ID of the quad to update.</param>
        /// <param name="srcRect">The area within the texture to update.</param>
        /// <param name="textureWidth">The width of the texture.</param>
        /// <param name="textureHeight">The height of the texture.</param>
        /// <param name="tintColor">The color to apply to the texture area being rendered.</param>
        public void UpdateQuad(int quadID, Rectangle srcRect, int textureWidth, int textureHeight, Color tintColor)
        {
            var quadData = CreateQuadWithTextureCoordinates(srcRect, textureWidth, textureHeight);

            // Update the color
            quadData.Vertex1.TintColor = tintColor.ToGLColor();
            quadData.Vertex2.TintColor = tintColor.ToGLColor();
            quadData.Vertex3.TintColor = tintColor.ToGLColor();
            quadData.Vertex4.TintColor = tintColor.ToGLColor();

            // Update the quad ID
            quadData.Vertex1.TransformIndex = quadID;
            quadData.Vertex2.TransformIndex = quadID;
            quadData.Vertex3.TransformIndex = quadID;
            quadData.Vertex4.TransformIndex = quadID;

            var offset = this.totalQuadSizeInBytes * quadID;
            this.gl.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(offset), this.totalQuadSizeInBytes, quadData);
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
        /// <param name="disposing">True to dispose of managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                this.gl.DeleteVertexArray(this.vertexArrayID);
                this.gl.DeleteBuffer(this.vertexBufferID);
                this.gl.DeleteBuffer(this.indexBufferID);

                this.disposedValue = true;
            }
        }

        /// <summary>
        /// Creates a single quad of data to be sent to the GPU.
        /// </summary>
        /// <returns>The quad data.</returns>
        private static QuadData CreateQuad() => new QuadData
        {
            Vertex1 = new VertexData()
            {
                Vertex = new Vector3(-1, 1, 0), // Top Left
                TextureCoord = new Vector2(0, 1),
            },

            Vertex2 = new VertexData()
            {
                Vertex = new Vector3(1, 1, 0), // Top Right
                TextureCoord = new Vector2(1, 1),
            },

            Vertex3 = new VertexData()
            {
                Vertex = new Vector3(1, -1, 0), // Bottom Right
                TextureCoord = new Vector2(1, 0),
            },

            Vertex4 = new VertexData()
            {
                Vertex = new Vector3(-1, -1, 0), // Bottom Left
                TextureCoord = new Vector2(0, 0),
            },
        };

        /// <summary>
        /// Calculates the texture coordinates in the given <paramref name="quad"/> based on the area of the texture
        /// to render and the texture size.
        /// </summary>
        /// <param name="quad">The quad to update.</param>
        /// <param name="srcRect">The area of the rectangle used to calculate the texture coordinates.</param>
        /// <param name="textureWidth">The with of the texture.</param>
        /// <param name="textureHeight">The height of the texture.</param>
        private static QuadData CreateQuadWithTextureCoordinates(Rectangle srcRect, int textureWidth, int textureHeight)
        {
            var quad = CreateQuad();

            // TODO: Condense this code down
            var topLeftCornerX = srcRect.Left.MapValue(0, textureWidth, 0, 1);
            var topLeftCornerY = srcRect.Top.MapValue(0, textureHeight, 1, 0);
            var topLeftCoord = new Vector2(topLeftCornerX, topLeftCornerY);

            var topRightCornerX = srcRect.Right.MapValue(0, textureWidth, 0, 1);
            var topRightCornerY = srcRect.Top.MapValue(0, textureHeight, 1, 0);
            var topRightCoord = new Vector2(topRightCornerX, topRightCornerY);

            var bottomRightCornerX = srcRect.Right.MapValue(0, textureWidth, 0, 1);
            var bottomRightCornerY = srcRect.Bottom.MapValue(0, textureHeight, 1, 0);
            var bottomRightCoord = new Vector2(bottomRightCornerX, bottomRightCornerY);

            var bottomLeftCornerX = srcRect.Left.MapValue(0, textureWidth, 0, 1);
            var bottomLeftCornerY = srcRect.Bottom.MapValue(0, textureHeight, 1, 0);
            var bottomLeftCoord = new Vector2(bottomLeftCornerX, bottomLeftCornerY);

            quad.Vertex1.TextureCoord = topLeftCoord;
            quad.Vertex2.TextureCoord = topRightCoord;
            quad.Vertex3.TextureCoord = bottomRightCoord;
            quad.Vertex4.TextureCoord = bottomLeftCoord;

            return quad;
        }

        /// <summary>
        /// Initializes the <see cref="GPUBuffer{T}"/>.
        /// </summary>
        /// <param name="totalQuads">The total number or quads to render per batch.</param>
        private void Init(int totalQuads)
        {
            CreateVertexBuffer(totalQuads);
            CreateIndexBuffer(totalQuads);

            this.vertexArrayID = this.gl.GenVertexArray();

            // Bind the buffers to setup the attrib pointers
            BindVertexArray();
            BindVertexBuffer();
            BindIndexBuffer();

            SetupAttribPointers(this.vertexArrayID);
        }

        /// <summary>
        /// Unbinds the index buffer that is attached to the vertex array.
        /// </summary>
        private void UnbindIndexBuffer() => this.gl.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

        /// <summary>
        /// Unbinds the vertex buffer.
        /// </summary>
        private void UnbindVertexBuffer() => this.gl.BindBuffer(BufferTarget.ArrayBuffer, 0);

        /// <summary>
        /// Sets up the attribute pointers in the vertex buffer to hold vertex buffer data for each pixel.
        /// </summary>
        /// <param name="vertexArrayID">The ID of the vertex array.</param>
        private void SetupAttribPointers(int vertexArrayID)
        {
            var props = typeof(T).GetFields();

            /*TODO:
             * 3. Possibly use an custom attribute to set the shader location of a field
             * 4. Need to check if type is a struct and throw exception if it is not
             */

            var offset = 0;
            var previousSize = 0; // The element size of the previous field

            for (var i = 0; i < props.Length; i++)
            {
                var stride = this.totalVertexBytes;

                // The number of float elements in the field. Ex: Vector3 has a size of 3
                var size = VertexDataAnalyzer.TotalItemsForType(props[i].FieldType);

                // The type of OpenGL VertexAttribPointerType based on the field type
                var attribType = VertexDataAnalyzer.GetVertexPointerType(props[i].FieldType);

                this.gl.EnableVertexArrayAttrib(vertexArrayID, i);

                offset = i == 0 ? 0 : offset + (previousSize * VertexDataAnalyzer.GetTypeByteSize(typeof(float)));
                this.gl.VertexAttribPointer(i, size, attribType, false, stride, offset);

                previousSize = size;
            }
        }

        /// <summary>
        /// Creates a vertex buffer large enough to hold the total number of quads
        /// in the the <paramref name="totalQuads"/> param.
        /// </summary>
        /// <param name="totalQuads">The total number of quads of data that can be held in the GPU's vertex buffer.</param>
        private void CreateVertexBuffer(int totalQuads)
        {
            this.totalVertexBytes = VertexDataAnalyzer.GetTotalBytesForStruct(typeof(T));
            this.totalQuadSizeInBytes = this.totalVertexBytes * 4;

            this.vertexBufferID = this.gl.GenBuffer();

            var quadData = new List<QuadData>();

            for (var i = 0; i < totalQuads; i++)
            {
                quadData.Add(CreateQuad());
            }

            AllocateVertexBuffer(totalQuads);
        }

        /// <summary>
        /// Creates an index buffer that is large enough to hold enough indices for the
        /// total number quads in the <paramref name="totalQuads"/> param.
        /// </summary>
        /// <param name="totalQuads">The total number of quads of data that can be held in the GPU's vertex buffer.</param>
        private void CreateIndexBuffer(int totalQuads)
        {
            this.indexBufferID = this.gl.GenBuffer();

            var indexData = new List<uint>();

            for (uint i = 0; i < totalQuads; i++)
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
        /// Allocates enough memory for the vertex buffer to hold the given quad <paramref name="totalQUads"/> items.
        /// </summary>
        /// <param name="totalQUads">The total number of quads that the vertex buffer can hold.</param>
        private void AllocateVertexBuffer(int totalQUads)
        {
            var sizeInBytes = this.totalQuadSizeInBytes * totalQUads;

            BindVertexBuffer();
            this.gl.BufferData(BufferTarget.ArrayBuffer, sizeInBytes, IntPtr.Zero, BufferUsageHint.DynamicDraw);
            UnbindVertexBuffer();
        }

        /// <summary>
        /// Binds the vertex buffer.
        /// </summary>
        private void BindVertexBuffer() => this.gl.BindBuffer(BufferTarget.ArrayBuffer, this.vertexBufferID);

        /// <summary>
        /// Uploads the given <paramref name="data"/> to the GPU.
        /// </summary>
        /// <param name="data">The data to upload.</param>
        private void UploadIndexBufferData(uint[] data)
        {
            BindIndexBuffer();

            this.gl.BufferData(
                BufferTarget.ElementArrayBuffer,
                data.Length * sizeof(uint),
                data,
                BufferUsageHint.DynamicDraw);

            UnbindIndexBuffer();
        }

        /// <summary>
        /// Binds the index buffer.
        /// </summary>
        private void BindIndexBuffer() => this.gl.BindBuffer(BufferTarget.ElementArrayBuffer, this.indexBufferID);

        /// <summary>
        /// Binds the vertex array.
        /// </summary>
        private void BindVertexArray() => this.gl.BindVertexArray(this.vertexArrayID);
    }
}

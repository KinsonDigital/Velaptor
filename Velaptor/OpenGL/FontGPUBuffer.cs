// <copyright file="FontGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Exceptions;
    using VelObservable = Velaptor.Observables.Core.IObservable<bool>;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Updates font data in the GPU buffer.
    /// </summary>
    [GPUBufferName("Font")]
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    internal class FontGPUBuffer : GPUBufferBase<SpriteBatchItem>
    {
        private const string BufferNotInitMsg = "The font buffer has not been initialized.";

        /// <summary>
        /// Initializes a new instance of the <see cref="FontGPUBuffer"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="glExtensions">Invokes helper methods for OpenGL function calls.</param>
        /// <param name="glInitObservable">Receives a notification when OpenGL has been initialized.</param>
        /// <param name="shutDownObservable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        public FontGPUBuffer(
            IGLInvoker gl,
            IGLInvokerExtensions glExtensions,
            VelObservable glInitObservable,
            VelObservable shutDownObservable)
            : base(gl, glExtensions, glInitObservable, shutDownObservable)
        {
        }

        /// <inheritdoc/>
        protected internal override float[] GenerateData()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            var result = new List<TextureQuadData>();

            for (var i = 0u; i < BatchSize; i++)
            {
                result.AddRange(new TextureQuadData[]
                {
                    new ()
                    {
                        Vertex1 = default,
                        Vertex2 = default,
                        Vertex3 = default,
                        Vertex4 = default,
                    },
                });
            }

            return result.ToVertexArray();
        }

        /// <inheritdoc/>
        protected internal override uint[] GenerateIndices()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            var result = new List<uint>();

            for (var i = 0u; i < BatchSize; i++)
            {
                var maxIndex = result.Count <= 0 ? 0 : result.Max() + 1;

                result.AddRange(new[]
                {
                    maxIndex,
                    maxIndex + 1u,
                    maxIndex + 2u,
                    maxIndex + 2u,
                    maxIndex + 1u,
                    maxIndex + 3u,
                });
            }

            return result.ToArray();
        }

        /// <inheritdoc/>
        protected internal override void PrepareForUpload()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            BindVAO();
        }

        /// <inheritdoc/>
        protected internal override void SetupVAO()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            GLExtensions.BeginGroup("Setup Font Buffer Vertex Attributes");

            var stride = TextureVertexData.Stride();

            const uint vertexPosOffset = 0u;
            const uint vertexPosSize = 2u * sizeof(float);
            GL.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, stride, vertexPosOffset);
            GL.EnableVertexAttribArray(0);

            const uint textureCoordOffset = vertexPosOffset + vertexPosSize;
            const uint textureCoordSize = 2u * sizeof(float);
            GL.VertexAttribPointer(1, 2, GLVertexAttribPointerType.Float, false, stride, textureCoordOffset);
            GL.EnableVertexAttribArray(1);

            const uint tintClrOffset = textureCoordOffset + textureCoordSize;
            GL.VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, stride, tintClrOffset);
            GL.EnableVertexAttribArray(2);

            GLExtensions.EndGroup();
        }

        /// <inheritdoc/>
        protected internal override void UploadVertexData(SpriteBatchItem textureQuad, uint batchIndex)
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            GLExtensions.BeginGroup($"Update Font Quad - BatchItem({batchIndex})");

            // Construct the quad rect to determine the vertex positions sent to the GPU
            var quadRect = new RectangleF(textureQuad.DestRect.X, textureQuad.DestRect.Y, textureQuad.SrcRect.Width, textureQuad.SrcRect.Height);

            var left = quadRect.X;
            var bottom = quadRect.Y + quadRect.Height;
            var right = quadRect.X + quadRect.Width;
            var top = quadRect.Y;

            var topLeft = new Vector2(left, top);
            var bottomLeft = new Vector2(left, bottom);
            var bottomRight = new Vector2(right, bottom);
            var topRight = new Vector2(right, top);

            topLeft.Y -= quadRect.Height;
            bottomLeft.Y -= quadRect.Height;
            bottomRight.Y -= quadRect.Height;
            topRight.Y -= quadRect.Height;

            var angle = textureQuad.Angle;
            var origin = new Vector2(quadRect.X, quadRect.Y);

            topLeft = topLeft.RotateAround(origin, angle);
            bottomLeft = bottomLeft.RotateAround(origin, angle);
            bottomRight = bottomRight.RotateAround(origin, angle);
            topRight = topRight.RotateAround(origin, angle);

            var textureWidth = textureQuad.DestRect.Width;
            var textureHeight = textureQuad.DestRect.Height;

            // Convert the texture quad vertex positions to NDC values
            var quadDataItem = default(TextureQuadData);
            quadDataItem.Vertex1.VertexPos = topLeft.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex2.VertexPos = bottomLeft.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex3.VertexPos = topRight.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex4.VertexPos = bottomRight.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);

            // Update the color
            quadDataItem.Vertex1.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex2.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex3.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex4.TintColor = textureQuad.TintColor;

            var textureTopLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Top);
            var textureBottomLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Bottom);
            var textureTopRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Top);
            var textureBottomRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Bottom);

            // Update the texture coordinates
            quadDataItem.Vertex1.TextureCoord = textureTopLeft.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex2.TextureCoord = textureBottomLeft.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex3.TextureCoord = textureTopRight.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex4.TextureCoord = textureBottomRight.ToNDCTextureCoords(textureWidth, textureHeight);

            var totalBytes = TextureQuadData.GetTotalBytes();
            var data = quadDataItem.ToArray();
            var offset = totalBytes * batchIndex;

            BindVBO();

            GL.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, totalBytes, data);

            UnbindVBO();

            GLExtensions.EndGroup();
        }
    }
}

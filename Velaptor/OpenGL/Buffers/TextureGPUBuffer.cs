// <copyright file="TextureGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers
{
    // ReSharper disable RedundantNameQualifier
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Velaptor.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.OpenGL.Exceptions;
    using Velaptor.OpenGL.GPUData;
    using Velaptor.Reactables.Core;
    using Velaptor.Reactables.ReactableData;
    using NETRect = System.Drawing.Rectangle;

    // ReSharper restore RedundantNameQualifier

    /// <summary>
    /// Updates texture data in the GPU buffer.
    /// </summary>
    [GPUBufferName("Texture")]
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    internal class TextureGPUBuffer : GPUBufferBase<SpriteBatchItem>
    {
        private const string BufferNotInitMsg = "The texture buffer has not been initialized.";

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureGPUBuffer"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        /// <param name="openGLService">Provides OpenGL related helper methods.</param>
        /// <param name="glInitReactable">Receives a notification when OpenGL has been initialized.</param>
        /// <param name="shutDownReactable">Sends out a notification that the application is shutting down.</param>
        /// <exception cref="ArgumentNullException">
        ///     Invoked when any of the parameters are null.
        /// </exception>
        public TextureGPUBuffer(
            IGLInvoker gl,
            IOpenGLService openGLService,
            IReactable<GLInitData> glInitReactable,
            IReactable<ShutDownData> shutDownReactable)
            : base(gl, openGLService, glInitReactable, shutDownReactable)
        {
        }

        /// <inheritdoc/>
        protected internal override void UploadVertexData(SpriteBatchItem textureQuad, uint batchIndex)
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            OpenGLService.BeginGroup($"Update Texture Quad - BatchItem({batchIndex}) Data");

            float srcRectWidth;
            float srcRectHeight;

            switch (textureQuad.Effects)
            {
                case RenderEffects.None:
                    srcRectWidth = textureQuad.SrcRect.Width * -1;
                    srcRectHeight = textureQuad.SrcRect.Height * -1;
                    break;
                case RenderEffects.FlipHorizontally:
                    srcRectWidth = textureQuad.SrcRect.Width;
                    srcRectHeight = textureQuad.SrcRect.Height * -1;
                    break;
                case RenderEffects.FlipVertically:
                    srcRectWidth = textureQuad.SrcRect.Width * -1;
                    srcRectHeight = textureQuad.SrcRect.Height;
                    break;
                case RenderEffects.FlipBothDirections:
                    srcRectWidth = textureQuad.SrcRect.Width;
                    srcRectHeight = textureQuad.SrcRect.Height;
                    break;
                default:
                    throw new InvalidRenderEffectsException(
                        $"The '{nameof(RenderEffects)}' value of '{(int)textureQuad.Effects}' is not valid.");
            }

            // Construct the quad rect to determine the vertex positions sent to the GPU
            var quadRect = new RectangleF(textureQuad.DestRect.X, textureQuad.DestRect.Y, srcRectWidth, srcRectHeight);

            // Calculate the scale on the X and Y axis to calculate the size
            var resolvedSize = textureQuad.Size - 1f;
            var width = quadRect.Width + (quadRect.Width * resolvedSize);
            var height = quadRect.Height + (quadRect.Height * resolvedSize);

            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            var left = quadRect.X - halfWidth;
            var bottom = quadRect.Y + halfHeight;
            var right = quadRect.X + halfWidth;
            var top = quadRect.Y - halfHeight;

            var topLeft = new Vector2(left, top);
            var bottomLeft = new Vector2(left, bottom);
            var bottomRight = new Vector2(right, bottom);
            var topRight = new Vector2(right, top);

            var angle = textureQuad.Angle + 180;
            var origin = new Vector2(quadRect.X, quadRect.Y);
            topLeft = topLeft.RotateAround(origin, angle);
            bottomLeft = bottomLeft.RotateAround(origin, angle);
            bottomRight = bottomRight.RotateAround(origin, angle);
            topRight = topRight.RotateAround(origin, angle);

            // Convert the texture quad vertex positions to NDC values
            var quadDataItem = default(TextureQuadData);
            quadDataItem.Vertex1.VertexPos = topLeft.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex2.VertexPos = bottomLeft.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex3.VertexPos = topRight.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);
            quadDataItem.Vertex4.VertexPos = bottomRight.ToNDC(textureQuad.ViewPortSize.Width, textureQuad.ViewPortSize.Height);

            var textureWidth = textureQuad.DestRect.Width;
            var textureHeight = textureQuad.DestRect.Height;

            // Setup the corners of the sub texture to render
            var textureTopLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Top);
            var textureBottomLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Bottom);
            var textureTopRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Top);
            var textureBottomRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Bottom);

            // Convert the texture coordinates to NDC values
            quadDataItem.Vertex1.TextureCoord = textureTopLeft.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex2.TextureCoord = textureBottomLeft.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex3.TextureCoord = textureTopRight.ToNDCTextureCoords(textureWidth, textureHeight);
            quadDataItem.Vertex4.TextureCoord = textureBottomRight.ToNDCTextureCoords(textureWidth, textureHeight);

            // Update the color
            quadDataItem.Vertex1.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex2.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex3.TintColor = textureQuad.TintColor;
            quadDataItem.Vertex4.TintColor = textureQuad.TintColor;

            var totalBytes = TextureQuadData.GetTotalBytes();
            var data = quadDataItem.ToArray();
            var offset = totalBytes * batchIndex;

            OpenGLService.BindVBO(VBO);

            GL.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, totalBytes, data);

            OpenGLService.UnbindVBO();

            OpenGLService.EndGroup();
        }

        /// <inheritdoc/>
        protected internal override void PrepareForUpload()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            OpenGLService.BindVAO(VAO);
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
        protected internal override void SetupVAO()
        {
            if (IsInitialized is false)
            {
                throw new BufferNotInitializedException(BufferNotInitMsg);
            }

            OpenGLService.BeginGroup("Setup Texture Buffer Vertex Attributes");

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

            OpenGLService.EndGroup();
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
    }
}

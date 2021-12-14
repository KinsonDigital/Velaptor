// <copyright file="TextureGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Numerics;
    using Exceptions;
    using Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Velaptor.Observables;
    using System.Drawing;
    using NETRect = System.Drawing.Rectangle;

    // TODO: Rename QuadData to TextureQuadData
    // TODO: Properly dispose of buffers on object dispose

    /// <summary>
    /// Updates texture data in the GPU buffer.
    /// </summary>
    [GPUBufferName("Texture")]
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    internal class TextureGPUBuffer : GPUBufferBase<SpriteBatchItem>
    {
        private TextureQuadData[] quadData = Array.Empty<TextureQuadData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureGPUBuffer"/> class.
        /// </summary>
        /// <param name="gl">Invokes OpenGL functions.</param>
        public TextureGPUBuffer(IGLInvoker gl, OpenGLInitObservable glObservable)
            : base(gl, glObservable)
        {
        }

        protected override void UpdateVertexData(SpriteBatchItem textureQuad, uint batchIndex)
        {
            if (IsInitialized is false)
            {
                // TODO: Create custom exception and implement here
                throw new Exception("Buffer not initialized");
            }

            this.GL.BeginGroup($"Update Texture Quad {batchIndex} Data");

            float srcRectWidth;
            float srcRectHeight;

            switch (textureQuad.Effects)
            {
                case RenderEffects.None:
                    srcRectWidth = textureQuad.SrcRect.Width;
                    srcRectHeight = textureQuad.SrcRect.Height;
                    break;
                case RenderEffects.FlipHorizontally:
                    srcRectWidth = textureQuad.SrcRect.Width * -1;
                    srcRectHeight = textureQuad.SrcRect.Height;
                    break;
                case RenderEffects.FlipVertically:
                    srcRectWidth = textureQuad.SrcRect.Width;
                    srcRectHeight = textureQuad.SrcRect.Height * -1;
                    break;
                case RenderEffects.FlipBothDirections:
                    srcRectWidth = textureQuad.SrcRect.Width * -1;
                    srcRectHeight = textureQuad.SrcRect.Height * -1;
                    break;
                default:
                    throw new InvalidRenderEffectsException(
                        $"The '{nameof(RenderEffects)}' value of '{(int)textureQuad.Effects}' is not valid.");
            }

            // Convert the tint color to a normalized 4 component vector
            var red = (byte)textureQuad.TintColor.R.MapValue(0f, 255f, 0f, 1f);
            var green = (byte)textureQuad.TintColor.G.MapValue(0f, 255f, 0f, 1f);
            var blue = (byte)textureQuad.TintColor.B.MapValue(0f, 255f, 0f, 1f);
            var alpha = (byte)textureQuad.TintColor.A.MapValue(0f, 255f, 0f, 1f);

            // Construct the quad rect to determine the vertex positions sent to the GPU
            var quadRect = new RectangleF(textureQuad.DestRect.X, textureQuad.DestRect.Y, textureQuad.SrcRect.Width, textureQuad.SrcRect.Height);

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

            // Rotate the sub texture corners
            var srcRectHalfWidth = textureQuad.SrcRect.Width / 2;
            var srcRectHalfHeight = textureQuad.SrcRect.Height / 2;

            // Convert the texture coordinates to NDC values
            var srcRectOrigin = new Vector2(textureQuad.SrcRect.Left - srcRectHalfWidth, textureQuad.SrcRect.Top + srcRectHalfHeight);
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

            BindVBO();

            this.GL.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, totalBytes, data);

            UnbindVBO();

            this.GL.EndGroup();
        }

        protected override void PrepareForUse()
        {
            if (IsInitialized is false)
            {
                // TODO: Create custom exception and implement here
                throw new Exception("Buffer not initialized");
            }

            BindVAO();
        }

        protected override float[] GenerateData()
        {
            if (IsInitialized is false)
            {
                // TODO: Create custom exception and implement here
                throw new Exception("Buffer not initialized");
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

            this.quadData = result.ToArray();

            return result.ToVertexArray();
        }

        protected override void SetupVAO()
        {
            if (IsInitialized is false)
            {
                // TODO: Create custom exception and implement here
                throw new Exception("Buffer not initialized");
            }

            var stride = TextureVertexData.Stride();

            unsafe
            {
                var vertexPosOffset = 0u;
                const uint vertexPosSize = 2u * sizeof(float);
                this.GL.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, stride, vertexPosOffset);
                this.GL.EnableVertexAttribArray(0);

                var textureCoordOffset = vertexPosOffset + vertexPosSize;
                const uint textureCoordSize = 2u * sizeof(float);
                this.GL.VertexAttribPointer(1, 2, GLVertexAttribPointerType.Float, false, stride, textureCoordOffset);
                this.GL.EnableVertexAttribArray(1);

                var tintClrOffset = textureCoordOffset + textureCoordSize;
                this.GL.VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, stride, tintClrOffset);
                this.GL.EnableVertexAttribArray(2);
            }
        }

        protected override uint[] GenerateIndices()
        {
            if (IsInitialized is false)
            {
                // TODO: Create custom exception and implement here
                throw new Exception("Buffer not initialized");
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

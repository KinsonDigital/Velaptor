// <copyright file="FontGPUBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Graphics;
    using Velaptor.NativeInterop.OpenGL;
    using Observables;

    [GPUBufferName("Font")]
    [SpriteBatchSize(ISpriteBatch.BatchSize)]
    internal class FontGPUBuffer : GPUBufferBase<SpriteBatchItem>
    {
        public FontGPUBuffer(IGLInvoker gl, OpenGLInitObservable glObservable)
            : base(gl, glObservable)
        {
        }

        protected override float[] GenerateData()
        {
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

        protected override uint[] GenerateIndices()
        {
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

        protected override void PrepareForUse()
        {
            BindVAO();
        }

        protected override void SetupVAO()
        {
            uint stride = TextureVertexData.Stride();

            unsafe
            {
                var vertexPosOffset = 0u;
                const uint vertexPosSize = 2u * sizeof(float);
                GL.VertexAttribPointer(0, 2, GLVertexAttribPointerType.Float, false, stride, vertexPosOffset);
                GL.EnableVertexAttribArray(0);

                var textureCoordOffset = vertexPosOffset + vertexPosSize;
                const uint textureCoordSize = 2u * sizeof(float);
                GL.VertexAttribPointer(1, 2, GLVertexAttribPointerType.Float, false, stride, textureCoordOffset);
                GL.EnableVertexAttribArray(1);

                var tintClrOffset = textureCoordOffset + textureCoordSize;
                GL.VertexAttribPointer(2, 4, GLVertexAttribPointerType.Float, false, stride, tintClrOffset);
                GL.EnableVertexAttribArray(2);
            }
        }

        protected override void UpdateVertexData(SpriteBatchItem textureQuad, uint batchIndex)
        {
            GL.BeginGroup($"Update Font Quad - BatchItem({batchIndex})");

            // TODO: Perform a check for the viewport size here.  The values of inifinity and 0 are not aloud

            // Construct the quad rect to determine the vertex positions sent to the GPU
            var quadRect = new RectangleF(textureQuad.DestRect.X, textureQuad.DestRect.Y, textureQuad.SrcRect.Width, textureQuad.SrcRect.Height);

            var halfWidth = quadRect.Width / 2f;
            var halfHeight = quadRect.Height / 2f;

            var left = quadRect.X - halfWidth;
            var bottom = quadRect.Y + halfHeight;
            var right = quadRect.X + halfWidth;
            var top = quadRect.Y - halfHeight;

            var topLeft = new Vector2(left, top);
            var bottomLeft = new Vector2(left, bottom);
            var bottomRight = new Vector2(right, bottom);
            var topRight = new Vector2(right, top);

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

            GL.EndGroup();
        }
    }

}

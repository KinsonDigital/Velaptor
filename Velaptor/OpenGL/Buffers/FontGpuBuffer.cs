// <copyright file="FontGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using Batching;
using Carbonate.Fluent;
using Exceptions;
using Factories;
using GpuData;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using ReactableData;

/// <summary>
/// Updates font data in the GPU buffer.
/// </summary>
[GpuBufferName("Font")]
internal sealed class FontGpuBuffer : GpuBufferBase<FontGlyphBatchItem>
{
    private const string BufferNotInitMsg = "The font buffer has not been initialized.";

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public FontGpuBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, reactableFactory)
    {
        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        var subscription = ISubscriptionBuilder.Create()
            .WithId(PushNotifications.BatchSizeChangedId)
            .WithName(this.GetExecutionMemberName(nameof(PushNotifications.BatchSizeChangedId)))
            .BuildOneWayReceive<BatchSizeData>(data =>
            {
                if (data.TypeOfBatch != BatchType.Font)
                {
                    return;
                }

                BatchSize = data.BatchSize;

                if (IsInitialized)
                {
                    ResizeBatch();
                }
            });

        batchSizeReactable.Subscribe(subscription);
    }

    /// <inheritdoc/>
    protected internal override float[] GenerateData()
    {
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        var result = new List<TextureGpuData>();

        for (var i = 0u; i < BatchSize; i++)
        {
            result.AddRange(new TextureGpuData[] { new (default, default, default, default) });
        }

        return OpenGLExtensionMethods.ToArray(result);
    }

    /// <inheritdoc/>
    protected internal override uint[] GenerateIndices()
    {
        if (!IsInitialized)
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
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BindVAO(VAO);
    }

    /// <inheritdoc/>
    protected internal override void SetupVAO()
    {
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BeginGroup("Setup Font Buffer Vertex Attributes");

        var stride = TextureVertexData.GetStride();

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
    protected internal override void UploadVertexData(FontGlyphBatchItem textureQuad, uint batchIndex)
    {
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BeginGroup($"Update Font Quad - BatchItem({batchIndex})");

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

        var vertex1 = topLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex2 = bottomLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex3 = topRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex4 = bottomRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height);

        var textureTopLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Top);
        var textureBottomLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Bottom);
        var textureTopRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Top);
        var textureBottomRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Bottom);

        var textureWidth = textureQuad.DestRect.Width;
        var textureHeight = textureQuad.DestRect.Height;

        // Update the texture coordinates
        textureTopLeft = textureTopLeft.ToNDCTextureCoords(textureWidth, textureHeight);
        textureBottomLeft = textureBottomLeft.ToNDCTextureCoords(textureWidth, textureHeight);
        textureTopRight = textureTopRight.ToNDCTextureCoords(textureWidth, textureHeight);
        textureBottomRight = textureBottomRight.ToNDCTextureCoords(textureWidth, textureHeight);

        // Convert the texture quad vertex positions to NDC values
        var quadDataItem = new TextureGpuData(
            new TextureVertexData(vertex1, textureTopLeft, textureQuad.TintColor),
            new TextureVertexData(vertex2, textureBottomLeft, textureQuad.TintColor),
            new TextureVertexData(vertex3, textureTopRight, textureQuad.TintColor),
            new TextureVertexData(vertex4, textureBottomRight, textureQuad.TintColor));

        var totalBytes = TextureGpuData.GetTotalBytes();
        var data = quadDataItem.ToArray();
        var offset = totalBytes * batchIndex;

        OpenGLService.BindVBO(VBO);

        GL.BufferSubData(GLBufferTarget.ArrayBuffer, (nint)offset, totalBytes, data);

        OpenGLService.UnbindVBO();

        OpenGLService.EndGroup();
    }
}

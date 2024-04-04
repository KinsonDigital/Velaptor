// <copyright file="TextureGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.OpenGL.Buffers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Batching;
using Carbonate;
using Exceptions;
using Factories;
using GpuData;
using Graphics;
using NativeInterop.OpenGL;
using NativeInterop.Services;
using Velaptor.Exceptions;

/// <summary>
/// Updates texture data in the GPU buffer.
/// </summary>
[GpuBufferName("Texture")]
internal sealed class TextureGpuBuffer : GpuBufferBase<TextureBatchItem>
{
    private readonly IDisposable unsubscriber;
    private const string BufferNotInitMsg = "The texture buffer has not been initialized.";

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGpuBuffer"/> class.
    /// </summary>
    /// <param name="gl">Invokes OpenGL functions.</param>
    /// <param name="openGLService">Provides OpenGL related helper methods.</param>
    /// <param name="reactableFactory">Creates reactables for sending and receiving notifications with or without data.</param>
    /// <exception cref="ArgumentNullException">
    ///     Invoked when any of the parameters are null.
    /// </exception>
    public TextureGpuBuffer(
        IGLInvoker gl,
        IOpenGLService openGLService,
        IReactableFactory reactableFactory)
            : base(gl, openGLService, reactableFactory)
    {
        var batchSizeReactable = reactableFactory.CreateBatchSizeReactable();

        this.unsubscriber = batchSizeReactable.CreateOneWayReceive(
            PushNotifications.BatchSizeChangedId,
            (data) =>
            {
                if (data.TypeOfBatch != BatchType.Texture)
                {
                    return;
                }

                BatchSize = data.BatchSize;

                if (IsInitialized)
                {
                    ResizeBatch();
                }
            },
            () => this.unsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    protected internal override void UploadVertexData(TextureBatchItem textureQuad, uint batchIndex)
    {
        if (!IsInitialized)
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
        var quadRect = textureQuad.DestRect with { Width = srcRectWidth, Height = srcRectHeight };

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

        var vertex1 = topLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex2 = bottomLeft.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex3 = topRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height);
        var vertex4 = bottomRight.ToNDC(ViewPortSize.Width, ViewPortSize.Height);

        // Sets up the corners of the sub texture to render
        var textureTopLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Top);
        var textureBottomLeft = new Vector2(textureQuad.SrcRect.Left, textureQuad.SrcRect.Bottom);
        var textureTopRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Top);
        var textureBottomRight = new Vector2(textureQuad.SrcRect.Right, textureQuad.SrcRect.Bottom);

        var textureWidth = textureQuad.DestRect.Width;
        var textureHeight = textureQuad.DestRect.Height;

        // Convert the texture coordinates to NDC values
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
    protected internal override void SetupVAO()
    {
        if (!IsInitialized)
        {
            throw new BufferNotInitializedException(BufferNotInitMsg);
        }

        OpenGLService.BeginGroup("Setup Texture Buffer Vertex Attributes");

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
}

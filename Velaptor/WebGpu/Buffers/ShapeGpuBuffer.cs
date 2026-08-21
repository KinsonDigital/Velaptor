// <copyright file="ShapeGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using Graphics;
using Batching;
using Carbonate;
using Factories;

/// <summary>
/// Manages GPU vertex and index buffers for rendering rounded rectangles.
/// Each batch item uses 4 vertices (64-byte stride) and 6 indices.
/// </summary>
/// <remarks>
/// <para>
/// Per-vertex layout (stride = 64 bytes = 16 × f32):
/// <code>
/// [ 0..1] position          vec2   offset  0   (NDC)
/// [ 2..5] shape             vec4   offset  8   (centerX, centerY, width, height in pixels)
/// [ 6..9] color             vec4   offset 24   (R, G, B, A as 0-255 floats)
/// [10]     isFilled          f32    offset 40   (0.0 = border, 1.0 = solid)
/// [11]     borderThickness   f32    offset 44
/// [12]     topLeftRadius     f32    offset 48
/// [13]     topRightRadius    f32    offset 52
/// [14]     bottomRightRadius f32    offset 56
/// [15]     bottomLeftRadius  f32    offset 60
/// </code>
/// </para>
/// </remarks>
internal sealed class ShapeGpuBuffer : WebGpuBufferBase<ShapeBatchItem>
{
    private const uint RectVertexSize = 64; // 16 floats × 4 bytes
    private const uint RectIndexItemSize = 4; // uint32
    private const uint VerticesPerRect = 4;
    private const uint IndicesPerRect = 6;
    private readonly IDisposable resizeBufferSubscriber;
    private readonly IDisposable wgpuReadyUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="reactableFactory">Creates reactables.</param>
    public ShapeGpuBuffer(IGraphicsDevice gd, IReactableFactory reactableFactory)
        : base(gd)
    {
        var resizeBufferReactable = reactableFactory.CreateResizeBufferReactable();
        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        this.resizeBufferSubscriber = resizeBufferReactable.CreateOneWayReceive(PushNotifications.ResizeBufferId,
            data => EnsureCapacity(data.TotalShapeItems),
            () => this.resizeBufferSubscriber?.Dispose());

        this.wgpuReadyUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.WgpuReady,
            Initialize,
            () => this.wgpuReadyUnsubscriber?.Dispose());
    }

    /// <inheritdoc/>
    private protected override uint VertexSizeInBytes => RectVertexSize;

    /// <inheritdoc/>
    private protected override uint IndexItemSizeInBytes => RectIndexItemSize;

    /// <inheritdoc/>
    private protected override uint VerticesPerItem => VerticesPerRect;

    /// <inheritdoc/>
    private protected override uint IndicesPerItem => IndicesPerRect;

    /// <inheritdoc/>
    private protected override void BuildItemData(
        ShapeBatchItem item,
        uint itemIndex,
        out float[] vertexData,
        out uint[] indexData)
    {
        var centerX = item.Position.X;
        var centerY = item.Position.Y;
        var width = item.Width;
        var height = item.Height;

        var topLeft = item.CornerRadius.TopLeft;
        var topRight = item.CornerRadius.TopRight;
        var bottomRight = item.CornerRadius.BottomRight;
        var bottomLeft = item.CornerRadius.BottomLeft;

        // Per-vertex colors for gradient support.
        // Vertex layout: 0=top-left, 1=top-right, 2=bottom-left, 3=bottom-right.
        // The GPU interpolates between per-vertex colors to produce the gradient effect.
        var (vertexRed0, vertexGreen0, vertexBlue0, vertexAlpha0) = GetVertexColor(item, VertexPosition.TopLeft);
        var (vertexRed1, vertexGreen1, vertexBlue1, vertexAlpha1) = GetVertexColor(item, VertexPosition.TopRight);
        var (vertexRed2, vertexGreen2, vertexBlue2, vertexAlpha2) = GetVertexColor(item, VertexPosition.BottomLeft);
        var (vertexRed3, vertexGreen3, vertexBlue3, vertexAlpha3) = GetVertexColor(item, VertexPosition.BottomRight);

        var isFilled = item.IsSolid ? 1f : 0f;
        var borderThickness = item.BorderThickness;

        var left = centerX - item.HalfWidth;
        var right = centerX + item.HalfWidth;
        var top = centerY - item.HalfHeight;
        var bottom = centerY + item.HalfHeight;

        // 4 vertices × 16 floats each
        vertexData = new float[VerticesPerRect * 16];

        // Vertex 0: top-left
        var tlNdc = ToNDC(left, top);
        SetVertex(
            vertexData,
            0,
            tlNdc.X,
            tlNdc.Y,
            centerX,
            centerY,
            width,
            height,
            vertexRed0,
            vertexGreen0,
            vertexBlue0,
            vertexAlpha0,
            isFilled,
            borderThickness,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft);

        // Vertex 1: top-right
        var trNdc = ToNDC(right, top);
        SetVertex(
            vertexData,
            1,
            trNdc.X,
            trNdc.Y,
            centerX,
            centerY,
            width,
            height,
            vertexRed1,
            vertexGreen1,
            vertexBlue1,
            vertexAlpha1,
            isFilled,
            borderThickness,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft);

        // Vertex 2: bottom-left
        var blNdc = ToNDC(left, bottom);
        SetVertex(
            vertexData,
            2,
            blNdc.X,
            blNdc.Y,
            centerX,
            centerY,
            width,
            height,
            vertexRed2,
            vertexGreen2,
            vertexBlue2,
            vertexAlpha2,
            isFilled,
            borderThickness,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft);

        // Vertex 3: bottom-right
        var brNdc = ToNDC(right, bottom);
        SetVertex(
            vertexData,
            3,
            brNdc.X,
            brNdc.Y,
            centerX,
            centerY,
            width,
            height,
            vertexRed3,
            vertexGreen3,
            vertexBlue3,
            vertexAlpha3,
            isFilled,
            borderThickness,
            topLeft,
            topRight,
            bottomRight,
            bottomLeft);

        var baseV = itemIndex * VerticesPerRect;
        indexData =
        [
            baseV,
            baseV + 1,
            baseV + 2,
            baseV + 2,
            baseV + 1,
            baseV + 3,
        ];
    }

    /// <summary>
    /// Gets the RGBA color for the specified vertex, applying gradient logic.
    /// </summary>
    /// <param name="item">The batch item containing shape and color data.</param>
    /// <param name="vertexIndex">
    /// The vertex index: 0=top-left, 1=top-right, 2=bottom-left, 3=bottom-right.
    /// </param>
    /// <returns>A tuple of (R, G, B, A) as 0-255 floats for the vertex.</returns>
    private static (float R, float G, float B, float A) GetVertexColor(ShapeBatchItem item, VertexPosition vertexIndex) =>
#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
        item.GradientType switch
        {
            ColorGradient.None => (item.Color.R, item.Color.G, item.Color.B, item.Color.A),
            ColorGradient.Horizontal => vertexIndex switch
            {
                // top left or bottom left
                VertexPosition.TopLeft or VertexPosition.BottomLeft =>
                    (item.GradientStart.R, item.GradientStart.G, item.GradientStart.B, item.GradientStart.A),

                // top right or bottom right
                VertexPosition.TopRight or VertexPosition.BottomRight =>
                    (item.GradientStop.R, item.GradientStop.G, item.GradientStop.B, item.GradientStop.A),
            },
            ColorGradient.Vertical => vertexIndex switch
            {
                // top left or top right
                VertexPosition.TopLeft or VertexPosition.TopRight =>
                    (item.GradientStart.R, item.GradientStart.G, item.GradientStart.B, item.GradientStart.A),

                // bottom left or bottom right
                VertexPosition.BottomLeft or VertexPosition.BottomRight =>
                    (item.GradientStop.R, item.GradientStop.G, item.GradientStop.B, item.GradientStop.A),
            },
        };
#pragma warning restore CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.

    /// <summary>
    /// Writes one vertex (16 floats) into the flat vertex array.
    /// </summary>
    private static void SetVertex(float[] verts,
        uint vertexIndex,
        float posX,
        float posY,
        float centerX,
        float centerY,
        float width,
        float height,
        float r,
        float g,
        float b,
        float a,
        float isFilled,
        float borderThickness,
        float topLeft,
        float topRight,
        float bottomRight,
        float bottomLeft)
    {
        var o = vertexIndex * 16;

        verts[o + 0] = posX;
        verts[o + 1] = posY;
        verts[o + 2] = centerX;
        verts[o + 3] = centerY;
        verts[o + 4] = width;
        verts[o + 5] = height;
        verts[o + 6] = r;
        verts[o + 7] = g;
        verts[o + 8] = b;
        verts[o + 9] = a;
        verts[o + 10] = isFilled;
        verts[o + 11] = borderThickness;
        verts[o + 12] = topLeft;
        verts[o + 13] = topRight;
        verts[o + 14] = bottomRight;
        verts[o + 15] = bottomLeft;
    }
}

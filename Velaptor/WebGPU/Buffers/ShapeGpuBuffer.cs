// <copyright file="ShapeGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU.Buffers;

using System;
using System.Numerics;
using OpenGL.Batching;

/// <summary>
/// Manages GPU vertex and index buffers for rendering rounded rectangles.
/// Each batch item uses 4 vertices (64-byte stride) and 6 indices.
/// </summary>
/// <remarks>
/// <para>
/// Per-vertex layout (stride = 64 bytes = 16 × f32):
/// <code>
/// [ 0.. 1] position          vec2   offset  0   (NDC)
/// [ 2.. 5] shape             vec4   offset  8   (centerX, centerY, width, height in pixels)
/// [ 6.. 9] color             vec4   offset 24   (R, G, B, A as 0-255 floats)
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialCapacity">Number of rectangles to pre-allocate space for.</param>
    public ShapeGpuBuffer(GraphicsDevice gd, uint initialCapacity = 100)
        : base(gd, initialCapacity)
    {
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
        var w = item.Width;
        var h = item.Height;

        var tl = item.CornerRadius.TopLeft;
        var tr = item.CornerRadius.TopRight;
        var br = item.CornerRadius.BottomRight;
        var bl = item.CornerRadius.BottomLeft;

        var r = (float)item.Color.R;
        var gVal = (float)item.Color.G;
        var bVal = (float)item.Color.B;
        var a = (float)item.Color.A;

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
        SetVertex(vertexData, 0, tlNdc.X, tlNdc.Y, centerX, centerY, w, h, r, gVal, bVal, a, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 1: top-right
        var trNdc = ToNDC(right, top);
        SetVertex(vertexData, 1, trNdc.X, trNdc.Y, centerX, centerY, w, h, r, gVal, bVal, a, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 2: bottom-left
        var blNdc = ToNDC(left, bottom);
        SetVertex(vertexData, 2, blNdc.X, blNdc.Y, centerX, centerY, w, h, r, gVal, bVal, a, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 3: bottom-right
        var brNdc = ToNDC(right, bottom);
        SetVertex(vertexData, 3, brNdc.X, brNdc.Y, centerX, centerY, w, h, r, gVal, bVal, a, isFilled, borderThickness, tl, tr, br, bl);

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
    /// Writes one vertex (16 floats) into the flat vertex array.
    /// </summary>
    private static void SetVertex(float[] verts, uint vertexIndex,
        float posX, float posY,
        float centerX, float centerY, float width, float height,
        float r, float g, float b, float a,
        float isFilled, float borderThickness,
        float topLeft, float topRight, float bottomRight, float bottomLeft)
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

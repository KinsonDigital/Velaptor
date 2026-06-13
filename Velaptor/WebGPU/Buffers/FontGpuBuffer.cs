// <copyright file="FontGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU.Buffers;

using System;
using System.Numerics;
using Graphics;
using OpenGL.Batching;

/// <summary>
/// Manages GPU vertex and index buffers for rendering font glyph quads.
/// Each glyph uses 4 vertices (32-byte stride) and 6 indices forming two triangles.
/// Uses the same vertex layout as <see cref="TextureGpuBuffer"/>.
/// </summary>
internal sealed class FontGpuBuffer : WebGpuBufferBase<FontGlyphBatchItem>
{
    private const uint QuadVertexSize = 32; // 8 floats × 4 bytes
    private const uint QuadIndexItemSize = 4; // uint32
    private const uint QuadsPerItem = 4;
    private const uint IndicesPerQuad = 6;
    private const uint FloatsPerVertex = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialCapacity">Number of quads to pre-allocate space for.</param>
    public FontGpuBuffer(GraphicsDevice gd, uint initialCapacity = 100)
        : base(gd, initialCapacity)
    {
    }

    /// <inheritdoc/>
    private protected override uint VertexSizeInBytes => QuadVertexSize;

    /// <inheritdoc/>
    private protected override uint IndexItemSizeInBytes => QuadIndexItemSize;

    /// <inheritdoc/>
    private protected override uint VerticesPerItem => QuadsPerItem;

    /// <inheritdoc/>
    private protected override uint IndicesPerItem => IndicesPerQuad;

    /// <inheritdoc/>
    private protected override void BuildItemData(
        FontGlyphBatchItem item,
        uint itemIndex,
        out float[] vertexData,
        out uint[] indexData)
    {
        var center = new Vector2(item.DestRect.X, item.DestRect.Y);

        float srcRectWidth;
        float srcRectHeight;

        switch (item.Effects)
        {
            case RenderEffects.None:
                srcRectWidth = item.SrcRect.Width * -1;
                srcRectHeight = item.SrcRect.Height * -1;
                break;
            case RenderEffects.FlipHorizontally:
                srcRectWidth = item.SrcRect.Width;
                srcRectHeight = item.SrcRect.Height * -1;
                break;
            case RenderEffects.FlipVertically:
                srcRectWidth = item.SrcRect.Width * -1;
                srcRectHeight = item.SrcRect.Height;
                break;
            case RenderEffects.FlipBothDirections:
                srcRectWidth = item.SrcRect.Width;
                srcRectHeight = item.SrcRect.Height;
                break;
            default:
                srcRectWidth = 0;
                srcRectHeight = 0;
                break;
        }

        var resolvedSize = item.Size - 1f;
        var totalWidth = srcRectWidth + (srcRectWidth * resolvedSize);
        var totalHeight = srcRectHeight + (srcRectHeight * resolvedSize);

        var halfW = totalWidth / 2f;
        var halfH = totalHeight / 2f;

        var left = center.X - halfW;
        var bottom = center.Y + halfH;
        var right = center.X + halfW;
        var top = center.Y - halfH;

        var topLeft = new Vector2(left, top);
        var bottomLeft = new Vector2(left, bottom);
        var bottomRight = new Vector2(right, bottom);
        var topRight = new Vector2(right, top);

        var angle = item.Angle + 180;
        topLeft = RotateAround(topLeft, center, angle);
        bottomLeft = RotateAround(bottomLeft, center, angle);
        bottomRight = RotateAround(bottomRight, center, angle);
        topRight = RotateAround(topRight, center, angle);

        var tlNdc = ToNDC(topLeft.X, topLeft.Y);
        var blNdc = ToNDC(bottomLeft.X, bottomLeft.Y);
        var trNdc = ToNDC(topRight.X, topRight.Y);
        var brNdc = ToNDC(bottomRight.X, bottomRight.Y);

        var uLeft = item.SrcRect.Left / item.DestRect.Width;
        var uRight = item.SrcRect.Right / item.DestRect.Width;
        var vTop = item.SrcRect.Top / item.DestRect.Height;
        var vBottom = item.SrcRect.Bottom / item.DestRect.Height;

        switch (item.Effects)
        {
            case RenderEffects.FlipHorizontally:
                (uLeft, uRight) = (uRight, uLeft);
                break;
            case RenderEffects.FlipVertically:
                (vTop, vBottom) = (vBottom, vTop);
                break;
            case RenderEffects.FlipBothDirections:
                (uLeft, uRight) = (uRight, uLeft);
                (vTop, vBottom) = (vBottom, vTop);
                break;
        }

        var r = (float)item.TintColor.R;
        var g = (float)item.TintColor.G;
        var b = (float)item.TintColor.B;
        var a = (float)item.TintColor.A;

        vertexData = new float[QuadsPerItem * FloatsPerVertex];

        SetVertex(vertexData, 0, tlNdc.X, tlNdc.Y, uLeft, vTop, r, g, b, a);
        SetVertex(vertexData, 1, blNdc.X, blNdc.Y, uLeft, vBottom, r, g, b, a);
        SetVertex(vertexData, 2, trNdc.X, trNdc.Y, uRight, vTop, r, g, b, a);
        SetVertex(vertexData, 3, brNdc.X, brNdc.Y, uRight, vBottom, r, g, b, a);

        var baseV = itemIndex * QuadsPerItem;
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

    private static void SetVertex(
        float[] verts,
        uint vertexIndex,
        float posX, float posY,
        float u, float v,
        float r, float g, float b, float a)
    {
        var o = vertexIndex * FloatsPerVertex;
        verts[o + 0] = posX;
        verts[o + 1] = posY;
        verts[o + 2] = u;
        verts[o + 3] = v;
        verts[o + 4] = r;
        verts[o + 5] = g;
        verts[o + 6] = b;
        verts[o + 7] = a;
    }

    private static Vector2 RotateAround(Vector2 point, Vector2 origin, float angleDegrees)
    {
        var radians = angleDegrees * MathF.PI / 180f;
        var cos = MathF.Cos(radians);
        var sin = MathF.Sin(radians);
        var dx = point.X - origin.X;
        var dy = point.Y - origin.Y;

        return new Vector2(
            origin.X + (dx * cos) - (dy * sin),
            origin.Y + (dx * sin) + (dy * cos));
    }
}

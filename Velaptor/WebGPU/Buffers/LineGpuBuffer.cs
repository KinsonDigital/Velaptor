// <copyright file="LineGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGPU.Buffers;

using System;
using System.Linq;
using System.Numerics;
using ExtensionMethods;
using OpenGL.Batching;

/// <summary>
/// Manages GPU vertex and index buffers for rendering 2-D lines.
/// Lines are expanded to quads (rectangle from 2 endpoints) on the CPU,
/// then 4 NDC vertices + 6 indices are uploaded per batch item.
/// </summary>
/// <remarks>
/// <para>
/// Per-vertex layout (stride = 24 bytes = 6 × f32):
/// <code>
/// [0..1] position  vec2   offset  0   (NDC x, y)
/// [2..5] color     vec4   offset  8   (R, G, B, A as 0-255 floats)
/// </code>
/// </para>
/// </remarks>
internal sealed class LineGpuBuffer : WebGpuBufferBase<LineBatchItem>
{
    private const uint LineVertexSize = 24; // 6 floats × 4 bytes
    private const uint LineIndexItemSize = 4; // uint32
    private const uint VerticesPerLine = 4;
    private const uint IndicesPerLine = 6;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialCapacity">Number of lines to pre-allocate space for.</param>
    public LineGpuBuffer(GraphicsDevice gd, uint initialCapacity = 100)
        : base(gd, initialCapacity)
    {
    }

    /// <inheritdoc/>
    private protected override uint VertexSizeInBytes => LineVertexSize;

    /// <inheritdoc/>
    private protected override uint IndexItemSizeInBytes => LineIndexItemSize;

    /// <inheritdoc/>
    private protected override uint VerticesPerItem => VerticesPerLine;

    /// <inheritdoc/>
    private protected override uint IndicesPerItem => IndicesPerLine;

    /// <inheritdoc/>
    private protected override void BuildItemData(
        LineBatchItem item,
        uint itemIndex,
        out float[] vertexData,
        out uint[] indexData)
    {
        // Expand the 2-point line into a 4-corner rectangle
        var lineRectPoints = item.CreateRectFromLine().ToArray();

        var tlNdc = ToNDC(lineRectPoints[0].X, lineRectPoints[0].Y);
        var trNdc = ToNDC(lineRectPoints[1].X, lineRectPoints[1].Y);
        var brNdc = ToNDC(lineRectPoints[2].X, lineRectPoints[2].Y);
        var blNdc = ToNDC(lineRectPoints[3].X, lineRectPoints[3].Y);

        var r = (float)item.Color.R;
        var g = (float)item.Color.G;
        var b = (float)item.Color.B;
        var a = (float)item.Color.A;

        // 4 vertices × 6 floats each (posX, posY, r, g, b, a)
        vertexData = new float[VerticesPerLine * 6];

        SetVertex(vertexData, 0, tlNdc.X, tlNdc.Y, r, g, b, a);
        SetVertex(vertexData, 1, blNdc.X, blNdc.Y, r, g, b, a);
        SetVertex(vertexData, 2, trNdc.X, trNdc.Y, r, g, b, a);
        SetVertex(vertexData, 3, brNdc.X, brNdc.Y, r, g, b, a);

        var baseV = itemIndex * VerticesPerLine;
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
    /// Writes one vertex (6 floats) into the flat vertex array.
    /// </summary>
    private static void SetVertex(
        float[] verts,
        uint vertexIndex,
        float posX, float posY,
        float r, float g, float b, float a)
    {
        var o = vertexIndex * 6;
        verts[o + 0] = posX;
        verts[o + 1] = posY;
        verts[o + 2] = r;
        verts[o + 3] = g;
        verts[o + 4] = b;
        verts[o + 5] = a;
    }
}

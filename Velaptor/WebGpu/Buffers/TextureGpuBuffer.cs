// <copyright file="TextureGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using System.Numerics;
using Batching;
using Graphics;

/// <summary>
/// Manages GPU vertex and index buffers for rendering textured quads and font glyphs.
/// Each batch item uses 4 vertices (32-byte stride) and 6 indices forming two triangles.
/// </summary>
/// <remarks>
/// <para>
/// Per-vertex layout (stride = 32 bytes = 8 × f32):
/// <code>
/// [0..1] position   vec2   offset  0   (NDC x, y)
/// [2..3] uv         vec2   offset  8   (u, v in [0, 1])
/// [4..7] tintColor  vec4   offset 16   (R, G, B, A as 0-255 floats)
/// </code>
/// </para>
/// </remarks>
internal sealed class TextureGpuBuffer : WebGpuBufferBase<TextureBatchItem>
{
    private const uint QuadVertexSize = 32; // 8 floats × 4 bytes
    private const uint QuadIndexItemSize = 4; // uint32
    private const uint QuadsPerItem = 4;
    private const uint IndicesPerQuad = 6;
    private const uint FloatsPerVertex = 8;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialCapacity">Number of quads to pre-allocate space for.</param>
    public TextureGpuBuffer(IGraphicsDevice gd, uint initialCapacity = 100)
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
        TextureBatchItem item,
        uint itemIndex,
        out float[] vertexData,
        out uint[] indexData)
    {
        var center = new Vector2(item.DestRect.X, item.DestRect.Y);

        // Compute quad dimensions directly from srcRect and size (no sign-flip approach).
        // Flip effects are applied purely through UV swaps below — this avoids the
        // double-flip that occurred when sign-flipped vertices + 180° rotation + UV
        // swaps all interacted (the OpenGL approach doesn't translate to WebGPU).
        var scaledW = item.SrcRect.Width * item.Size;
        var scaledH = item.SrcRect.Height * item.Size;
        var halfW = scaledW / 2f;
        var halfH = scaledH / 2f;

        var left = center.X - halfW;
        var top = center.Y - halfH;
        var right = center.X + halfW;
        var bottom = center.Y + halfH;

        var topLeft = new Vector2(left, top);
        var topRight = new Vector2(right, top);
        var bottomLeft = new Vector2(left, bottom);
        var bottomRight = new Vector2(right, bottom);

        if (item.Angle != 0f)
        {
            topLeft = RotateAround(topLeft, center, item.Angle);
            topRight = RotateAround(topRight, center, item.Angle);
            bottomLeft = RotateAround(bottomLeft, center, item.Angle);
            bottomRight = RotateAround(bottomRight, center, item.Angle);
        }

        // Convert to NDC
        var tlNdc = ToNDC(topLeft.X, topLeft.Y);
        var blNdc = ToNDC(bottomLeft.X, bottomLeft.Y);
        var trNdc = ToNDC(topRight.X, topRight.Y);
        var brNdc = ToNDC(bottomRight.X, bottomRight.Y);

        // Source rect → UV [0, 1].
        // V is flipped because ImageService.Load flips every image vertically during load
        // (matches OpenGL's ToNDCTextureCoordY behavior).
        var uLeft = item.SrcRect.Left / item.DestRect.Width;
        var uRight = item.SrcRect.Right / item.DestRect.Width;
        var vTop = 1.0f - (item.SrcRect.Top / item.DestRect.Height);
        var vBottom = 1.0f - (item.SrcRect.Bottom / item.DestRect.Height);

        // Apply flip effects by swapping UV edges
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

        // Tint color as 0-255 floats
        var r = (float)item.TintColor.R;
        var g = (float)item.TintColor.G;
        var b = (float)item.TintColor.B;
        var a = (float)item.TintColor.A;

        // 4 vertices × 8 floats each
        vertexData = new float[QuadsPerItem * FloatsPerVertex];

        // Vertices: 0=topLeft, 1=topRight, 2=bottomLeft, 3=bottomRight
        SetVertex(vertexData, 0, tlNdc.X, tlNdc.Y, uLeft, vTop, r, g, b, a);
        SetVertex(vertexData, 1, trNdc.X, trNdc.Y, uRight, vTop, r, g, b, a);
        SetVertex(vertexData, 2, blNdc.X, blNdc.Y, uLeft, vBottom, r, g, b, a);
        SetVertex(vertexData, 3, brNdc.X, brNdc.Y, uRight, vBottom, r, g, b, a);

        // Two CCW triangles: (0,1,2) + (2,1,3) → (topLeft,topRight,bottomLeft) + (bottomLeft,topRight,bottomRight)
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

    /// <summary>
    /// Writes one vertex (8 floats) into the flat array.
    /// </summary>
    private static void SetVertex(float[] verts,
        uint vertexIndex,
        float posX,
        float posY,
        float u,
        float v,
        float r,
        float g,
        float b,
        float a)
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

    /// <summary>
    /// Rotates a point clockwise around an origin (same as Velaptor.GameHelpers.RotateAround).
    /// </summary>
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

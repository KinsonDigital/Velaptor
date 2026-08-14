// <copyright file="FontGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using System.Numerics;
using Batching;
using Carbonate;
using Carbonate.OneWay;
using Factories;
using Graphics;
using ReactableData;

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
    private readonly IDisposable resizeBufferSubscriber;
    private readonly IDisposable wgpuReadyUnsubscriber;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBuffer"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="reactableFactory">Creates reactables.</param>
    public FontGpuBuffer(IGraphicsDevice gd, IReactableFactory reactableFactory)
        : base(gd)
    {
        var resizeBufferReactable = reactableFactory.CreateResizeBufferReactable();
        var pushReactable = reactableFactory.CreateNoDataPushReactable();

        this.resizeBufferSubscriber = resizeBufferReactable.CreateOneWayReceive(PushNotifications.ResizeBufferId,
            data => EnsureCapacity(data.TotalFontItems),
            () => this.resizeBufferSubscriber?.Dispose());

        this.wgpuReadyUnsubscriber = pushReactable.CreateNonReceiveOrRespond(
            PushNotifications.WgpuReady,
            Initialize,
            () => this.wgpuReadyUnsubscriber?.Dispose());
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
        // Match OpenGL's UploadVertexData positioning logic:
        // DestRect is the bottom-left anchor of the quad, NOT the center.
        // The quad extends right by SrcRect.Width and upward by SrcRect.Height.
        //
        // Glyph metrics are pre-scaled by the renderer (ApplySize), so the
        // buffer uses SrcRect values as-is without additional size adjustment.
        var left = item.DestRect.X;
        var bottom = item.DestRect.Y + item.SrcRect.Height;
        var right = item.DestRect.X + item.SrcRect.Width;
        var top = item.DestRect.Y;

        var topLeft = new Vector2(left, top);
        var bottomLeft = new Vector2(left, bottom);
        var bottomRight = new Vector2(right, bottom);
        var topRight = new Vector2(right, top);

        // Shift all Y coordinates upward by the glyph height (matching OpenGL).
        // This compensates for the pixel-space Y-down convention so that
        // DestRect.Y becomes the bottom edge in NDC space.
        var quadH = item.SrcRect.Height;
        topLeft.Y -= quadH;
        bottomLeft.Y -= quadH;
        bottomRight.Y -= quadH;
        topRight.Y -= quadH;

        // Rotate around the quad's origin (top-left corner of DestRect)
        var origin = new Vector2(item.DestRect.X, item.DestRect.Y);
        var angle = item.Angle;

        topLeft = RotateAround(topLeft, origin, angle);
        bottomLeft = RotateAround(bottomLeft, origin, angle);
        bottomRight = RotateAround(bottomRight, origin, angle);
        topRight = RotateAround(topRight, origin, angle);

        var tlNdc = ToNDC(topLeft.X, topLeft.Y);
        var blNdc = ToNDC(bottomLeft.X, bottomLeft.Y);
        var trNdc = ToNDC(topRight.X, topRight.Y);
        var brNdc = ToNDC(bottomRight.X, bottomRight.Y);

        // Font atlas images are vertically flipped during loading (see FontLoader.FlipVertically).
        // V coordinates must be inverted to compensate: the original top of the glyph
        // (lower V in screen space) maps to a higher UV value in the flipped texture.
        // This mirrors OpenGL's ToNDCTextureCoordY: y.MapValue(0f, textureHeight, 1f, 0f).
        var uLeft = item.SrcRect.Left / item.DestRect.Width;
        var uRight = item.SrcRect.Right / item.DestRect.Width;
        var vTop = 1.0f - (item.SrcRect.Top / item.DestRect.Height);
        var vBottom = 1.0f - (item.SrcRect.Bottom / item.DestRect.Height);

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

    /// <summary>
    /// Sets the given vertex data at the given vertex at the <paramref name="vertexIndex"/>.
    /// </summary>
    /// <param name="verts">The vertices.</param>
    /// <param name="vertexIndex">The index of the vertex in the vertices array.</param>
    /// <param name="posX">The X position.</param>
    /// <param name="posY">The Y position.</param>
    /// <param name="u">The horizontal texture coordinate (0–1) mapping to the glyph region in the font atlas.</param>
    /// <param name="v">The vertical texture coordinate (0–1) mapping to the glyph region in the font atlas.</param>
    /// <param name="r">The red color component.</param>
    /// <param name="g">The green color component.</param>
    /// <param name="b">The blue color component.</param>
    /// <param name="a">The alpha color component.</param>
    private static void SetVertex(
        float[] verts,
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
    /// Rotates the given <paramref name="point"/> around the given <paramref name="origin"/> by the given <paramref name="angleDegrees"/>.
    /// </summary>
    /// <param name="point">The point to rotate.</param>
    /// <param name="origin">The origin to rotate the <paramref name="point"/> around.</param>
    /// <param name="angleDegrees">The angle of rotation.</param>
    /// <returns>The new rotated point.</returns>
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

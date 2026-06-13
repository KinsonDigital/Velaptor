// <copyright file="GraphicsRectBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Drawing;
using System.Numerics;
using Handles;
using Silk.NET.WebGPU;

/// <summary>
/// Manages the GPU vertex buffer and index buffer used to render rounded
/// rectangles. Provides methods to upload a <see cref="RectShape"/> to the
/// GPU and bind the buffers for drawing.
/// </summary>
/// <remarks>
/// <para>
/// Each rectangle is represented by <b>4 vertices</b> (64 bytes each, 9 attributes)
/// and <b>6 indices</b> (2 triangles forming a quad). Vertex positions are
/// pre-transformed to NDC on the CPU — no camera matrix is needed.
/// </para>
/// <para>
/// The bounding box (center, width, height) is uploaded in <b>pixel</b> coordinates
/// so the fragment shader can use <c>@builtin(position)</c> for per-pixel corner
/// math, just like Velaptor's <c>gl_FragCoord</c>.
/// </para>
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
internal sealed class GraphicsRectBuffer : IDisposable
{
    private const uint VertexSizeInBytes = 64;  // 16 floats × 4 bytes
    private const uint IndexItemSizeInBytes = 4;  // uint32
    private const uint VerticesPerRect = 4;
    private const uint IndicesPerRect = 6;

    private readonly GraphicsDevice gd;
    private SafeVertexBufferHandle vertexBuffer;
    private SafeIndexBufferHandle indexBuffer;
    private uint vertexBufferSizeInBytes;
    private uint indexBufferSizeInBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsRectBuffer"/> class.
    /// Pre-allocates GPU buffers for the given number of rectangles.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialRectCount">Number of rectangles to allocate space for.</param>
    public GraphicsRectBuffer(GraphicsDevice gd, uint initialRectCount = 64)
    {
        this.gd = gd;
        Capacity = 0;
        Allocate(initialRectCount);
    }

    /// <summary>
    /// Gets the number of rectangles the current GPU buffers can hold.
    /// </summary>
    public uint Capacity { get; private set; }

    /// <summary>
    /// Gets or sets the current window size in pixels for NDC calculations.
    /// </summary>
    public Vector2 WindowSize { get; set; } = new (800f, 600f);

    /// <summary>
    /// Binds the vertex buffer and index buffer, then issues an indexed draw
    /// call for <paramref name="rectCount"/> rectangles.
    /// </summary>
    /// <param name="pass">The active render pass encoder.</param>
    /// <param name="rectCount">Number of rectangles to draw.</param>
    /// <param name="firstRect">Index of the first rectangle in the buffer.</param>
    public void Draw(SafeRenderPassEncoderHandle pass, uint rectCount = 1, uint firstRect = 0)
    {
        this.gd.Wgpu.RenderPassEncoderSetVertexBuffer(
            pass, 0,
            this.vertexBuffer.DangerousGetHandle(),
            0, this.vertexBufferSizeInBytes);

        this.gd.Wgpu.RenderPassEncoderSetIndexBuffer(
            pass,
            this.indexBuffer.DangerousGetHandle(),
            IndexFormat.Uint32, 0, this.indexBufferSizeInBytes);

        this.gd.Wgpu.RenderPassEncoderDrawIndexed(
            pass,
            indexCount: IndicesPerRect * rectCount,
            instanceCount: 1,
            firstIndex: IndicesPerRect * firstRect,
            baseVertex: 0,
            firstInstance: 0);
    }

    /// <summary>
    /// Uploads a single <see cref="RectShape"/> to the GPU vertex and index buffers.
    /// The rectangle is written at slot <paramref name="rectIndex"/>.
    /// </summary>
    /// <param name="rect">The rectangle to upload.</param>
    /// <param name="rectIndex">Which rectangle slot to write to (0-based).</param>
    /// <remarks>
    /// Re-allocates the GPU buffers if <paramref name="rectIndex"/> exceeds capacity.
    /// </remarks>
    public void Upload(RectShape rect, uint rectIndex = 0)
    {
        if (rectIndex >= Capacity)
        {
            Allocate(rectIndex + 1);
        }

        BuildShapeData(rect, rectIndex, out var vertexData, out var indexData);
        UploadToGpu(vertexData, indexData, rectIndex);
    }

    /// <summary>
    /// Releases the vertex and index buffers.
    /// </summary>
    public void Dispose()
    {
        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();
    }

    /// <summary>
    /// Converts a <see cref="RectShape"/> to packed vertex and index arrays.
    /// </summary>
    /// <param name="rect">The rectangle to convert.</param>
    /// <param name="rectIndex">Slot index (affects generated indices).</param>
    /// <param name="vertexData">Output: packed float array of vertex data.</param>
    /// <param name="indexData">Output: uint array of index data.</param>
    private void BuildShapeData(RectShape rect, uint rectIndex, out float[] vertexData, out uint[] indexData)
    {
        var centerX = rect.Position.X;
        var centerY = rect.Position.Y;
        var w = rect.Width;
        var h = rect.Height;

        var tl = rect.CornerRadius.TopLeft;
        var tr = rect.CornerRadius.TopRight;
        var br = rect.CornerRadius.BottomRight;
        var bl = rect.CornerRadius.BottomLeft;

        var r = (float)rect.Color.R;
        var g = (float)rect.Color.G;
        var b = (float)rect.Color.B;
        var aVal = (float)rect.Color.A;

        var isFilled = rect.IsSolid ? 1f : 0f;
        var borderThickness = rect.BorderThickness;

        var halfW = rect.HalfWidth;
        var halfH = rect.HalfHeight;

        var left = centerX - halfW;
        var right = centerX + halfW;
        var top = centerY - halfH;
        var bottom = centerY + halfH;

        // 4 vertices × 16 floats each
        vertexData = new float[VerticesPerRect * 16];

        // Vertex 0: bottom-left
        var blNdc = ToNDC(left, bottom);
        SetVertex(vertexData, 0, blNdc.X, blNdc.Y, centerX, centerY, w, h, r, g, b, aVal, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 1: bottom-right
        var brNdc = ToNDC(right, bottom);
        SetVertex(vertexData, 1, brNdc.X, brNdc.Y, centerX, centerY, w, h, r, g, b, aVal, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 2: top-left
        var tlNdc = ToNDC(left, top);
        SetVertex(vertexData, 2, tlNdc.X, tlNdc.Y, centerX, centerY, w, h, r, g, b, aVal, isFilled, borderThickness, tl, tr, br, bl);

        // Vertex 3: top-right
        var trNdc = ToNDC(right, top);
        SetVertex(vertexData, 3, trNdc.X, trNdc.Y, centerX, centerY, w, h, r, g, b, aVal, isFilled, borderThickness, tl, tr, br, bl);

        // Indices: 2 triangles forming a quad
        var baseV = rectIndex * VerticesPerRect;
        indexData = new uint[6]
        {
            baseV,          // bottom-left
            baseV + 1,      // bottom-right
            baseV + 2,      // top-left
            baseV + 2,      // top-left (reused)
            baseV + 1,      // bottom-right (reused)
            baseV + 3,      // top-right
        };
    }

    /// <summary>
    /// Writes one vertex (16 floats) into the flat vertex array at the given vertex index.
    /// </summary>
    private static void SetVertex(float[] verts, uint vertexIndex,
        float posX, float posY,
        float centerX, float centerY, float width, float height,
        float r, float g, float b, float a,
        float isFilled, float borderThickness,
        float topLeft, float topRight, float bottomRight, float bottomLeft)
    {
        var o = vertexIndex * 16;

        verts[o + 0]  = posX;            // position.x
        verts[o + 1]  = posY;            // position.y
        verts[o + 2]  = centerX;         // shape.x
        verts[o + 3]  = centerY;         // shape.y
        verts[o + 4]  = width;           // shape.z
        verts[o + 5]  = height;          // shape.w
        verts[o + 6]  = r;               // color.r
        verts[o + 7]  = g;               // color.g
        verts[o + 8]  = b;               // color.b
        verts[o + 9]  = a;               // color.a
        verts[o + 10] = isFilled;        // isFilled
        verts[o + 11] = borderThickness; // borderThickness
        verts[o + 12] = topLeft;         // topLeftRadius
        verts[o + 13] = topRight;        // topRightRadius
        verts[o + 14] = bottomRight;     // bottomRightRadius
        verts[o + 15] = bottomLeft;      // bottomLeftRadius
    }

    /// <summary>
    /// Converts pixel coordinates to WebGPU NDC (-1 to +1).
    /// WebGPU clip space is Y-up: NDC Y=+1 is the top of the screen, NDC Y=-1 is the bottom.
    /// Pixel Y=0 is the top of the screen, so the Y range must be inverted: [0, screenH] → [+1, -1].
    /// </summary>
    private Vector2 ToNDC(float pixelX, float pixelY)
    {
        var screenW = WindowSize.X;
        var screenH = WindowSize.Y;

        return new Vector2(
            MapValue(pixelX, 0f, screenW, -1f, 1f),
            MapValue(pixelY, 0f, screenH, 1f, -1f));
    }

    private static float MapValue(float value, float fromStart, float fromStop, float toStart, float toStop)
        => toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));

    private void Allocate(uint minRectCount)
    {
        var newCapacity = Math.Max(minRectCount, Capacity > 0 ? Capacity * 2 : 64);
        Capacity = newCapacity;

        this.vertexBufferSizeInBytes = Capacity * VerticesPerRect * VertexSizeInBytes;
        this.indexBufferSizeInBytes = Capacity * IndicesPerRect * IndexItemSizeInBytes;

        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();

        unsafe
        {
            ReadOnlySpan<byte> vbLabel = "Rect Vertex Buffer"u8;

            fixed (byte* vbLabelPtr = vbLabel)
            {
                var vbDesc = new BufferDescriptor
                {
                    Label = vbLabelPtr,
                    Size = this.vertexBufferSizeInBytes,
                    Usage = BufferUsage.Vertex | BufferUsage.CopyDst,
                    MappedAtCreation = false,
                };

                this.vertexBuffer = new SafeVertexBufferHandle(this.gd.Wgpu, this.gd.Handle, ref vbDesc);
            }

            ReadOnlySpan<byte> ibLabel = "Rect Index Buffer"u8;

            fixed (byte* ibLabelPtr = ibLabel)
            {
                var ibDesc = new BufferDescriptor
                {
                    Label = ibLabelPtr,
                    Size = this.indexBufferSizeInBytes,
                    Usage = BufferUsage.Index | BufferUsage.CopyDst,
                    MappedAtCreation = false,
                };

                this.indexBuffer = new SafeIndexBufferHandle(this.gd.Wgpu, this.gd.Handle, ref ibDesc);
            }
        }
    }

    private unsafe void UploadToGpu(float[] vertexData, uint[] indexData, uint rectIndex)
    {
        var vbOffset = rectIndex * VerticesPerRect * VertexSizeInBytes;

        fixed (float* vbPtr = vertexData)
        {
            this.gd.Wgpu.QueueWriteBuffer(
                this.gd.Queue,
                this.vertexBuffer.DangerousGetHandle(),
                vbOffset,
                (nint)vbPtr,
                (nuint)(vertexData.Length * sizeof(float)));
        }

        var ibOffset = rectIndex * IndicesPerRect * IndexItemSizeInBytes;

        fixed (uint* ibPtr = indexData)
        {
            this.gd.Wgpu.QueueWriteBuffer(
                this.gd.Queue,
                this.indexBuffer.DangerousGetHandle(),
                ibOffset,
                (nint)ibPtr,
                (nuint)(indexData.Length * sizeof(uint)));
        }
    }
}

// <copyright file="GraphicsTextureBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace csharp_webgpu;

using System.Drawing;
using System.Numerics;
using Handles;
using Silk.NET.WebGPU;

/// <summary>
/// Manages GPU vertex and index buffers for rendering textured quads with support for
/// position, clockwise rotation, scale, tint color, flip effects, and partial source rectangles —
/// matching the feature set of <c>Velaptor.Graphics.Renderers.ITextureRenderer</c>.
/// </summary>
/// <remarks>
/// <para>
/// Each quad uses <b>4 vertices</b> and <b>6 indices</b> (two triangles).
/// Vertex positions are computed in screen pixel coordinates (top-left origin, Y-down) on the
/// CPU, then converted to NDC before upload — the same approach used by
/// <c>Velaptor.OpenGL.Buffers.TextureGpuBuffer</c>.
/// </para>
/// <para>
/// Per-vertex layout (stride = 32 bytes = 8 × f32):
/// <code>
/// [0..1] position   vec2   offset  0   (NDC x, y)
/// [2..3] uv         vec2   offset  8   (u, v in [0, 1])
/// [4..7] tintColor  vec4   offset 16   (R, G, B, A as 0-255 floats)
/// </code>
/// </para>
/// </remarks>
internal sealed class GraphicsTextureBuffer : IDisposable
{
    private const uint VertexSizeInBytes = 32;   // 8 floats × 4 bytes
    private const uint IndexItemSizeInBytes = 4;  // uint32
    private const uint VerticesPerQuad = 4;
    private const uint IndicesPerQuad = 6;
    private const uint FloatsPerVertex = 8;

    private readonly GraphicsDevice gd;
    private SafeVertexBufferHandle vertexBuffer;
    private SafeIndexBufferHandle indexBuffer;
    private uint vertexBufferSizeInBytes;
    private uint indexBufferSizeInBytes;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsTextureBuffer"/> class.
    /// Pre-allocates GPU buffers for the given number of quads.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialQuadCount">Number of quads to pre-allocate space for.</param>
    public GraphicsTextureBuffer(GraphicsDevice gd, uint initialQuadCount = 64)
    {
        this.gd = gd;
        Capacity = 0;
        Allocate(initialQuadCount);
    }

    /// <summary>
    /// Gets the number of quads the current GPU buffers can hold.
    /// </summary>
    public uint Capacity { get; private set; }

    /// <summary>
    /// Gets or sets the window size in pixels used for NDC conversion.
    /// Update this and re-upload quads after a window resize.
    /// </summary>
    public Vector2 WindowSize { get; set; } = new(800f, 600f);

    /// <summary>
    /// Uploads a single <see cref="TextureQuad"/> to the GPU at <paramref name="quadIndex"/>.
    /// Grows the buffer automatically if the index exceeds current capacity.
    /// </summary>
    /// <param name="quad">The quad data: position, size, angle, tint, effects, source rect.</param>
    /// <param name="quadIndex">Zero-based slot in the GPU buffer to write to.</param>
    public void Upload(TextureQuad quad, uint quadIndex = 0)
    {
        if (quadIndex >= Capacity)
        {
            Allocate(quadIndex + 1);
        }

        BuildQuadData(quad, quadIndex, out var vertexData, out var indexData);
        UploadToGpu(vertexData, indexData, quadIndex);
    }

    /// <summary>
    /// Binds the vertex and index buffers to the active render pass and issues an indexed draw call.
    /// </summary>
    /// <param name="pass">The active render pass encoder.</param>
    /// <param name="quadCount">Number of quads to draw.</param>
    /// <param name="firstQuad">Index of the first quad to draw.</param>
    public void Draw(SafeRenderPassEncoderHandle pass, uint quadCount = 1, uint firstQuad = 0)
    {
        unsafe
        {
            var encoder = (RenderPassEncoder*)pass.DangerousGetHandle();

            this.gd.Wgpu.RenderPassEncoderSetVertexBuffer(
                encoder, 0,
                (Buffer*)this.vertexBuffer.DangerousGetHandle(),
                0, this.vertexBufferSizeInBytes);

            this.gd.Wgpu.RenderPassEncoderSetIndexBuffer(
                encoder,
                (Buffer*)this.indexBuffer.DangerousGetHandle(),
                IndexFormat.Uint32, 0, this.indexBufferSizeInBytes);

            this.gd.Wgpu.RenderPassEncoderDrawIndexed(
                encoder,
                indexCount: IndicesPerQuad * quadCount,
                instanceCount: 1,
                firstIndex: IndicesPerQuad * firstQuad,
                baseVertex: 0,
                firstInstance: 0);
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();
    }

    /// <summary>
    /// Converts a <see cref="TextureQuad"/> into packed vertex and index arrays.
    /// Mirrors the algorithm in <c>Velaptor.OpenGL.Buffers.TextureGpuBuffer.UploadVertexData</c>.
    /// </summary>
    private void BuildQuadData(
        TextureQuad quad,
        uint quadIndex,
        out float[] vertexData,
        out uint[] indexData)
    {
        var center = quad.Position;

        // Resolve the source rect: empty → use the full texture.
        var src = quad.SrcRect.IsEmpty
            ? new RectangleF(0, 0, quad.Width, quad.Height)
            : quad.SrcRect;

        // Rendered quad dimensions after scaling.
        var scaledW = src.Width  * quad.Size;
        var scaledH = src.Height * quad.Size;
        var halfW = scaledW / 2f;
        var halfH = scaledH / 2f;

        // Compute the four corner positions in screen pixel space (Y-down, top-left origin).
        var topLeft     = new Vector2(center.X - halfW, center.Y - halfH);
        var topRight    = new Vector2(center.X + halfW, center.Y - halfH);
        var bottomLeft  = new Vector2(center.X - halfW, center.Y + halfH);
        var bottomRight = new Vector2(center.X + halfW, center.Y + halfH);

        // Rotate corners clockwise around the center if a non-zero angle is set.
        if (quad.Angle != 0f)
        {
            topLeft     = RotateAround(topLeft,     center, quad.Angle);
            topRight    = RotateAround(topRight,    center, quad.Angle);
            bottomLeft  = RotateAround(bottomLeft,  center, quad.Angle);
            bottomRight = RotateAround(bottomRight, center, quad.Angle);
        }

        // Convert to WebGPU NDC: X [-1,+1] left→right, Y [-1,+1] bottom→top.
        var tlNdc = ToNDC(topLeft.X,     topLeft.Y);
        var trNdc = ToNDC(topRight.X,    topRight.Y);
        var blNdc = ToNDC(bottomLeft.X,  bottomLeft.Y);
        var brNdc = ToNDC(bottomRight.X, bottomRight.Y);

        // Normalise the source rect pixel coords into UV space [0, 1].
        // WebGPU UV convention: U=0 left, U=1 right, V=0 top, V=1 bottom.
        var uLeft   = src.Left   / quad.Width;
        var uRight  = src.Right  / quad.Width;
        var vTop    = src.Top    / quad.Height;
        var vBottom = src.Bottom / quad.Height;

        // Apply flip effects by swapping UV edges (mirrors Velaptor's sign-flip approach).
        switch (quad.Effects)
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

        // Tint color as 0-255 floats; the fragment shader normalises to [0, 1].
        var r = (float)quad.TintColor.R;
        var g = (float)quad.TintColor.G;
        var b = (float)quad.TintColor.B;
        var a = (float)quad.TintColor.A;

        // 4 vertices × 8 floats each (posX, posY, u, v, r, g, b, a).
        vertexData = new float[VerticesPerQuad * FloatsPerVertex];

        SetVertex(vertexData, 0, tlNdc.X, tlNdc.Y, uLeft,  vTop,    r, g, b, a); // top-left
        SetVertex(vertexData, 1, trNdc.X, trNdc.Y, uRight, vTop,    r, g, b, a); // top-right
        SetVertex(vertexData, 2, blNdc.X, blNdc.Y, uLeft,  vBottom, r, g, b, a); // bottom-left
        SetVertex(vertexData, 3, brNdc.X, brNdc.Y, uRight, vBottom, r, g, b, a); // bottom-right

        var baseV = quadIndex * VerticesPerQuad;
        indexData = new uint[6]
        {
            baseV,      // top-left
            baseV + 1,  // top-right
            baseV + 2,  // bottom-left
            baseV + 2,  // bottom-left  (reused)
            baseV + 1,  // top-right    (reused)
            baseV + 3,  // bottom-right
        };
    }

    /// <summary>
    /// Writes one vertex (8 floats) into the flat array at the given vertex index.
    /// </summary>
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

    /// <summary>
    /// Rotates <paramref name="point"/> clockwise around <paramref name="origin"/> in screen
    /// pixel space (Y-down). Mirrors <c>Velaptor.GameHelpers.RotateAround</c>.
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

    /// <summary>
    /// Converts screen pixel coordinates to WebGPU NDC.
    /// WebGPU clip space is Y-up (NDC Y=+1 = top of screen), so Y is inverted.
    /// Mirrors <c>Velaptor.OpenGL.OpenGLExtensionMethods.ToNDC</c>.
    /// </summary>
    private Vector2 ToNDC(float pixelX, float pixelY)
    {
        var screenW = WindowSize.X;
        var screenH = WindowSize.Y;

        return new Vector2(
            MapValue(pixelX, 0f, screenW, -1f,  1f),
            MapValue(pixelY, 0f, screenH,  1f, -1f));
    }

    private static float MapValue(float value, float fromStart, float fromStop, float toStart, float toStop)
        => toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));

    private void Allocate(uint minQuadCount)
    {
        var newCapacity = Math.Max(minQuadCount, Capacity > 0 ? Capacity * 2 : 64);
        Capacity = newCapacity;

        this.vertexBufferSizeInBytes = Capacity * VerticesPerQuad * VertexSizeInBytes;
        this.indexBufferSizeInBytes  = Capacity * IndicesPerQuad  * IndexItemSizeInBytes;

        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();

        unsafe
        {
            ReadOnlySpan<byte> vbLabel = "Texture Vertex Buffer"u8;

            fixed (byte* vbLabelPtr = vbLabel)
            {
                var vbDesc = new BufferDescriptor
                {
                    Label = vbLabelPtr,
                    Size = this.vertexBufferSizeInBytes,
                    Usage = BufferUsage.Vertex | BufferUsage.CopyDst,
                    MappedAtCreation = false,
                };

                this.vertexBuffer = new SafeVertexBufferHandle(this.gd.Wgpu, this.gd.Handle, vbDesc);
            }

            ReadOnlySpan<byte> ibLabel = "Texture Index Buffer"u8;

            fixed (byte* ibLabelPtr = ibLabel)
            {
                var ibDesc = new BufferDescriptor
                {
                    Label = ibLabelPtr,
                    Size = this.indexBufferSizeInBytes,
                    Usage = BufferUsage.Index | BufferUsage.CopyDst,
                    MappedAtCreation = false,
                };

                this.indexBuffer = new SafeIndexBufferHandle(this.gd.Wgpu, this.gd.Handle, ibDesc);
            }
        }
    }

    private unsafe void UploadToGpu(float[] vertexData, uint[] indexData, uint quadIndex)
    {
        var vbOffset = quadIndex * VerticesPerQuad * VertexSizeInBytes;

        fixed (float* vbPtr = vertexData)
        {
            this.gd.Wgpu.QueueWriteBuffer(
                (Queue*)this.gd.Queue.DangerousGetHandle(),
                (Buffer*)this.vertexBuffer.DangerousGetHandle(),
                vbOffset,
                vbPtr,
                (nuint)(vertexData.Length * sizeof(float)));
        }

        var ibOffset = quadIndex * IndicesPerQuad * IndexItemSizeInBytes;

        fixed (uint* ibPtr = indexData)
        {
            this.gd.Wgpu.QueueWriteBuffer(
                (Queue*)this.gd.Queue.DangerousGetHandle(),
                (Buffer*)this.indexBuffer.DangerousGetHandle(),
                ibOffset,
                ibPtr,
                (nuint)(indexData.Length * sizeof(uint)));
        }
    }
}

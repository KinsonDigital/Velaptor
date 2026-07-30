// <copyright file="WebGpuBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using System.Numerics;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;

/// <summary>
/// Base functionality for WebGPU vertex/index buffer management.
/// Handles allocation, resize, and draw call dispatch for batched quad rendering.
/// </summary>
/// <typeparam name="TData">The batch item struct type.</typeparam>
internal abstract class WebGpuBufferBase<TData> : IDisposable
    where TData : struct
{
    private const uint DefaultCapacity = 64;
    private readonly IGraphicsDevice gd;
    private readonly uint pendingInitialCapacity;
    private SafeVertexBufferHandle? vertexBuffer;
    private SafeIndexBufferHandle? indexBuffer;
    private uint vertexBufferSizeInBytes;
    private uint indexBufferSizeInBytes;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebGpuBufferBase{TData}"/> class.
    /// </summary>
    /// <param name="gd">The graphics device.</param>
    /// <param name="initialCapacity">Number of batch items to pre-allocate space for.</param>
    private protected WebGpuBufferBase(IGraphicsDevice gd, uint initialCapacity = DefaultCapacity)
    {
        this.gd = gd;
        Capacity = 0;
        this.pendingInitialCapacity = initialCapacity;
    }

    /// <summary>
    /// Gets a value indicating whether the GPU buffers have been allocated.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public bool IsInitialized => Capacity > 0;

    /// <summary>
    /// Gets the number of batch items the current GPU buffers can hold.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public uint Capacity { get; private set; }

    /// <summary>
    /// Gets or sets the window size in pixels used for NDC conversion.
    /// </summary>
    public Vector2 WindowSize { get; set; } = new (800f, 600f);

    /// <summary>
    /// Gets the size in bytes of a single vertex.
    /// </summary>
    private protected abstract uint VertexSizeInBytes { get; }

    /// <summary>
    /// Gets the size in bytes of a single index element.
    /// </summary>
    private protected abstract uint IndexItemSizeInBytes { get; }

    /// <summary>
    /// Gets the number of vertices per batch item (quad = 4).
    /// </summary>
    private protected abstract uint VerticesPerItem { get; }

    /// <summary>
    /// Gets the number of indices per batch item (quad = 6).
    /// </summary>
    private protected abstract uint IndicesPerItem { get; }

    /// <summary>
    /// Gets the graphics device invoker for WebGPU calls.
    /// </summary>
    private IWgpuInvoker Wgpu => this.gd.Wgpu;

    /// <summary>
    /// Gets the graphics device handle.
    /// </summary>
    private SafeDeviceHandle Device => this.gd.Handle ?? throw new InvalidOperationException("GraphicsDevice not initialized.");

    /// <summary>
    /// Gets the device queue handle.
    /// </summary>
    private SafeQueueHandle Queue => this.gd.Queue ?? throw new InvalidOperationException("GraphicsDevice queue not initialized.");

    /// <summary>
    /// Ensures the GPU buffers can hold at least <paramref name="itemCount"/> items,
    /// re-allocating if necessary. Call this before starting an upload loop to avoid
    /// mid-loop resizes that would invalidate previously recorded draw commands.
    /// </summary>
    /// <param name="itemCount">The minimum number of batch items the buffer must support.</param>
    public void EnsureCapacity(uint itemCount)
    {
        EnsureInitialized();

        if (itemCount > Capacity)
        {
            Allocate(itemCount);
        }
    }

    /// <summary>
    /// Allocates GPU vertex and index buffers. Must be called after the WebGPU device
    /// has been initialized.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    public void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        Allocate(this.pendingInitialCapacity);
    }

    /// <summary>
    /// Uploads a single batch item to the GPU at <paramref name="itemIndex"/>.
    /// Grows the buffer automatically if the index exceeds current capacity.
    /// </summary>
    /// <param name="data">The batch item data to upload.</param>
    /// <param name="itemIndex">Zero-based slot in the GPU buffer to write to.</param>
    public void UploadData(TData data, uint itemIndex = 0)
    {
        EnsureInitialized();

        if (itemIndex >= Capacity)
        {
            Allocate(itemIndex + 1);
        }

        BuildItemData(data, itemIndex, out var vertexData, out var indexData);
        UploadToGpu(vertexData, indexData, itemIndex);
    }

    /// <summary>
    /// Binds the vertex and index buffers to the active render pass and issues an indexed draw call.
    /// </summary>
    /// <param name="pass">The active render pass encoder.</param>
    /// <param name="itemCount">Number of batch items to draw.</param>
    /// <param name="firstItem">Index of the first batch item to draw.</param>
    public void Draw(SafeRenderPassEncoderHandle pass, uint itemCount = 1, uint firstItem = 0)
    {
        if (this.vertexBuffer is null)
        {
            throw new Exception("The vertex buffer cannot be null.");
        }

        if (this.indexBuffer is null)
        {
            throw new Exception("The vertex index buffer cannot be null.");
        }

        EnsureInitialized();
        this.gd.Wgpu.RenderPassEncoderSetVertexBuffer(
            pass,
            0,
            this.vertexBuffer.DangerousGetHandle(),
            0,
            this.vertexBufferSizeInBytes);

        this.gd.Wgpu.RenderPassEncoderSetIndexBuffer(
            pass,
            this.indexBuffer.DangerousGetHandle(),
            Silk.NET.WebGPU.IndexFormat.Uint32,
            0,
            this.indexBufferSizeInBytes);

        this.gd.Wgpu.RenderPassEncoderDrawIndexed(
            pass,
            indexCount: IndicesPerItem * itemCount,
            instanceCount: 1,
            firstIndex: IndicesPerItem * firstItem,
            baseVertex: 0,
            firstInstance: 0);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (this.isDisposed)
        {
            return;
        }

        this.isDisposed = true;
        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Converts screen pixel coordinates to WebGPU NDC.
    /// WebGPU clip space is Y-up (NDC Y=+1 = top of screen), so Y is inverted
    /// from pixel space (Y=0 = top).
    /// </summary>
    /// <param name="pixelX">The X location of the pixel.</param>
    /// <param name="pixelY">The Y location of the pixel.</param>
    /// <returns>The Vector2 position result of the X and Y pixel.</returns>
    protected Vector2 ToNDC(float pixelX, float pixelY)
    {
        var screenW = WindowSize.X;
        var screenH = WindowSize.Y;

        return new Vector2(
            MapValue(pixelX, 0f, screenW, -1f, 1f),
            MapValue(pixelY, 0f, screenH, 1f, -1f));
    }

    /// <summary>
    /// Converts a batch item to packed vertex and index arrays.
    /// </summary>
    /// <param name="data">The batch item data.</param>
    /// <param name="itemIndex">The slot index in the buffer.</param>
    /// <param name="vertexData">Output: packed float array of vertex data.</param>
    /// <param name="indexData">Output: uint array of index data.</param>
    private protected abstract void BuildItemData(
        TData data,
        uint itemIndex,
        out float[] vertexData,
        out uint[] indexData);

    /// <summary>
    /// Linear interpolation from one range to another.
    /// </summary>
    private static float MapValue(float value, float fromStart, float fromStop, float toStart, float toStop)
        => toStart + ((toStop - toStart) * ((value - fromStart) / (fromStop - fromStart)));

    /// <summary>
    /// Ensures the GPU buffers have been allocated, initializing them if necessary.
    /// Called automatically before any upload or draw operation.
    /// </summary>
    private void EnsureInitialized()
    {
        if (!IsInitialized)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Allocates (or re-allocates) GPU vertex and index buffers for at least <paramref name="minItemCount"/> items.
    /// </summary>
    private void Allocate(uint minItemCount)
    {
        var newCapacity = Math.Max(minItemCount, Capacity > 0 ? Capacity * 2 : DefaultCapacity);
        Capacity = newCapacity;

        this.vertexBufferSizeInBytes = Capacity * VerticesPerItem * VertexSizeInBytes;
        this.indexBufferSizeInBytes = Capacity * IndicesPerItem * IndexItemSizeInBytes;

        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();

        this.vertexBuffer = Wgpu.DeviceCreateVertexBuffer(
            Device,
            $"{GetType().Name} Vertex Buffer",
            this.vertexBufferSizeInBytes,
            Silk.NET.WebGPU.BufferUsage.Vertex | Silk.NET.WebGPU.BufferUsage.CopyDst);

        this.indexBuffer = Wgpu.DeviceCreateIndexBuffer(
            Device,
            $"{GetType().Name} Index Buffer",
            this.indexBufferSizeInBytes,
            Silk.NET.WebGPU.BufferUsage.Index | Silk.NET.WebGPU.BufferUsage.CopyDst);
    }

    /// <summary>
    /// Uploads vertex and index data to the GPU at the given item offset.
    /// </summary>
    private void UploadToGpu(float[] vertexData, uint[] indexData, uint itemIndex)
    {
        if (this.vertexBuffer is null)
        {
            throw new Exception("The vertex buffer cannot be null.");
        }

        if (this.indexBuffer is null)
        {
            throw new Exception("The vertex index buffer cannot be null.");
        }

        var vbOffset = itemIndex * VerticesPerItem * VertexSizeInBytes;
        Wgpu.QueueWriteBuffer(Queue, this.vertexBuffer.DangerousGetHandle(), vbOffset, vertexData);

        var ibOffset = itemIndex * IndicesPerItem * IndexItemSizeInBytes;
        Wgpu.QueueWriteBuffer(Queue, this.indexBuffer.DangerousGetHandle(), ibOffset, indexData);
    }
}

// <copyright file="WebGpuBufferBase.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using System.Numerics;
using NativeInterop.WebGpu;
using NativeInterop.WebGpu.Handles;
using Silk.NET.WebGPU;

/// <summary>
/// Base functionality for WebGPU vertex/index buffer management.
/// Handles allocation, resize, and draw call dispatch for batched quad rendering.
/// </summary>
/// <typeparam name="TData">The batch item struct type.</typeparam>
internal abstract class WebGpuBufferBase<TData> : IWebGpuBuffer<TData>
    where TData : struct
{
    // TODO: Look into why sometimes it takes longer to close down the window.  the first time I experienced this was with a capacity of 2000.
    // It happens with a low number too. It might not have anything to do with the capacity.

    private const uint DefaultCapacity = 64;
    private readonly IGraphicsDevice grfxDevice;
    private SafeVertexBufferHandle? vertexBuffer;
    private SafeIndexBufferHandle? indexBuffer;
    private uint vertexBufferSizeInBytes;
    private uint indexBufferSizeInBytes;
    private bool isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebGpuBufferBase{TData}"/> class.
    /// </summary>
    /// <param name="grfxDevice">The graphics device.</param>
    private protected WebGpuBufferBase(IGraphicsDevice grfxDevice)
    {
        this.grfxDevice = grfxDevice;
        Capacity = 0;
    }

    /// <inheritdoc/>
    public bool IsInitialized => Capacity > 0;

    /// <inheritdoc/>
    // ReSharper disable once MemberCanBePrivate.Global
    public uint Capacity { get; private set; }

    /// <inheritdoc/>
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
    private IWgpuInvoker Wgpu => this.grfxDevice.Wgpu;

    /// <summary>
    /// Allocates GPU vertex and index buffers. Must be called after the WebGPU device
    /// has been initialized.
    /// </summary>
    protected void Initialize()
    {
        if (IsInitialized)
        {
            return;
        }

        Allocate(DefaultCapacity);
    }

    // TODO: Look into make this protected.  This is invoked via a reactable anyway, not executed externally.
    /// <inheritdoc/>
    public void EnsureCapacity(uint requiredCapacity)
    {
        if (!IsInitialized)
        {
            throw new InvalidOperationException($"The buffer must be initialized before calling {nameof(EnsureCapacity)}().");
        }

        if (requiredCapacity > Capacity)
        {
            Allocate(requiredCapacity);
        }
    }

    /// <inheritdoc/>
    public void UploadData(TData data, uint itemIndex = 0)
    {
        if (itemIndex >= Capacity)
        {
            Allocate(itemIndex + 1);
        }

        BuildItemData(data, itemIndex, out var vertexData, out var indexData);
        UploadToGpu(vertexData, indexData, itemIndex);
    }

    /// <inheritdoc/>
    public void Draw(SafeRenderPassEncoderHandle pass, uint itemCount = 1, uint firstItem = 0)
    {
        if (this.vertexBuffer is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeVertexBufferHandle)}' cannot be null. Cannot write to buffer.");
        }

        if (this.indexBuffer is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeIndexBufferHandle)}' cannot be null. Cannot write to buffer.");
        }

        this.grfxDevice.Wgpu.RenderPassEncoderSetVertexBuffer(
            pass,
            0,
            this.vertexBuffer.DangerousGetHandle(),
            0,
            this.vertexBufferSizeInBytes);

        this.grfxDevice.Wgpu.RenderPassEncoderSetIndexBuffer(
            pass,
            this.indexBuffer.DangerousGetHandle(),
            IndexFormat.Uint32,
            0,
            this.indexBufferSizeInBytes);

        this.grfxDevice.Wgpu.RenderPassEncoderDrawIndexed(
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
    /// Allocates (or re-allocates) GPU vertex and index buffers for at least <paramref name="minItemCount"/> items.
    /// </summary>
    private void Allocate(uint minItemCount)
    {
        if (this.grfxDevice.Handle is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeDeviceHandle)}' cannot be null. Cannot create vertex and index buffers.");
        }

        var newCapacity = Math.Max(minItemCount, Capacity > 0 ? Capacity * 2 : DefaultCapacity);
        Capacity = newCapacity;

        this.vertexBufferSizeInBytes = Capacity * VerticesPerItem * VertexSizeInBytes;
        this.indexBufferSizeInBytes = Capacity * IndicesPerItem * IndexItemSizeInBytes;

        this.vertexBuffer?.Dispose();
        this.indexBuffer?.Dispose();

        this.vertexBuffer = Wgpu.DeviceCreateVertexBuffer(
            this.grfxDevice.Handle,
            $"{GetType().Name} Vertex Buffer",
            this.vertexBufferSizeInBytes,
            BufferUsage.Vertex | BufferUsage.CopyDst);

        this.indexBuffer = Wgpu.DeviceCreateIndexBuffer(
            this.grfxDevice.Handle,
            $"{GetType().Name} Index Buffer",
            this.indexBufferSizeInBytes,
            BufferUsage.Index | BufferUsage.CopyDst);
    }

    /// <summary>
    /// Uploads vertex and index data to the GPU at the given item offset.
    /// </summary>
    private void UploadToGpu(float[] vertexData, uint[] indexData, uint itemIndex)
    {
        if (this.vertexBuffer is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeVertexBufferHandle)}' cannot be null. Cannot write to buffer.");
        }

        if (this.indexBuffer is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeIndexBufferHandle)}' cannot be null. Cannot write to buffer.");
        }

        if (this.grfxDevice.Queue is null)
        {
            throw new InvalidOperationException($"The '{nameof(SafeQueueHandle)}' cannot be null. Cannot write to buffer.");
        }

        var vbOffset = itemIndex * VerticesPerItem * VertexSizeInBytes;
        Wgpu.QueueWriteBuffer(this.grfxDevice.Queue, this.vertexBuffer.DangerousGetHandle(), vbOffset, vertexData);

        var ibOffset = itemIndex * IndicesPerItem * IndexItemSizeInBytes;
        Wgpu.QueueWriteBuffer(this.grfxDevice.Queue, this.indexBuffer.DangerousGetHandle(), ibOffset, indexData);
    }
}

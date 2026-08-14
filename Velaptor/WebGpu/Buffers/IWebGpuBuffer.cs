// <copyright file="IWebGpuBuffer.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace Velaptor.WebGpu.Buffers;

using System;
using System.Numerics;
using NativeInterop.WebGpu.Handles;

internal interface IWebGpuBuffer<TData> : IDisposable
{
    /// <summary>
    /// Gets a value indicating whether the GPU buffers have been allocated.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    bool IsInitialized { get; }

    /// <summary>
    /// Gets the number of batch items the current GPU buffers can hold.
    /// </summary>
    // ReSharper disable once MemberCanBePrivate.Global
    uint Capacity { get; }

    /// <summary>
    /// Gets or sets the window size in pixels used for NDC conversion.
    /// </summary>
    Vector2 WindowSize { get; set; }

    /// <summary>
    /// Ensures the GPU buffers can hold at least <paramref name="requiredCapacity"/> items,
    /// re-allocating if necessary. Call this before starting an upload loop to avoid
    /// mid-loop resizes that would invalidate previously recorded draw commands.
    /// </summary>
    /// <param name="requiredCapacity">The minimum number of batch items the buffer must support.</param>
    void EnsureCapacity(uint requiredCapacity);

    /// <summary>
    /// Uploads a single batch item to the GPU at <paramref name="itemIndex"/>.
    /// Grows the buffer automatically if the index exceeds current capacity.
    /// </summary>
    /// <param name="data">The batch item data to upload.</param>
    /// <param name="itemIndex">Zero-based slot in the GPU buffer to write to.</param>
    void UploadData(TData data, uint itemIndex = 0);

    /// <summary>
    /// Binds the vertex and index buffers to the active render pass and issues an indexed draw call.
    /// </summary>
    /// <param name="pass">The active render pass encoder.</param>
    /// <param name="itemCount">Number of batch items to draw.</param>
    /// <param name="firstItem">Index of the first batch item to draw.</param>
    void Draw(SafeRenderPassEncoderHandle pass, uint itemCount = 1, uint firstItem = 0);
}

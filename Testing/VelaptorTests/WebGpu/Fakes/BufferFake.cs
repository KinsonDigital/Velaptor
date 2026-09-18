// <copyright file="BufferFake.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Fakes;

using System.Numerics;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Buffers;

/// <summary>
/// A concrete implementation of <see cref="WebGpuBufferBase{TData}"/> used for testing.
/// </summary>
internal sealed class BufferFake : WebGpuBufferBase<int>
{
    private const uint VertsPerItem = 4;
    private const uint IndicesPerItemValue = 6;
    private const uint VtxSize = 8;
    private const uint IdxSize = 4;

    /// <summary>
    /// Initializes a new instance of the <see cref="BufferFake"/> class.
    /// </summary>
    /// <param name="grfxDevice">The mocked graphics device.</param>
    public BufferFake(IGraphicsDevice grfxDevice)
        : base(grfxDevice)
    {
    }

    /// <inheritdoc/>
    private protected override uint VertexSizeInBytes => VtxSize;

    /// <inheritdoc/>
    private protected override uint IndexItemSizeInBytes => IdxSize;

    /// <inheritdoc/>
    private protected override uint VerticesPerItem => VertsPerItem;

    /// <inheritdoc/>
    private protected override uint IndicesPerItem => IndicesPerItemValue;

    /// <summary>
    /// Exposes the protected <see cref="WebGpuBufferBase{T}.ToNDC"/> method for testing.
    /// </summary>
    /// <param name="x">The X position.</param>
    /// <param name="y">The Y position.</param>
    /// <returns>The NDC position vector.</returns>
    public Vector2 CallToNDC(float x, float y) => ToNDC(x, y);

    /// <inheritdoc cref="WebGpuBufferBase{T}.Initialize"/>
    public new void Initialize() => base.Initialize();

    /// <summary>
    /// Exposes the protected <see cref="WebGpuBufferBase{TData}.EnsureCapacity"/> method for testing.
    /// </summary>
    /// <param name="requiredCapacity">The required capacity.</param>
    public new void EnsureCapacity(uint requiredCapacity) => base.EnsureCapacity(requiredCapacity);

    /// <summary>
    /// Builds the item data.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="itemIndex">The item index.</param>
    /// <param name="vertexData">The vertex data.</param>
    /// <param name="indexData">The index data.</param>
    private protected override void BuildItemData(int data, uint itemIndex, out float[] vertexData, out uint[] indexData)
    {
        vertexData = [data, data, data, data];
        var baseV = itemIndex * VertsPerItem;
        indexData = [baseV, baseV + 1, baseV + 2, baseV + 2, baseV + 1, baseV + 3];
    }
}

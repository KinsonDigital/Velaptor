// <copyright file="LineGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Buffers;

using System.Numerics;
using Carbonate.OneWay;
using Color = System.Drawing.Color;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.Factories;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Batching;
using Velaptor.WebGpu.Buffers;
using Xunit;

/// <summary>
/// Tests the <see cref="LineGpuBuffer"/> class.
/// </summary>
public class LineGpuBufferTests
{
    private const uint VertexDataLength = 24; // 4 vertices × 6 floats
    private const uint IndexDataLength = 6;
    private readonly IGraphicsDevice mockDevice;
    private readonly IReactableFactory mockReactableFactory;
    private float[]? capturedVertexData;
    private uint[]? capturedIndexData;

    /// <summary>
    /// Initializes a new instance of the <see cref="LineGpuBufferTests"/> class.
    /// </summary>
    public LineGpuBufferTests()
    {
        const nint unsafeDevice = 0x11;
        const nint unsafeQueue = 0x22;
        const nint unsafeVb = 0x33;
        const nint unsafeIb = 0x44;

        var mockWgpu = Substitute.For<IWgpuInvoker>();

        var deviceHandle = new SafeDeviceHandle(mockWgpu, unsafeDevice);
        mockWgpu.DeviceGetQueue(deviceHandle).Returns(unsafeQueue);
        var queueHandle = new SafeQueueHandle(mockWgpu, deviceHandle);

        var vbHandle = new SafeVertexBufferHandle(mockWgpu, unsafeVb);
        var ibHandle = new SafeIndexBufferHandle(mockWgpu, unsafeIb);

        mockWgpu.DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(vbHandle);

        mockWgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(ibHandle);

        mockWgpu.When(x => x.QueueWriteBuffer(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<ulong>(),
            Arg.Any<float[]>()))
            .Do(callInfo => { this.capturedVertexData = callInfo.Arg<float[]>(); });

        mockWgpu.When(x => x.QueueWriteBuffer(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<ulong>(),
            Arg.Any<uint[]>()))
            .Do(callInfo => { this.capturedIndexData = callInfo.Arg<uint[]>(); });

        this.mockDevice = Substitute.For<IGraphicsDevice>();
        this.mockDevice.Wgpu.Returns(mockWgpu);
        this.mockDevice.Handle.Returns(deviceHandle);
        this.mockDevice.Queue.Returns(queueHandle);

        var mockBufferCapReactable = Substitute.For<IPushReactable<RequiredBufferCapacityData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateResizeBufferReactable().Returns(mockBufferCapReactable);
    }

    #region Method Tests
    [Fact]
    public void UploadData_WithHorizontalLine_UploadsCorrectVertexAndIndexData()
    {
        // Arrange — horizontal line at y=200 from x=200 to x=600, thickness=2
        var item = new LineBatchItem(
            p1: new Vector2(200, 200),
            p2: new Vector2(600, 200),
            color: Color.FromArgb(255, 128, 64, 32),
            thickness: 2);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert
        this.capturedVertexData.ShouldNotBeNull();
        this.capturedVertexData.Length.ShouldBe((int)VertexDataLength);
        this.capturedIndexData.ShouldNotBeNull();
        this.capturedIndexData.Length.ShouldBe((int)IndexDataLength);

        // All NDC position components must be in [-1, 1]
        for (var v = 0; v < 4; v++)
        {
            var baseIdx = v * 6;
            this.capturedVertexData[baseIdx].ShouldBeInRange(-1f, 1f); // posX
            this.capturedVertexData[baseIdx + 1].ShouldBeInRange(-1f, 1f); // posY
        }

        // All 4 vertices get the same color
        for (var v = 0; v < 4; v++)
        {
            var baseIdx = v * 6;
            this.capturedVertexData[baseIdx + 2].ShouldBe(128f); // r
            this.capturedVertexData[baseIdx + 3].ShouldBe(64f); // g
            this.capturedVertexData[baseIdx + 4].ShouldBe(32f); // b
            this.capturedVertexData[baseIdx + 5].ShouldBe(255f); // a
        }

        // Index data: two triangles (0,1,2) and (2,1,3)
        this.capturedIndexData[0].ShouldBe(0u);
        this.capturedIndexData[1].ShouldBe(1u);
        this.capturedIndexData[2].ShouldBe(2u);
        this.capturedIndexData[3].ShouldBe(2u);
        this.capturedIndexData[4].ShouldBe(1u);
        this.capturedIndexData[5].ShouldBe(3u);
    }

    [Fact]
    public void UploadData_WithNonZeroItemIndex_OffsetsIndexData()
    {
        // Arrange
        var item = new LineBatchItem(
            p1: new Vector2(200, 200),
            p2: new Vector2(600, 200),
            color: Color.FromArgb(255, 128, 64, 32),
            thickness: 2);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act — upload at slot 3 (base vertex = 3 * 4 = 12)
        sut.UploadData(item, itemIndex: 3);

        // Assert
        this.capturedIndexData.ShouldNotBeNull();

        var expectedBaseV = 3u * 4u; // itemIndex * VerticesPerLine
        this.capturedIndexData[0].ShouldBe(expectedBaseV);
        this.capturedIndexData[1].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[2].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[3].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[4].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[5].ShouldBe(expectedBaseV + 3);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="LineGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private LineGpuBuffer CreateSystemUnderTest() => new (this.mockDevice, this.mockReactableFactory);
}

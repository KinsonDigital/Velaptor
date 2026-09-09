// <copyright file="FontGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Buffers;

using System.Drawing;
using System.Numerics;
using Carbonate.OneWay;
using Color = System.Drawing.Color;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.ReactableData;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Batching;
using Velaptor.WebGpu.Buffers;
using Xunit;

/// <summary>
/// Tests the <see cref="FontGpuBuffer"/> class.
/// </summary>
public class FontGpuBufferTests
{
    private const nint UnsafeDeviceHandle = 0x11;
    private const nint UnsafeQueueHandle = 0x22;
    private const nint UnsafeVertexBufferHandle = 0x33;
    private const nint UnsafeIndexBufferHandle = 0x44;
    private const uint VertexDataLength = 32; // 4 vertices × 8 floats
    private const uint IndexDataLength = 6; // 2 triangles × 3 indices
    private readonly IGraphicsDevice mockDevice;
    private readonly IReactableFactory mockReactableFactory;
    private float[]? capturedVertexData;
    private uint[]? capturedIndexData;

    /// <summary>
    /// Initializes a new instance of the <see cref="FontGpuBufferTests"/> class.
    /// </summary>
    public FontGpuBufferTests()
    {
        var mockWgpu = Substitute.For<IWgpuInvoker>();

        var deviceHandle = new SafeDeviceHandle(mockWgpu, UnsafeDeviceHandle);
        mockWgpu.DeviceGetQueue(deviceHandle).Returns(UnsafeQueueHandle);
        var queueHandle = new SafeQueueHandle(mockWgpu, deviceHandle);

        var vertexBufferHandle = new SafeVertexBufferHandle(mockWgpu, UnsafeVertexBufferHandle);
        var indexBufferHandle = new SafeIndexBufferHandle(mockWgpu, UnsafeIndexBufferHandle);

        mockWgpu.DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(vertexBufferHandle);

        mockWgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(indexBufferHandle);

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
    public void UploadData_WithPlainGlyph_UploadsCorrectVertexAndIndexData()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert
        this.capturedVertexData.ShouldNotBeNull();
        this.capturedVertexData.Length.ShouldBe((int)VertexDataLength);
        this.capturedIndexData.ShouldNotBeNull();
        this.capturedIndexData.Length.ShouldBe((int)IndexDataLength);

        // Vertex 0 — top-left:  NDC(400,150) = (0, 0.5), UV=(0,1)
        this.capturedVertexData[0].ShouldBe(0f); // posX
        this.capturedVertexData[1].ShouldBe(0.5f); // posY
        this.capturedVertexData[2].ShouldBe(0f); // u
        this.capturedVertexData[3].ShouldBe(1f); // v
        this.capturedVertexData[4].ShouldBe(128f); // r
        this.capturedVertexData[5].ShouldBe(64f); // g
        this.capturedVertexData[6].ShouldBe(32f); // b
        this.capturedVertexData[7].ShouldBe(255f); // a

        // Vertex 1 — bottom-left: NDC(400,300) = (0, 0), UV=(0,0)
        this.capturedVertexData[8].ShouldBe(0f); // posX
        this.capturedVertexData[9].ShouldBe(0f); // posY
        this.capturedVertexData[10].ShouldBe(0f); // u
        this.capturedVertexData[11].ShouldBe(0f); // v
        this.capturedVertexData[12].ShouldBe(128f); // r
        this.capturedVertexData[13].ShouldBe(64f); // g
        this.capturedVertexData[14].ShouldBe(32f); // b
        this.capturedVertexData[15].ShouldBe(255f); // a

        // Vertex 2 — top-right: NDC(600,150) = (0.5, 0.5), UV=(1,1)
        this.capturedVertexData[16].ShouldBe(0.5f); // posX
        this.capturedVertexData[17].ShouldBe(0.5f); // posY
        this.capturedVertexData[18].ShouldBe(1f); // u
        this.capturedVertexData[19].ShouldBe(1f); // v
        this.capturedVertexData[20].ShouldBe(128f); // r
        this.capturedVertexData[21].ShouldBe(64f); // g
        this.capturedVertexData[22].ShouldBe(32f); // b
        this.capturedVertexData[23].ShouldBe(255f); // a

        // Vertex 3 — bottom-right: NDC(600,300) = (0.5, 0), UV=(1,0)
        this.capturedVertexData[24].ShouldBe(0.5f); // posX
        this.capturedVertexData[25].ShouldBe(0f); // posY
        this.capturedVertexData[26].ShouldBe(1f); // u
        this.capturedVertexData[27].ShouldBe(0f); // v
        this.capturedVertexData[28].ShouldBe(128f); // r
        this.capturedVertexData[29].ShouldBe(64f); // g
        this.capturedVertexData[30].ShouldBe(32f); // b
        this.capturedVertexData[31].ShouldBe(255f); // a

        // Index data: two triangles (0,1,2) and (2,1,3)
        this.capturedIndexData[0].ShouldBe(0u);
        this.capturedIndexData[1].ShouldBe(1u);
        this.capturedIndexData[2].ShouldBe(2u);
        this.capturedIndexData[3].ShouldBe(2u);
        this.capturedIndexData[4].ShouldBe(1u);
        this.capturedIndexData[5].ShouldBe(3u);
    }

    [Fact]
    public void UploadData_WithFlipHorizontally_SwapsUCoordinates()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipHorizontally,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert — uLeft and uRight are swapped; v is unchanged
        this.capturedVertexData.ShouldNotBeNull();

        // Top-left vertex: u should be 1 (was 0), v = 1 (unchanged)
        this.capturedVertexData[2].ShouldBe(1f); // u
        this.capturedVertexData[3].ShouldBe(1f); // v

        // Bottom-left vertex: u = 1 (was 0), v = 0 (unchanged)
        this.capturedVertexData[10].ShouldBe(1f); // u
        this.capturedVertexData[11].ShouldBe(0f); // v

        // Top-right vertex: u = 0 (was 1), v = 1 (unchanged)
        this.capturedVertexData[18].ShouldBe(0f); // u
        this.capturedVertexData[19].ShouldBe(1f); // v

        // Bottom-right vertex: u = 0 (was 1), v = 0 (unchanged)
        this.capturedVertexData[26].ShouldBe(0f); // u
        this.capturedVertexData[27].ShouldBe(0f); // v
    }

    [Fact]
    public void UploadData_WithFlipVertically_SwapsVCoordinates()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipVertically,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert — vTop and vBottom are swapped; u is unchanged
        this.capturedVertexData.ShouldNotBeNull();

        // Top-left vertex: u = 0 (unchanged), v = 0 (was 1)
        this.capturedVertexData[2].ShouldBe(0f); // u
        this.capturedVertexData[3].ShouldBe(0f); // v

        // Bottom-left vertex: u = 0 (unchanged), v = 1 (was 0)
        this.capturedVertexData[10].ShouldBe(0f); // u
        this.capturedVertexData[11].ShouldBe(1f); // v

        // Top-right vertex: u = 1 (unchanged), v = 0 (was 1)
        this.capturedVertexData[18].ShouldBe(1f); // u
        this.capturedVertexData[19].ShouldBe(0f); // v

        // Bottom-right vertex: u = 1 (unchanged), v = 1 (was 0)
        this.capturedVertexData[26].ShouldBe(1f); // u
        this.capturedVertexData[27].ShouldBe(1f); // v
    }

    [Fact]
    public void UploadData_WithFlipBothDirections_SwapsBothUAndV()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipBothDirections,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert — both u and v pairs are swapped
        this.capturedVertexData.ShouldNotBeNull();

        // Top-left: u = 1 (was 0), v = 0 (was 1)
        this.capturedVertexData[2].ShouldBe(1f); // u
        this.capturedVertexData[3].ShouldBe(0f); // v

        // Bottom-left: u = 1 (was 0), v = 1 (was 0)
        this.capturedVertexData[10].ShouldBe(1f); // u
        this.capturedVertexData[11].ShouldBe(1f); // v

        // Top-right: u = 0 (was 1), v = 0 (was 1)
        this.capturedVertexData[18].ShouldBe(0f); // u
        this.capturedVertexData[19].ShouldBe(0f); // v

        // Bottom-right: u = 0 (was 1), v = 1 (was 0)
        this.capturedVertexData[26].ShouldBe(0f); // u
        this.capturedVertexData[27].ShouldBe(1f); // v
    }

    [Fact]
    public void UploadData_WithNonZeroItemIndex_OffsetsIndexData()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act — upload at slot 5 (base vertex = 5 * 4 = 20)
        sut.UploadData(item, itemIndex: 5);

        // Assert
        this.capturedIndexData.ShouldNotBeNull();

        var expectedBaseV = 5u * 4u; // itemIndex * QuadsPerItem = 5 * 4 = 20
        this.capturedIndexData[0].ShouldBe(expectedBaseV);
        this.capturedIndexData[1].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[2].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[3].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[4].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[5].ShouldBe(expectedBaseV + 3);
    }

    [Fact]
    public void UploadData_WithRotation_RotatesVerticesAroundOrigin()
    {
        // Arrange
        var item = new FontGlyphBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            glyph: 'A',
            size: 1,
            angle: 90,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        sut.UploadData(item, itemIndex: 0);

        // Assert — after 90° rotation around (400,300), Y-shift by -150, then NDC
        this.capturedVertexData.ShouldNotBeNull();

        const float tolerance = 0.001f;

        // Vertex 0 — rotated top-left → (550, 300) NDC(0.375, 0)
        this.capturedVertexData[0].ShouldBe(0.375f, tolerance);
        this.capturedVertexData[1].ShouldBe(0f, tolerance);

        // Vertex 1 — rotated bottom-left → (400, 300) NDC(0, 0)
        this.capturedVertexData[8].ShouldBe(0f, tolerance);
        this.capturedVertexData[9].ShouldBe(0f, tolerance);

        // Vertex 2 — rotated top-right → (550, 500) NDC(0.375, -0.666666)
        this.capturedVertexData[16].ShouldBe(0.375f, tolerance);
        this.capturedVertexData[17].ShouldBe(-0.666666f, tolerance);

        // Vertex 3 — rotated bottom-right → (400, 500) NDC(0, -0.666666)
        this.capturedVertexData[24].ShouldBe(0f, tolerance);
        this.capturedVertexData[25].ShouldBe(-0.666666f, tolerance);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="FontGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private FontGpuBuffer CreateSystemUnderTest() => new (this.mockDevice, this.mockReactableFactory);
}

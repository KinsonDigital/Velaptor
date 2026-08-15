// <copyright file="TextureGpuBufferTests.cs" company="KinsonDigital">
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
/// Tests the <see cref="TextureGpuBuffer"/> class.
/// </summary>
public class TextureGpuBufferTests
{
    private const uint VertexDataLength = 32;
    private const uint IndexDataLength = 6;
    private readonly IGraphicsDevice mockDevice;
    private readonly IReactableFactory mockReactableFactory;
    private float[]? capturedVertexData;
    private uint[]? capturedIndexData;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureGpuBufferTests"/> class.
    /// </summary>
    public TextureGpuBufferTests()
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
    public void UploadData_WithPlainTexture_UploadsCorrectVertexAndIndexData()
    {
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();
        this.capturedVertexData.Length.ShouldBe((int)VertexDataLength);
        this.capturedIndexData.ShouldNotBeNull();
        this.capturedIndexData.Length.ShouldBe((int)IndexDataLength);

        // Vertex order: 0=TL, 1=TR, 2=BL, 3=BR
        // V0 — top-left: NDC(300,225)=(-0.25,0.25), UV=(0,1)
        this.capturedVertexData[0].ShouldBe(-0.25f);
        this.capturedVertexData[1].ShouldBe(0.25f);
        this.capturedVertexData[2].ShouldBe(0f);
        this.capturedVertexData[3].ShouldBe(1f);
        this.capturedVertexData[4].ShouldBe(128f);
        this.capturedVertexData[5].ShouldBe(64f);
        this.capturedVertexData[6].ShouldBe(32f);
        this.capturedVertexData[7].ShouldBe(255f);

        // V1 — top-right: NDC(500,225)=(0.25,0.25), UV=(1,1)
        this.capturedVertexData[8].ShouldBe(0.25f);
        this.capturedVertexData[9].ShouldBe(0.25f);
        this.capturedVertexData[10].ShouldBe(1f);
        this.capturedVertexData[11].ShouldBe(1f);
        this.capturedVertexData[12].ShouldBe(128f);
        this.capturedVertexData[13].ShouldBe(64f);
        this.capturedVertexData[14].ShouldBe(32f);
        this.capturedVertexData[15].ShouldBe(255f);

        // V2 — bottom-left: NDC(300,375)=(-0.25,-0.25), UV=(0,0)
        this.capturedVertexData[16].ShouldBe(-0.25f);
        this.capturedVertexData[17].ShouldBe(-0.25f);
        this.capturedVertexData[18].ShouldBe(0f);
        this.capturedVertexData[19].ShouldBe(0f);
        this.capturedVertexData[20].ShouldBe(128f);
        this.capturedVertexData[21].ShouldBe(64f);
        this.capturedVertexData[22].ShouldBe(32f);
        this.capturedVertexData[23].ShouldBe(255f);

        // V3 — bottom-right: NDC(500,375)=(0.25,-0.25), UV=(1,0)
        this.capturedVertexData[24].ShouldBe(0.25f);
        this.capturedVertexData[25].ShouldBe(-0.25f);
        this.capturedVertexData[26].ShouldBe(1f);
        this.capturedVertexData[27].ShouldBe(0f);
        this.capturedVertexData[28].ShouldBe(128f);
        this.capturedVertexData[29].ShouldBe(64f);
        this.capturedVertexData[30].ShouldBe(32f);
        this.capturedVertexData[31].ShouldBe(255f);

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
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipHorizontally,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        this.capturedVertexData[2].ShouldBe(1f); // V0 u (was 0)
        this.capturedVertexData[3].ShouldBe(1f); // V0 v
        this.capturedVertexData[10].ShouldBe(0f); // V1 u (was 1)
        this.capturedVertexData[11].ShouldBe(1f); // V1 v
        this.capturedVertexData[18].ShouldBe(1f); // V2 u (was 0)
        this.capturedVertexData[19].ShouldBe(0f); // V2 v
        this.capturedVertexData[26].ShouldBe(0f); // V3 u (was 1)
        this.capturedVertexData[27].ShouldBe(0f); // V3 v
    }

    [Fact]
    public void UploadData_WithFlipVertically_SwapsVCoordinates()
    {
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipVertically,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        this.capturedVertexData[2].ShouldBe(0f); // V0 u
        this.capturedVertexData[3].ShouldBe(0f); // V0 v (was 1)
        this.capturedVertexData[10].ShouldBe(1f); // V1 u
        this.capturedVertexData[11].ShouldBe(0f); // V1 v (was 1)
        this.capturedVertexData[18].ShouldBe(0f); // V2 u
        this.capturedVertexData[19].ShouldBe(1f); // V2 v (was 0)
        this.capturedVertexData[26].ShouldBe(1f); // V3 u
        this.capturedVertexData[27].ShouldBe(1f); // V3 v (was 0)
    }

    [Fact]
    public void UploadData_WithFlipBothDirections_SwapsBothUAndV()
    {
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.FlipBothDirections,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        this.capturedVertexData[2].ShouldBe(1f); // V0 u (was 0)
        this.capturedVertexData[3].ShouldBe(0f); // V0 v (was 1)
        this.capturedVertexData[10].ShouldBe(0f); // V1 u (was 1)
        this.capturedVertexData[11].ShouldBe(0f); // V1 v (was 1)
        this.capturedVertexData[18].ShouldBe(1f); // V2 u (was 0)
        this.capturedVertexData[19].ShouldBe(1f); // V2 v (was 0)
        this.capturedVertexData[26].ShouldBe(0f); // V3 u (was 1)
        this.capturedVertexData[27].ShouldBe(1f); // V3 v (was 0)
    }

    [Fact]
    public void UploadData_WithRotation_RotatesVerticesAroundCenter()
    {
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 90,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        const float tolerance = 0.001f;

        // 90° CW around (400,300): TL(300,225)→(475,200)→NDC(0.1875,0.3333)
        // TR(500,225)→(475,400)→NDC(0.1875,-0.3333)
        // BL(300,375)→(325,200)→NDC(-0.1875,0.3333)
        // BR(500,375)→(325,400)→NDC(-0.1875,-0.3333)
        this.capturedVertexData[0].ShouldBe(0.1875f, tolerance);
        this.capturedVertexData[1].ShouldBe(0.333333f, tolerance);
        this.capturedVertexData[8].ShouldBe(0.1875f, tolerance);
        this.capturedVertexData[9].ShouldBe(-0.333333f, tolerance);
        this.capturedVertexData[16].ShouldBe(-0.1875f, tolerance);
        this.capturedVertexData[17].ShouldBe(0.333333f, tolerance);
        this.capturedVertexData[24].ShouldBe(-0.1875f, tolerance);
        this.capturedVertexData[25].ShouldBe(-0.333333f, tolerance);
    }

    [Fact]
    public void UploadData_WithNonZeroItemIndex_OffsetsIndexData()
    {
        var item = new TextureBatchItem(
            srcRect: new RectangleF(0, 0, 200, 150),
            destRect: new RectangleF(400, 300, 200, 150),
            size: 1,
            angle: 0,
            tintColor: Color.FromArgb(255, 128, 64, 32),
            effects: RenderEffects.None,
            textureId: 1);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 3);

        this.capturedIndexData.ShouldNotBeNull();

        var expectedBaseV = 3u * 4u;
        this.capturedIndexData[0].ShouldBe(expectedBaseV);
        this.capturedIndexData[1].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[2].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[3].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[4].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[5].ShouldBe(expectedBaseV + 3);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureGpuBuffer CreateSystemUnderTest() => new (this.mockDevice, this.mockReactableFactory);
}

// <copyright file="ShapeGpuBufferTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Buffers;

using System;
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
/// Tests the <see cref="ShapeGpuBuffer"/> class.
/// </summary>
public class ShapeGpuBufferTests
{
    private const uint VertexDataLength = 64;
    private const uint IndexDataLength = 6;
    private readonly IGraphicsDevice mockDevice;
    private readonly IReactableFactory mockReactableFactory;
    private float[]? capturedVertexData;
    private uint[]? capturedIndexData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShapeGpuBufferTests"/> class.
    /// </summary>
    public ShapeGpuBufferTests()
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
    public void UploadData_WithSolidShape_UploadsCorrectVertexAndIndexData()
    {
        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: Color.FromArgb(255, 128, 64, 32),
            isSolid: true,
            borderThickness: 3,
            cornerRadius: new CornerRadius(10, 20, 30, 40),
            gradientType: ColorGradient.None,
            gradientStart: default,
            gradientStop: default);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();
        this.capturedVertexData.Length.ShouldBe((int)VertexDataLength);
        this.capturedIndexData.ShouldNotBeNull();
        this.capturedIndexData.Length.ShouldBe((int)IndexDataLength);

        // Vertex 0 — top-left: NDC(300,225)=(-0.25,0.25)
        this.capturedVertexData[0].ShouldBe(-0.25f);
        this.capturedVertexData[1].ShouldBe(0.25f);
        this.capturedVertexData[2].ShouldBe(400f);
        this.capturedVertexData[3].ShouldBe(300f);
        this.capturedVertexData[4].ShouldBe(200f);
        this.capturedVertexData[5].ShouldBe(150f);
        this.capturedVertexData[6].ShouldBe(128f);
        this.capturedVertexData[7].ShouldBe(64f);
        this.capturedVertexData[8].ShouldBe(32f);
        this.capturedVertexData[9].ShouldBe(255f);
        this.capturedVertexData[10].ShouldBe(1f);
        this.capturedVertexData[11].ShouldBe(3f);
        this.capturedVertexData[12].ShouldBe(10f);
        this.capturedVertexData[13].ShouldBe(20f);
        this.capturedVertexData[14].ShouldBe(30f);
        this.capturedVertexData[15].ShouldBe(40f);

        // Vertex 1 — top-right: NDC(500,225)=(0.25,0.25)
        this.capturedVertexData[16].ShouldBe(0.25f);
        this.capturedVertexData[17].ShouldBe(0.25f);
        this.capturedVertexData[22].ShouldBe(128f);
        this.capturedVertexData[23].ShouldBe(64f);
        this.capturedVertexData[24].ShouldBe(32f);
        this.capturedVertexData[25].ShouldBe(255f);
        this.capturedVertexData[26].ShouldBe(1f);

        // Vertex 2 — bottom-left: NDC(300,375)=(-0.25,-0.25)
        this.capturedVertexData[32].ShouldBe(-0.25f);
        this.capturedVertexData[33].ShouldBe(-0.25f);

        // Vertex 3 — bottom-right: NDC(500,375)=(0.25,-0.25)
        this.capturedVertexData[48].ShouldBe(0.25f);
        this.capturedVertexData[49].ShouldBe(-0.25f);

        this.capturedIndexData[0].ShouldBe(0u);
        this.capturedIndexData[1].ShouldBe(1u);
        this.capturedIndexData[2].ShouldBe(2u);
        this.capturedIndexData[3].ShouldBe(2u);
        this.capturedIndexData[4].ShouldBe(1u);
        this.capturedIndexData[5].ShouldBe(3u);
    }

    [Fact]
    public void UploadData_WithHorizontalGradient_AssignsGradientColorsPerVertex()
    {
        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: default,
            isSolid: true,
            borderThickness: 0,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.Horizontal,
            gradientStart: Color.FromArgb(255, 255, 0, 0),
            gradientStop: Color.FromArgb(255, 0, 0, 255));

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        // V0 (top-left) → start red, V1 (top-right) → stop blue
        this.capturedVertexData[6].ShouldBe(255f);
        this.capturedVertexData[7].ShouldBe(0f);
        this.capturedVertexData[8].ShouldBe(0f);
        this.capturedVertexData[22].ShouldBe(0f);
        this.capturedVertexData[23].ShouldBe(0f);
        this.capturedVertexData[24].ShouldBe(255f);

        // V2 (bottom-left) → start red, V3 (bottom-right) → stop blue
        this.capturedVertexData[38].ShouldBe(255f);
        this.capturedVertexData[39].ShouldBe(0f);
        this.capturedVertexData[40].ShouldBe(0f);
        this.capturedVertexData[54].ShouldBe(0f);
        this.capturedVertexData[55].ShouldBe(0f);
        this.capturedVertexData[56].ShouldBe(255f);
    }

    [Fact]
    public void UploadData_WithVerticalGradient_AssignsGradientColorsPerVertex()
    {
        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: default,
            isSolid: true,
            borderThickness: 0,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.Vertical,
            gradientStart: Color.FromArgb(255, 255, 0, 0),
            gradientStop: Color.FromArgb(255, 0, 0, 255));

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        // V0, V1 (top) → start red, V2, V3 (bottom) → stop blue
        this.capturedVertexData[6].ShouldBe(255f);
        this.capturedVertexData[7].ShouldBe(0f);
        this.capturedVertexData[8].ShouldBe(0f);
        this.capturedVertexData[22].ShouldBe(255f);
        this.capturedVertexData[23].ShouldBe(0f);
        this.capturedVertexData[24].ShouldBe(0f);
        this.capturedVertexData[38].ShouldBe(0f);
        this.capturedVertexData[39].ShouldBe(0f);
        this.capturedVertexData[40].ShouldBe(255f);
        this.capturedVertexData[54].ShouldBe(0f);
        this.capturedVertexData[55].ShouldBe(0f);
        this.capturedVertexData[56].ShouldBe(255f);
    }

    [Fact]
    public void UploadData_WithBorderShape_SetsIsFilledToZero()
    {
        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: Color.FromArgb(255, 128, 64, 32),
            isSolid: false,
            borderThickness: 5,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.None,
            gradientStart: default,
            gradientStop: default);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 0);

        this.capturedVertexData.ShouldNotBeNull();

        this.capturedVertexData[10].ShouldBe(0f);
        this.capturedVertexData[11].ShouldBe(5f);
        this.capturedVertexData[26].ShouldBe(0f);
        this.capturedVertexData[42].ShouldBe(0f);
        this.capturedVertexData[58].ShouldBe(0f);
    }

    [Fact]
    public void UploadData_WithNonZeroItemIndex_OffsetsIndexData()
    {
        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: Color.FromArgb(255, 128, 64, 32),
            isSolid: true,
            borderThickness: 0,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.None,
            gradientStart: default,
            gradientStop: default);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        sut.UploadData(item, itemIndex: 7);

        this.capturedIndexData.ShouldNotBeNull();

        var expectedBaseV = 7u * 4u;
        this.capturedIndexData[0].ShouldBe(expectedBaseV);
        this.capturedIndexData[1].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[2].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[3].ShouldBe(expectedBaseV + 2);
        this.capturedIndexData[4].ShouldBe(expectedBaseV + 1);
        this.capturedIndexData[5].ShouldBe(expectedBaseV + 3);
    }

    [Fact]
    public void UploadData_WithNullVertexBuffer_ThrowsException()
    {
        // Arrange — override mock so vertex buffer allocation returns null
        this.mockDevice.Wgpu.DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns((SafeVertexBufferHandle?)null);

        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: Color.FromArgb(255, 128, 64, 32),
            isSolid: true,
            borderThickness: 0,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.None,
            gradientStart: default,
            gradientStop: default);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        var act = () => sut.UploadData(item, itemIndex: 0);

        // Assert
        act.ShouldThrow<Exception>()
            .Message.ShouldBe("The vertex buffer cannot be null.");
    }

    [Fact]
    public void UploadData_WithNullIndexBuffer_ThrowsException()
    {
        // Arrange — override mock so index buffer allocation returns null
        this.mockDevice.Wgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns((SafeIndexBufferHandle?)null);

        var item = new ShapeBatchItem(
            position: new Vector2(400, 300),
            width: 200,
            height: 150,
            color: Color.FromArgb(255, 128, 64, 32),
            isSolid: true,
            borderThickness: 0,
            cornerRadius: new CornerRadius(0, 0, 0, 0),
            gradientType: ColorGradient.None,
            gradientStart: default,
            gradientStop: default);

        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(800, 600);

        // Act
        var act = () => sut.UploadData(item, itemIndex: 0);

        // Assert
        act.ShouldThrow<Exception>()
            .Message.ShouldBe("The vertex index buffer cannot be null.");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="ShapeGpuBuffer"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private ShapeGpuBuffer CreateSystemUnderTest() => new (this.mockDevice, this.mockReactableFactory);
}

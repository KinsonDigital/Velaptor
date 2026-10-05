// <copyright file="WebGpuBufferBaseTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu.Buffers;

using System;
using System.Numerics;
using Fakes;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.WebGpu;
using Velaptor.WebGpu.Buffers;
using Xunit;

/// <summary>
/// Tests the <see cref="WebGpuBufferBase{TData}"/> class through a concrete test subclass.
/// </summary>
public class WebGpuBufferBaseTests
{
    private const uint VertsPerItem = 4;
    private const uint IndicesPerItem = 6;
    private const uint VtxSize = 8;
    private const uint IdxSize = 4;
    private const uint DefCapacity = 64;
    private readonly IWgpuInvoker mockWgpu;
    private readonly IGraphicsDevice mockDevice;
    private readonly SafeDeviceHandle deviceHandle;

    public WebGpuBufferBaseTests()
    {
        const nint unsafeDevice = 0x11;
        const nint unsafeQueue = 0x22;

        this.mockWgpu = Substitute.For<IWgpuInvoker>();

        var vertBufferHandle = new SafeVertexBufferHandle(this.mockWgpu, 0x33);
        var indexBufferHandle = new SafeIndexBufferHandle(this.mockWgpu, 0x44);
        this.mockWgpu.DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(vertBufferHandle);
        this.mockWgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns(indexBufferHandle);

        this.deviceHandle = new SafeDeviceHandle(this.mockWgpu, unsafeDevice);
        this.mockWgpu.DeviceGetQueue(this.deviceHandle).Returns(unsafeQueue);
        var queueHandle = new SafeQueueHandle(this.mockWgpu, this.deviceHandle);

        this.mockDevice = Substitute.For<IGraphicsDevice>();
        this.mockDevice.Wgpu.Returns(this.mockWgpu);
        this.mockDevice.Handle.Returns(this.deviceHandle);
        this.mockDevice.Queue.Returns(queueHandle);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WhenInvoked_SetsDefaultState()
    {
        // Arrange & Act
        var sut = CreateSystemUnderTest();

        // Assert
        sut.IsInitialized.ShouldBeFalse();
        sut.Capacity.ShouldBe(0u);
        sut.WindowSize.ShouldBe(new Vector2(800f, 600f));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Initialize_WhenInvoked_AllocatesBuffersAtDefaultCapacity()
    {
        // Arrange
        var vertBufferHandle = new SafeVertexBufferHandle(this.mockWgpu, 0x33);
        var indexBufferHandle = new SafeIndexBufferHandle(this.mockWgpu, 0x44);
        this.mockWgpu.DeviceCreateVertexBuffer(
            this.deviceHandle,
            Arg.Any<string>(),
            DefCapacity * VertsPerItem * VtxSize,
            BufferUsage.Vertex | BufferUsage.CopyDst).Returns(vertBufferHandle);
        this.mockWgpu.DeviceCreateIndexBuffer(
            this.deviceHandle,
            Arg.Any<string>(),
            DefCapacity * IndicesPerItem * IdxSize,
            BufferUsage.Index | BufferUsage.CopyDst).Returns(indexBufferHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();

        // Assert
        sut.IsInitialized.ShouldBeTrue();
        sut.Capacity.ShouldBe(DefCapacity);
    }

    [Fact]
    public void Initialize_WhenAlreadyInitialized_DoesNotReallocate()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        sut.Initialize();

        // Assert
        this.mockWgpu.Received(1).DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>());
        this.mockWgpu.Received(1).DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>());
    }

    [Fact]
    public void EnsureCapacity_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.EnsureCapacity(123);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The buffer must be initialized before calling EnsureCapacity().");
    }

    [Fact]
    public void EnsureCapacity_WhenItemCountLessThanCapacity_DoesNotReallocate()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.EnsureCapacity(10);

        // Assert
        sut.Capacity.ShouldBe(DefCapacity);
        this.mockWgpu.Received(1).DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>());
    }

    [Fact]
    public void EnsureCapacity_WhenItemCountGreaterThanCapacity_Reallocates()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.EnsureCapacity(100);

        // Assert
        sut.Capacity.ShouldBeGreaterThanOrEqualTo(100u);
    }

    [Fact]
    public void UploadData_WithNullDeviceHandle_ThrowsException()
    {
        // Arrange
        this.mockDevice.Handle.Returns((SafeDeviceHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadData(123);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(SafeDeviceHandle)}' cannot be null. Cannot create vertex and index buffers.");
    }

    [Fact]
    public void UploadData_WithNullVertexBufferHandle_ThrowsException()
    {
        // Arrange
        this.mockWgpu.DeviceCreateVertexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns((SafeVertexBufferHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadData(123);

        // Assert
        act.ShouldThrow<Exception>()
            .Message.ShouldBe($"The '{nameof(SafeVertexBufferHandle)}' cannot be null. Cannot write to buffer.");
    }

    [Fact]
    public void UploadData_WithNullIndexBufferHandle_ThrowsException()
    {
        // Arrange
        this.mockWgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns((SafeIndexBufferHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadData(123);

        // Assert
        act.ShouldThrow<Exception>()
            .Message.ShouldBe($"The '{nameof(SafeIndexBufferHandle)}' cannot be null. Cannot write to buffer.");
    }

    [Fact]
    public void UploadData_WithNullQueueHandle_ThrowsException()
    {
        // Arrange
        this.mockDevice.Queue.Returns((SafeQueueHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.UploadData(123);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(SafeQueueHandle)}' cannot be null. Cannot write to buffer.");
    }

    [Fact]
    public void UploadData_WhenInvoked_UploadsToCorrectGpuOffset()
    {
        // Arrange
        ulong? capturedVbOffset = null;
        ulong? capturedIbOffset = null;
        this.mockWgpu.When(x => x.QueueWriteBuffer(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<ulong>(),
            Arg.Any<float[]>()))
            .Do(callInfo => { capturedVbOffset = callInfo.Arg<ulong>(); });
        this.mockWgpu.When(x => x.QueueWriteBuffer(
            Arg.Any<SafeQueueHandle>(),
            Arg.Any<nint>(),
            Arg.Any<ulong>(),
            Arg.Any<uint[]>()))
            .Do(callInfo => { capturedIbOffset = callInfo.Arg<ulong>(); });

        var sut = CreateSystemUnderTest();

        // Act
        sut.UploadData(42, itemIndex: 2);

        // Assert
        capturedVbOffset.ShouldBe(2u * VertsPerItem * VtxSize);
        capturedIbOffset.ShouldBe(2u * IndicesPerItem * IdxSize);
    }

    [Fact]
    public void UploadData_WhenItemIndexExceedsCapacity_ReallocatesThenUploads()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.UploadData(42, itemIndex: 100);

        // Assert
        sut.Capacity.ShouldBeGreaterThanOrEqualTo(101u);
    }

    [Fact]
    public void Draw_WhenInvoked_SetsBuffersAndIssuesDraw()
    {
        // Arrange
        var passHandle = new SafeRenderPassEncoderHandle(this.mockWgpu, 0x55);
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.Draw(passHandle, itemCount: 3, firstItem: 2);

        // Assert
        this.mockWgpu.Received(1).RenderPassEncoderSetVertexBuffer(
            passHandle, 0, 0x33, 0, DefCapacity * VertsPerItem * VtxSize);
        this.mockWgpu.Received(1).RenderPassEncoderSetIndexBuffer(
            passHandle, 0x44, IndexFormat.Uint32, 0, DefCapacity * IndicesPerItem * IdxSize);
        this.mockWgpu.Received(1).RenderPassEncoderDrawIndexed(
            passHandle,
            indexCount: IndicesPerItem * 3,
            instanceCount: 1,
            firstIndex: IndicesPerItem * 2,
            baseVertex: 0,
            firstInstance: 0);
    }

    [Fact]
    public void Draw_WithNullVertexBuffer_ThrowsException()
    {
        // Arrange
        var passHandle = new SafeRenderPassEncoderHandle(this.mockWgpu, 0x55);
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Draw(passHandle);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(SafeVertexBufferHandle)}' cannot be null. Cannot write to buffer.");
    }

    [Fact]
    public void Draw_WithNullIndexBuffer_ThrowsException()
    {
        // Arrange
        this.mockWgpu.DeviceCreateIndexBuffer(
            Arg.Any<SafeDeviceHandle>(),
            Arg.Any<string>(),
            Arg.Any<ulong>(),
            Arg.Any<BufferUsage>()).Returns((SafeIndexBufferHandle?)null);

        var passHandle = new SafeRenderPassEncoderHandle(this.mockWgpu, 0x55);
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        var act = () => sut.Draw(passHandle);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(SafeIndexBufferHandle)}' cannot be null. Cannot write to buffer.");
    }

    [Theory]
    [InlineData(0, 0, 800, 600, -1f, 1f)]
    [InlineData(800, 0, 800, 600, 1f, 1f)]
    [InlineData(0, 600, 800, 600, -1f, -1f)]
    [InlineData(800, 600, 800, 600, 1f, -1f)]
    [InlineData(400, 300, 800, 600, 0f, 0f)]
    public void ToNDC_WithVariousPixelPositions_ReturnsCorrectNDC(float pixelX,
        float pixelY,
        float screenW,
        float screenH,
        float expectedX,
        float expectedY)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.WindowSize = new Vector2(screenW, screenH);

        // Act
        var result = sut.CallToNDC(pixelX, pixelY);

        // Assert
        result.X.ShouldBe(expectedX, 0.001f);
        result.Y.ShouldBe(expectedY, 0.001f);
    }
    #endregion

    #region Dispose Tests
    [Fact]
    public void Dispose_WhenInvoked_ReleasesBuffers()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).BufferDestroy(0x33);
        this.mockWgpu.Received(1).BufferRelease(0x33);
        this.mockWgpu.Received(1).BufferDestroy(0x44);
        this.mockWgpu.Received(1).BufferRelease(0x44);
    }

    [Fact]
    public void Dispose_WhenAlreadyDisposed_DoesNotReleaseAgain()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWgpu.Received(1).BufferDestroy(0x33);
        this.mockWgpu.Received(1).BufferRelease(0x33);
    }
    #endregion

    private BufferFake CreateSystemUnderTest() => new (this.mockDevice);
}

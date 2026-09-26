// <copyright file="GraphicsLinePipelineTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu;

using System;
using NSubstitute;
using Shouldly;
using Silk.NET.WebGPU;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.NativeInterop.WebGpu.Structures;
using Velaptor.WebGpu;
using Xunit;

/// <summary>
/// Tests the <see cref="GraphicsLinePipeline"/>.
/// </summary>
public class GraphicsLinePipelineTests
{
    private const nint UnsafeDeviceHandle = 0x11;
    private readonly SafeShaderModuleHandle fragShaderHandle;
    private readonly SafeShaderModuleHandle vertShaderHandle;
    private readonly SafePipelineLayoutHandle pipelineLayoutHandle;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IGraphicsDevice mockGrfxDevice;
    private readonly IGraphicsSurface mockSurface;
    private readonly IGraphicsShader mockShader;
    private Action<SafeShaderModuleHandle, SafeShaderModuleHandle>? shaderInitCallback;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsLinePipelineTests"/> class.
    /// </summary>
    public GraphicsLinePipelineTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();
        this.pipelineLayoutHandle = new SafePipelineLayoutHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);

        this.mockWgpuInvoker.DeviceCreatePipelineLayout(Arg.Any<SafeDeviceHandle>(), Arg.Any<string>())
            .Returns(this.pipelineLayoutHandle);

        this.vertShaderHandle = new SafeShaderModuleHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);
        this.fragShaderHandle = new SafeShaderModuleHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);

        var grfxDeviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);

        this.mockGrfxDevice = Substitute.For<IGraphicsDevice>();
        this.mockGrfxDevice.Wgpu.Returns(this.mockWgpuInvoker);
        this.mockGrfxDevice.Handle.Returns(grfxDeviceHandle);

        this.mockSurface = Substitute.For<IGraphicsSurface>();

        this.mockShader = Substitute.For<IGraphicsShader>();
        this.mockShader.Initialize(Arg.Any<IGraphicsDevice>(),
            Arg.Any<TypeOfShader>(),
            Arg.Do<Action<SafeShaderModuleHandle, SafeShaderModuleHandle>>(
                cb => this.shaderInitCallback = cb));
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsLinePipeline(null, this.mockSurface, this.mockShader); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'grfxDevice')");
    }

    [Fact]
    public void Ctor_WithNullSurfaceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsLinePipeline(this.mockGrfxDevice, null, this.mockShader); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'surface')");
    }

    [Fact]
    public void Ctor_WithNullShaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsLinePipeline(this.mockGrfxDevice, this.mockSurface, null); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'shader')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Initialize_WithNullDeviceHandle_ThrowsException()
    {
        // Arrange
        this.mockGrfxDevice.Handle.Returns((SafeDeviceHandle?)null);

        var sut = CreateSystemUnderTest();
        sut.Initialize();

        // Act
        var act = () => this.shaderInitCallback(this.vertShaderHandle, this.fragShaderHandle);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe($"The '{nameof(SafeDeviceHandle)}' cannot be null. Cannot build line pipeline.");
    }

    [Fact]
    public void Initialize_WhenInvoked_InitializesPipeline()
    {
        // Arrange
        var deviceHandle = new SafeDeviceHandle(this.mockWgpuInvoker, UnsafeDeviceHandle);
        const TextureFormat textureFormat = TextureFormat.Rgba16Uint;
        SafeRenderPipelineDescriptor? actualDescriptor = null;

        var expectedBlendState = new BlendState(new BlendComponent
        {
            SrcFactor = BlendFactor.SrcAlpha, DstFactor = BlendFactor.OneMinusSrcAlpha, Operation = BlendOperation.Add,
        }, new BlendComponent
        {
            SrcFactor = BlendFactor.One, DstFactor = BlendFactor.OneMinusSrcAlpha, Operation = BlendOperation.Add,
        });

        var expectedPrimitiveState = new PrimitiveState
        {
            Topology = PrimitiveTopology.TriangleList, FrontFace = FrontFace.Ccw, CullMode = CullMode.None,
        };
        var expectedMultisampleState = new MultisampleState { Count = 1, Mask = uint.MaxValue, };

        this.mockWgpuInvoker.When(x
            => x.DeviceCreateRenderPipeline(Arg.Any<SafeDeviceHandle>(), Arg.Any<SafeRenderPipelineDescriptor>()))
            .Do(callInfo =>
            {
                actualDescriptor = callInfo.Arg<SafeRenderPipelineDescriptor>();
            });

        this.mockGrfxDevice.Handle.Returns(deviceHandle);

        this.mockSurface.Format.Returns(textureFormat);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        this.shaderInitCallback(this.vertShaderHandle, this.fragShaderHandle);
        sut.Initialize();

        // Assert
        this.mockShader.Received(1).Initialize(this.mockGrfxDevice, TypeOfShader.Line, Arg.Any<Action<SafeShaderModuleHandle, SafeShaderModuleHandle>>());
        this.mockWgpuInvoker.Received(1).DeviceCreatePipelineLayout(deviceHandle, "Line Pipeline Layout");
        this.mockWgpuInvoker.Received(1).DeviceCreateRenderPipeline(deviceHandle, Arg.Any<SafeRenderPipelineDescriptor>());
        actualDescriptor.ShouldNotBeNull();
        actualDescriptor.Value.Layout.ShouldBe(this.pipelineLayoutHandle);

        // Assert vertex
        actualDescriptor.Value.Vertex.Module.ShouldBe(this.vertShaderHandle);
        actualDescriptor.Value.Vertex.EntryPoint.ShouldBe("vs_main");
        actualDescriptor.Value.Vertex.Buffers.ShouldHaveSingleItem();
        actualDescriptor.Value.Vertex.Buffers[0].ArrayStride.ShouldBe(24u);
        actualDescriptor.Value.Vertex.Buffers[0].StepMode.ShouldBe(VertexStepMode.Vertex);
        actualDescriptor.Value.Vertex.Buffers[0].Attributes.Length.ShouldBe(2);
        actualDescriptor.Value.Vertex.Buffers[0].Attributes[0]
            .ShouldBe(new VertexAttribute { Format = VertexFormat.Float32x2, Offset = 0, ShaderLocation = 0 });
        actualDescriptor.Value.Vertex.Buffers[0].Attributes[1]
            .ShouldBe(new VertexAttribute { Format = VertexFormat.Float32x4, Offset = 8, ShaderLocation = 1 });

        // Assert fragment
        actualDescriptor.Value.Fragment.Module.ShouldBe(this.fragShaderHandle);
        actualDescriptor.Value.Fragment.EntryPoint.ShouldBe("fs_main");
        actualDescriptor.Value.Fragment.Targets.ShouldHaveSingleItem();
        actualDescriptor.Value.Fragment.Targets[0].Format.ShouldBe(textureFormat);
        actualDescriptor.Value.Fragment.Targets[0].WriteMask.ShouldBe(ColorWriteMask.All);
        actualDescriptor.Value.Fragment.Targets[0].Blend.ShouldBe(expectedBlendState);

        // Primitive and multisample
        actualDescriptor.Value.Primitive.ShouldBe(expectedPrimitiveState);
        actualDescriptor.Value.Multisample.ShouldBe(expectedMultisampleState);

        this.mockWgpuInvoker.Received(1).PipelineLayoutRelease(UnsafeDeviceHandle);
    }

    [Fact]
    public void Bind_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Bind(null);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Pipeline has not been initialized. Call Initialize() first.");
    }

    [Fact]
    public void Bind_WithNullHandle_ThrowsException()
    {
        // Arrange
        // Arrange
        this.mockWgpuInvoker.DeviceCreateRenderPipeline(Arg.Any<SafeDeviceHandle>(), Arg.Any<SafeRenderPipelineDescriptor>())
            .Returns((SafeRenderPipelineHandle?)null);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        var act = () => sut.Bind(null);

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Pipeline has not been initialized. Call Initialize() first.");
    }

    [Fact]
    public void Bind_WhenInvoked_BindsPipeline()
    {
        // Arrange
        var expectedRenderPassEncoderHandle = new SafeRenderPassEncoderHandle(this.mockWgpuInvoker, 0x1234);
        var renderPipelineHandle = new SafeRenderPipelineHandle(this.mockWgpuInvoker, 0x5678);
        this.mockWgpuInvoker.DeviceCreateRenderPipeline(Arg.Any<SafeDeviceHandle>(), Arg.Any<SafeRenderPipelineDescriptor>())
            .Returns(renderPipelineHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        this.shaderInitCallback(this.vertShaderHandle, this.fragShaderHandle);
        sut.Bind(expectedRenderPassEncoderHandle);

        // Assert
        this.mockWgpuInvoker.Received(1).RenderPassEncoderSetPipeline(expectedRenderPassEncoderHandle, renderPipelineHandle);
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfPipeline()
    {
        // Arrange
        var renderPipelineHandle = new SafeRenderPipelineHandle(this.mockWgpuInvoker, 0x5678);
        this.mockWgpuInvoker.DeviceCreateRenderPipeline(Arg.Any<SafeDeviceHandle>(), Arg.Any<SafeRenderPipelineDescriptor>())
            .Returns(renderPipelineHandle);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize();
        this.shaderInitCallback(this.vertShaderHandle, this.fragShaderHandle);
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).RenderPipelineRelease(0x5678);
    }

    [Fact]
    public void Dispose_WhenNotInitialized_DoesNotThrowException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Dispose();

        // Assert
        act.ShouldNotThrow();
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GraphicsLinePipeline"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GraphicsLinePipeline CreateSystemUnderTest() => new (this.mockGrfxDevice, this.mockSurface, this.mockShader);
}

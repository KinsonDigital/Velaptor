// <copyright file="GraphicsShaderTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.WebGpu;

using System;
using System.IO.Abstractions;
using NSubstitute;
using Shouldly;
using Velaptor.NativeInterop.WebGpu;
using Velaptor.NativeInterop.WebGpu.Handles;
using Velaptor.Services;
using Velaptor.WebGpu;
using Xunit;

/// <summary>
/// Tests the <see cref="GraphicsShader"/>.
/// </summary>
public class GraphicsShaderTests
{
    private const string ShaderName = "testShader";
    private const nint UnsafeVertexHandle = 0x1111;
    private const nint UnsafeFragmentHandle = 0x2222;
    private readonly SafeShaderModuleHandle vertexHandle;
    private readonly SafeShaderModuleHandle fragmentHandle;
    private readonly IWgpuInvoker mockWgpuInvoker;
    private readonly IEmbeddedResourceLoaderService<string> mockResourceLoader;
    private readonly IPath mockPath;
    private readonly IGraphicsDevice mockDevice;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShaderTests"/> class.
    /// </summary>
    public GraphicsShaderTests()
    {
        this.mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        this.vertexHandle = new SafeShaderModuleHandle(this.mockWgpuInvoker, UnsafeVertexHandle);
        this.fragmentHandle = new SafeShaderModuleHandle(this.mockWgpuInvoker, UnsafeFragmentHandle);

        this.mockResourceLoader = Substitute.For<IEmbeddedResourceLoaderService<string>>();
        this.mockResourceLoader.LoadResource($"{ShaderName}.vert.wgsl").Returns("vert source");
        this.mockResourceLoader.LoadResource($"{ShaderName}.frag.wgsl").Returns("frag source");

        this.mockPath = Substitute.For<IPath>();

        this.mockDevice = Substitute.For<IGraphicsDevice>();
        this.mockDevice.CreateShaderModule("vert source").Returns(this.vertexHandle);
        this.mockDevice.CreateShaderModule("frag source").Returns(this.fragmentHandle);
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullResourceLoaderServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsShader(null, this.mockPath, ShaderName); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'resourceLoaderService')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsShader(this.mockResourceLoader, null, ShaderName); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void VertexHandle_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.VertexHandle;
        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Shader has not been initialized. Call Initialize() first.");
    }

    [Fact]
    public void VertexHandle_WhenInitialized_ReturnsHandle()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize(this.mockDevice);
        var actual = sut.VertexHandle;

        // Assert
        actual.ShouldBe(this.vertexHandle);
    }

    [Fact]
    public void FragmentHandle_WhenNotInitialized_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => _ = sut.FragmentHandle;

        // Assert
        act.ShouldThrow<InvalidOperationException>()
            .Message.ShouldBe("Shader has not been initialized. Call Initialize() first.");
    }

    [Fact]
    public void FragmentHandle_WhenInitialized_ReturnsHandle()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize(this.mockDevice);
        var actual = sut.FragmentHandle;

        // Assert
        actual.ShouldBe(this.fragmentHandle);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Initialize_WhenInvoked_InitializesShader()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize(this.mockDevice);
        sut.Initialize(this.mockDevice);

        // Assert
        this.mockResourceLoader.Received(1).LoadResource($"{ShaderName}.vert.wgsl");
        this.mockResourceLoader.Received(1).LoadResource($"{ShaderName}.frag.wgsl");
        this.mockDevice.Received(1).CreateShaderModule("vert source");
        this.mockDevice.Received(1).CreateShaderModule("frag source");

        sut.VertexHandle.ShouldBe(this.vertexHandle);
        sut.FragmentHandle.ShouldBe(this.fragmentHandle);
    }

    [Fact]
    public void Initialize_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Initialize(null);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'gd')");
    }

    [Fact]
    public void Dispose_WhenInvoked_DisposesOfShader()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize(this.mockDevice);
        sut.Dispose();
        sut.Dispose();

        // Assert
        this.mockWgpuInvoker.Received(1).ShaderModuleRelease(UnsafeVertexHandle);
        this.mockWgpuInvoker.Received(1).ShaderModuleRelease(UnsafeFragmentHandle);
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
    /// Creates a new instance of <see cref="GraphicsShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GraphicsShader CreateSystemUnderTest() => new (this.mockResourceLoader, this.mockPath, ShaderName);
}

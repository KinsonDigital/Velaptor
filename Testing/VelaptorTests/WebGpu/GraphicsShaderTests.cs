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
    private const nint UnsafeVertexHandle = 0x1111;
    private const nint UnsafeFragmentHandle = 0x2222;
    private readonly SafeShaderModuleHandle vertexHandle;
    private readonly SafeShaderModuleHandle fragmentHandle;
    private readonly IEmbeddedResourceLoaderService<string> mockResourceLoader;
    private readonly IPath mockPath;
    private readonly IGraphicsDevice mockGrfxDevice;

    /// <summary>
    /// Initializes a new instance of the <see cref="GraphicsShaderTests"/> class.
    /// </summary>
    public GraphicsShaderTests()
    {
        var mockWgpuInvoker = Substitute.For<IWgpuInvoker>();

        this.vertexHandle = new SafeShaderModuleHandle(mockWgpuInvoker, UnsafeVertexHandle);
        this.fragmentHandle = new SafeShaderModuleHandle(mockWgpuInvoker, UnsafeFragmentHandle);

        this.mockResourceLoader = Substitute.For<IEmbeddedResourceLoaderService<string>>();
        this.mockResourceLoader.LoadResource("Texture.vert.wgsl").Returns("vert source");
        this.mockResourceLoader.LoadResource("Texture.frag.wgsl").Returns("frag source");

        this.mockPath = Substitute.For<IPath>();

        this.mockGrfxDevice = Substitute.For<IGraphicsDevice>();
        this.mockGrfxDevice.CreateShaderModule("vert source").Returns(this.vertexHandle);
        this.mockGrfxDevice.CreateShaderModule("frag source").Returns(this.fragmentHandle);
    }

    #region Ctor Tests
    [Fact]
    public void Ctor_WithNullResourceLoaderServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsShader(null, this.mockPath); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'resourceLoaderService')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => { _ = new GraphicsShader(this.mockResourceLoader, null); };

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Initialize_WithInvalidShaderType_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Initialize(this.mockGrfxDevice, (TypeOfShader)123456, null);

        // Assert
        act.ShouldThrow<ArgumentException>()
            .Message.ShouldBe($"The '{nameof(TypeOfShader)}.{123456}' is invalid. Could not create vertex and fragment shaders.");
    }

    [Fact]
    public void Initialize_WhenInvoked_InitializesShader()
    {
        // Arrange
        var onInitInvoked = false;
        SafeShaderModuleHandle? vertHandle = null;
        SafeShaderModuleHandle? fragHandle = null;

        var sut = CreateSystemUnderTest();

        // Act
        sut.Initialize(this.mockGrfxDevice, TypeOfShader.Texture, (vHandle, fHandle) =>
        {
            onInitInvoked = true;
            vertHandle = vHandle;
            fragHandle = fHandle;
        });

        // Assert
        this.mockResourceLoader.Received(1).LoadResource("Texture.vert.wgsl");
        this.mockResourceLoader.Received(1).LoadResource("Texture.frag.wgsl");
        this.mockGrfxDevice.Received(1).CreateShaderModule("vert source");
        this.mockGrfxDevice.Received(1).CreateShaderModule("frag source");

        onInitInvoked.ShouldBeTrue();
        vertHandle.ShouldBe(this.vertexHandle);
        fragHandle.ShouldBe(this.fragmentHandle);
    }

    [Fact]
    public void Initialize_WithNullGraphicsDeviceParam_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Initialize(null, TypeOfShader.Texture, null);

        // Assert
        act.ShouldThrow<ArgumentNullException>().Message.ShouldBe("Value cannot be null. (Parameter 'grfxDevice')");
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="GraphicsShader"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private GraphicsShader CreateSystemUnderTest() => new (this.mockResourceLoader, this.mockPath);
}

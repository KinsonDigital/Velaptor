// <copyright file="TextureShaderResourceLoaderServiceTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.OpenGL.Services;

using System;
using System.IO.Abstractions;
using Helpers;
using Moq;
using Velaptor.OpenGL.Services;
using Velaptor.Services;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureShaderResourceLoaderService"/> class.
/// </summary>
public class TextureShaderResourceLoaderServiceTests
{
    private const string ProcessedVertShaderSample = "uniform mat4 uTransform[10];";
    private const string TextureShaderName = "test-source";
    private const string NoProcessingFragShaderSample = "int totalClrs = 4;";
    private readonly string unprocessedFragShaderSample = "uniform mat4 uTransform[10];";
    private readonly Mock<IEmbeddedResourceLoaderService<string>> mockResourceLoaderService;
    private readonly Mock<IPath> mockPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureShaderResourceLoaderServiceTests"/> class.
    /// </summary>
    public TextureShaderResourceLoaderServiceTests()
    {
        const string fragFileName = $"{TextureShaderName}.frag";
        const string vertFileName = $"{TextureShaderName}.vert";

        this.mockPath = new Mock<IPath>();
        this.mockPath.Setup(m => m.HasExtension(fragFileName)).Returns(true);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(fragFileName)).Returns(TextureShaderName);

        this.mockPath.Setup(m => m.HasExtension(vertFileName)).Returns(true);
        this.mockPath.Setup(m => m.GetFileNameWithoutExtension(vertFileName)).Returns(TextureShaderName);

        this.mockResourceLoaderService = new Mock<IEmbeddedResourceLoaderService<string>>();
        this.mockResourceLoaderService.Setup(m
                => m.LoadResource(fragFileName))
            .Returns(NoProcessingFragShaderSample);
        this.mockResourceLoaderService.Setup(m
                => m.LoadResource(vertFileName))
            .Returns(this.unprocessedFragShaderSample);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullResourceLoaderServiceParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureShaderResourceLoaderService(
                null,
                this.mockPath.Object);
        }, "The parameter must not be null. (Parameter 'resourceLoaderService')");
    }

    [Fact]
    public void Ctor_WithNullPathParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureShaderResourceLoaderService(
                this.mockResourceLoaderService.Object,
                null);
        }, "The parameter must not be null. (Parameter 'path')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadVertSource_WhenInvoked_ReturnsCorrectSourceCode()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        const string vertFileName = $"{TextureShaderName}.vert";

        // Act
        var actual = sut.LoadVertSource(TextureShaderName);

        // Assert
        this.mockResourceLoaderService.Verify(m => m.LoadResource(vertFileName), Times.Once);
        Assert.Equal(ProcessedVertShaderSample, actual);
    }

    [Fact]
    public void LoadFragSource_WhenInvoked_ReturnsCorrectSourceCode()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.LoadFragSource(TextureShaderName);

        // Assert
        Assert.Equal(NoProcessingFragShaderSample, actual);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureShaderResourceLoaderService"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureShaderResourceLoaderService CreateSystemUnderTest()
        => new (this.mockResourceLoaderService.Object, this.mockPath.Object);
}

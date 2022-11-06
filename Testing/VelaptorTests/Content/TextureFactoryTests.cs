// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System;
using Moq;
using Velaptor.Content.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Reactables.Core;
using Velaptor.Reactables.ReactableData;
using VelaptorTests.Helpers;
using Xunit;

namespace VelaptorTests.Content;

/// <summary>
/// Tests the <see cref="TextureFactory"/> class.
/// </summary>
public class TextureFactoryTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactable<DisposeTextureData>> mockDisposeTexturesReactable;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
    /// </summary>
    public TextureFactoryTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();
        this.mockDisposeTexturesReactable = new Mock<IReactable<DisposeTextureData>>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvoker_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureFactory(
                null,
                this.mockGLService.Object,
                this.mockDisposeTexturesReactable.Object);
        }, "The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLService_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureFactory(
                this.mockGL.Object,
                null,
                this.mockDisposeTexturesReactable.Object);
        }, "The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullDisposeTexturesReactableParam_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            _ = new TextureFactory(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        }, "The parameter must not be null. (Parameter 'disposeTexturesReactable')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WhenUsingNullOrEmptyName_ThrowsException(string name)
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            factory.Create(name, "test-path", new ImageData(null, 1, 2));
        }, "The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WhenUsingNullOrEmptyFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var factory = CreateFactory();

        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            factory.Create("test-name", filePath, new ImageData(null, 1, 2));
        }, "The string parameter must not be null or empty. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WhenInvoked_WorksCorrectly()
    {
        // Arrange
        var factory = CreateFactory();

        // Act
        factory.Create("test-name", "test-path", new ImageData(null, 1, 2));

        // Assert
        // NOTE: These are only here to prove that the same injected objects are the ones being used.
        this.mockGL.Verify(m => m.GenTexture(), Times.Once);
        this.mockGLService.Verify(m => m.LabelTexture(It.IsAny<uint>(), It.IsAny<string>()), Times.Once);
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureFactory CreateFactory() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockDisposeTexturesReactable.Object);
}

// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using Carbonate.UniDirectional;
using FluentAssertions;
using Moq;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureFactory"/> class.
/// </summary>
public class TextureFactoryTests
{
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IReactableFactory> mockReactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
    /// </summary>
    public TextureFactoryTests()
    {
        this.mockGL = new Mock<IGLInvoker>();
        this.mockGLService = new Mock<IOpenGLService>();

        var mockDisposeReactable = new Mock<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateDisposeTextureReactable()).Returns(mockDisposeReactable.Object);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLInvoker_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                null,
                this.mockGLService.Object,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLService_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockGL.Object,
                null,
                this.mockReactableFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockGL.Object,
                this.mockGLService.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WhenUsingNullOrEmptyName_ThrowsException(string name)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create(name, "test-path", new ImageData(null, 1, 2));

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Create_WhenUsingNullOrEmptyFilePath_ThrowsException(string filePath)
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create("test-name", filePath, new ImageData(null, 1, 2));

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The string parameter must not be null or empty. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WhenInvoked_WorksCorrectly()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Create("test-name", "test-path", new ImageData(null, 1, 2));

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
    private TextureFactory CreateSystemUnderTest() => new (
        this.mockGL.Object,
        this.mockGLService.Object,
        this.mockReactableFactory.Object);
}

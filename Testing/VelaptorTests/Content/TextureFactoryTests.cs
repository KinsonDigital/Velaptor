// <copyright file="TextureFactoryTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Drawing;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
using Velaptor.Content.Factories;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="TextureFactory"/> class.
/// </summary>
public class TextureFactoryTests
{
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IReactableFactory mockReactableFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureFactoryTests"/> class.
    /// </summary>
    public TextureFactoryTests()
    {
        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGLService = Substitute.For<IOpenGLService>();

        var mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(mockDisposeReactable);
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
                this.mockGLService,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLService_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockGL,
                null,
                this.mockReactableFactory);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new TextureFactory(
                this.mockGL,
                this.mockGLService,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }
    #endregion

    #region Method Tests
    [Fact]
    public void Create_WithNullName_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create(null, "test-path", new ImageData(new Color[0, 0]));

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Create_WithEmptyName_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create(string.Empty, "test-path", new ImageData(new Color[0, 0]));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Create_WithNullFilePath_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create("test-name", null, new ImageData(new Color[0, 0]));

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WithEmptyFilePath_ThrowsException()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var act = () => sut.Create("test-name", string.Empty, new ImageData(new Color[0, 0]));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'filePath')");
    }

    [Fact]
    public void Create_WhenInvoked_WorksCorrectly()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Create("test-name", "test-path", new ImageData(new Color[4, 4]));

        // Assert
        // NOTE: These are only here to prove that the same injected objects are the ones being used.
        this.mockGL.Received(1).GenTexture();
        this.mockGLService.Received(1).LabelTexture(Arg.Any<uint>(), Arg.Any<string>());
    }
    #endregion

    /// <summary>
    /// Creates a new instance of <see cref="TextureFactory"/> for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private TextureFactory CreateSystemUnderTest() => new (
        this.mockGL,
        this.mockGLService,
        this.mockReactableFactory);
}

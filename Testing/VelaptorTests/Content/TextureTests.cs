// <copyright file="TextureTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.NativeInterop.Services;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Xunit;

/// <summary>
/// Tests the <see cref="Texture"/> class.
/// </summary>
public class TextureTests
{
    private const string TextureName = "test-texture";
    private const string TexturePath = @"C:\temp\test-texture.png";
    private const uint TextureId = 1234;
    private readonly IGLInvoker mockGL;
    private readonly IOpenGLService mockGLService;
    private readonly IDisposable mockDisposeUnsubscriber;
    private readonly IReactableFactory mockReactableFactory;
    private readonly ImageData imageData;
    private IReceiveSubscription<DisposeTextureData>? disposeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureTests"/> class.
    /// </summary>
    public TextureTests()
    {
        this.imageData = new ImageData(new Color[2, 3]);

        /*NOTE:
         * Create the bytes in the ARGB byte layout.
         * OpenGL expects the layout to be RGBA.  The texture class changes
         * this layout to meet OpenGL requirements.
         */
        for (var y = 0; y < this.imageData.Height; y++)
        {
            for (var x = 0; x < this.imageData.Width; x++)
            {
                this.imageData.Pixels[x, y] = y switch
                {
                    0 => Color.FromArgb(255, 255, 0, 0), // Row 1
                    1 => Color.FromArgb(255, 0, 255, 0), // Row 2
                    2 => Color.FromArgb(255, 0, 0, 255), // Row 3
                    _ => throw new Exception($"Row '{y}' does not exist when setting up image data for test."),
                };

                // If the first row
                switch (y)
                {
                    case 0: // Row 1
                        this.imageData.Pixels[x, y] = Color.FromArgb(255, 255, 0, 0);
                        break;
                    case 1: // Row 2
                        this.imageData.Pixels[x, y] = Color.FromArgb(255, 0, 255, 0);
                        break;
                    case 2: // Row 3
                        this.imageData.Pixels[x, y] = Color.FromArgb(255, 0, 0, 255);
                        break;
                }
            }
        }

        this.mockGL = Substitute.For<IGLInvoker>();
        this.mockGL.GenTexture().Returns(TextureId);

        this.mockGLService = Substitute.For<IOpenGLService>();
        this.mockDisposeUnsubscriber = Substitute.For<IDisposable>();

        var mockDisposeReactable = Substitute.For<IPushReactable<DisposeTextureData>>();
        mockDisposeReactable.Subscribe(Arg.Any<IReceiveSubscription<DisposeTextureData>>())
            .Returns(this.mockDisposeUnsubscriber)
            .AndDoes(callInfo =>
            {
                var reactor = callInfo.Arg<IReceiveSubscription<DisposeTextureData>>();
                reactor.Should().NotBeNull("It is required for unit testing.");
                this.disposeReactor = reactor;
            });

        this.mockReactableFactory = Substitute.For<IReactableFactory>();
        this.mockReactableFactory.CreateDisposeTextureReactable().Returns(mockDisposeReactable);
    }

    #region Constructor Tests

    [Fact]
    public void InternalCtor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            null,
            this.mockGLService,
            this.mockReactableFactory,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'gl')");
    }

    [Fact]
    public void InternalCtor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockGL,
            null,
            this.mockReactableFactory,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void InternalCtor_WithNullReactableFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockGL,
            this.mockGLService,
            null,
            TextureName,
            TexturePath,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void InternalCtor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            null,
            TexturePath,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void InternalCtor_WithEmptyName_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            string.Empty,
            TexturePath,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void InternalCtor_WithNullFilePath_ThrowsException()
    {
        // Act & Assert
        var act = () => new Texture(
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            TextureName,
            null,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'filePath')");
    }

    [Fact]
    public void InternalCtor_WithEmptyFilePath_ThrowsException()
    {
        // Act & Assert
        var act = () => new Texture(
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            TextureName,
            string.Empty,
            this.imageData);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'filePath')");
    }

    [Fact]
    public void InternalCtor_WithEmptyImageData_ThrowsException()
    {
        // Act & Assert
        var act = () => CreateSystemUnderTest(true);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("The image data must not be empty. (Parameter 'imageData')");
    }

    [Fact]
    public void InternalCtor_WhenInvoked_UploadsTextureDataToGpu()
    {
        // Arrange
        var expectedPixelData = new List<byte>();

        // NOTE: Swap from ARGB to RGBA byte layout because this is expected by OpenGL
        for (var y = 0; y < this.imageData.Height; y++)
        {
            var rowBytes = new List<byte>();

            for (var x = 0; x < this.imageData.Width; x++)
            {
                rowBytes.Add(this.imageData.Pixels[x, y].R);
                rowBytes.Add(this.imageData.Pixels[x, y].G);
                rowBytes.Add(this.imageData.Pixels[x, y].B);
                rowBytes.Add(this.imageData.Pixels[x, y].A);
            }

            expectedPixelData.AddRange(rowBytes);
            rowBytes.Clear();
        }

        // Act
        _ = new Texture(
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            "test-texture.png",
            @"C:\temp\test-texture.png",
            this.imageData);

        // Assert
        this.mockGLService.Received(1).LabelTexture(TextureId, "test-texture.png");
        this.mockGL.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMinFilter,
            GLTextureMinFilter.Linear);

        this.mockGL.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMagFilter,
            GLTextureMagFilter.Linear);

        this.mockGL.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureWrapS,
            GLTextureWrapMode.ClampToEdge);

        this.mockGL.Received(1).TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureWrapT,
            GLTextureWrapMode.ClampToEdge);

        var expectedPixelArray = expectedPixelData.ToArray();

        this.mockGL.Received(1).TexImage2D<byte>(
            GLTextureTarget.Texture2D,
            0,
            GLInternalFormat.Rgba,
            2u,
            3u,
            0,
            GLPixelFormat.Rgba,
            GLPixelType.UnsignedByte,
            Arg.Is<byte[]>(actualPixelArray => actualPixelArray.SequenceEqual(expectedPixelArray)));

        this.mockGLService.Received(1).BindTexture2D(TextureId);
        this.mockGLService.Received(1).UnbindTexture2D();
    }

    #endregion

    #region Prop Tests

    [Fact]
    public void Id_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Id;

        // Assert
        actual.Should().Be(TextureId);
    }

    [Fact]
    public void Name_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        actual.Should().Be(TextureName);
    }

    [Fact]
    public void Path_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.FilePath;

        // Assert
        actual.Should().Be(TexturePath);
    }

    [Fact]
    public void Width_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Width;

        // Assert
        actual.Should().Be(2u);
    }

    [Fact]
    public void Height_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Height;

        // Assert
        actual.Should().Be(3u);
    }

    #endregion

    #region Method Tests

    [Fact]
    public void ReactableNotifications_WithDifferentTextureID_DoesNotDisposeOfTexture()
    {
        // Arrange
        var disposeTextureData = new DisposeTextureData { TextureId = 456u };

        CreateSystemUnderTest();

        // Act
        this.disposeReactor?.OnReceive(disposeTextureData);

        // Assert
        this.mockGL.DidNotReceive().DeleteTexture(Arg.Any<uint>());
        this.mockDisposeUnsubscriber.DidNotReceive().Dispose();
    }

    [Fact]
    public void ReactableNotifications_WhenPushingDisposeTextureNotification_DisposesOfTexture()
    {
        // Arrange
        var disposeTextureData = new DisposeTextureData { TextureId = TextureId };

        CreateSystemUnderTest();

        // Act
        this.disposeReactor?.OnReceive(disposeTextureData);

        // Assert
        // this.mockGL.Verify(m => m.DeleteTexture(TextureId), Times.Once());
        this.mockGL.Received(1).DeleteTexture(TextureId);
    }

    #endregion

    /// <summary>
    /// Creates a texture for the purpose of testing.
    /// </summary>
    /// <returns>The texture instance to test.</returns>
    private Texture CreateSystemUnderTest(bool useEmptyData = false)
        => new (
            this.mockGL,
            this.mockGLService,
            this.mockReactableFactory,
            TextureName,
            TexturePath,
            useEmptyData ? default : this.imageData);
}

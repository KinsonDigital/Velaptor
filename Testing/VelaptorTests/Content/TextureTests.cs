// <copyright file="TextureTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Collections.Generic;
using System.Drawing;
using Carbonate.Core.OneWay;
using Carbonate.OneWay;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
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
    private readonly Mock<IGLInvoker> mockGL;
    private readonly Mock<IOpenGLService> mockGLService;
    private readonly Mock<IDisposable> mockDisposeUnsubscriber;
    private readonly Mock<IReactableFactory> mockReactableFactory;
    private readonly ImageData imageData;
    private IReceiveSubscription<DisposeTextureData>? disposeReactor;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureTests"/> class.
    /// </summary>
    public TextureTests()
    {
        this.imageData = new ImageData(new Color[2, 3], 2, 3);

        /*NOTE:
         * Create the bytes in the ARGB byte layout.
         * OpenGL expects the layout to be RGBA.  The texture class changes this
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

        this.mockGL = new Mock<IGLInvoker>();
        this.mockGL.Setup(m => m.GenTexture()).Returns(TextureId);

        this.mockGLService = new Mock<IOpenGLService>();
        this.mockDisposeUnsubscriber = new Mock<IDisposable>();

        var mockDisposeReactable = new Mock<IPushReactable<DisposeTextureData>>();
        mockDisposeReactable.Setup(m => m.Subscribe(It.IsAny<IReceiveSubscription<DisposeTextureData>>()))
            .Returns(this.mockDisposeUnsubscriber.Object)
            .Callback<IReceiveSubscription<DisposeTextureData>>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");
                this.disposeReactor = reactor;
            });

        this.mockReactableFactory = new Mock<IReactableFactory>();
        this.mockReactableFactory.Setup(m => m.CreateDisposeTextureReactable()).Returns(mockDisposeReactable.Object);
    }

    #region Constructor Tests
    #region Public Constructors
    [Fact]
    public void PublicCtor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture(null, default(ImageData));
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void PublicCtor_WithEmptyName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture(string.Empty, default(ImageData));
        };

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'name')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithNameAndImageDataOverloadAndNullOrEmptyImageDataFilePath_ThrowsException(string filePath)
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture("test-name", new ImageData(new Color[1, 1], 1, 1, filePath));
        };

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The image data must have a file path associated with it. (Parameter 'imageData')");
    }

    [Fact]
    public void Ctor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture(null, "test-path");
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WithEmptyName_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture(string.Empty, "test-path");
        };

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'name')");
    }

    [Fact]
    public void Ctor_WithNullFilePath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture("test-name", null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'filePath')");
    }

    [Fact]
    public void Ctor_WithEmptyFilePath_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Texture("test-name", string.Empty);
        };

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("The value cannot be an empty string. (Parameter 'filePath')");
    }
    #endregion

    #region Internal Constructors
    [Fact]
    public void InternalCtor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            null,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
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
            this.mockGL.Object,
            null,
            this.mockReactableFactory.Object,
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
            this.mockGL.Object,
            this.mockGLService.Object,
            null,
            TextureName,
            TexturePath,
            this.imageData);

        // Assrt
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("Value cannot be null. (Parameter 'reactableFactory')");
    }

    [Fact]
    public void InternalCtor_WithNullName_ThrowsException()
    {
        // Arrange & Act
        var act = () => new Texture(
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
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
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
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
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
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
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
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

        // NOTE: Swap the from ARGB to RGBA byte layout because this is expected by OpenGL
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
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
            "test-texture.png",
            @"C:\temp\test-texture.png",
            this.imageData);

        // Assert
        this.mockGLService.Verify(m => m.LabelTexture(TextureId, "test-texture.png"),
            Times.Once());
        this.mockGL.Verify(m => m.TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMinFilter,
            GLTextureMinFilter.Linear), Times.Once());

        this.mockGL.Verify(m => m.TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureMagFilter,
            GLTextureMagFilter.Linear), Times.Once());

        this.mockGL.Verify(m => m.TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureWrapS,
            GLTextureWrapMode.ClampToEdge), Times.Once());

        this.mockGL.Verify(m => m.TexParameter(
            GLTextureTarget.Texture2D,
            GLTextureParameterName.TextureWrapT,
            GLTextureWrapMode.ClampToEdge), Times.Once());

        var expectedPixelArray = expectedPixelData.ToArray();

        this.mockGL.Verify(m => m.TexImage2D<byte>(
            GLTextureTarget.Texture2D,
            0,
            GLInternalFormat.Rgba,
            2u,
            3u,
            0,
            GLPixelFormat.Rgba,
            GLPixelType.UnsignedByte,
            expectedPixelArray), Times.Once());

        this.mockGLService.Verify(m => m.BindTexture2D(TextureId), Times.Once);
        this.mockGLService.Verify(m => m.UnbindTexture2D(), Times.Once);
    }
    #endregion
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
        this.mockGL.Verify(m => m.DeleteTexture(It.IsAny<uint>()), Times.Never);
        this.mockDisposeUnsubscriber.Verify(m => m.Dispose(), Times.Never);
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
        this.mockGL.Verify(m => m.DeleteTexture(TextureId), Times.Once());
    }
    #endregion

    /// <summary>
    /// Creates a texture for the purpose of testing.
    /// </summary>
    /// <returns>The texture instance to test.</returns>
    private Texture CreateSystemUnderTest(bool useEmptyData = false)
        => new (
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactableFactory.Object,
            TextureName,
            TexturePath,
            useEmptyData ? default : this.imageData);
}

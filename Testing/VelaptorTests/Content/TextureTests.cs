// <copyright file="TextureTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content;

using System;
using System.Collections.Generic;
using System.Drawing;
using Carbonate;
using FluentAssertions;
using Moq;
using Velaptor.Content;
using Velaptor.Graphics;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.OpenGL;
using Velaptor.ReactableData;
using Helpers;
using Velaptor;
using Velaptor.Exceptions;
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
    private readonly Mock<IReactable> mockReactable;
    private readonly Mock<IDisposable> mockDisposeUnsubscriber;
    private readonly ImageData imageData;
    private IReactor? disposeReactor;

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
                    0 => this.imageData.Pixels[x, y] = Color.FromArgb(255, 255, 0, 0), // Row 1
                    1 => this.imageData.Pixels[x, y] = Color.FromArgb(255, 0, 255, 0), // Row 2
                    2 => this.imageData.Pixels[x, y] = Color.FromArgb(255, 0, 0, 255), // Row 3
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

        this.mockReactable = new Mock<IReactable>();
        this.mockReactable.Setup(m =>
                m.Subscribe(It.IsAny<IReactor>()))
            .Returns(this.mockDisposeUnsubscriber.Object)
            .Callback<IReactor>(reactor =>
            {
                reactor.Should().NotBeNull("it is required for unit testing.");

                this.disposeReactor = reactor;
            });
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullGLParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new Texture(
                null,
                this.mockGLService.Object,
                this.mockReactable.Object,
                TextureName,
                TexturePath,
                this.imageData);
        }, "The parameter must not be null. (Parameter 'gl')");
    }

    [Fact]
    public void Ctor_WithNullOpenGLServiceParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new Texture(
                this.mockGL.Object,
                null,
                this.mockReactable.Object,
                TextureName,
                TexturePath,
                this.imageData);
        }, "The parameter must not be null. (Parameter 'openGLService')");
    }

    [Fact]
    public void Ctor_WithNullReactableParam_ThrowsException()
    {
        // Arrange & Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new Texture(
                this.mockGL.Object,
                this.mockGLService.Object,
                null,
                TextureName,
                TexturePath,
                this.imageData);
        }, "The parameter must not be null. (Parameter 'reactable')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithEmptyOrNullNameParam_ThrowsException(string name)
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new Texture(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockReactable.Object,
                name,
                TexturePath,
                this.imageData);
        }, "The string parameter must not be null or empty. (Parameter 'name')");
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Ctor_WithEmptyOrNullFilePath_ThrowsException(string filePath)
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
        {
            var unused = new Texture(
                this.mockGL.Object,
                this.mockGLService.Object,
                this.mockReactable.Object,
                TextureName,
                filePath,
                this.imageData);
        }, "The string parameter must not be null or empty. (Parameter 'filePath')");
    }

    [Fact]
    public void Ctor_WithEmptyImageData_ThrowsException()
    {
        // Act & Assert
        AssertExtensions.ThrowsWithMessage<ArgumentException>(() =>
        {
            var unused = CreateSystemUnderTest(true);
        }, "The image data must not be empty. (Parameter 'imageData')");
    }

    [Fact]
    public void Ctor_WhenInvoked_UploadsTextureDataToGPU()
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
        var unused = new Texture(
            this.mockGL.Object,
            this.mockGLService.Object,
            this.mockReactable.Object,
            "test-texture.png",
            $@"C:\temp\test-texture.png",
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

    #region Prop Tests
    [Fact]
    public void Id_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Id;

        // Assert
        Assert.Equal(TextureId, actual);
    }

    [Fact]
    public void Name_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Name;

        // Assert
        Assert.Equal(TextureName, actual);
    }

    [Fact]
    public void Path_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.FilePath;

        // Assert
        Assert.Equal(TexturePath, actual);
    }

    [Fact]
    public void Width_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Width;

        // Assert
        Assert.Equal(2u, actual);
    }

    [Fact]
    public void Height_WhenCreatingTexture_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Height;

        // Assert
        Assert.Equal(3u, actual);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void ReactableNotifications_WhenMessageIsNull_ThrowsException()
    {
        // Arrange
        var expected = $"There was an issue with the '{nameof(Texture)}.Constructor()' subscription source";
        expected += $" for subscription ID '{NotificationIds.TextureDisposedId}'.";

        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<DisposeTextureData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => null);

        _ = CreateSystemUnderTest();

        // Act
        var act = () => this.disposeReactor.OnNext(mockMessage.Object);

        // Assert
        act.Should().Throw<PushNotificationException>()
            .WithMessage(expected);
    }

    [Fact]
    public void ReactableNotifications_WithDifferentTextureID_DoesNotDisposeOfTexture()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<DisposeTextureData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => new DisposeTextureData { TextureId = 456u });

        CreateSystemUnderTest();

        // Act
        this.disposeReactor?.OnNext(mockMessage.Object);

        // Assert
        this.mockGL.Verify(m => m.DeleteTexture(It.IsAny<uint>()), Times.Never);
        this.mockDisposeUnsubscriber.Verify(m => m.Dispose(), Times.Never);
    }

    [Fact]
    public void ReactableNotifications_WhenPushingDisposeTextureNotification_DisposesOfTexture()
    {
        // Arrange
        var mockMessage = new Mock<IMessage>();
        mockMessage.Setup(m => m.GetData<DisposeTextureData>(It.IsAny<Action<Exception>?>()))
            .Returns<Action<Exception>?>(_ => new DisposeTextureData { TextureId = TextureId });

        CreateSystemUnderTest();

        // Act
        this.disposeReactor?.OnNext(mockMessage.Object);

        // Assert
        this.mockGL.Verify(m => m.DeleteTexture(TextureId), Times.Once());
        this.mockDisposeUnsubscriber.Verify(m => m.Dispose(), Times.Once);
    }

    [Fact]
    public void ReactableNotifications_WhenSendingOnCompleted_DiposesOfTexture()
    {
        // Arrange
        _ = CreateSystemUnderTest();

        // Act
        this.disposeReactor.OnComplete();

        // Assert
        this.mockGL.Verify(m => m.DeleteTexture(TextureId), Times.Once());
        this.mockDisposeUnsubscriber.Verify(m => m.Dispose(), Times.Once);
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
            this.mockReactable.Object,
            TextureName,
            TexturePath,
            useEmptyData ? default : this.imageData);
}

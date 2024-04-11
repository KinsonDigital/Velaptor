// <copyright file="ContentExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

// ReSharper disable InvokeAsExtensionMethod
namespace VelaptorTests.ExtensionMethods;

using FluentAssertions;
using Helpers;
using NSubstitute;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.ExtensionMethods;
using Xunit;

/// <summary>
/// Tests the <see cref="ContentExtensions"/> clas.
/// </summary>
public class ContentExtensionsTests
{
    #region Method Tests
    [Fact]
    public void Load_WhenLoadingFontWithNullFontName_ThrowsException()
    {
        // Arrange
        var mockFontLoader = Substitute.For<ILoader<IFont>>();

        // Act
        var act = () => ContentExtensions.Load(mockFontLoader, null, 12);

        // Assert
        act.Should().ThrowArgNullException().WithNullParamMsg("fontName");
    }

    [Fact]
    public void Load_WhenLoadingFontWithEmptyFontName_ThrowsException()
    {
        // Arrange
        var mockFontLoader = Substitute.For<ILoader<IFont>>();

        // Act
        var act = () => ContentExtensions.Load(mockFontLoader, string.Empty, 12);

        // Assert
        act.Should().ThrowArgException().WithEmptyStringParamMsg("fontName");
    }

    [Theory]
    [InlineData("test-font", 12, "test-font.ttf|size:12")]
    [InlineData("test-font.ttf", 14, "test-font.ttf|size:14")]
    [InlineData("test-font.invalid-extension", 16, "test-font.ttf|size:16")]
    public void Load_WhenLoadingFonts_LoadsFont(string fontName, uint size, string expected)
    {
        // Arrange
        var mockFont = Substitute.For<IFont>();
        var mockFontLoader = Substitute.For<ILoader<IFont>>();
        mockFontLoader.Load(Arg.Any<string>()).Returns(mockFont);

        // Act
        mockFontLoader.Load(fontName, size);

        // Assert
        mockFontLoader.Received(1).Load(expected);
    }

    [Fact]
    public void Load_WhenLoadingAtlasDataWithNullAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAtlasData>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, null);

        // Assert
        act.Should().ThrowArgNullException().WithNullParamMsg("atlasPathOrName");
    }

    [Fact]
    public void Load_WhenLoadingAtlasDataWithEmptyAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAtlasData>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, string.Empty);

        // Assert
        act.Should().ThrowArgException().WithEmptyStringParamMsg("atlasPathOrName");
    }

    [Fact]
    public void Load_WhenLoadingAudioWithNullAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAudio>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, null, AudioBuffer.Full);

        // Assert
        act.Should().ThrowArgNullException().WithNullParamMsg("audioPathOrName");
    }

    [Fact]
    public void Load_WhenLoadingAudioWithEmptyAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAudio>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, string.Empty, AudioBuffer.Full);

        // Assert
        act.Should().ThrowArgException().WithEmptyStringParamMsg("audioPathOrName");
    }

    [Fact]
    public void Load_WhenLoadingTexturesWithNullAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<ITexture>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, null);

        // Assert
        act.Should().ThrowArgNullException().WithNullParamMsg("texturePathOrName");
    }

    [Fact]
    public void Load_WhenLoadingTexturesWithEmptyAtlasName_ThrowsException()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<ITexture>>();

        // Act
        var act = () => ContentExtensions.Load(mockLoader, string.Empty);

        // Assert
        act.Should().ThrowArgException().WithEmptyStringParamMsg("texturePathOrName");
    }

    [Fact]
    public void Unload_WithNullTexture_DoesNotThrowExceptionOrAttemptUnload()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<ITexture>>();

        // Act
        var act = () => mockLoader.Unload(null);

        // Assert
        act.Should().NotThrow();
        mockLoader.DidNotReceive().Unload("test-file-path");
    }

    [Fact]
    public void Unload_WhenUnloadingTexture_UnloadsContent()
    {
        // Arrange
        var mockTexture = Substitute.For<ITexture>();
        mockTexture.FilePath.Returns("test-file-path");
        var mockLoader = Substitute.For<ILoader<ITexture>>();

        // Act
        mockLoader.Unload(mockTexture);

        // Assert
        mockLoader.Received(1).Unload("test-file-path");
    }

    [Fact]
    public void Unload_WithNullFont_DoesNotThrowExceptionOrAttemptUnload()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IFont>>();

        // Act
        var act = () => mockLoader.Unload(null);

        // Assert
        act.Should().NotThrow();
        mockLoader.DidNotReceive().Unload("test-file-path");
    }

    [Fact]
    public void Unload_WhenUnloadingFont_UnloadsContent()
    {
        // Arrange
        var mockFont = Substitute.For<IFont>();
        mockFont.FilePath.Returns("test-file-path");
        mockFont.Size.Returns(12u);

        var mockLoader = Substitute.For<ILoader<IFont>>();

        // Act
        mockLoader.Unload(mockFont);

        // Assert
        mockLoader.Received(1).Unload("test-file-path|size:12");
    }

    [Fact]
    public void Unload_WithNullSound_DoesNotThrowExceptionOrAttemptUnload()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAudio>>();

        // Act
        var act = () => mockLoader.Unload(null);

        // Assert
        act.Should().NotThrow();
        mockLoader.DidNotReceive().Unload("test-file-path");
    }

    [Fact]
    public void Unload_WhenUnloadingSound_UnloadsContent()
    {
        // Arrange
        var mockSound = Substitute.For<IAudio>();
        mockSound.FilePath.Returns("test-file-path");
        var mockLoader = Substitute.For<ILoader<IAudio>>();

        // Act
        mockLoader.Unload(mockSound);

        // Assert
        mockLoader.Received(1).Unload("test-file-path");
    }

    [Fact]
    public void Unload_WithNullAtlasData_DoesNotThrowExceptionOrAttemptUnload()
    {
        // Arrange
        var mockLoader = Substitute.For<ILoader<IAtlasData>>();

        // Act
        var act = () => mockLoader.Unload(null);

        // Assert
        act.Should().NotThrow();
        mockLoader.DidNotReceive().Unload("test-file-path");
    }

    [Fact]
    public void Unload_WhenUnloadingAtlas_UnloadsContent()
    {
        // Arrange
        var mockAtlasData = Substitute.For<IAtlasData>();
        mockAtlasData.AtlasDataFilePath.Returns("test-file-path");
        var mockLoader = Substitute.For<ILoader<IAtlasData>>();

        // Act
        mockLoader.Unload(mockAtlasData);

        // Assert
        mockLoader.Received(1).Unload("test-file-path");
    }
    #endregion
}

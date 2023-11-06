// <copyright file="ContentExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

using FluentAssertions;
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
    [Theory]
    [InlineData("test-font", 12, "test-font.ttf|size:12")]
    [InlineData("test-font.ttf", 14, "test-font.ttf|size:14")]
    [InlineData("test-font.invalid-extension", 16, "test-font.ttf|size:16")]
    public void Load_WhenInvokingILoaderOfTypeIFont_LoadsFont(string fontName, uint size, string expected)
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
        var mockLoader = Substitute.For<ILoader<ISound>>();

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
        var mockSound = Substitute.For<ISound>();
        mockSound.FilePath.Returns("test-file-path");
        var mockLoader = Substitute.For<ILoader<ISound>>();

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

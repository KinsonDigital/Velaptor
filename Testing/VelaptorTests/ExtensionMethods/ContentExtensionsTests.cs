// <copyright file="ContentExtensionsTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.ExtensionMethods;

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
    #endregion
}

// <copyright file="LabelTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Moq;
using Velaptor;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.UI;
using Xunit;
using VelFontStyle = Velaptor.Content.Fonts.FontStyle;

/// <summary>
/// Tests the <see cref="Label"/> class.
/// </summary>
public class LabelTests
{
    private const int LabelLayer = int.MaxValue - 1;
    private const string TextValue = "hello world";
    private readonly Mock<ILoader<IFont>> mockFontLoader;
    private readonly Mock<IFont> mockFont;
    private readonly Mock<IAppInput<KeyboardState>> mockKeyboard;
    private readonly Mock<IAppInput<MouseState>> mockMouse;
    private readonly Mock<IFontRenderer> mockFontRenderer;

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelTests"/> class.
    /// </summary>
    public LabelTests()
    {
        this.mockFont = new Mock<IFont>();
        this.mockFont.SetupGet(p => p.Size).Returns(12u);
        this.mockFont.SetupGet(p => p.FilePath).Returns("font-file-path");
        MockGlyphs(TextValue);

        this.mockFontLoader = new Mock<ILoader<IFont>>();
        this.mockFontLoader.Setup(m => m.Load(It.IsAny<string>()))
            .Returns(this.mockFont.Object);

        this.mockKeyboard = new Mock<IAppInput<KeyboardState>>();
        this.mockMouse = new Mock<IAppInput<MouseState>>();

        this.mockFontRenderer = new Mock<IFontRenderer>();
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullFontLoaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Label(
                null,
                this.mockKeyboard.Object,
                this.mockMouse.Object);
        };

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'fontLoader')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Text_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Text = "test-value";
        var actual = sut.Text;

        // Assert
        actual.Should().Be("test-value");
    }

    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Point(11, 22);
        var actual = sut.Position;

        // Assert
        actual.Should().Be(new Point(11, 22));
    }

    [Fact]
    public void Position_WhenContentIsNotLoaded_CreatesCorrectCharacterBounds()
    {
        // Arrange
        const string labelText = "xunit";

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(100, 200);
        sut.Width = 108;
        sut.Text = labelText;

        // Act
        var actual = sut.CharacterBounds;

        // Assert
        actual.Should().BeEmpty();
        this.mockFont.Verify(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()), Times.Never);
    }

    [Fact]
    public void Position_WhenContentIsLoaded_CreatesCorrectCharacterBounds()
    {
        // Arrange
        const string labelText = "xunit";
        var characters = labelText.ToArray();

        var sut = CreateSystemUnderTest();
        sut.AutoSize = false;
        var mockedCharBounds = new List<(char character, RectangleF bounds)>
        {
            (characters[0], RectangleF.Empty),
            (characters[1], RectangleF.Empty),
            (characters[2], RectangleF.Empty),
            (characters[3], RectangleF.Empty),
            (characters[4], RectangleF.Empty),
        };

        this.mockFont.Setup(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()))
            .Returns(mockedCharBounds);

        sut.Position = new Point(100, 200);
        sut.Width = 108;
        sut.Text = labelText;
        sut.LoadContent();

        // Act
        var actual = sut.CharacterBounds;

        // Assert
        actual.Count.Should().Be(labelText.Length);
        this.mockFont.Verify(m => m.GetCharacterBounds(labelText, new Vector2(46, 200)), Times.Once);
    }

    [Fact]
    public void AutoSize_WhenGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSize;

        // Assert
        actual.Should().BeTrue($"the default value of the '{nameof(Label.AutoSize)}' property must be true.");
    }

    [Fact]
    public void AutoSize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.AutoSize = false;

        // Assert
        sut.AutoSize.Should().BeFalse();
    }

    [Theory]
    [InlineData(true, 123u, 0u)]
    [InlineData(false, 123u, 123u)]
    public void Width_WhenSettingValue_ReturnsCorrectResult(
        bool autoSize,
        uint width,
        uint expected)
    {
        // Arrange
        const string testText = "xunit";
        this.mockFont.Setup(m => m.Measure(testText)).Returns(new SizeF(expected, 20));

        var sut = CreateSystemUnderTest();
        sut.AutoSize = autoSize;
        sut.Text = testText;
        sut.LoadContent();
        sut.Width = width;

        // Act
        var actual = sut.Width;

        // Assert
        actual.Should().Be(expected);
    }

    [Theory]
    [InlineData(true, 123u, 0u)]
    [InlineData(false, 123u, 123u)]
    public void Height_WhenSettingValue_ReturnsCorrectResult(
        bool autoSize,
        uint height,
        uint expected)
    {
        // Arrange
        const string testText = "xunit";
        this.mockFont.Setup(m => m.Measure(testText)).Returns(new SizeF(10, expected));

        var sut = CreateSystemUnderTest();
        sut.AutoSize = autoSize;
        sut.Text = testText;
        sut.LoadContent();
        sut.Height = height;

        // Act
        var actual = sut.Height;

        // Assert
        actual.Should().Be(expected);
    }

    [Fact]
    public void Style_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        this.mockFont.SetupProperty(p => p.Style);

        var sut = CreateSystemUnderTest();

        // Act
        sut.Style = FontStyle.Italic;

        // Assert
        sut.Style.Should().Be(FontStyle.Italic);
    }

    [Fact]
    public void WidthAndHeight_WhenTextIsEmpty_CalculatesCorrectResults()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        sut.Text = string.Empty;
        sut.LoadContent();

        // Act
        var actualWidth = sut.Width;
        var actualHeight = sut.Height;

        // Assert
        actualWidth.Should().Be(0);
        actualHeight.Should().Be(0);
    }

    [Fact]
    public void Color_WhenNotGettingDefaultValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Color;

        // Assert
        actual.Should().Be(Color.Black);
    }

    [Fact]
    public void Color_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Color = Color.FromArgb(11, 22, 33, 44);
        var actual = sut.Color;

        // Assert
        actual.Should().Be(Color.FromArgb(11, 22, 33, 44));
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadContent_WhenInvoked_LoadsCorrectFont()
    {
        // Arrange
        var expected = new ReadOnlyCollection<(char, RectangleF)>(
            new[]
            {
                ('a', new RectangleF(1f, 2f, 3f, 4f)),
                ('b', new RectangleF(5f, 6f, 7f, 8f)),
            });

        const string testText = "test-value";
        var position = new Vector2(11f, 22f);
        this.mockFont.Setup(m => m.GetCharacterBounds(testText, position))
            .Returns(() => expected);

        var sut = CreateSystemUnderTest();
        sut.Text = testText;
        sut.Position = position.ToPoint();

        // Act
        sut.LoadContent();
        sut.LoadContent();

        // Assert
        sut.CharacterBounds.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void LoadContent_WhenInvoked_CreatesCorrectCharacterBounds()
    {
        // Arrange
        const string labelText = "test-value";
        var characters = labelText.ToArray();

        var sut = CreateSystemUnderTest();
        var mockedCharBounds = new List<(char character, RectangleF bounds)>
        {
            (characters[0], RectangleF.Empty),
            (characters[1], RectangleF.Empty),
            (characters[2], RectangleF.Empty),
            (characters[3], RectangleF.Empty),
            (characters[4], RectangleF.Empty),
            (characters[5], RectangleF.Empty),
            (characters[6], RectangleF.Empty),
            (characters[7], RectangleF.Empty),
            (characters[8], RectangleF.Empty),
            (characters[9], RectangleF.Empty),
        };

        this.mockFont.Setup(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()))
            .Returns(mockedCharBounds);

        sut.AutoSize = false;
        sut.Position = new Point(100, 200);
        sut.Width = 108;
        sut.Text = labelText;
        sut.LoadContent();

        // Act
        var actual = sut.CharacterBounds;

        // Assert
        actual.Count.Should().Be(labelText.Length);
        this.mockFont.Verify(m => m.GetCharacterBounds(labelText, new Vector2(46, 200)), Times.Once);
    }

    [Fact]
    public void UnloadContent_WhenLoadedAndNotDisposed_UnloadsContent()
    {
        // Arrange
        var expected = $"{this.mockFont.Object.FilePath}|size:12";
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        this.mockFontLoader.Verify(m => m.Unload(expected), Times.Once);
    }

    [Fact]
    public void UnloadContent_WhenNotLoaded_DoesNotUnloadContent()
    {
        // Arrange
        var expected = $"{this.mockFont.Object.FilePath}|size:12";
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();

        // Assert
        this.mockFontLoader.Verify(m => m.Unload(expected), Times.Never);
    }

    [Fact]
    public void Render_WhenInvokedWithoutLoadedContent_DoesNotRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Visible = true;

        // Act
        sut.Render("test-value", 123);

        // Assert
        this.mockFontRenderer.Verify(m => m.Render(It.IsAny<IFont>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>(),
            It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WhenInvokedWhileNotVisible_DoesNotRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Visible = false;

        // Act
        sut.LoadContent();
        sut.Render("test-value", 123);

        // Assert
        this.mockFontRenderer.Verify(m => m.Render(It.IsAny<IFont>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>(),
            It.IsAny<int>()), Times.Never);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Render_WithNullOrEmptyText_DoesNotRender(string text)
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Text = string.Empty;
        sut.LoadContent();

        // Act
        sut.Render(text, 123);

        // Assert
        this.mockFontRenderer.Verify(m => m.Render(It.IsAny<IFont>(),
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<float>(),
            It.IsAny<float>(),
            It.IsAny<Color>(),
            It.IsAny<int>()), Times.Never);
    }
    #endregion

    /// <summary>
    /// Mocks the glyphs for the given <param name="text"></param>.
    /// </summary>
    /// <param name="text">The text glyphs to mock for the font object.</param>
    private void MockGlyphs(string text)
    {
        this.mockFont.SetupGet(p => p.Metrics)
            .Returns(() =>
            {
                var result = new List<GlyphMetrics>();

                for (var i = 0; i < text.Length; i++)
                {
                    var charIndex = i;
                    var alreadyAdded = result.Exists(g => g.Glyph == text[charIndex]);

                    if (!alreadyAdded)
                    {
                        result.Add(new GlyphMetrics
                        {
                            Glyph = text[charIndex],
                            GlyphWidth = charIndex,
                            Ascender = i + 1,
                            GlyphHeight = charIndex * 10,
                        });
                    }
                }

                return new ReadOnlyCollection<GlyphMetrics>(result);
            });
    }

    /// <summary>
    /// Creates a new sut for the purpose of testing.
    /// </summary>
    /// <returns>The instance to test.</returns>
    private Label CreateSystemUnderTest() =>
        new (this.mockFontLoader.Object,
            this.mockKeyboard.Object,
            this.mockMouse.Object);
}

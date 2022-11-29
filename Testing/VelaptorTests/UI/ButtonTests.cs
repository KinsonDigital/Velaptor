// <copyright file="ButtonTests.cs" company="KinsonDigital">
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
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Input;
using Velaptor.UI;
using Helpers;
using Xunit;

/// <summary>
/// Tests the <see cref="Button"/> class.
/// </summary>
public class ButtonTests
{
    private const string ButtonTextValue = "test-value";
    private const string TextureName = "sut-face-small";
    private readonly Mock<IContentLoader> mockContentLoader;
    private readonly Mock<ITexture> mockTexture;
    private readonly Mock<IFont> mockFont;
    private readonly Mock<IUIControlFactory> mockControlFactory;
    private readonly Mock<IAppInput<MouseState>> mockMouse;
    private readonly Label label;

    /// <summary>
    /// Initializes a new instance of the <see cref="ButtonTests"/> class.
    /// </summary>
    public ButtonTests()
    {
        this.mockTexture = new Mock<ITexture>();
        this.mockTexture.SetupGet(p => p.Width).Returns(200);
        this.mockTexture.SetupGet(p => p.Height).Returns(100);

        var glyphMetrics = new[]
        {
            new GlyphMetrics()
            {
                Ascender = 1, Descender = 2, CharIndex = 3,
                Glyph = 'c', GlyphWidth = 4, GlyphHeight = 5,
                HoriBearingX = 6, HoriBearingY = 7, XMin = 8,
                XMax = 9, YMin = 11, YMax = 22,
                HorizontalAdvance = 33, GlyphBounds = new Rectangle(44, 55, 66, 77),
            },
            new GlyphMetrics()
            {
                Ascender = 11, Descender = 22, CharIndex = 33,
                Glyph = 'c', GlyphWidth = 44, GlyphHeight = 55,
                HoriBearingX = 66, HoriBearingY = 77, XMin = 88,
                XMax = 99, YMin = 111, YMax = 222,
                HorizontalAdvance = 333, GlyphBounds = new Rectangle(444, 555, 666, 777),
            },
        };

        this.mockFont = new Mock<IFont>();
        this.mockFont.SetupGet(p => p.Metrics).Returns(new ReadOnlyCollection<GlyphMetrics>(glyphMetrics));
        MockGlyphs(ButtonTextValue);

        this.mockContentLoader = new Mock<IContentLoader>();
        this.mockContentLoader.Setup(m => m.LoadTexture(TextureName))
            .Returns(this.mockTexture.Object);
        this.mockContentLoader.Setup(m => m.LoadFont("TimesNewRoman", 12))
            .Returns(this.mockFont.Object);

        this.mockMouse = new Mock<IAppInput<MouseState>>();

        this.label = new Label(this.mockContentLoader.Object, this.mockFont.Object, this.mockMouse.Object);

        this.mockControlFactory = new Mock<IUIControlFactory>();
        this.mockControlFactory.Setup(m => m.CreateLabel(It.IsAny<string>(), It.IsAny<IFont>()))
            .Returns(this.label);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullContentLoaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Button(null, this.mockControlFactory.Object, this.mockMouse.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'contentLoader')");
    }

    [Fact]
    public void Ctor_WithNullControlFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Button(this.mockContentLoader.Object, null, this.mockMouse.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'controlFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Point(11, 22);
        var sut = CreateSystemUnderTest();

        // Act
        sut.Position = new Point(11, 22);
        var actual = sut.Position;

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AutoSize_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSize;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void AutoSize_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.AutoSize = false;

        // Assert
        Assert.False(sut.AutoSize);
    }

    [Fact]
    public void AutoSize_WhenTurningOffAutoSize_SetsWidthAngHeightBeforeAutoSizeWasTurnedOn()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.AutoSize = false;
        sut.Width = 123u;
        sut.Height = 456u;
        sut.AutoSize = true;

        // Act
        sut.AutoSize = false;

        // Assert
        AssertExtensions.EqualWithMessage(123u, sut.Width, "The width is incorrect.");
        AssertExtensions.EqualWithMessage(456u, sut.Height, "The height is incorrect.");
    }

    [Theory]
    [InlineData(true, 50u)]
    [InlineData(false, 40u)]
    public void Width_WhenSettingValueWithAutoSizeTurnedOn_ReturnsCorrectResult(
        bool autoSize,
        uint expectedWidth)
    {
        // Arrange
        this.mockFont.Setup(m => m.Measure(It.IsAny<string>())).Returns(new SizeF(30, 40));
        this.label.AutoSize = true;
        var sut = CreateSystemUnderTest();
        sut.LoadContent();
        sut.AutoSize = autoSize;

        // Act
        sut.Width = 40;

        // Assert
        Assert.Equal(expectedWidth, sut.Width);
    }

    [Fact]
    public void Text_WhenSettingValueBeforeLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.Text = ButtonTextValue;
        var actual = sut.Text;

        // Assert
        Assert.Equal(ButtonTextValue, actual);
    }

    [Fact]
    public void Text_WhenSettingValueAfterLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.Text = "test-value";
        var actual = sut.Text;

        // Assert
        Assert.Equal("test-value", actual);
    }

    [Fact]
    public void BorderVisible_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.BorderVisible;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void BorderVisible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderVisible = true;

        // Assert
        Assert.True(sut.BorderVisible);
    }

    [Fact]
    public void BorderColor_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.BorderColor;

        // Assert
        Assert.Equal(Color.SlateGray, actual);
    }

    [Fact]
    public void BorderColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderColor = Color.Red;

        // Assert
        Assert.Equal(Color.Red, sut.BorderColor);
    }

    [Fact]
    public void BorderThickness_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderThickness = 123u;

        // Assert
        Assert.Equal(123u, sut.BorderThickness);
    }

    [Fact]
    public void FaceColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.FaceColor = Color.CornflowerBlue;

        // Assert
        Assert.Equal(Color.CornflowerBlue, sut.FaceColor);
    }

    [Fact]
    public void CornerRadius_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new CornerRadius(10f, 20f, 30f, 40f);
        var sut = CreateSystemUnderTest();

        // Act
        sut.CornerRadius = new CornerRadius(10f, 20f, 30f, 40f);

        // Assert
        Assert.Equal(expected, sut.CornerRadius);
    }

    [Fact]
    public void FontSize_WhenSettingValue_SetsAndReturnsLabelFontSize()
    {
        // Arrange
        this.mockFont.SetupProperty(p => p.Size);
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.FontSize = 123u;

        // Assert
        Assert.Equal(123u, sut.FontSize);
    }

    [Fact]
    public void Enabled_WhenSettingValueBeforeLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Enabled;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void Enabled_WhenSettingValueAfterLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.Enabled = false;

        // Assert
        Assert.False(sut.Enabled);
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadContent_WhenInvoked_LoadsContent()
    {
        // Arrange
        this.mockContentLoader.Setup(m => m.LoadFont("TimesNewRoman-Regular.ttf", 12))
            .Returns(this.mockFont.Object);

        var sut = CreateSystemUnderTest();
        sut.Text = "test-value";
        sut.Enabled = false;
        sut.Position = new Point(11, 22);

        // Act
        sut.LoadContent();
        sut.LoadContent();

        // Assert
        Assert.Equal("test-value", sut.Text);
        Assert.False(sut.Enabled);
        Assert.Equal(new Point(11, 22), sut.Position);
        this.mockContentLoader.Verify(m => m.LoadFont("TimesNewRoman-Regular.ttf", 12), Times.Once);
    }

    [Fact]
    public void UnloadContent_WhenInvoked_UnloadsContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.UnloadContent();

        // Assert
        Assert.False(sut.IsLoaded);
    }

    [Fact]
    public void UnloadContent_WhenAlreadyUnloaded_DoesNotUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();

        // Assert
        Assert.False(sut.IsLoaded);
        this.mockContentLoader.Verify(m => m.UnloadTexture(this.mockTexture.Object), Times.Never);
    }

    [Fact]
    public void Render_WithNoLoadedContentAndVisible_DoesNotRender()
    {
        // Arrange
        var mockRenderer = new Mock<IRenderer>();
        var sut = CreateSystemUnderTest();
        sut.Visible = true;

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        mockRenderer.Verify(m =>
            m.Render(It.IsAny<ITexture>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>(),
                It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WithLoadedContentAndInvisible_DoesNotRender()
    {
        // Arrange
        var mockRenderer = new Mock<IRenderer>();
        var sut = CreateSystemUnderTest();
        sut.Visible = false;
        sut.LoadContent();

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        mockRenderer.Verify(m =>
            m.Render(It.IsAny<ITexture>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>(),
                It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WithNullTexture_DoesNotRender()
    {
        // Arrange
        var mockRenderer = new Mock<IRenderer>();

        this.mockContentLoader.Setup(m => m.LoadTexture(TextureName))
            .Returns(() =>
            {
                ITexture? nullTexture = null;

#pragma warning disable 8603
                return nullTexture;
#pragma warning restore 8603
            });
        var sut = CreateSystemUnderTest();
        sut.Visible = true;
        sut.LoadContent();

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        mockRenderer.Verify(m =>
            m.Render(It.IsAny<ITexture>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<Color>(),
                It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WhenInvoked_RendersButtonFace()
    {
        // Arrange
        var actual = default(RectShape);

        var mockRenderer = new Mock<IRenderer>();
        mockRenderer.Setup(m => m.Render(It.IsAny<RectShape>(), It.IsAny<int>()))
            .Callback<RectShape, int>((rectangle, _) =>
            {
                // Only capture the sut face rectangle
                if (rectangle.IsFilled)
                {
                    actual = rectangle;
                }
            });

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(400, 600);
        sut.BorderVisible = false;
        sut.Visible = true;
        sut.Enabled = true;
        sut.AutoSize = false;
        sut.Width = 100;
        sut.Height = 50;
        sut.BorderThickness = 123u;
        sut.CornerRadius = new CornerRadius(11, 22, 33, 44);
        sut.LoadContent();

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        mockRenderer.Verify(m => m.Render(actual, 0), Times.Once);

        // Assert that the values of the rectangle were set correctly
        AssertExtensions.TypeMemberEquals(
            new Vector2(400, 600),
            actual.Position,
            nameof(RectShape),
            nameof(RectShape.Position));
        AssertExtensions.TypeMemberTrue(actual.IsFilled, nameof(RectShape), nameof(RectShape.IsFilled));
        AssertExtensions.TypeMemberEquals(
            0u,
            actual.BorderThickness,
            nameof(RectShape),
            nameof(RectShape.BorderThickness));
        AssertExtensions.TypeMemberEquals(
            Color.DarkGray,
            actual.Color,
            nameof(RectShape),
            nameof(RectShape.BorderThickness));
        AssertExtensions.TypeMemberEquals(
            100u,
            actual.Width,
            nameof(RectShape),
            nameof(RectShape.Width));
        AssertExtensions.TypeMemberEquals(
            50u,
            actual.Height,
            nameof(RectShape),
            nameof(RectShape.Height));
        AssertExtensions.TypeMemberEquals(
            11f,
            actual.CornerRadius.TopLeft,
            nameof(RectShape),
            $"{nameof(RectShape.CornerRadius)}.{nameof(RectShape.CornerRadius.TopLeft)}");
        AssertExtensions.TypeMemberEquals(
            22f,
            actual.CornerRadius.BottomLeft,
            nameof(RectShape),
            $"{nameof(RectShape.CornerRadius)}.{nameof(RectShape.CornerRadius.BottomLeft)}");
        AssertExtensions.TypeMemberEquals(
            33f,
            actual.CornerRadius.BottomRight,
            nameof(RectShape),
            $"{nameof(RectShape.CornerRadius)}.{nameof(RectShape.CornerRadius.BottomRight)}");
        AssertExtensions.TypeMemberEquals(
            44f,
            actual.CornerRadius.TopRight,
            nameof(RectShape),
            $"{nameof(RectShape.CornerRadius)}.{nameof(RectShape.CornerRadius.TopRight)}");
    }

    [Fact]
    public void Render_MouseOverButtonAndLeftMouseButtonUp_RendersButtonWithCorrectMouseHoverColor()
    {
        // Arrange
        var expected = Color.FromArgb(1, 2, 3, 4);
        var btnFace = default(RectShape);

        var mouseState = default(MouseState);
        mouseState.SetPosition(400, 600);

        this.mockMouse.Setup(m => m.GetState())
            .Returns(mouseState);

        var mockRenderer = new Mock<IRenderer>();
        mockRenderer.Setup(m => m.Render(It.IsAny<RectShape>(), It.IsAny<int>()))
            .Callback<RectShape, int>((rectangle, _) =>
            {
                btnFace = rectangle;
            });

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(400, 600);
        sut.BorderVisible = false;
        sut.Visible = true;
        sut.Enabled = true;
        sut.AutoSize = false;
        sut.MouseHoverColor = Color.FromArgb(1, 2, 3, 4);
        sut.Width = 100;
        sut.Height = 50;
        sut.BorderThickness = 123u;
        sut.CornerRadius = new CornerRadius(11, 22, 33, 44);
        sut.LoadContent();
        sut.Update(default);

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        Assert.Equal(expected, btnFace.Color);
    }

    [Fact]
    public void Render_MouseOverButtonAndLeftMouseButtonDown_RendersButtonWithCorrectMouseHoverColor()
    {
        // Arrange
        var expected = Color.FromArgb(1, 2, 3, 4);
        var btnFace = default(RectShape);

        var mouseState = default(MouseState);
        mouseState.SetPosition(400, 600);
        mouseState.SetButtonState(MouseButton.LeftButton, true);

        this.mockMouse.Setup(m => m.GetState())
            .Returns(mouseState);

        var mockRenderer = new Mock<IRenderer>();
        mockRenderer.Setup(m => m.Render(It.IsAny<RectShape>(), It.IsAny<int>()))
            .Callback<RectShape, int>((rectangle, _) =>
            {
                btnFace = rectangle;
            });

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(400, 600);
        sut.BorderVisible = false;
        sut.Visible = true;
        sut.Enabled = true;
        sut.AutoSize = false;
        sut.MouseDownColor = Color.FromArgb(1, 2, 3, 4);
        sut.Width = 100;
        sut.Height = 50;
        sut.BorderThickness = 123u;
        sut.CornerRadius = new CornerRadius(11, 22, 33, 44);
        sut.LoadContent();
        sut.Update(default);

        // Act
        sut.Render(mockRenderer.Object);

        // Assert
        Assert.Equal(expected, btnFace.Color);
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
                    var alreadyAdded = result.Any(g => g.Glyph == text[charIndex]);

                    if (alreadyAdded is false)
                    {
                        result.Add(new GlyphMetrics { Glyph = text[charIndex], GlyphWidth = charIndex, GlyphHeight = charIndex * 10 });
                    }
                }

                return new ReadOnlyCollection<GlyphMetrics>(result);
            });
    }

    /// <summary>
    /// Creates a new sut for the purpose of testing.
    /// </summary>
    /// <returns>The sut instance to test.</returns>
    private Button CreateSystemUnderTest()
        => new (this.mockContentLoader.Object,
            this.mockControlFactory.Object,
            this.mockMouse.Object);
}

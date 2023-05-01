// <copyright file="ButtonTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

#pragma warning disable CS8509
namespace VelaptorTests.UI;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Helpers;
using Moq;
using Velaptor.Content;
using Velaptor.Content.Fonts;
using Velaptor.Factories;
using Velaptor.Graphics;
using Velaptor.Graphics.Renderers;
using Velaptor.Input;
using Velaptor.UI;
using Xunit;

/// <summary>
/// Tests the <see cref="Button"/> class.
/// </summary>
public class ButtonTests
{
    private const string DefaultRegularFont = "TimesNewRoman-Regular.ttf";
    private const int BorderLayer = int.MaxValue - 2;
    private const string ButtonTextValue = "test-value";
    private const string TextureName = "sut-face-small";
    private readonly Mock<IContentLoader> mockContentLoader;
    private readonly Mock<ITexture> mockTexture;
    private readonly Mock<IFont> mockFont;
    private readonly Mock<IUIControlFactory> mockControlFactory;
    private readonly Mock<IAppInput<KeyboardState>> mockKeyboard;
    private readonly Mock<IAppInput<MouseState>> mockMouse;
    private readonly Mock<IRendererFactory> mockRenderFactory;
    private readonly Label label;
    private readonly Mock<IShapeRenderer> mockShapeRenderer;
    private readonly Mock<IFontRenderer> mockFontRenderer;

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
            new GlyphMetrics
            {
                Ascender = 1, Descender = 2, CharIndex = 3,
                Glyph = 'c', GlyphWidth = 4, GlyphHeight = 5,
                HoriBearingX = 6, HoriBearingY = 7, XMin = 8,
                XMax = 9, YMin = 11, YMax = 22,
                HorizontalAdvance = 33, GlyphBounds = new Rectangle(44, 55, 66, 77),
            },
            new GlyphMetrics
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
        this.mockContentLoader.Setup(m => m.LoadFont(DefaultRegularFont, 12))
            .Returns(this.mockFont.Object);

        this.mockKeyboard = new Mock<IAppInput<KeyboardState>>();
        this.mockMouse = new Mock<IAppInput<MouseState>>();

        this.mockShapeRenderer = new Mock<IShapeRenderer>();
        this.mockFontRenderer = new Mock<IFontRenderer>();

        this.mockRenderFactory = new Mock<IRendererFactory>();
        this.mockRenderFactory.Setup(m => m.CreateShapeRenderer())
            .Returns(this.mockShapeRenderer.Object);
        this.mockRenderFactory.Setup(m => m.CreateFontRenderer())
            .Returns(this.mockFontRenderer.Object);

        this.label = new Label(
            this.mockContentLoader.Object,
            this.mockKeyboard.Object,
            this.mockMouse.Object,
            this.mockRenderFactory.Object);

        this.mockControlFactory = new Mock<IUIControlFactory>();
        this.mockControlFactory.Setup(m => m.CreateLabel(It.IsAny<string>()))
            .Returns(this.label);
    }

    #region Constructor Tests
    [Fact]
    public void Ctor_WithNullContentLoaderParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Button(
                null,
                this.mockControlFactory.Object,
                this.mockKeyboard.Object,
                this.mockMouse.Object,
                this.mockRenderFactory.Object);
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
            _ = new Button(
                this.mockContentLoader.Object,
                null,
                this.mockKeyboard.Object,
                this.mockMouse.Object,
                this.mockRenderFactory.Object);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'controlFactory')");
    }

    [Fact]
    public void Ctor_WithNullRendererFactoryParam_ThrowsException()
    {
        // Arrange & Act
        var act = () =>
        {
            _ = new Button(
                this.mockContentLoader.Object,
                this.mockControlFactory.Object,
                this.mockKeyboard.Object,
                this.mockMouse.Object,
                null);
        };

        // Assert
        act.Should()
            .Throw<ArgumentNullException>()
            .WithMessage("The parameter must not be null. (Parameter 'rendererFactory')");
    }
    #endregion

    #region Prop Tests
    [Fact]
    public void Position_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var expected = new Point(11, 22);
        var sut = CreateSystemUnderTest();
        sut.LoadContent();

        // Act
        sut.Position = new Point(11, 22);
        var actual = sut.Position;

        // Assert
        actual.Should().BeEquivalentTo(expected);
        this.label.Position.Should().Be(new Point(11, 22));
    }

    [Fact]
    public void AutoSize_WhenGettingDefaultValue_ReturnsTrue()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.AutoSize;

        // Assert
        actual.Should().BeTrue();
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
        sut.Width.Should().Be(123u, "The width is incorrect.");
        sut.Height.Should().Be(456u, "The height is incorrect.");
    }

    [Theory]
    [InlineData(true, 50u)]
    [InlineData(false, 40u)]
    public void Width_WhenSettingValueWithAutoSizeTurnedOn_ReturnsCorrectResult(
        bool autoSize,
        uint expected)
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
        Assert.Equal(expected, sut.Width);
        sut.Width.Should().Be(expected);
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
        actual.Should().Be(ButtonTextValue);
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
        actual.Should().Be("test-value");
    }

    [Fact]
    public void BorderVisible_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.BorderVisible;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void BorderVisible_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderVisible = true;

        // Assert
        sut.BorderVisible.Should().BeTrue();
    }

    [Fact]
    public void BorderColor_WhenGettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.BorderColor;

        // Assert
        actual.Should().Be(Color.SlateGray);
    }

    [Fact]
    public void BorderColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderColor = Color.Red;

        // Assert
        sut.BorderColor.Should().Be(Color.Red);
    }

    [Fact]
    public void BorderThickness_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.BorderThickness = 123u;

        // Assert
        sut.BorderThickness.Should().Be(123u);
    }

    [Fact]
    public void FaceColor_WhenSettingValue_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.FaceColor = Color.CornflowerBlue;

        // Assert
        sut.FaceColor.Should().Be(Color.CornflowerBlue);
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
        sut.CornerRadius.Should().Be(expected);
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
        sut.FontSize.Should().Be(123u);
    }

    [Fact]
    public void Enabled_WhenSettingValueBeforeLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        var actual = sut.Enabled;

        // Assert
        actual.Should().BeTrue();
    }

    [Fact]
    public void Enabled_WhenSettingValueAfterLoadingContent_ReturnsCorrectResult()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.LoadContent();
        sut.LoadContent();

        // Act
        sut.Enabled = false;

        // Assert
        sut.Enabled.Should().BeFalse();
    }
    #endregion

    #region Method Tests
    [Fact]
    public void LoadContent_WhenInvoked_LoadsContent()
    {
        // Arrange
        this.mockContentLoader.Setup(m => m.LoadFont(DefaultRegularFont, 12))
            .Returns(this.mockFont.Object);

        var sut = CreateSystemUnderTest();
        sut.Text = "test-value";
        sut.Enabled = false;
        sut.Position = new Point(11, 22);

        // Act
        sut.LoadContent();
        sut.LoadContent();

        // Assert
        sut.Text.Should().Be("test-value");
        sut.Enabled.Should().BeFalse();
        sut.Position.Should().Be(new Point(11, 22));
        this.mockContentLoader.VerifyOnce(m => m.LoadFont(DefaultRegularFont, 12));
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
        sut.IsLoaded.Should().BeFalse();
    }

    [Fact]
    public void UnloadContent_WhenAlreadyUnloaded_DoesNotUnloadContent()
    {
        // Arrange
        var sut = CreateSystemUnderTest();

        // Act
        sut.UnloadContent();

        // Assert
        sut.IsLoaded.Should().BeFalse();
        this.mockContentLoader.Verify(m => m.UnloadTexture(this.mockTexture.Object), Times.Never);
    }

    [Fact]
    public void Render_WithNoLoadedContentAndVisible_DoesNotRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Visible = true;

        // Act
        sut.Render();

        // Assert
        this.mockShapeRenderer.Verify(m =>
            m.Render(It.IsAny<RectShape>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WithLoadedContentAndInvisible_DoesNotRender()
    {
        // Arrange
        var sut = CreateSystemUnderTest();
        sut.Visible = false;
        sut.LoadContent();

        // Act
        sut.Render();

        // Assert
        this.mockShapeRenderer.Verify(m =>
            m.Render(It.IsAny<RectShape>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public void Render_WhenInvoked_RendersButtonFace()
    {
        // Arrange
        var expected = default(RectShape);
        expected.Position = new Vector2(400f, 600f);
        expected.IsSolid = true;
        expected.Color = Color.DarkGray;
        expected.Width = 100u;
        expected.Height = 50f;
        expected.CornerRadius = new CornerRadius(11f, 22f, 33f, 44f);
        expected.Top = 574f;
        expected.Bottom = 625f;

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(400, 600);
        sut.BorderVisible = false;
        sut.Visible = true;
        sut.Enabled = true;
        sut.AutoSize = false;
        sut.Width = 100;
        sut.Height = 50;
        sut.BorderThickness = 0u;
        sut.CornerRadius = new CornerRadius(11, 22, 33, 44);
        sut.LoadContent();

        // Act
        sut.Render();

        // Assert
        this.mockShapeRenderer.Verify(m =>
            m.Render(expected, It.IsAny<int>()), Times.Once);
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

        this.mockShapeRenderer.Setup(m => m.Render(It.IsAny<RectShape>(), It.IsAny<int>()))
            .Callback<RectShape, int>((rectangle, _) => btnFace = rectangle);

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
        sut.Render();

        // Assert
        btnFace.Color.Should().Be(expected);
    }

    [Fact]
    public void Render_MouseOverButtonAndLeftMouseButtonDown_RendersButtonWithCorrectMouseHoverColor()
    {
        // Arrange
        var totalGetStateInvokes = 0;
        var expected = Color.FromArgb(1, 2, 3, 4);
        var btnFace = default(RectShape);

        var mouseDownState = default(MouseState);
        mouseDownState.SetPosition(400, 600);
        mouseDownState.SetButtonState(MouseButton.LeftButton, true);

        var mouseUpState = default(MouseState);
        mouseUpState.SetPosition(400, 600);
        mouseUpState.SetButtonState(MouseButton.LeftButton, false);

        this.mockMouse.Setup(m => m.GetState())
            .Returns(() =>
            {
                totalGetStateInvokes++;
                return totalGetStateInvokes switch
                {
                    1 => mouseDownState,
                    2 => mouseUpState,
                    3 => mouseDownState,
                };
            });

        this.mockShapeRenderer.Setup(m => m.Render(It.IsAny<RectShape>(), It.IsAny<int>()))
            .Callback<RectShape, int>((rectangle, _) => btnFace = rectangle);

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
        sut.Update(default);
        sut.Update(default);

        // Act
        sut.Render();

        // Assert
        btnFace.Color.Should().Be(expected);
    }

    [Fact]
    public void Render_WithBorderSetToVisible_RendersBorder()
    {
        // Arrange
        var expected = default(RectShape);
        expected.Position = new Vector2(1, 2);
        expected.IsSolid = false;
        expected.BorderThickness = 3;
        expected.Color = Color.CornflowerBlue;
        expected.Width = 4;
        expected.Height = 5;
        expected.CornerRadius = new CornerRadius(6, 7, 8, 9);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(1, 2);
        sut.BorderColor = Color.CornflowerBlue;
        sut.BorderThickness = 3;
        sut.Width = 4;
        sut.Height = 5;
        sut.CornerRadius = new CornerRadius(6, 7, 8, 9);
        sut.AutoSize = false;

        sut.LoadContent();

        // Act
        sut.Render();

        // Assert
        this.mockShapeRenderer.VerifyOnce(m => m.Render(expected, BorderLayer));
    }

    [Fact]
    public void Render_WhenTextIsWiderThanButton_OnlyRendersRequiredCharacters()
    {
        // Arrange
        // NOTE: Anything with the value '999' signifies that it is not used in the code being tested.
        var charBounds = new[]
        {
            // The height and Y position of the chars are not used for determining left and right positioning.
            // With the set X position and width of the chars, the width of the text should be 90 pixels.
            // There is not space between each char in the data below.
            ('t', new RectangleF(new PointF(155f,  999), new SizeF(10, 999))),
            ('e', new RectangleF(new PointF(165f, 999), new SizeF(10, 999))),
            ('s', new RectangleF(new PointF(175f, 999), new SizeF(10, 999))),
            ('t', new RectangleF(new PointF(185f, 999), new SizeF(10, 999))),
            ('-', new RectangleF(new PointF(195f, 999), new SizeF(10, 999))),
            ('v', new RectangleF(new PointF(205f, 999), new SizeF(10, 999))),
            ('a', new RectangleF(new PointF(215f, 999), new SizeF(10, 999))),
            ('l', new RectangleF(new PointF(225f, 999), new SizeF(10, 999))),
            ('u', new RectangleF(new PointF(235f, 999), new SizeF(10, 999))),
            ('e', new RectangleF(new PointF(245f, 999), new SizeF(10, 999))),
        };

        this.mockFont.Setup(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()))
            .Returns<string, Vector2>((_, _) => charBounds);

        var sut = CreateSystemUnderTest();
        sut.Position = new Point(200, 999);
        sut.BorderColor = Color.CornflowerBlue;
        sut.BorderThickness = 3;
        sut.Width = 70;
        sut.Height = 40;
        sut.CornerRadius = new CornerRadius(6, 7, 8, 9);
        sut.AutoSize = false;
        sut.Text = "test-value";

        this.label.AutoSize = false;
        this.label.Height = 30;
        this.label.Width = 90;

        sut.LoadContent();

        // Act
        sut.Render();

        // Assert
        this.mockFontRenderer.Verify(m =>
            m.Render(It.IsAny<IFont>(),
                "st-va",
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<float>(),
                It.IsAny<float>(),
                It.IsAny<Color>(),
                It.IsAny<int>()), Times.Once);
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
            this.mockKeyboard.Object,
            this.mockMouse.Object,
            this.mockRenderFactory.Object);
}

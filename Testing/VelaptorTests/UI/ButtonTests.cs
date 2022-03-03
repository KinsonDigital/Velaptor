// <copyright file="ButtonTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Drawing;
    using System.Linq;
    using System.Numerics;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Button"/> class.
    /// </summary>
    public class ButtonTests
    {
        private const string ButtonTextValue = "test-value";
        private const string TextureName = "button-face-small";
        private readonly Mock<IContentLoader> mockContentLoader;
        private readonly Mock<ITexture> mockTexture;
        private readonly Mock<IFont> mockFont;
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

            this.label = new Label(this.mockContentLoader.Object, this.mockFont.Object);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullContentLoaderParamWith2Args_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Button(null, this.label);
            }, "The parameter must not be null. (Parameter 'contentLoader')");
        }

        [Fact]
        public void Ctor2Args_WithContentLoaderAndLabelParams_LabelPropertyNotNull()
        {
            // Arrange & Act
            var button = new Button(this.mockContentLoader.Object, this.label);

            // Assert
            Assert.NotNull(button.Label);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Position_WhenSettingValue_ReturnsCorrectValue()
        {
            // Arrange
            var expected = new Point(11, 22);
            var button = CreateButton();

            // Act
            button.Position = new Point(11, 22);
            var actual = button.Position;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void AutoSize_WhenGettingDefaultValue_ReturnsTrue()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.AutoSize;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void AutoSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.AutoSize = false;

            // Assert
            Assert.False(button.AutoSize);
        }

        [Fact]
        public void AutoSize_WhenTurningOffAutoSize_SetsWidthAngHeightBeforeAutoSizeWasTurnedOn()
        {
            // Arrange
            var button = CreateButton();
            button.AutoSize = false;
            button.Width = 123u;
            button.Height = 456u;
            button.AutoSize = true;

            // Act
            button.AutoSize = false;

            // Assert
            AssertExtensions.EqualWithMessage(123u, button.Width, "The width is incorrect.");
            AssertExtensions.EqualWithMessage(456u, button.Height, "The height is incorrect.");
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
            var button = CreateButton();
            button.AutoSize = autoSize;

            // Act
            button.Width = 40;

            // Assert
            Assert.Equal(expectedWidth, button.Width);
        }

        [Fact]
        public void Text_WhenSettingValueBeforeLoadingContent_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.Text = ButtonTextValue;
            var actual = button.Text;

            // Assert
            Assert.Equal(ButtonTextValue, actual);
        }

        [Fact]
        public void Text_WhenSettingValueAfterLoadingContent_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();
            button.LoadContent();

            // Act
            button.Text = "test-value";
            var actual = button.Text;

            // Assert
            Assert.Equal("test-value", actual);
        }

        [Fact]
        public void BorderVisible_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.BorderVisible;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void BorderVisible_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.BorderVisible = true;

            // Assert
            Assert.True(button.BorderVisible);
        }

        [Fact]
        public void BorderColor_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.BorderColor;

            // Assert
            Assert.Equal(Color.SlateGray, actual);
        }

        [Fact]
        public void BorderColor_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.BorderColor = Color.Red;

            // Assert
            Assert.Equal(Color.Red, button.BorderColor);
        }

        [Fact]
        public void BorderThickness_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.BorderThickness;

            // Assert
            Assert.Equal(2u, actual);
        }

        [Fact]
        public void BorderThickness_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.BorderThickness = 123u;

            // Assert
            Assert.Equal(123u, button.BorderThickness);
        }

        [Fact]
        public void FaceColor_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.FaceColor;

            // Assert
            Assert.Equal(Color.DarkGray, actual);
        }

        [Fact]
        public void FaceColor_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.FaceColor = Color.CornflowerBlue;
            var actual = button.FaceColor;

            // Assert
            Assert.Equal(Color.CornflowerBlue, actual);
        }

        [Fact]
        public void CornerRadius_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new CornerRadius(6f);
            var button = CreateButton();

            // Act
            var actual = button.CornerRadius;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CornerRadius_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var expected = new CornerRadius(10f, 20f, 30f, 40f);
            var button = CreateButton();

            // Act
            button.CornerRadius = new CornerRadius(10f, 20f, 30f, 40f);

            // Assert
            Assert.Equal(expected, button.CornerRadius);
        }

        [Fact]
        public void FontSize_WhenSettingValue_SetsAndReturnsLabelFontSize()
        {
            // Arrange
            this.mockFont.SetupProperty(p => p.Size);
            var button = CreateButton();
            button.LoadContent();

            // Act
            button.FontSize = 123u;

            // Assert
            Assert.Equal(123u, button.FontSize);
        }

        [Fact]
        public void Enabled_WhenSettingValueBeforeLoadingContent_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();

            // Act
            var actual = button.Enabled;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void Enabled_WhenSettingValueAfterLoadingContent_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();
            button.LoadContent();

            // Act
            button.Enabled = false;

            // Assert
            Assert.False(button.Enabled);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvoked_LoadsContent()
        {
            // Arrange
            this.mockContentLoader.Setup(m => m.LoadFont("TimesNewRoman-Regular.ttf", 12))
                .Returns(this.mockFont.Object);

            var button = new Button(this.mockContentLoader.Object, null);
            button.Text = "test-value";
            button.Enabled = false;
            button.Position = new Point(11, 22);

            // Act
            button.LoadContent();

            // Assert
            Assert.Equal("test-value", button.Text);
            Assert.False(button.Enabled);
            Assert.Equal(new Point(11, 22), button.Position);
            this.mockContentLoader.Verify(m => m.LoadFont("TimesNewRoman-Regular.ttf", 12), Times.Once);
        }

        [Fact]
        public void UnloadContent_WhenInvoked_UnloadsContent()
        {
            // Arrange
            var button = CreateButton();
            button.LoadContent();

            // Act
            button.UnloadContent();

            // Assert
            Assert.False(button.IsLoaded);
        }

        [Fact]
        public void UnloadContent_WhenAlreadyUnloaded_DoesNotUnloadContent()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.UnloadContent();

            // Assert
            Assert.False(button.IsLoaded);
            this.mockContentLoader.Verify(m => m.UnloadTexture(this.mockTexture.Object), Times.Never);
        }

        [Fact]
        public void Render_WithNoLoadedContentAndVisible_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Visible = true;

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                             It.IsAny<int>(),
                             It.IsAny<int>(),
                             It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WithLoadedContentAndInvisible_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Visible = false;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WithNullTexture_DoesNotRender()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();

            this.mockContentLoader.Setup(m => m.LoadTexture(TextureName))
                .Returns(() =>
                {
                    ITexture? nullTexture = null;

#pragma warning disable 8603
                    return nullTexture;
#pragma warning restore 8603
                });
            var button = CreateButton();
            button.Visible = true;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(It.IsAny<ITexture>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvoked_RendersButtonFace()
        {
            // Arrange
            var actual = default(RectShape);

            var mockSpriteBatch = new Mock<ISpriteBatch>();
            mockSpriteBatch.Setup(m => m.Render(It.IsAny<RectShape>()))
                .Callback<RectShape>((rectangle) =>
                {
                    // Only capture the button face rectangle
                    if (rectangle.IsFilled)
                    {
                        actual = rectangle;
                    }
                });

            var button = CreateButton();
            button.Position = new Point(400, 600);
            button.BorderVisible = false;
            button.Visible = true;
            button.Enabled = true;
            button.AutoSize = false;
            button.Width = 100;
            button.Height = 50;
            button.BorderThickness = 123u;
            button.CornerRadius = new CornerRadius(11, 22, 33, 44);
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m => m.Render(actual), Times.Once);

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
        /// Creates a new button for the purpose of testing.
        /// </summary>
        /// <returns>The button instance to test.</returns>
        private Button CreateButton()
        {
            return new Button(this.mockContentLoader.Object, this.label);
        }
    }
}

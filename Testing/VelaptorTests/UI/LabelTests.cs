// <copyright file="LabelTests.cs" company="KinsonDigital">
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
    using Velaptor;
    using Velaptor.Content;
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.UI;
    using VelaptorTests.Helpers;
    using Xunit;
    using VelFontStyle = Velaptor.Content.Fonts.FontStyle;

    /// <summary>
    /// Tests the <see cref="Label"/> class.
    /// </summary>
    public class LabelTests
    {
        private const string TextValue = "hello world";
        private readonly Mock<IContentLoader> mockContentLoader;
        private readonly Mock<IFont> mockFont;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabelTests"/> class.
        /// </summary>
        public LabelTests()
        {
            this.mockFont = new Mock<IFont>();
            MockGlyphs(TextValue);

            this.mockContentLoader = new Mock<IContentLoader>();
            this.mockContentLoader.Setup(m => m.LoadFont(It.IsAny<string>(), It.IsAny<uint>()))
                .Returns(this.mockFont.Object);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullContentLoaderParam_ThrowsException()
        {
            // Arrange & Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Label(null, this.mockFont.Object);
            }, "The parameter must not be null. (Parameter 'contentLoader')");
        }

        [Fact]
        public void Ctor_WithNullFontParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Label(this.mockContentLoader.Object, null);
            }, "The parameter must not be null. (Parameter 'font')");
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Text_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Text = "test-value";
            var actual = label.Text;

            // Assert
            Assert.Equal("test-value", actual);
        }

        [Fact]
        public void Position_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Position = new Point(11, 22);
            var actual = label.Position;

            // Assert
            Assert.Equal(new Point(11, 22), actual);
        }

        [Fact]
        public void Position_WhenContentIsNotLoaded_CreatesCorrectCharacterBounds()
        {
            // Arrange
            const string labelText = "xunit";

            var label = CreateLabel();
            label.Position = new Point(100, 200);
            label.Width = 108;
            label.Text = labelText;

            // Act
            var actual = label.CharacterBounds;

            // Assert
            Assert.Empty(actual);
            this.mockFont.Verify(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()), Times.Never);
        }

        [Fact]
        public void Position_WhenContentIsLoaded_CreatesCorrectCharacterBounds()
        {
            // Arrange
            const string labelText = "xunit";
            var characters = labelText.ToArray();

            var label = CreateLabel();
            label.AutoSize = false;
            var mockedCharBounds = new List<(char character, RectangleF bounds)>()
            {
                (characters[0], RectangleF.Empty),
                (characters[1], RectangleF.Empty),
                (characters[2], RectangleF.Empty),
                (characters[3], RectangleF.Empty),
                (characters[4], RectangleF.Empty),
            };

            this.mockFont.Setup(m => m.GetCharacterBounds(It.IsAny<string>(), It.IsAny<Vector2>()))
                .Returns(mockedCharBounds);

            label.Position = new Point(100, 200);
            label.Width = 108;
            label.Text = labelText;
            label.LoadContent();

            // Act
            var actual = label.CharacterBounds;

            // Assert
            Assert.Equal(labelText.Length, actual.Count);
            this.mockFont.Verify(m => m.GetCharacterBounds(labelText, new Vector2(46, 200)), Times.Once);
        }

        [Fact]
        public void AutoSize_WhenGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            var actual = label.AutoSize;

            // Assert
            Assert.True(actual, $"The expected default value of the '{nameof(Label.AutoSize)}' property must be true.");
        }

        [Fact]
        public void AutoSize_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.AutoSize = false;

            // Assert
            Assert.False(label.AutoSize);
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
            var label = CreateLabel();
            label.AutoSize = autoSize;
            label.Text = testText;
            label.LoadContent();

            // Act
            label.Width = width;
            var actual = label.Width;

            // Assert
            Assert.Equal(expected, actual);
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
            var label = CreateLabel();
            label.AutoSize = autoSize;
            label.Text = testText;
            label.LoadContent();

            // Act
            label.Height = height;
            var actual = label.Height;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Style_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFont.SetupProperty(p => p.Style);
            var label = CreateLabel();

            // Act
            label.Style = FontStyle.Italic;

            // Assert
            Assert.Equal(FontStyle.Italic, label.Style);
        }

        [Fact]
        public void WidthAndHeight_WhenTextIsEmpty_CalculatesCorrectResults()
        {
            // Arrange
            var label = CreateLabel();

            label.Text = string.Empty;
            label.LoadContent();

            // Act
            var actualWidth = label.Width;
            var actualHeight = label.Height;

            // Assert
            Assert.Equal(0u, actualWidth);
            Assert.Equal(0u, actualHeight);
        }

        [Fact]
        public void Color_WhenNotGettingDefaultValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            var actual = label.Color;

            // Assert
            Assert.Equal(Color.Black, actual);
        }

        [Fact]
        public void Color_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.Color = Color.FromArgb(11, 22, 33, 44);
            var actual = label.Color;

            // Assert
            Assert.Equal(Color.FromArgb(11, 22, 33, 44), actual);
        }

        [Fact]
        public void Font_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var label = CreateLabel();
            label.LoadContent();

            // Act
            var actual = label.Font;

            // Assert
            Assert.Same(this.mockFont.Object, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvoked_LoadsCorrectFont()
        {
            // Arrange
            var expectedCharBounds = new ReadOnlyCollection<(char, RectangleF)>(
                new[]
                {
                    ('a', new RectangleF(1f, 2f, 3f, 4f)),
                    ('b', new RectangleF(5f, 6f, 7f, 8f)),
                });

            const string testText = "test-value";
            var position = new Vector2(11f, 22f);
            this.mockFont.Setup(m => m.GetCharacterBounds(testText, position))
                .Returns(() => expectedCharBounds);
            var label = CreateLabel();
            label.Text = testText;
            label.Position = position.ToPoint();

            // Act
            label.LoadContent();
            label.LoadContent();

            // Assert
            Assert.Equal(expectedCharBounds, label.CharacterBounds);
        }

        [Fact]
        public void LoadContent_WhenInvoked_CreatesCorrectCharacterBounds()
        {
            // Arrange
            const string labelText = "test-value";
            var characters = labelText.ToArray();

            var label = CreateLabel();
            var mockedCharBounds = new List<(char character, RectangleF bounds)>()
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

            label.AutoSize = false;
            label.Position = new Point(100, 200);
            label.Width = 108;
            label.Text = labelText;
            label.LoadContent();

            // Act
            var actual = label.CharacterBounds;

            // Assert
            Assert.Equal(labelText.Length, actual.Count);
            this.mockFont.Verify(m => m.GetCharacterBounds(labelText, new Vector2(46, 200)), Times.Once);
        }

        [Fact]
        public void UnloadContent_WhenLoadedAndNotDisposed_UnloadsContent()
        {
            // Arrange
            var label = CreateLabel();
            label.LoadContent();

            // Act
            label.UnloadContent();

            // Assert
            this.mockContentLoader.Verify(m => m.UnloadFont(this.mockFont.Object), Times.Once);
        }

        [Fact]
        public void UnloadContent_WhenNotLoaded_DoesNotUnloadContent()
        {
            // Arrange
            var label = CreateLabel();

            // Act
            label.UnloadContent();

            // Assert
            this.mockContentLoader.Verify(m => m.UnloadFont(this.mockFont.Object), Times.Never);
        }

        [Fact]
        public void Render_WithNullRenderer_ThrowsException()
        {
            // Arrange
            var label = CreateLabel();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                label.Render(null);
            }, "The parameter must not be null. (Parameter 'renderer')");
        }

        [Fact]
        public void Render_WhenInvokedWithoutLoadedContent_DoesNotRender()
        {
            // Arrange
            var mockRenderer = new Mock<IRenderer>();
            var label = CreateLabel();
            label.Visible = true;

            // Act
            label.Render(mockRenderer.Object);

            // Assert
            mockRenderer.Verify(m => m.Render(It.IsAny<IFont>(),
                                                               It.IsAny<string>(),
                                                               It.IsAny<int>(),
                                                               It.IsAny<int>(),
                                                               It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvokedWhileNotVisible_DoesNotRender()
        {
            // Arrange
            var mockRenderer = new Mock<IRenderer>();
            var label = CreateLabel();
            label.Visible = false;

            // Act
            label.LoadContent();
            label.Render(mockRenderer.Object);

            // Assert
            mockRenderer.Verify(m =>
                m.Render(It.IsAny<IFont>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WithNullOrEmptyText_DoesNotRender()
        {
            // Arrange
            var mockRenderer = new Mock<IRenderer>();
            var label = CreateLabel();
            label.Text = string.Empty;
            label.LoadContent();

            // Act
            label.Render(mockRenderer.Object);

            // Assert
            mockRenderer.Verify(m =>
                m.Render(
                    It.IsAny<IFont>(),
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<float>(),
                    It.IsAny<float>(),
                    It.IsAny<Color>()), Times.Never);
        }

        [Fact]
        public void Render_WhenInvoked_RendersText()
        {
            // Arrange
            var mockRenderer = new Mock<IRenderer>();

            var label = CreateLabel();
            label.Text = TextValue;
            label.Position = new Point(100, 200);
            label.Visible = true;
            label.Color = Color.FromArgb(11, 22, 33, 44);
            label.LoadContent();

            // Act
            label.Render(mockRenderer.Object);

            // Assert
            mockRenderer.Verify(m =>
                m.Render(this.mockFont.Object,
                    TextValue,
                    100,
                    200,
                    1f,
                    0f,
                    Color.FromArgb(11, 22, 33, 44)), Times.Once());
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
        /// Creates a new label for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private Label CreateLabel() => new (this.mockContentLoader.Object, this.mockFont.Object);
    }
}

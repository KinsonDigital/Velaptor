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
    using Moq;
    using Velaptor.Content;
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
        }

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

        [Theory]
        [InlineData(false, 0u)]
        [InlineData(true, 200u)]
        public void Width_WhenGettingValueBeforeLoadingContent_ReturnsCorrectResult(bool loadContent, uint expected)
        {
            // Arrange
            var button = CreateButton();

            if (loadContent)
            {
                button.LoadContent();
            }

            // Act
            var actual = button.Width;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(false, 0u)]
        [InlineData(true, 100u)]
        public void Height_WhenGettingValueBeforeLoadingContent_ReturnsCorrectResult(bool loadContent, uint expected)
        {
            // Arrange
            var button = CreateButton();

            if (loadContent)
            {
                button.LoadContent();
            }

            // Act
            var actual = button.Height;

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Size_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var button = CreateButton();
            button.LoadContent();

            // Act
            var actual = button.Size;

            // Assert
            Assert.Equal(1f, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void LoadContent_WhenInvoked_LoadsContent()
        {
            // Arrange
            var button = CreateButton();

            // Act
            button.LoadContent();
            button.LoadContent();

            // Assert
            this.mockContentLoader.Verify(m => m.LoadTexture(TextureName), Times.Once);
            this.mockContentLoader.Verify(m => m.LoadFont("times", 12), Times.Once);
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
            button.LoadContent();
            button.Dispose();

            // Act
            button.UnloadContent();

            // Assert
            Assert.False(button.IsLoaded);
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
        public void Render_WhenInvoked_RendersTexture()
        {
            // Arrange
            var mockSpriteBatch = new Mock<ISpriteBatch>();
            var button = CreateButton();
            button.Position = new Point(10, 20);
            button.Visible = true;
            button.LoadContent();

            // Act
            button.Render(mockSpriteBatch.Object);

            // Assert
            mockSpriteBatch.Verify(m =>
                m.Render(this.mockTexture.Object,
                    10,
                    20,
                    Color.White), Times.Once);
        }

        [Fact]
        public void Dispose_WithNoLoadedContent_DoesNotThrowException()
        {
            // Arrange
            var button = CreateButton();

            // Act & Assert
            AssertExtensions.DoesNotThrow<NullReferenceException>(() =>
            {
                button.Dispose();
            });

            this.mockTexture.Verify(m => m.Dispose(), Times.Never);
        }

        [Fact]
        public void Dispose_WithLoadedContent_DisposesOfButton()
        {
            // Arrange
            this.mockTexture.SetupGet(p => p.IsPooled).Returns(false);
            var button = CreateButton();
            button.LoadContent();

            // Act & Assert
            button.Dispose();
            button.Dispose();

            this.mockTexture.Verify(m => m.Dispose(), Times.Once);
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
            => new Button(this.mockContentLoader.Object);
    }
}

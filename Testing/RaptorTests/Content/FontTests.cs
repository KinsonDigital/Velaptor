// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace RaptorTests.Content
{
    using System.Drawing;
    using Moq;
    using Raptor.Graphics;
    using Xunit;

    public class FontTests
    {
        private readonly Mock<ITexture> mockFontTexture;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTests"/> class.
        /// </summary>
        public FontTests() => this.mockFontTexture = new Mock<ITexture>();

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsPropertyValues()
        {
            // Arrange
            var glyphMetrics = new GlyphMetrics[0];

            // Act
            var font = new Font(this.mockFontTexture.Object, glyphMetrics, "test-name", 14, "test-path");

            // Assert
            Assert.Same(font.FontTextureAtlas, this.mockFontTexture.Object);
            Assert.Equal(font.Length, glyphMetrics.Length);
            Assert.Equal("test-name", font.Name);
            Assert.Equal(14, font.Size);
            Assert.Equal("test-path", font.Path);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void Iterator_WhenGettingValueAtIndex_ReturnsCorrectReuslt()
        {
            // Arrange
            var glyphMetrics = new GlyphMetrics[]
            {
                new GlyphMetrics()
                {
                    Ascender = 1, Descender = 2, CharIndex = 3,
                    Glyph = 'c', GlyphWidth = 4, GlyphHeight = 5,
                    HoriBearingX = 6, HoriBearingY = 7, XMin = 8,
                    XMax = 9, YMin = 11, YMax = 22,
                    HorizontalAdvance = 33, AtlasBounds = new Rectangle(44, 55, 66, 77),
                },
                new GlyphMetrics()
                {
                    Ascender = 11, Descender = 22, CharIndex = 33,
                    Glyph = 'c', GlyphWidth = 44, GlyphHeight = 55,
                    HoriBearingX = 66, HoriBearingY = 77, XMin = 88,
                    XMax = 99, YMin = 111, YMax = 222,
                    HorizontalAdvance = 333, AtlasBounds = new Rectangle(444, 555, 666, 777),
                },
            };

            var font = new Font(this.mockFontTexture.Object, glyphMetrics, It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>());

            // Act
            var actual = font[0];

            // Assert
            Assert.Equal(glyphMetrics[0], actual);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesOfTextureAndFont()
        {
            // Arrange
            var font = new Font(this.mockFontTexture.Object, new GlyphMetrics[0], It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>());

            // Act
            font.Dispose();
            font.Dispose();

            // Assert
            this.mockFontTexture.Verify(m => m.Dispose(), Times.Once());
        }
        #endregion
    }
}

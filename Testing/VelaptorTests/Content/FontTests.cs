// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.Drawing;
    using Moq;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
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
            var glyphMetrics = Array.Empty<GlyphMetrics>();
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            // Act
            var font = new Font(this.mockFontTexture.Object, glyphMetrics, settings, It.IsAny<char[]>(), "test-name", "test-path");

            // Assert
            Assert.Same(font.FontTextureAtlas, this.mockFontTexture.Object);
            Assert.Equal(font.Metrics.Count, glyphMetrics.Length);
            Assert.Equal("test-name", font.Name);
            Assert.Equal(14, font.Size);
            Assert.Equal(FontStyle.Regular, font.Style);
            Assert.Equal("test-path", font.Path);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void Metrics_WhenGettingValueAtIndex_ReturnsCorrectResult()
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

            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                glyphMetrics,
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            var actual = font.Metrics[0];

            // Assert
            Assert.Equal(glyphMetrics[0], actual);
        }

        [Fact]
        public void IsPooled_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                It.IsAny<GlyphMetrics[]>(),
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            font.IsPooled = true;
            var actual = font.IsPooled;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void HasKerning_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            // Act
            var font = new Font(
                this.mockFontTexture.Object,
                It.IsAny<GlyphMetrics[]>(),
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>())
            {
                HasKerning = true,
            };

            var actual = font.HasKerning;

            // Assert
            Assert.True(actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void GetAvailableGlyphCharacters_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                It.IsAny<GlyphMetrics[]>(),
                settings,
                new[] { 'a', 'b', 'c', 'd' },
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            var actual = font.GetAvailableGlyphCharacters();

            // Assert
            Assert.Equal(new[] { 'a', 'b', 'c', 'd' }, actual);
        }

        [Fact]
        public void Dispose_WhenInvoked_DisposesFont()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            // Simulate that the texture has been pooled
            this.mockFontTexture.SetupProperty(p => p.IsPooled);
            this.mockFontTexture.Object.IsPooled = true;

            var font = new Font(
                this.mockFontTexture.Object,
                Array.Empty<GlyphMetrics>(),
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            font.Dispose();
            font.Dispose();

            // Assert
            Assert.False(font.FontTextureAtlas.IsPooled);
            this.mockFontTexture.Verify(m => m.Dispose(), Times.Once());
        }

        [Fact]
        public void Dispose_WhilePooled_ThrowsException()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                Array.Empty<GlyphMetrics>(),
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());
            font.IsPooled = true;

            // Act & Assert
            Assert.Throws<PooledDisposalException>(() =>
            {
                font.Dispose();
            });
        }
        #endregion
    }
}

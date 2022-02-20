// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content.Fonts
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Moq;
    using Newtonsoft.Json;
    using Velaptor.Content;
    using Velaptor.Content.Caching;
    using Velaptor.Content.Exceptions;
    using Velaptor.Content.Fonts;
    using Velaptor.Content.Fonts.Services;
    using Velaptor.Graphics;
    using Velaptor.Services;
    using VelaptorTests.Helpers;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Font"/> class.
    /// </summary>
    public class FontTests : IDisposable
    {
        private const char InvalidCharacter = '□';
        private const string DirPath = @"C:\test-dir\fonts\";
        private const string FontName = "test-font";
        private const string FontExtension = ".ttf";
        private readonly string fontFilePath;
        private readonly IntPtr facePtr = new (5678);
        private readonly Mock<IFontService> mockFontService;
        private readonly Mock<IFontStatsService> mockFontStatsService;
        private readonly Mock<ITexture> mockTexture;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly Mock<IItemCache<string, ITexture>> mockTextureCache;
        private readonly string sampleTestDataDirPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\SampleTestData\";
        private Dictionary<char, GlyphMetrics> glyphMetrics = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTests"/> class.
        /// </summary>
        public FontTests()
        {
            this.fontFilePath = $"{DirPath}{FontName}{FontExtension}";

            const string glyphTestDataFileName = "glyph-test-data.json";
            var glyphMetricFilePath = $"{this.sampleTestDataDirPath}{glyphTestDataFileName}";
            var glyphMetricData = File.ReadAllText(glyphMetricFilePath);

            var glyphMetricItems = JsonConvert.DeserializeObject<GlyphMetrics[]>(glyphMetricData);

            foreach (var metric in glyphMetricItems)
            {
                this.glyphMetrics.Add(metric.Glyph, metric);
            }

            this.mockFontService = new Mock<IFontService>();
            this.mockFontService.Setup(m => m.CreateFontFace(this.fontFilePath)).Returns(this.facePtr);
            this.mockFontService.Setup(m => m.CreateGlyphMetrics(this.facePtr, null))
                .Returns(this.glyphMetrics);
            this.mockFontService.Setup(m => m.HasKerning(this.facePtr)).Returns(true);

            this.mockFontStatsService = new Mock<IFontStatsService>();

            this.mockFontAtlasService = new Mock<IFontAtlasService>();
            this.mockTextureCache = new Mock<IItemCache<string, ITexture>>();

            this.mockTexture = new Mock<ITexture>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WithNullTextureParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    null,
                    this.mockFontService.Object,
                    this.mockFontStatsService.Object,
                    this.mockFontAtlasService.Object,
                    this.mockTextureCache.Object,
                    FontName,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The parameter must not be null. (Parameter 'texture')");
        }

        [Fact]
        public void Ctor_WithNullFontServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    this.mockTexture.Object,
                    null,
                    this.mockFontStatsService.Object,
                    this.mockFontAtlasService.Object,
                    this.mockTextureCache.Object,
                    FontName,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The parameter must not be null. (Parameter 'fontService')");
        }

        [Fact]
        public void Ctor_WithNullFontStatsServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    this.mockTexture.Object,
                    this.mockFontService.Object,
                    null,
                    this.mockFontAtlasService.Object,
                    this.mockTextureCache.Object,
                    FontName,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The parameter must not be null. (Parameter 'fontStatsService')");
        }

        [Fact]
        public void Ctor_WithNullFontAtlasServiceParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    this.mockTexture.Object,
                    this.mockFontService.Object,
                    this.mockFontStatsService.Object,
                    null,
                    this.mockTextureCache.Object,
                    FontName,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The parameter must not be null. (Parameter 'fontAtlasService')");
        }

        [Fact]
        public void Ctor_WithNullTextureCacheParam_ThrowsException()
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    this.mockTexture.Object,
                    this.mockFontService.Object,
                    this.mockFontStatsService.Object,
                    this.mockFontAtlasService.Object,
                    null,
                    FontName,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The parameter must not be null. (Parameter 'textureCache')");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Ctor_WithNullNameParam_ThrowsException(string name)
        {
            // Act & Assert
            AssertExtensions.ThrowsWithMessage<ArgumentNullException>(() =>
            {
                _ = new Font(
                    this.mockTexture.Object,
                    this.mockFontService.Object,
                    this.mockFontStatsService.Object,
                    this.mockFontAtlasService.Object,
                    this.mockTextureCache.Object,
                    name,
                    this.fontFilePath,
                    12u,
                    true,
                    this.glyphMetrics.Values.ToArray());
            }, "The string parameter must not be null or empty. (Parameter 'name')");
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsPropertyValues()
        {
            // Arrange
            this.mockTexture.SetupGet(p => p.FilePath);
            this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Italic);
            this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");

            // Act
            var font = CreateFont();

            // Assert
            Assert.Same(this.mockTexture.Object, font.FontTextureAtlas);
            AssertExtensions.EqualWithMessage(font.Metrics.Count, this.glyphMetrics.Count, $"Total Glyph Metrics");
            AssertExtensions.EqualWithMessage(FontName, font.Name, $"Property: {nameof(font.Name)}");
            AssertExtensions.EqualWithMessage("test-font-family", font.FamilyName, $"Property: {nameof(font.FamilyName)}");
            AssertExtensions.EqualWithMessage(true, font.HasKerning, $"Property: {nameof(font.HasKerning)}");
            AssertExtensions.EqualWithMessage(this.fontFilePath, font.FilePath, $"Property: {nameof(font.FilePath)}");
        }

        [Fact]
        public void Ctor_WithAtLeastOneOrMoreFontStyles_SetsFontSource()
        {
            // Arrange
            const string familyName = "test-font-family";
            const FontStyle boldItalic = FontStyle.Bold | FontStyle.Italic;
            var contentFontStats = new FontStats[]
            {
                new () { Style = FontStyle.Regular, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
                new () { Style = FontStyle.Bold, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
                new () { Style = FontStyle.Italic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
                new () { Style = boldItalic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            };

            this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns(familyName);
            this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Bold);
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(familyName))
                .Returns(contentFontStats);

            // Act
            var font = CreateFont();

            // Assert
            Assert.Equal(FontSource.AppContent, font.Source);
        }

        [Fact]
        public void Ctor_WithMissingStylesInContentButExistsInSystem_SetsFontSource()
        {
            // Arrange
            const string familyName = "test-font-family";
            const FontStyle boldItalic = FontStyle.Bold | FontStyle.Italic;
            var contentFontStats = new FontStats[]
            {
                new () { Style = FontStyle.Regular, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
                new () { Style = FontStyle.Bold, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
                new () { Style = FontStyle.Italic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            };
            var systemFontStats = new FontStats[]
            {
                new () { Style = boldItalic, Source = FontSource.AppContent, FamilyName = familyName, FontFilePath = this.fontFilePath },
            };

            this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns(familyName);
            this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(boldItalic);
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(familyName))
                .Returns(contentFontStats);
            this.mockFontStatsService.Setup(m => m.GetSystemStatsForFontFamily(familyName))
                .Returns(systemFontStats);

            // Act
            var font = CreateFont();

            // Assert
            Assert.Equal(FontSource.AppContent, font.Source);
            Assert.Equal(boldItalic, font.Style);
        }

        [Fact]
        public void Ctor_WithNoFontStyles_SetsFontSourceToUnknown()
        {
            // Arrange
            this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");
            this.mockFontService.Setup(m => m.GetFontStyle(this.fontFilePath)).Returns(FontStyle.Bold);
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily("test-font-family"))
                .Returns(Array.Empty<FontStats>());

            // Act
            var font = CreateFont();

            // Assert
            Assert.Equal(FontSource.Unknown, font.Source);
        }

        [Fact]
        public void Ctor_WhenInvoked_SetsPropertiesToCorrectValues()
        {
            // Arrange
            var font = CreateFont();

            // Actual
            var actualName = font.Name;
            var actualFilePath = font.FilePath;
            var actualIsDefaultFont = font.IsDefaultFont;

            // Assert
            Assert.Equal(FontName, actualName);
            Assert.Equal(this.fontFilePath, actualFilePath);
            Assert.True(actualIsDefaultFont);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void AvailableStylesForFamily_WhenNoStylesExist_ReturnsEmpty()
        {
            // Arrange
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
                .Returns(Array.Empty<FontStats>());

            var font = CreateFont();

            // Act
            var actual = font.AvailableStylesForFamily;

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void AvailableStylesForFamily_WhenAnyStylesExist_Returns()
        {
            // Arrange
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
                .Returns(new FontStats[] { new () { Style = FontStyle.Bold } });

            var font = CreateFont();

            // Act
            var actual = font.AvailableStylesForFamily;

            // Assert
            Assert.Single(actual);
        }

        [Fact]
        public void LineSpacing_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFontService.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12))
                .Returns(0.5f);
            var font = CreateFont();

            // Act
            var actual = font.LineSpacing;

            // Assert
            Assert.Equal(0.5, actual);
        }

        [Fact]
        public void Style_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
                .Returns(new FontStats[] { new () { Style = FontStyle.Italic, FontFilePath = this.fontFilePath } });

            var font = CreateFont();

            // Act
            font.Style = FontStyle.Italic;
            var actual = font.Style;

            // Assert
            Assert.Equal(FontStyle.Italic, actual);
        }

        [Fact]
        public void Style_WhenUsingStyleThatDoesNotExist_ThrowsException()
        {
            // Arrange
            this.mockFontService.Setup(m => m.GetFamilyName(this.fontFilePath)).Returns("test-font-family");
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
                .Returns(new FontStats[] { new () { Style = FontStyle.Bold } });

            var font = CreateFont();

            // Act & Assert
            AssertExtensions.ThrowsWithMessage<LoadFontException>(() =>
            {
                font.Style = FontStyle.Italic;
            }, "The font style 'Italic' does not exist for the font family 'test-font-family'.");
        }

        [Fact]
        public void Size_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFontStatsService.Setup(m => m.GetContentStatsForFontFamily(It.IsAny<string>()))
                .Returns(new FontStats[] { new () { Style = FontStyle.Regular, FontFilePath = this.fontFilePath } });

            var font = CreateFont();

            // Act
            font.Size = 22;
            var actual = font.Size;

            // Assert
            Assert.Equal(22u, actual);
        }
        #endregion

        #region Method Tests
        [Fact]
        public void GetKerning_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFontService.Setup(m => m.GetKerning(this.facePtr, 11, 22)).Returns(33);
            var font = CreateFont();

            // Act
            var actual = font.GetKerning(11, 22);

            // Assert
            this.mockFontService.Verify(m => m.GetKerning(this.facePtr, 11, 22), Times.Once);
            Assert.Equal(33, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Measure_WithNullOrEmptyText_ReturnsEmptySize(string text)
        {
            // Arrange
            var font = CreateFont();

            // Act
            var actual = font.Measure(text);

            // Assert
            Assert.Equal(0, actual.Width);
            Assert.Equal(0, actual.Height);
        }

        [Fact]
        public void Measure_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            const string text = "hello\nworld";

            this.mockFontService.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12)).Returns(2f);
            this.mockFontService.Setup(m => m.HasKerning(this.facePtr)).Returns(true);
            MockGlyphKernings(text);

            var font = new Font(
                this.mockTexture.Object,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                "test-name",
                DirPath,
                It.IsAny<uint>(),
                It.IsAny<bool>(),
                this.glyphMetrics.Values.ToArray());

            // Act
            var actual = font.Measure(text);

            // Assert
            Assert.Equal(103, actual.Width);
            Assert.Equal(31, actual.Height);
        }

        [Fact]
        public void ToGlyphMetrics_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            const string text = "test©";
            var font = CreateFont();

            // Act
            var actual = font.ToGlyphMetrics(text);

            // Assert
            Assert.Equal(5, actual.Length);
            Assert.Equal('t', actual[0].Glyph);
            Assert.Equal('e', actual[1].Glyph);
            Assert.Equal('s', actual[2].Glyph);
            Assert.Equal('t', actual[3].Glyph);
            Assert.Equal(InvalidCharacter, actual[4].Glyph);
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose() => this.glyphMetrics = null;

        /// <summary>
        /// Creates a new instance of <see cref="Font"/> for the purpose of testing.
        /// </summary>
        /// <returns>The instance to test.</returns>
        private Font CreateFont(uint size = 12)
            => new (
                this.mockTexture.Object,
                this.mockFontService.Object,
                this.mockFontStatsService.Object,
                this.mockFontAtlasService.Object,
                this.mockTextureCache.Object,
                FontName,
                this.fontFilePath,
                size,
                true,
                this.glyphMetrics.Values.ToArray());

        /// <summary>
        /// Mocks the kerning value for each character in the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to mock the kerning values for.</param>
        private void MockGlyphKernings(string text)
        {
            if (this.glyphMetrics is null || this.glyphMetrics.Count <= 0)
            {
                Assert.True(false, $"Cannot run test with the static class member '{this.glyphMetrics}' being null or empty.");
            }

            /* NOTE:
             * For the text 'hello\nworld', the kerning values should be mocked for each character below
             * h = 1
             * e = 2
             * l = 3
             * l = 4
             * o = 5
             * w = 6
             * o = 6
             * r = 7
             * l = 8
             * d = 9
             */
            var leftGlyphIndex = 0u;
            var rightGlyphIndex = 0u;

            for (var i = 0; i < text.Length; i++)
            {
                var glyphChar = text[i];

                if (i > 0)
                {
                    var foundGlyphMetric = (from m in this.glyphMetrics.Values
                        where m.Glyph == glyphChar
                        select m).FirstOrDefault();

                    rightGlyphIndex = foundGlyphMetric.CharIndex;
                }

                var leftIndex = leftGlyphIndex;
                var rightIndex = rightGlyphIndex;
                this.mockFontService.Setup(m => m.GetKerning(this.facePtr, leftIndex, rightIndex))
                    .Returns(i + 1);

                leftGlyphIndex = rightGlyphIndex;
            }
        }
    }
}

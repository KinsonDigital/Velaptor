// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

namespace VelaptorTests.Content
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FreeTypeSharp.Native;
    using Moq;
    using Newtonsoft.Json;
    using Velaptor.Content;
    using Velaptor.Content.Exceptions;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.FreeType;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Font"/> class.
    /// </summary>
    public class FontTests : IDisposable
    {
        private const char InvalidCharacter = '□';
        private readonly char[] glyphChars =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };
        private readonly IntPtr facePtr = new (1234);
        private readonly Mock<ITexture> mockFontTexture;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IFreeTypeExtensions> mockFreeTypeExtensions;
        private readonly string sampleTestDataDirPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\SampleTestData\";
        private GlyphMetrics[]? glyphMetrics;

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTests"/> class.
        /// </summary>
        public FontTests()
        {
            this.mockFontTexture = new Mock<ITexture>();
            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();
            this.mockFreeTypeInvoker.Setup(m => m.FT_Get_Face()).Returns(this.facePtr);

            this.mockFreeTypeExtensions = new Mock<IFreeTypeExtensions>();

            const string glyphTestDataFileName = "glyph-test-data.json";
            var glyphMetricFilePath = $"{this.sampleTestDataDirPath}{glyphTestDataFileName}";
            var glyphMetricData = File.ReadAllText(glyphMetricFilePath);

            this.glyphMetrics = JsonConvert.DeserializeObject<GlyphMetrics[]>(glyphMetricData);
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsPropertyValues()
        {
            // Arrange
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            // Act
            var font = new Font(this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics,
                settings,
                It.IsAny<char[]>(),
                "test-name",
                "test-path");

            // Assert
            Assert.Same(font.FontTextureAtlas, this.mockFontTexture.Object);
            Assert.Equal(font.Metrics.Count, this.glyphMetrics.Length);
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
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics,
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            var actual = font.Metrics[0];

            // Assert
            Assert.Equal(this.glyphMetrics[0], actual);
        }

        [Fact]
        public void LineSpacing_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var fontSettings = new FontSettings();
            this.mockFreeTypeExtensions.Setup(m => m.GetFontScaledLineSpacing(this.facePtr)).Returns(0.5f);
            var font = CreateFont(fontSettings);

            // Act
            var actual = font.LineSpacing;

            // Assert
            Assert.Equal(32, actual);
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

            var font = CreateFont(settings);

            // Act
            font.IsPooled = true;
            var actual = font.IsPooled;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void HasKerning_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFreeTypeExtensions.Setup(m => m.HasKerning(this.facePtr)).Returns(true);
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            // Act
            var font = new Font(
                this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                Array.Empty<GlyphMetrics>(),
                settings,
                It.IsAny<char[]>(),
                It.IsAny<string>(),
                It.IsAny<string>());

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
            var expected = new[] { 'a', 'b', 'c', 'd' };
            var settings = new FontSettings()
            {
                Size = 14,
                Style = FontStyle.Regular,
            };

            var font = new Font(
                this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                Array.Empty<GlyphMetrics>(),
                settings,
                new[] { 'a', 'b', 'c', 'd' },
                It.IsAny<string>(),
                It.IsAny<string>());

            // Act
            var actual = font.GetAvailableGlyphCharacters();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(true,  1u, 2u, 10f)]
        [InlineData(false, 1u, 2u, 0f)]
        [InlineData(true, 0u, 2u, 0f)]
        [InlineData(true, 1u, 0u, 0f)]
        public void GetKerning_WhenInvoked_ReturnsCorrectResult(bool hasKerning, uint leftGlyphIndex, uint rightGlyphIndex, float expected)
        {
            // Arrange
            var ftVector = default(FT_Vector);
            ftVector.x = new IntPtr(640);
            this.mockFreeTypeInvoker.Setup(m
                    => m.FT_Get_Kerning(this.facePtr, leftGlyphIndex, rightGlyphIndex, (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT))
                        .Returns(ftVector);
            this.mockFreeTypeExtensions.Setup(m => m.HasKerning(this.facePtr)).Returns(hasKerning);
            var fontSettings = new FontSettings();
            var font = CreateFont(fontSettings);

            // Act
            var actual = font.GetKerning(leftGlyphIndex, rightGlyphIndex);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Measure_WithNullOrEmptyText_ReturnsEmptySize(string text)
        {
            // Arrange
            var font = CreateFont(new FontSettings());

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
            var availableGlyphChars = new[] { 'h', 'e', 'l', 'o', 'w', 'r', 'd' };

            this.mockFreeTypeExtensions.Setup(m => m.GetFontScaledLineSpacing(this.facePtr)).Returns(2f);
            this.mockFreeTypeExtensions.Setup(m => m.HasKerning(this.facePtr)).Returns(true);
            MockGlyphKernings(text);

            var fontSettings = new FontSettings();
            var font = new Font(
                this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics,
                fontSettings,
                availableGlyphChars,
                "test-font",
                "test-path");

            // Act
            var actual = font.Measure(text);

            // Assert
            Assert.Equal(103, actual.Width);
            Assert.Equal(159, actual.Height);
        }

        [Fact]
        public void ToGlyphMetrics_WhenInvoked_ReturnsCorrectResult()
        {
            // Arrange
            const string text = "test©";
            var font = CreateFont(new FontSettings());

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

            var font = CreateFont(settings);

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

            var font = CreateFont(settings);
            font.IsPooled = true;

            // Act & Assert
            Assert.Throws<PooledDisposalException>(() =>
            {
                font.Dispose();
            });
        }
        #endregion

        /// <inheritdoc/>
        public void Dispose() => this.glyphMetrics = null;

        /// <summary>
        /// Creates a new instance of <see cref="Font"/> for the purpose of testing.
        /// </summary>
        /// <param name="settings">The font settings to use for the test.</param>
        /// <returns>The instance to test.</returns>
        private Font CreateFont(FontSettings settings)
            => new (
                this.mockFontTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics,
                settings,
                this.glyphChars,
                It.IsAny<string>(),
                It.IsAny<string>());

        /// <summary>
        /// Mocks the kerning value for each character in the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">The text to mock the kerning values for.</param>
        private void MockGlyphKernings(string text)
        {
            if (this.glyphMetrics is null || this.glyphMetrics.Length <= 0)
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
                    var foundGlyphMetric = (from m in this.glyphMetrics
                        where m.Glyph == glyphChar
                        select m).FirstOrDefault();

                    rightGlyphIndex = foundGlyphMetric.CharIndex;
                }

                var ftVector = default(FT_Vector);
                ftVector.x = new IntPtr(i + 1);

                var leftIndex = leftGlyphIndex;
                var rightIndex = rightGlyphIndex;
                this.mockFreeTypeInvoker.Setup(m
                        => m.FT_Get_Kerning(
                            this.facePtr,
                            leftIndex,
                            rightIndex,
                            (uint)FT_Kerning_Mode.FT_KERNING_DEFAULT))
                    .Returns(ftVector);

                leftGlyphIndex = rightGlyphIndex;
            }
        }
    }
}

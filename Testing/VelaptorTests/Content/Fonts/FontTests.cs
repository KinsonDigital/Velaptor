// <copyright file="FontTests.cs" company="KinsonDigital">
// Copyright (c) KinsonDigital. All rights reserved.
// </copyright>

using System.Collections.Generic;
using System.Drawing;
using Velaptor.NativeInterop.OpenGL;
using Velaptor.Services;

namespace VelaptorTests.Content.Fonts
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
    using Velaptor.Content.Fonts;
    using Velaptor.Graphics;
    using Velaptor.NativeInterop.FreeType;
    using Xunit;

    /// <summary>
    /// Tests the <see cref="Font"/> class.
    /// </summary>
    public class FontTests : IDisposable
    {
        private const char InvalidCharacter = '□';
        private const string FontPath = @"C:\test-dir\fonts\";
        private readonly char[] glyphChars =
        {
            'a', 'b', 'c', 'd', 'e',  'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            'A', 'B', 'C', 'D', 'E',  'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            '0', '1', '2', '3', '4',  '5', '6', '7', '8', '9', '`', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '-', '=',
            '~', '_', '+', '[', ']', '\\', ';', '\'', ',', '.', '/', '{', '}', '|', ':', '"', '<', '>', '?', ' ',
        };
        private readonly IntPtr libraryPtr = new (1234);
        private readonly IntPtr facePtr = new (5678);
        private readonly Mock<IGLInvoker> mockGL;
        private readonly Mock<IGLInvokerExtensions> mockGLExtensions;
        private readonly Mock<IFreeTypeInvoker> mockFreeTypeInvoker;
        private readonly Mock<IFreeTypeExtensions> mockFreeTypeExtensions;
        private readonly Mock<IFontAtlasService> mockFontAtlasService;
        private readonly Mock<IImageService> mockImageService;
        private readonly Mock<ITexture> mockTexture;
        private readonly string sampleTestDataDirPath = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\SampleTestData\";
        private Dictionary<char, GlyphMetrics> glyphMetrics = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="FontTests"/> class.
        /// </summary>
        public FontTests()
        {
            this.mockGL = new Mock<IGLInvoker>();
            this.mockGLExtensions = new Mock<IGLInvokerExtensions>();
            this.mockFreeTypeInvoker = new Mock<IFreeTypeInvoker>();
            this.mockFreeTypeInvoker.Setup(m => m.FT_Init_FreeType()).Returns(this.libraryPtr);

            const string glyphTestDataFileName = "glyph-test-data.json";
            var glyphMetricFilePath = $"{this.sampleTestDataDirPath}{glyphTestDataFileName}";
            var glyphMetricData = File.ReadAllText(glyphMetricFilePath);

            var glyphMetricItems = JsonConvert.DeserializeObject<GlyphMetrics[]>(glyphMetricData);

            foreach (var metric in glyphMetricItems)
            {
                this.glyphMetrics.Add(metric.Glyph, metric);
            }

            this.mockFreeTypeExtensions = new Mock<IFreeTypeExtensions>();
            this.mockFreeTypeExtensions.Setup(m => m.CreateFontFace(this.libraryPtr, FontPath)).Returns(this.facePtr);
            this.mockFreeTypeExtensions.Setup(m => m.CreateGlyphMetrics(this.facePtr, null))
                .Returns(this.glyphMetrics);

            this.mockFontAtlasService = new Mock<IFontAtlasService>();
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(It.IsAny<string>(), It.IsAny<int>()))
                .Returns((default, this.glyphMetrics.Values.ToArray()));

            this.mockImageService = new Mock<IImageService>();

            this.mockTexture = new Mock<ITexture>();
        }

        #region Constructor Tests
        [Fact]
        public void Ctor_WhenInvoked_SetsPropertyValues()
        {
            // Arrange
            var pixels = new[,]
            {
                { Color.FromArgb(1, 2, 3, 4), Color.FromArgb(5, 6, 7, 8) },
            };
            var atlasImageData = new ImageData(pixels, 11, 22);

            this.mockFreeTypeExtensions.Setup(m => m.GetFamilyName(FontPath)).Returns("test-font-family");
            this.mockFontAtlasService.Setup(m => m.CreateFontAtlas(FontPath, 12))
                .Returns((atlasImageData, this.glyphMetrics.Values.ToArray()));

            // Act
            var font = new Font(
                this.mockTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics.Values.ToArray(),
                "test-name",
                FontPath,
                It.IsAny<int>());

            // Assert
            // Check that the texture was properly created
            Assert.Equal("test-name", font.FontTextureAtlas.Name);
            Assert.Equal(FontPath, font.FontTextureAtlas.FilePath);
            Assert.True(font.FontTextureAtlas.IsPooled);

            Assert.Equal(font.Metrics.Count, this.glyphMetrics.Count);
            Assert.Equal("test-name", font.Name);
            Assert.Equal("test-font-family", font.FamilyName);
            Assert.Equal(12, font.Size);
            Assert.Equal(FontStyle.Regular, font.Style);
            Assert.Equal(FontPath, font.FilePath);
        }
        #endregion

        #region Prop Tests
        [Fact]
        public void LineSpacing_WhenGettingValue_ReturnsCorrectResult()
        {
            // Arrange
            this.mockFreeTypeExtensions.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12))
                .Returns(0.5f);
            var font = CreateFont();

            // Act
            var actual = font.LineSpacing;

            // Assert
            Assert.Equal(32, actual);
        }

        [Fact]
        public void IsPooled_WhenSettingValue_ReturnsCorrectResult()
        {
            // Arrange
            var font = CreateFont();

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

            // Act
            var font = new Font(
                this.mockTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics.Values.ToArray(),
                "test-name",
                FontPath,
                It.IsAny<int>());

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
            var font = new Font(
                this.mockTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics.Values.ToArray(),
                "test-name",
                FontPath,
                It.IsAny<int>());

            // Act
            var actual = font.GetAvailableGlyphCharacters();

            // Assert
            Assert.Equal(this.glyphChars, actual);
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
            var font = CreateFont();

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

            this.mockFreeTypeExtensions.Setup(m => m.GetFontScaledLineSpacing(this.facePtr, 12)).Returns(2f);
            this.mockFreeTypeExtensions.Setup(m => m.HasKerning(this.facePtr)).Returns(true);
            MockGlyphKernings(text);

            var font = new Font(
                this.mockTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics.Values.ToArray(),
                "test-name",
                FontPath,
                It.IsAny<int>());

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

        [Fact]
        public void Dispose_WhenInvoked_DisposesFont()
        {
            // Arrange
            var font = CreateFont();

            // Act
            font.Dispose();
            font.Dispose();

            // Assert
            Assert.False(font.FontTextureAtlas.IsPooled);
        }

        [Fact]
        public void Dispose_WhilePooled_ThrowsException()
        {
            // Arrange
            var font = CreateFont();
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
        /// <returns>The instance to test.</returns>
        private Font CreateFont(uint size = 12)
            => new (
                this.mockTexture.Object,
                this.mockFreeTypeInvoker.Object,
                this.mockFreeTypeExtensions.Object,
                this.glyphMetrics.Values.ToArray(),
                "test-name",
                FontPath,
                It.IsAny<int>());

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

        private GlyphMetrics[] CreateMetrics(string testText)
        {
            var start = 0;
            var result = new List<GlyphMetrics>();

            foreach (var character in testText)
            {
                var newGlyphMetric = default(GlyphMetrics);

                newGlyphMetric.Ascender = start + 1;
                newGlyphMetric.Descender = start + 2;
                newGlyphMetric.Glyph = character;
                newGlyphMetric.CharIndex = (uint)(start + 3);
                newGlyphMetric.GlyphBounds = new RectangleF(start + 4, start + 5, start + 6, start + 7);
                newGlyphMetric.GlyphHeight = start + 8;
                newGlyphMetric.GlyphWidth = start + 9;
                newGlyphMetric.HorizontalAdvance = start + 10;
                newGlyphMetric.XMax = start + 11;
                newGlyphMetric.XMin = start + 12;
                newGlyphMetric.YMax = start + 13;
                newGlyphMetric.YMin = start + 14;
                newGlyphMetric.HoriBearingX = start + 15;
                newGlyphMetric.HoriBearingY = start + 16;

                result.Add(newGlyphMetric);

                start += 16;
            }

            return result.ToArray();
        }
    }
}
